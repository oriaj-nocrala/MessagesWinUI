using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MessagesWinUI.Commands;
using MessagesWinUI.Helpers;
using MessagesWinUI.Interop;
using MessagesWinUI.Models;
using Microsoft.UI.Dispatching;
using Windows.Storage;

namespace MessagesWinUI.ViewModels;

/// <summary>
/// Main ViewModel for the application managing peers, conversations, and P2P communication
/// </summary>
public class MainWindowViewModel : BaseViewModel, IDisposable
{
    private readonly P2PMessenger _messenger;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly DispatcherQueueTimer _discoveryTimer;
    private readonly DispatcherQueueTimer _cleanupTimer;
    private bool _isDisposed = false;
    
    private string _currentUserName = "You";
    private string _connectionStatus = "";
    private int _peerCount = 0;
    private ConversationViewModel? _selectedConversation;
    private PeerInfo? _selectedPeer;
    private bool _isDiscovering = false;

    /// <summary>
    /// Collection of discovered peers
    /// </summary>
    public ObservableCollection<PeerInfo> Peers { get; }

    /// <summary>
    /// Collection of active conversations
    /// </summary>
    public ObservableCollection<ConversationViewModel> Conversations { get; }

    /// <summary>
    /// Currently selected/active conversation
    /// </summary>
    public ConversationViewModel? SelectedConversation
    {
        get => _selectedConversation;
        set => SetProperty(ref _selectedConversation, value);
    }

    /// <summary>
    /// Currently selected peer in the peers list
    /// </summary>
    public PeerInfo? SelectedPeer
    {
        get => _selectedPeer;
        set => SetProperty(ref _selectedPeer, value);
    }

    /// <summary>
    /// Current user's display name
    /// </summary>
    public string CurrentUserName
    {
        get => _currentUserName;
        set => SetProperty(ref _currentUserName, value);
    }

    /// <summary>
    /// Connection status text for display
    /// </summary>
    public string ConnectionStatus
    {
        get => _connectionStatus;
        set => SetProperty(ref _connectionStatus, value);
    }

    /// <summary>
    /// Number of discovered peers
    /// </summary>
    public int PeerCount
    {
        get => _peerCount;
        set => SetProperty(ref _peerCount, value, UpdatePeerCountDisplay);
    }

    /// <summary>
    /// Display text for peer count
    /// </summary>
    public string PeerCountDisplay { get; private set; } = "";

    /// <summary>
    /// Whether peer discovery is currently active
    /// </summary>
    public bool IsDiscovering
    {
        get => _isDiscovering;
        set => SetProperty(ref _isDiscovering, value);
    }

    /// <summary>
    /// Localized strings for UI binding
    /// </summary>
    public string AppTitle => LocalizationHelper.AppTitle;
    public string OnlineStatus => LocalizationHelper.Online;
    public string DiscoverPeersText => LocalizationHelper.DiscoverPeers;
    public string PeersText => LocalizationHelper.Peers;
    public string WelcomeTitleText => LocalizationHelper.WelcomeTitle;
    public string WelcomeMessageText => LocalizationHelper.WelcomeMessage;

    /// <summary>
    /// Command to start peer discovery
    /// </summary>
    public ICommand DiscoverPeersCommand { get; }

    /// <summary>
    /// Command to connect to a specific peer
    /// </summary>
    public ICommand ConnectToPeerCommand { get; }

    /// <summary>
    /// Command to send a message
    /// </summary>
    public ICommand SendMessageCommand { get; }

    /// <summary>
    /// Command to send a file
    /// </summary>
    public ICommand SendFileCommand { get; }

    /// <summary>
    /// Command to start a new conversation
    /// </summary>
    public ICommand StartConversationCommand { get; }

    /// <summary>
    /// Command to close a conversation
    /// </summary>
    public ICommand CloseConversationCommand { get; }

    public MainWindowViewModel()
    {
        // Initialize collections
        Peers = new ObservableCollection<PeerInfo>();
        Conversations = new ObservableCollection<ConversationViewModel>();
        
        // Initialize messenger
        _messenger = new P2PMessenger(Environment.UserName);
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        
        // Initialize commands
        DiscoverPeersCommand = new RelayCommand(StartDiscovery);
        ConnectToPeerCommand = new RelayCommand<string>(ConnectToPeer, CanConnectToPeer);
        SendMessageCommand = new RelayCommand<(string peerId, string message)>(SendMessage, CanSendMessage);
        SendFileCommand = new RelayCommand<(string peerId, StorageFile file)>(SendFile, CanSendFile);
        StartConversationCommand = new RelayCommand<string>(StartConversation);
        CloseConversationCommand = new RelayCommand<string>(CloseConversation);

        // Initialize timers
        _discoveryTimer = _dispatcherQueue.CreateTimer();
        _discoveryTimer.Interval = TimeSpan.FromSeconds(5);
        _discoveryTimer.Tick += DiscoveryTimer_Tick;

        _cleanupTimer = _dispatcherQueue.CreateTimer();
        _cleanupTimer.Interval = TimeSpan.FromSeconds(30);
        _cleanupTimer.Tick += CleanupTimer_Tick;

        // Setup messenger events
        SetupMessengerEvents();

        // Initialize connection status
        UpdateConnectionStatus();
    }

