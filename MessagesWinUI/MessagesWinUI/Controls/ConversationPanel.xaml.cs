using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MessagesWinUI.Models;
using MessagesWinUI.ViewModels;
using Windows.Storage.Pickers;
using Windows.Storage;

namespace MessagesWinUI.Controls;

/// <summary>
/// Professional conversation panel with message display and input controls
/// </summary>
public sealed partial class ConversationPanel : UserControl, System.ComponentModel.INotifyPropertyChanged
{
    private string _currentMessage = string.Empty;

    /// <summary>
    /// Event fired when a message is sent
    /// </summary>
    public event EventHandler<string>? MessageSent;

    /// <summary>
    /// Event fired when a file is selected to send
    /// </summary>
    public event EventHandler<StorageFile>? FileSent;

    /// <summary>
    /// Collection of messages in this conversation
    /// </summary>
    public ObservableCollection<MessageInfo> Messages { get; }

    /// <summary>
    /// Current message being typed
    /// </summary>
    public string CurrentMessage
    {
        get => _currentMessage;
        set
        {
            if (_currentMessage != value)
            {
                _currentMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasMessageText));
            }
        }
    }

    /// <summary>
    /// Whether there's text in the message input
    /// </summary>
    public bool HasMessageText => !string.IsNullOrWhiteSpace(CurrentMessage);

    /// <summary>
    /// Name of the peer being chatted with
    /// </summary>
    public static readonly DependencyProperty PeerNameProperty =
        DependencyProperty.Register(nameof(PeerName), typeof(string), typeof(ConversationPanel), 
            new PropertyMetadata(string.Empty));

    public string PeerName
    {
        get => (string)GetValue(PeerNameProperty);
        set => SetValue(PeerNameProperty, value);
    }

    /// <summary>
    /// Status of the peer being chatted with
    /// </summary>
    public static readonly DependencyProperty PeerStatusProperty =
        DependencyProperty.Register(nameof(PeerStatus), typeof(string), typeof(ConversationPanel), 
            new PropertyMetadata(string.Empty));

    public string PeerStatus
    {
        get => (string)GetValue(PeerStatusProperty);
        set => SetValue(PeerStatusProperty, value);
    }

    public ConversationPanel()
    {
        this.InitializeComponent();
        Messages = new ObservableCollection<MessageInfo>();
    }

    /// <summary>
    /// Adds a message to the conversation
    /// </summary>
    /// <param name="message">Message to add</param>
    public void AddMessage(MessageInfo message)
    {
        Messages.Add(message);
        
        // Scroll to bottom
        MessagesScrollViewer.DispatcherQueue.TryEnqueue(() =>
        {
            MessagesScrollViewer.ScrollToVerticalOffset(MessagesScrollViewer.ScrollableHeight);
        });
    }

    /// <summary>
    /// Clears all messages from the conversation
    /// </summary>
    public void ClearMessages()
    {
        Messages.Clear();
    }

    /// <summary>
    /// Sets the peer information for this conversation
    /// </summary>
    /// <param name="name">Peer name</param>
    /// <param name="status">Peer status</param>
    public void SetPeer(string name, string status)
    {
        PeerName = name;
        PeerStatus = status;
    }

    private void SendButton_Click(object sender, RoutedEventArgs e)
    {
        SendMessage();
    }

    private void MessageTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter && !e.KeyStatus.IsMenuKeyDown)
        {
            e.Handled = true;
            SendMessage();
        }
    }

    private void SendMessage()
    {
        if (!string.IsNullOrWhiteSpace(CurrentMessage))
        {
            var messageText = CurrentMessage.Trim();
            CurrentMessage = string.Empty; // Clear input
            MessageSent?.Invoke(this, messageText);
        }
    }

    private void EmojiButton_Click(object sender, RoutedEventArgs e)
    {
        EmojiTeachingTip.IsOpen = true;
    }

    private void EmojiPicker_EmojiSelected(object? sender, string emoji)
    {
        CurrentMessage += emoji;
        MessageTextBox.Focus(FocusState.Programmatic);
        MessageTextBox.SelectionStart = CurrentMessage.Length;
        EmojiTeachingTip.IsOpen = false;
    }

    private async void FileButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add("*"); // Allow all file types
            
            // Get the window handle for the picker
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                FileSent?.Invoke(this, file);
            }
        }
        catch (Exception ex)
        {
            // Handle file picker error
            System.Diagnostics.Debug.WriteLine($"File picker error: {ex.Message}");
        }
    }

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
}