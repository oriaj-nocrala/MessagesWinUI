using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MessagesWinUI.Interop;
using MessagesWinUI.Models;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml.Media;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI;
using System.Linq;

namespace MessagesWinUI;

/// <summary>
/// MSN Messenger-style P2P messaging window
/// </summary>
public sealed partial class MainWindow : Window
{
    private P2PMessenger? _messenger;
    private readonly ObservableCollection<PeerInfo> _peers = new();
    private PeerInfo? _selectedPeer;
    private readonly DispatcherTimer _discoveryTimer = new();
    
    // Message persistence system
    private readonly Dictionary<string, List<MessageInfo>> _messageHistory = new();
    private const int MaxMessagesPerPeer = 1000; // Limit messages to prevent memory issues
    
    // Tab system
    private readonly Dictionary<string, TabViewItem> _conversationTabs = new();
    private readonly Dictionary<string, (ScrollViewer ScrollViewer, StackPanel MessagesPanel, TextBox MessageInput)> _tabControls = new();

    public MainWindow()
    {
        InitializeComponent();
        
        // Setup closed event
        Closed += MainWindow_Closed;
        
        // Setup UI
        PeersListView.ItemsSource = _peers;
        
        // Setup discovery timer
        _discoveryTimer.Interval = TimeSpan.FromSeconds(5);
        _discoveryTimer.Tick += (s, e) => _messenger?.DiscoverPeers();
        
        // Initialize messenger after UI is activated
        Activated += MainWindow_Activated;
    }

    private bool _initialized = false;
    
    private void MainWindow_Activated(object sender, WindowActivatedEventArgs e)
    {
        // Initialize messenger only once after UI is activated
        if (!_initialized)
        {
            _initialized = true;
            InitializeMessenger();
        }
    }

    private void InitializeMessenger()
    {
        try
        {
            // Get user name
            string userName = Environment.UserName;
            UserNameTextBlock.Text = userName;
            
            // Try to create messenger with default ports
            _messenger = new P2PMessenger(userName);
            
            // Subscribe to events
            _messenger.PeerDiscovered += OnPeerDiscovered;
            _messenger.PeerConnected += OnPeerConnected;
            _messenger.PeerDisconnected += OnPeerDisconnected;
            _messenger.MessageReceived += OnMessageReceived;
            _messenger.Error += OnError;
            
            // Start messenger
            _messenger.Start();
            
            // Update UI
            StatusTextBlock.Text = "Online";
            ConnectionStatusTextBlock.Text = $"Listening on {_messenger.LocalIp}";
            
            // Start discovery
            _messenger.DiscoverPeers();
            _discoveryTimer.Start();
        }
        catch (System.DllNotFoundException dllEx)
        {
            ShowError($"P2P Library not found: {dllEx.Message}");
            StatusTextBlock.Text = "Offline (Library Error)";
            ConnectionStatusTextBlock.Text = "P2P functionality unavailable";
        }
        catch (System.EntryPointNotFoundException entryEx)
        {
            ShowError($"P2P Library incompatible: {entryEx.Message}");
            StatusTextBlock.Text = "Offline (Library Error)";
            ConnectionStatusTextBlock.Text = "P2P functionality unavailable";
        }
        catch (Exception ex)
        {
            ShowError($"Failed to initialize messenger: {ex.Message}");
            StatusTextBlock.Text = "Offline (Init Error)";
            ConnectionStatusTextBlock.Text = "Check error details";
        }
    }