    /// <summary>
    /// Initializes the P2P messenger and starts services
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            CurrentUserName = Environment.UserName;
            _messenger.Start();
            
            ConnectionStatus = LocalizationHelper.GetString("ListeningOn", new object[] { _messenger.LocalIp ?? "Unknown" });
            
            // Start discovery and cleanup
            _discoveryTimer.Start();
            _cleanupTimer.Start();
            
            // Initial peer discovery
            await Task.Run(() => _messenger.DiscoverPeers());
            IsDiscovering = true;
        }
        catch (Exception ex)
        {
            ConnectionStatus = LocalizationHelper.GetString("ConnectionFailed");
            System.Diagnostics.Debug.WriteLine($"Failed to initialize messenger: {ex.Message}");
        }
    }

    private void SetupMessengerEvents()
    {
        _messenger.PeerDiscovered += OnPeerDiscovered;
        _messenger.PeerConnected += OnPeerConnected;
        _messenger.PeerDisconnected += OnPeerDisconnected;
        _messenger.MessageReceived += OnMessageReceived;
        _messenger.FileReceived += OnFileReceived;
    }

    private void OnPeerDiscovered(object? sender, PeerDiscoveredEventArgs e)
    {
        if (_isDisposed) return;
        
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (_isDisposed) return;
            // Filter out self
            if (e.PeerName.Equals(CurrentUserName, StringComparison.OrdinalIgnoreCase))
                return;

            var existingPeer = Peers.FirstOrDefault(p => p.Id == e.PeerId);
            if (existingPeer != null)
            {
                existingPeer.UpdateLastSeen();
            }
            else
            {
                Peers.Add(new PeerInfo
                {
                    Id = e.PeerId,
                    Name = e.PeerName,
                    IsConnected = false,
                    LastSeen = DateTime.Now
                });
            }
            
            PeerCount = Peers.Count;
        });
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (_isDisposed) return;
        
        System.Diagnostics.Debug.WriteLine($"PeerConnected event fired for: {e.PeerId} ({e.PeerName})");
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (_isDisposed) return;
            var peer = Peers.FirstOrDefault(p => p.Id == e.PeerId);
            if (peer != null)
            {
                System.Diagnostics.Debug.WriteLine($"Updating peer {e.PeerName} to connected state. Was: {peer.IsConnected}");
                peer.IsConnected = true;
                peer.UpdateLastSeen();
                System.Diagnostics.Debug.WriteLine($"Peer {e.PeerName} is now connected: {peer.IsConnected}, Status: {peer.Status}");
                
                // Show advanced connection notification
                NotificationHelper.ShowConnectionStatusNotification(e.PeerName, true, e.PeerId);
                
                // Show celebration for first connection
                if (Peers.Count(p => p.IsConnected) == 1)
                {
                    NotificationHelper.ShowCelebrationNotification(
                        "First P2P Connection!", 
                        $"You're now connected to {e.PeerName}. Start chatting!",
                        "firstConnection");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Peer {e.PeerId} not found in Peers list");
            }
        });
    }

    private void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e)
    {
        if (_isDisposed) return;
        
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (_isDisposed) return;
            var peer = Peers.FirstOrDefault(p => p.Id == e.PeerId);
            if (peer != null)
            {
                peer.IsConnected = false;
                
                // Show advanced disconnection notification
                NotificationHelper.ShowConnectionStatusNotification(e.PeerName, false, e.PeerId);
            }
        });
    }

    private void OnMessageReceived(object? sender, MessageReceivedEventArgs e)
    {
        if (_isDisposed) return;
        
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (_isDisposed) return;
            var conversation = GetOrCreateConversation(e.SenderId, e.SenderName);
            
            var message = new MessageInfo
            {
                SenderName = e.SenderName,
                Content = e.Message,
                Timestamp = DateTime.Now,
                IsFromMe = false,
                MessageType = MessageType.Text
            };
            
            conversation.AddMessage(message);
            
            // Show advanced interactive notification for new message
            NotificationHelper.ShowAdvancedMessageNotification(e.SenderName, e.Message, e.SenderId);
        });
    }

    private void OnFileReceived(object? sender, FileReceivedEventArgs e)
    {
        if (_isDisposed) return;
        
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (_isDisposed) return;
            var conversation = GetOrCreateConversation(e.SenderId, e.SenderName);
            
            var message = new MessageInfo
            {
                SenderName = e.SenderName,
                Content = e.FileName,
                Timestamp = DateTime.Now,
                IsFromMe = false,
                MessageType = MessageType.File
            };
            
            conversation.AddMessage(message);
        });
    }

    private void StartDiscovery()
    {
        try
        {
            _messenger.DiscoverPeers();
            IsDiscovering = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to start discovery: {ex.Message}");
        }
    }

    private void ConnectToPeer(string? peerId)
    {
        if (string.IsNullOrEmpty(peerId))
        {
            System.Diagnostics.Debug.WriteLine("ConnectToPeer called with null/empty peerId");
            return;
        }

        System.Diagnostics.Debug.WriteLine($"Attempting to connect to peer: {peerId}");

        try
        {
            _messenger.ConnectToPeer(peerId);
            System.Diagnostics.Debug.WriteLine($"Native connect call completed for peer: {peerId}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to connect to peer {peerId}: {ex.Message}");
        }
    }

    private bool CanConnectToPeer(string? peerId)
    {
        if (string.IsNullOrEmpty(peerId)) return false;
        
        var peer = Peers.FirstOrDefault(p => p.Id == peerId);
        return peer != null && !peer.IsConnected;
    }

    private void SendMessage((string peerId, string message) args)
    {
        if (string.IsNullOrEmpty(args.peerId) || string.IsNullOrWhiteSpace(args.message))
            return;

        try
        {
            System.Diagnostics.Debug.WriteLine($"Sending message: '{args.message}' to peer: {args.peerId}");
            _messenger.SendTextMessage(args.peerId, args.message);
            
            var conversation = GetOrCreateConversation(args.peerId, GetPeerName(args.peerId));
            System.Diagnostics.Debug.WriteLine($"Got conversation: {conversation?.PeerName}");
            
            var message = new MessageInfo
            {
                SenderName = CurrentUserName,
                Content = args.message,
                Timestamp = DateTime.Now,
                IsFromMe = true,
                MessageType = MessageType.Text
            };
            
            System.Diagnostics.Debug.WriteLine($"Adding sent message to conversation: {message.Content}");
            conversation.AddMessage(message);
            System.Diagnostics.Debug.WriteLine($"Message added, conversation now has {conversation.Messages.Count} messages");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to send message: {ex.Message}");
        }
    }

    private bool CanSendMessage((string peerId, string message) args)
    {
        if (string.IsNullOrEmpty(args.peerId) || string.IsNullOrWhiteSpace(args.message))
            return false;
            
        var peer = Peers.FirstOrDefault(p => p.Id == args.peerId);
        return peer != null && peer.IsConnected;
    }

    private void SendFile((string peerId, StorageFile file) args)
    {
        if (string.IsNullOrEmpty(args.peerId) || args.file == null)
            return;

        try
        {
            _messenger.SendFile(args.peerId, args.file.Path);
            
            var conversation = GetOrCreateConversation(args.peerId, GetPeerName(args.peerId));
            
            var message = new MessageInfo
            {
                SenderName = CurrentUserName,
                Content = args.file.Name,
                Timestamp = DateTime.Now,
                IsFromMe = true,
                MessageType = MessageType.File
            };
            
            conversation.AddMessage(message);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to send file: {ex.Message}");
        }
    }

    private bool CanSendFile((string peerId, StorageFile file) args)
    {
        if (string.IsNullOrEmpty(args.peerId) || args.file == null)
            return false;
            
        var peer = Peers.FirstOrDefault(p => p.Id == args.peerId);
        return peer != null && peer.IsConnected;
    }

    private void StartConversation(string? peerId)
    {
        if (string.IsNullOrEmpty(peerId))
        {
            System.Diagnostics.Debug.WriteLine("StartConversation called with null/empty peerId");
            return;
        }
        
        System.Diagnostics.Debug.WriteLine($"StartConversation called for peerId: {peerId}");
        var peerName = GetPeerName(peerId);
        System.Diagnostics.Debug.WriteLine($"Peer name resolved: {peerName}");
        
        var conversation = GetOrCreateConversation(peerId, peerName);
        System.Diagnostics.Debug.WriteLine($"Conversation created/retrieved: {conversation?.PeerName}");
        
        SelectedConversation = conversation;
        System.Diagnostics.Debug.WriteLine($"SelectedConversation set to: {SelectedConversation?.PeerName}");
    }

    private void CloseConversation(string? peerId)
    {
        if (string.IsNullOrEmpty(peerId)) return;
        
        var conversation = Conversations.FirstOrDefault(c => c.PeerId == peerId);
        if (conversation != null)
        {
            Conversations.Remove(conversation);
            
            if (SelectedConversation == conversation)
            {
                SelectedConversation = Conversations.FirstOrDefault();
            }
        }
    }

    private ConversationViewModel GetOrCreateConversation(string peerId, string peerName)
    {
        var existing = Conversations.FirstOrDefault(c => c.PeerId == peerId);
        if (existing != null)
        {
            return existing;
        }

        var conversation = new ConversationViewModel(peerId, peerName);
        Conversations.Add(conversation);
        return conversation;
    }

    private string GetPeerName(string peerId)
    {
        var peer = Peers.FirstOrDefault(p => p.Id == peerId);
        return peer?.Name ?? peerId;
    }

    private void UpdatePeerCountDisplay()
    {
        PeerCountDisplay = LocalizationHelper.GetString("PeersDiscovered", new object[] { PeerCount });
        OnPropertyChanged(nameof(PeerCountDisplay));
    }

    private void UpdateConnectionStatus()
    {
        ConnectionStatus = LocalizationHelper.GetString("Disconnected");
    }

    private void DiscoveryTimer_Tick(DispatcherQueueTimer sender, object args)
    {
        try
        {
            _messenger.DiscoverPeers();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Discovery timer error: {ex.Message}");
        }
    }

    private void CleanupTimer_Tick(DispatcherQueueTimer sender, object args)
    {
        try
        {
            var stalePeers = Peers.Where(p => 
                !p.IsConnected && 
                DateTime.Now - p.LastSeen > TimeSpan.FromMinutes(2)
            ).ToList();

            foreach (var stalePeer in stalePeers)
            {
                Peers.Remove(stalePeer);
                
                // Close any conversations with stale peers
                var conversation = Conversations.FirstOrDefault(c => c.PeerId == stalePeer.Id);
                if (conversation != null)
                {
                    Conversations.Remove(conversation);
                    if (SelectedConversation == conversation)
                    {
                        SelectedConversation = Conversations.FirstOrDefault();
                    }
                }
            }

            if (stalePeers.Count > 0)
            {
                PeerCount = Peers.Count;
                ConnectionStatus = LocalizationHelper.GetString("CleanedUpPeers", 
                    new object[] { stalePeers.Count, Peers.Count });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Cleanup timer error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        try
        {
            if (_isDisposed) return;
            _isDisposed = true;
            
            System.Diagnostics.Debug.WriteLine("🧹 Starting ViewModel cleanup...");
            
            // Stop and dispose messenger FIRST to avoid callbacks during cleanup
            if (_messenger != null)
            {
                // Unsubscribe from all events BEFORE stopping to prevent UI access
                _messenger.PeerDiscovered -= OnPeerDiscovered;
                _messenger.PeerConnected -= OnPeerConnected;
                _messenger.PeerDisconnected -= OnPeerDisconnected;
                _messenger.MessageReceived -= OnMessageReceived;
                _messenger.FileReceived -= OnFileReceived;
                
                // Now safely stop and dispose
                _messenger.Stop();
                _messenger.Dispose();
                System.Diagnostics.Debug.WriteLine("✅ Messenger stopped and disposed");
            }
            
            // Stop timers after messenger is disposed
            if (_discoveryTimer != null)
            {
                _discoveryTimer.Stop();
                System.Diagnostics.Debug.WriteLine("✅ Discovery timer stopped");
            }
            
            if (_cleanupTimer != null)
            {
                _cleanupTimer.Stop();
                System.Diagnostics.Debug.WriteLine("✅ Cleanup timer stopped");
            }
            
            // Clear collections safely
            Conversations.Clear();
            Peers.Clear();
            
            // Cleanup notifications (but don't access UI elements)
            try
            {
                NotificationHelper.Cleanup();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Notification cleanup warning: {ex.Message}");
                // Don't rethrow - this is cleanup
            }
            
            System.Diagnostics.Debug.WriteLine("✅ ViewModel cleanup completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error during ViewModel disposal: {ex.Message}");
            // Don't rethrow exceptions during disposal
        }
    }
}