using System;
using MessagesWinUI.Helpers;
using MessagesWinUI.ViewModels;

namespace MessagesWinUI.Models;

/// <summary>
/// Represents a peer in the P2P network with connection status and metadata
/// </summary>
public class PeerInfo : BaseViewModel
{
    private bool _isConnected = false;
    private string _status = "";
    private string _id = string.Empty;
    private string _name = string.Empty;
    private DateTime _lastSeen = DateTime.Now;

    /// <summary>
    /// Unique identifier for the peer
    /// </summary>
    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    /// <summary>
    /// Display name of the peer
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Last time this peer was seen/active
    /// </summary>
    public DateTime LastSeen
    {
        get => _lastSeen;
        set => SetProperty(ref _lastSeen, value);
    }

    /// <summary>
    /// Whether this peer is currently connected
    /// </summary>
    public bool IsConnected
    {
        get => _isConnected;
        set => SetProperty(ref _isConnected, value, UpdateStatus);
    }

    /// <summary>
    /// Localized status string for display
    /// </summary>
    public string Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    /// <summary>
    /// Time since last seen as a human-readable string
    /// </summary>
    public string LastSeenDisplay => GetLastSeenDisplay();

    /// <summary>
    /// Updates status based on connection state
    /// </summary>
    private void UpdateStatus()
    {
        Status = _isConnected 
            ? LocalizationHelper.GetString("Connected") 
            : LocalizationHelper.GetString("Discovered");
        
        OnPropertyChanged(nameof(LastSeenDisplay));
    }

    /// <summary>
    /// Gets human-readable last seen time
    /// </summary>
    private string GetLastSeenDisplay()
    {
        var timeSpan = DateTime.Now - LastSeen;
        
        if (timeSpan.TotalMinutes < 1)
            return LocalizationHelper.GetString("JustNow");
        else if (timeSpan.TotalMinutes < 60)
            return LocalizationHelper.GetString("MinutesAgo", new object[] { (int)timeSpan.TotalMinutes });
        else if (timeSpan.TotalHours < 24)
            return LocalizationHelper.GetString("HoursAgo", new object[] { (int)timeSpan.TotalHours });
        else
            return LastSeen.ToString("MMM dd, HH:mm");
    }

    /// <summary>
    /// Updates last seen time to now
    /// </summary>
    public void UpdateLastSeen()
    {
        LastSeen = DateTime.Now;
        OnPropertyChanged(nameof(LastSeenDisplay));
    }
}

/// <summary>
/// Represents a message in a conversation with metadata and display properties
/// </summary>
public class MessageInfo : BaseViewModel
{
    private string _senderName = string.Empty;
    private string _content = string.Empty;
    private DateTime _timestamp = DateTime.Now;
    private bool _isFromMe = false;
    private MessageType _messageType = MessageType.Text;

    /// <summary>
    /// Name of the message sender
    /// </summary>
    public string SenderName
    {
        get => _senderName;
        set => SetProperty(ref _senderName, value);
    }

    /// <summary>
    /// Message content/text
    /// </summary>
    public string Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    /// <summary>
    /// When the message was sent
    /// </summary>
    public DateTime Timestamp
    {
        get => _timestamp;
        set => SetProperty(ref _timestamp, value);
    }

    /// <summary>
    /// Whether this message was sent by the current user
    /// </summary>
    public bool IsFromMe
    {
        get => _isFromMe;
        set => SetProperty(ref _isFromMe, value);
    }

    /// <summary>
    /// Type of message (text, file, system, etc.)
    /// </summary>
    public MessageType MessageType
    {
        get => _messageType;
        set => SetProperty(ref _messageType, value);
    }

    /// <summary>
    /// Formatted timestamp for display
    /// </summary>
    public string TimeDisplay => Timestamp.ToString("HH:mm");

    /// <summary>
    /// Full timestamp for tooltips
    /// </summary>
    public string FullTimeDisplay => Timestamp.ToString("MMM dd, yyyy HH:mm:ss");

    /// <summary>
    /// Whether message is from another user (for styling)
    /// </summary>
    public bool IsFromOther => !IsFromMe;
}

/// <summary>
/// Types of messages supported
/// </summary>
public enum MessageType
{
    Text,
    File,
    Image,
    System,
    Error
}