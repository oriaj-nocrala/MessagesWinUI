using System;
using System.Collections.ObjectModel;
using System.Linq;
using MessagesWinUI.Models;

namespace MessagesWinUI.ViewModels;

/// <summary>
/// ViewModel for a conversation with a specific peer
/// </summary>
public class ConversationViewModel : BaseViewModel
{
    private string _peerName;
    private string _peerStatus;
    private MessageInfo? _lastMessage;
    private int _unreadCount;

    /// <summary>
    /// Unique identifier for the peer in this conversation
    /// </summary>
    public string PeerId { get; }

    /// <summary>
    /// Display name of the peer
    /// </summary>
    public string PeerName
    {
        get => _peerName;
        set => SetProperty(ref _peerName, value);
    }

    /// <summary>
    /// Current status of the peer (Connected, Discovered, etc.)
    /// </summary>
    public string PeerStatus
    {
        get => _peerStatus;
        set => SetProperty(ref _peerStatus, value);
    }

    /// <summary>
    /// Collection of messages in this conversation
    /// </summary>
    public ObservableCollection<MessageInfo> Messages { get; }

    /// <summary>
    /// Last message in the conversation (for preview)
    /// </summary>
    public MessageInfo? LastMessage
    {
        get => _lastMessage;
        private set => SetProperty(ref _lastMessage, value);
    }

    /// <summary>
    /// Number of unread messages
    /// </summary>
    public int UnreadCount
    {
        get => _unreadCount;
        set => SetProperty(ref _unreadCount, value, () => OnPropertyChanged(nameof(HasUnreadMessages)));
    }

    /// <summary>
    /// Whether this conversation has unread messages
    /// </summary>
    public bool HasUnreadMessages => UnreadCount > 0;

    /// <summary>
    /// Display text for the conversation tab header
    /// </summary>
    public string TabHeader => HasUnreadMessages ? $"{PeerName} ({UnreadCount})" : PeerName;

    /// <summary>
    /// Preview text of the last message
    /// </summary>
    public string LastMessagePreview 
    { 
        get
        {
            if (LastMessage == null)
                return "No messages";
                
            var preview = LastMessage.MessageType switch
            {
                MessageType.File => $"📁 {LastMessage.Content}",
                MessageType.Image => $"🖼️ {LastMessage.Content}",
                MessageType.System => LastMessage.Content,
                _ => LastMessage.Content
            };
            
            return preview.Length > 50 ? preview.Substring(0, 47) + "..." : preview;
        }
    }

    /// <summary>
    /// Time of the last message for display
    /// </summary>
    public string LastMessageTime => LastMessage?.TimeDisplay ?? "";

    public ConversationViewModel(string peerId, string peerName)
    {
        PeerId = peerId ?? throw new ArgumentNullException(nameof(peerId));
        _peerName = peerName ?? throw new ArgumentNullException(nameof(peerName));
        _peerStatus = "Discovered";
        
        Messages = new ObservableCollection<MessageInfo>();
        Messages.CollectionChanged += Messages_CollectionChanged;
    }

    /// <summary>
    /// Adds a message to this conversation
    /// </summary>
    /// <param name="message">Message to add</param>
    public void AddMessage(MessageInfo message)
    {
        Messages.Add(message);
        
        if (!message.IsFromMe)
        {
            UnreadCount++;
        }
    }

    /// <summary>
    /// Marks all messages as read
    /// </summary>
    public void MarkAsRead()
    {
        UnreadCount = 0;
    }

    /// <summary>
    /// Clears all messages from this conversation
    /// </summary>
    public void ClearMessages()
    {
        Messages.Clear();
        LastMessage = null;
        UnreadCount = 0;
    }

    /// <summary>
    /// Updates the peer status
    /// </summary>
    /// <param name="isConnected">Whether peer is connected</param>
    public void UpdatePeerStatus(bool isConnected)
    {
        PeerStatus = isConnected ? "Connected" : "Discovered";
    }

    private void Messages_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        // Update last message when messages change
        LastMessage = Messages.LastOrDefault();
        
        // Update property notifications for UI
        OnPropertyChanged(nameof(LastMessagePreview));
        OnPropertyChanged(nameof(LastMessageTime));
        OnPropertyChanged(nameof(TabHeader));
    }
}