    private void OnPeerDiscovered(object? sender, PeerEventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            var existingPeer = _peers.FirstOrDefault(p => p.Id == e.PeerId);
            if (existingPeer == null)
            {
                _peers.Add(new PeerInfo
                {
                    Id = e.PeerId,
                    Name = e.PeerName,
                    IsConnected = false
                });
            }
            
            UpdatePeerCount();
        });
    }

    private void OnPeerConnected(object? sender, PeerEventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            var peer = _peers.FirstOrDefault(p => p.Id == e.PeerId);
            if (peer != null)
            {
                peer.IsConnected = true;
                
                // Auto-create and open conversation tab
                CreateConversationTab(e.PeerId, e.PeerName);
            }
        });
    }

    private void OnPeerDisconnected(object? sender, PeerEventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            var peer = _peers.FirstOrDefault(p => p.Id == e.PeerId);
            if (peer != null)
            {
                peer.IsConnected = false;
            }
        });
    }

    private void OnMessageReceived(object? sender, MessageReceivedEventArgs e)
    {
        DispatcherQueue.TryEnqueue(async () =>
        {
            await AddMessage(new MessageInfo
            {
                SenderName = e.PeerName,
                Content = e.Message,
                Timestamp = DateTime.Now,
                IsFromMe = false
            }, e.PeerId); // Specify the sender's peer ID
        });
    }

    private void OnError(object? sender, ErrorEventArgs e)
    {
        DispatcherQueue.TryEnqueue(() => ShowError(e.ErrorMessage));
    }

    private void ShowError(string message)
    {
        ConnectionStatusTextBlock.Text = $"Error: {message}";
        ConnectionStatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
    }

    private void UpdatePeerCount()
    {
        PeerCountTextBlock.Text = $"{_peers.Count} peers discovered";
    }

    private async Task AddMessage(MessageInfo message, string? peerId = null)
    {
        // Use current selected peer if not specified
        var targetPeerId = peerId ?? _selectedPeer?.Id;
        
        if (targetPeerId != null)
        {
            // Add to message history
            AddMessageToHistory(targetPeerId, message);
            
            // Add to the tab if it exists
            await AddMessageToTab(targetPeerId, message);
        }
    }

    private async Task AddMessageToTab(string peerId, MessageInfo message)
    {
        if (!_tabControls.ContainsKey(peerId))
        {
            // If no tab exists, create one (this handles cases where messages arrive before tab creation)
            var peer = _peers.FirstOrDefault(p => p.Id == peerId);
            if (peer != null)
            {
                CreateConversationTab(peerId, peer.Name);
            }
            else
            {
                return; // Can't create tab without peer info
            }
        }

        var (scrollViewer, messagesPanel, _) = _tabControls[peerId];
        
        // Create message element and add to the tab
        var messageElement = CreateMessageElement(message);
        messagesPanel.Children.Add(messageElement);
        
        // Animate if this is the active tab
        var isActiveTab = ConversationTabView.SelectedItem is TabViewItem activeTab && 
                         activeTab.Tag as string == peerId;
        
        if (isActiveTab)
        {
            // Add fade-in animation for active tab
            await AnimateMessageFadeIn(messageElement);
        }
        
        // Auto-scroll to bottom
        await Task.Delay(10); // Wait for layout
        scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null, false);
    }

    private async Task AnimateMessageFadeIn(Border messagePanel)
    {
        // Simple fade-in animation
        messagePanel.Opacity = 0;
        
        // Animate opacity from 0 to 1 over 300ms
        var storyboard = new Microsoft.UI.Xaml.Media.Animation.Storyboard();
        var opacityAnimation = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = new Microsoft.UI.Xaml.Media.Animation.QuadraticEase { EasingMode = Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseOut }
        };
        
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(opacityAnimation, messagePanel);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
        
        storyboard.Children.Add(opacityAnimation);
        
        var tcs = new TaskCompletionSource<bool>();
        storyboard.Completed += (s, e) => tcs.SetResult(true);
        
        storyboard.Begin();
        await tcs.Task;
    }

    private void AddMessageToHistory(string peerId, MessageInfo message)
    {
        if (!_messageHistory.ContainsKey(peerId))
        {
            _messageHistory[peerId] = new List<MessageInfo>();
        }
        
        _messageHistory[peerId].Add(message);
        
        // Limit messages per peer to prevent memory bloat
        if (_messageHistory[peerId].Count > MaxMessagesPerPeer)
        {
            _messageHistory[peerId].RemoveAt(0); // Remove oldest message
        }
    }
    
    private void LoadMessagesForPeer(string peerId)
    {
        // This method is now deprecated in favor of LoadMessagesForTab
        // Keep for backward compatibility with peer selection
    }
    
    private void AddMessageToUI(MessageInfo message, bool animate = true)
    {
        // This method is now deprecated in favor of AddMessageToTab and CreateMessageElement
        // Keep for backward compatibility if needed
    }
    
    private void ClearMessageHistory(string peerId)
    {
        if (_messageHistory.ContainsKey(peerId))
        {
            _messageHistory.Remove(peerId);
        }
    }
    
    private void ClearAllMessageHistory()
    {
        _messageHistory.Clear();
        // Clear all tabs
        foreach (var tab in _conversationTabs.Values.ToList())
        {
            ConversationTabView.TabItems.Remove(tab);
        }
        _conversationTabs.Clear();
        _tabControls.Clear();
    }

    private void DiscoverButton_Click(object sender, RoutedEventArgs e)
    {
        _messenger?.DiscoverPeers();
    }

    private void ConversationTabView_AddTabButtonClick(TabView sender, object args)
    {
        // Optional: Could show peer selection dialog
        // For now, we'll let tabs be created automatically when connecting to peers
    }

    private void ConversationTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        var tabItem = args.Tab;
        if (tabItem.Tag is string peerId)
        {
            CloseConversationTab(peerId);
        }
    }

    private void ConversationTabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ConversationTabView.SelectedItem is TabViewItem selectedTab)
        {
            if (selectedTab.Tag is string peerId)
            {
                _selectedPeer = _peers.FirstOrDefault(p => p.Id == peerId);
                
                // Focus the message input for this tab
                if (_tabControls.ContainsKey(peerId))
                {
                    _tabControls[peerId].MessageInput.Focus(FocusState.Programmatic);
                }
            }
        }
    }

    private void CreateConversationTab(string peerId, string peerName)
    {
        // Don't create duplicate tabs
        if (_conversationTabs.ContainsKey(peerId))
        {
            // Just select existing tab
            ConversationTabView.SelectedItem = _conversationTabs[peerId];
            return;
        }

        // Create new tab
        var tabItem = new TabViewItem
        {
            Header = peerName,
            Tag = peerId,
            IsClosable = true
        };

        // Create tab content with proper Grid layout (now working!)
        var mainGrid = new Grid
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        
        // Row definitions: Messages area (*) and Input area (Auto)
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Messages area - takes all available space
        var scrollViewer = new ScrollViewer
        {
            Padding = new Thickness(16),
            VerticalScrollMode = ScrollMode.Enabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        Grid.SetRow(scrollViewer, 0);

        var messagesPanel = new StackPanel
        {
            Spacing = 8
        };
        scrollViewer.Content = messagesPanel;
        mainGrid.Children.Add(scrollViewer);

        // Message input area - fixed at bottom with theme colors
        var inputBorder = new Border
        {
            Background = Application.Current.Resources["LayerFillColorDefaultBrush"] as Microsoft.UI.Xaml.Media.Brush,
            BorderBrush = Application.Current.Resources["CardStrokeColorDefaultBrush"] as Microsoft.UI.Xaml.Media.Brush,
            BorderThickness = new Thickness(0, 1, 0, 0),
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        Grid.SetRow(inputBorder, 1);

        var inputGrid = new Grid
        {
            Padding = new Thickness(16)
        };
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var messageTextBox = new TextBox
        {
            PlaceholderText = "Type your message here...",
            MinHeight = 36,
            AcceptsReturn = false,
            TextWrapping = TextWrapping.Wrap
        };
        messageTextBox.KeyDown += (s, e) => TabMessageTextBox_KeyDown(s, e, peerId);
        Grid.SetColumn(messageTextBox, 0);
        inputGrid.Children.Add(messageTextBox);

        var sendFileButton = new Button
        {
            Content = "📁",
            Margin = new Thickness(8, 0, 0, 0)
        };
        sendFileButton.Click += (s, e) => TabSendFileButton_Click(s, e, peerId);
        ToolTipService.SetToolTip(sendFileButton, "Send File");
        Grid.SetColumn(sendFileButton, 1);
        inputGrid.Children.Add(sendFileButton);

        var sendButton = new Button
        {
            Content = "Send",
            Margin = new Thickness(8, 0, 0, 0)
        };
        sendButton.Click += (s, e) => TabSendButton_Click(s, e, peerId);
        Grid.SetColumn(sendButton, 2);
        inputGrid.Children.Add(sendButton);

        inputBorder.Child = inputGrid;
        mainGrid.Children.Add(inputBorder);

        tabItem.Content = mainGrid;

        // Store references
        _conversationTabs[peerId] = tabItem;
        _tabControls[peerId] = (scrollViewer, messagesPanel, messageTextBox);

        // Add tab and select it
        ConversationTabView.TabItems.Add(tabItem);
        ConversationTabView.SelectedItem = tabItem;

        // Load existing messages
        LoadMessagesForTab(peerId);
    }

    private void CloseConversationTab(string peerId)
    {
        if (_conversationTabs.ContainsKey(peerId))
        {
            ConversationTabView.TabItems.Remove(_conversationTabs[peerId]);
            _conversationTabs.Remove(peerId);
            _tabControls.Remove(peerId);
            
            // Clear selected peer if this was the active tab
            if (_selectedPeer?.Id == peerId)
            {
                _selectedPeer = null;
            }
        }
    }

    private void LoadMessagesForTab(string peerId)
    {
        if (!_tabControls.ContainsKey(peerId))
            return;

        var (scrollViewer, messagesPanel, _) = _tabControls[peerId];
        
        // Clear current messages
        messagesPanel.Children.Clear();

        // Load messages from history if available
        if (_messageHistory.ContainsKey(peerId))
        {
            foreach (var message in _messageHistory[peerId])
            {
                var messageElement = CreateMessageElement(message);
                messagesPanel.Children.Add(messageElement);
            }
            
            // Scroll to bottom
            scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null, false);
        }
    }

    private void TabMessageTextBox_KeyDown(object sender, KeyRoutedEventArgs e, string peerId)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            SendTabMessage(peerId);
            e.Handled = true;
        }
    }

    private void TabSendButton_Click(object sender, RoutedEventArgs e, string peerId)
    {
        SendTabMessage(peerId);
    }

    private async void TabSendFileButton_Click(object sender, RoutedEventArgs e, string peerId)
    {
        var peer = _peers.FirstOrDefault(p => p.Id == peerId);
        if (peer == null || !peer.IsConnected)
        {
            ShowError("Peer is not connected");
            return;
        }

        try
        {
            var picker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.List
            };
            picker.FileTypeFilter.Add("*");
            
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                _messenger?.SendFile(peerId, file.Path);
                
                await AddMessage(new MessageInfo
                {
                    SenderName = "You",
                    Content = $"Sent file: {file.Name}",
                    Timestamp = DateTime.Now,
                    IsFromMe = true
                }, peerId);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Failed to send file: {ex.Message}");
        }
    }

    private async void SendTabMessage(string peerId)
    {
        if (!_tabControls.ContainsKey(peerId))
            return;

        var peer = _peers.FirstOrDefault(p => p.Id == peerId);
        if (peer == null || !peer.IsConnected)
        {
            ShowError("Please connect to the peer first");
            return;
        }

        var messageTextBox = _tabControls[peerId].MessageInput;
        var message = messageTextBox.Text.Trim();
        if (string.IsNullOrEmpty(message))
            return;

        try
        {
            _messenger?.SendTextMessage(peerId, message);
            
            await AddMessage(new MessageInfo
            {
                SenderName = "You",
                Content = message,
                Timestamp = DateTime.Now,
                IsFromMe = true
            }, peerId);
            
            messageTextBox.Text = string.Empty;
            messageTextBox.Focus(FocusState.Programmatic);
        }
        catch (Exception ex)
        {
            ShowError($"Failed to send message: {ex.Message}");
        }
    }

    private Border CreateMessageElement(MessageInfo message)
    {
        var messagePanel = new Border
        {
            Margin = new Thickness(0, 4, 0, 0),
            Padding = new Thickness(12, 8, 12, 8),
            CornerRadius = new CornerRadius(8),
            HorizontalAlignment = message.IsFromMe ? HorizontalAlignment.Right : HorizontalAlignment.Left,
            MaxWidth = 400
        };

        if (message.IsFromMe)
        {
            messagePanel.Background = new SolidColorBrush(Colors.DodgerBlue);
        }
        else
        {
            messagePanel.Background = new SolidColorBrush(Colors.LightGray);
        }

        var contentPanel = new StackPanel();
        
        var senderText = new TextBlock
        {
            Text = message.SenderName,
            FontSize = 11,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = message.IsFromMe ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black),
            Margin = new Thickness(0, 0, 0, 2)
        };
        
        var messageText = new TextBlock
        {
            Text = message.Content,
            TextWrapping = TextWrapping.Wrap,
            Foreground = message.IsFromMe ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black)
        };
        
        var timestampText = new TextBlock
        {
            Text = message.Timestamp.ToString("HH:mm"),
            FontSize = 10,
            Foreground = message.IsFromMe ? new SolidColorBrush(Colors.LightBlue) : new SolidColorBrush(Colors.Gray),
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 2, 0, 0)
        };

        contentPanel.Children.Add(senderText);
        contentPanel.Children.Add(messageText);
        contentPanel.Children.Add(timestampText);
        messagePanel.Child = contentPanel;
        
        return messagePanel;
    }

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string peerId)
        {
            try
            {
                _messenger?.ConnectToPeer(peerId);
            }
            catch (Exception ex)
            {
                ShowError($"Failed to connect: {ex.Message}");
            }
        }
    }

    private void PeersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PeersListView.SelectedItem is PeerInfo selectedPeer)
        {
            // Open or switch to conversation tab for this peer
            if (selectedPeer.IsConnected && !_conversationTabs.ContainsKey(selectedPeer.Id))
            {
                CreateConversationTab(selectedPeer.Id, selectedPeer.Name);
            }
            else if (_conversationTabs.ContainsKey(selectedPeer.Id))
            {
                // Switch to existing tab
                ConversationTabView.SelectedItem = _conversationTabs[selectedPeer.Id];
            }
        }
    }

    // Old UI methods removed - functionality moved to tab system


    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        _discoveryTimer.Stop();
        _messenger?.Stop();
        _messenger?.Dispose();
        
        // Clear message history to free memory
        ClearAllMessageHistory();
    }
}