using System;

namespace MessagesWinUI.Interop;

/// <summary>
/// Event types for P2P messenger events
/// </summary>
public enum P2PEventType
{
    PeerDiscovered = 1,
    PeerConnected = 2,
    PeerDisconnected = 3,
    MessageReceived = 4,
    FileReceived = 5,
    Error = 6
}

/// <summary>
/// Base class for P2P event arguments
/// </summary>
public abstract class P2PEventArgs : EventArgs
{
    public P2PEventType EventType { get; }
    public DateTime Timestamp { get; }

    protected P2PEventArgs(P2PEventType eventType)
    {
        EventType = eventType;
        Timestamp = DateTime.Now;
    }
}

/// <summary>
/// Event args for peer discovery events
/// </summary>
public class PeerDiscoveredEventArgs : P2PEventArgs
{
    public string PeerId { get; }
    public string PeerName { get; }

    public PeerDiscoveredEventArgs(string peerId, string peerName) 
        : base(P2PEventType.PeerDiscovered)
    {
        PeerId = peerId ?? throw new ArgumentNullException(nameof(peerId));
        PeerName = peerName ?? throw new ArgumentNullException(nameof(peerName));
    }
}

/// <summary>
/// Event args for peer connected events
/// </summary>
public class PeerConnectedEventArgs : P2PEventArgs
{
    public string PeerId { get; }
    public string PeerName { get; }

    public PeerConnectedEventArgs(string peerId, string peerName) 
        : base(P2PEventType.PeerConnected)
    {
        PeerId = peerId ?? throw new ArgumentNullException(nameof(peerId));
        PeerName = peerName ?? throw new ArgumentNullException(nameof(peerName));
    }
}

/// <summary>
/// Event args for peer disconnected events
/// </summary>
public class PeerDisconnectedEventArgs : P2PEventArgs
{
    public string PeerId { get; }
    public string PeerName { get; }

    public PeerDisconnectedEventArgs(string peerId, string peerName) 
        : base(P2PEventType.PeerDisconnected)
    {
        PeerId = peerId ?? throw new ArgumentNullException(nameof(peerId));
        PeerName = peerName ?? throw new ArgumentNullException(nameof(peerName));
    }
}

/// <summary>
/// Event args for file received events
/// </summary>
public class FileReceivedEventArgs : P2PEventArgs
{
    public string SenderId { get; }
    public string SenderName { get; }
    public string FileName { get; }
    public string FilePath { get; }

    public FileReceivedEventArgs(string senderId, string senderName, string fileName, string filePath) 
        : base(P2PEventType.FileReceived)
    {
        SenderId = senderId ?? throw new ArgumentNullException(nameof(senderId));
        SenderName = senderName ?? throw new ArgumentNullException(nameof(senderName));
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }
}

/// <summary>
/// Event args for message received events
/// </summary>
public class MessageReceivedEventArgs : P2PEventArgs
{
    public string SenderId { get; }
    public string SenderName { get; }
    public string Message { get; }

    public MessageReceivedEventArgs(string senderId, string senderName, string message) 
        : base(P2PEventType.MessageReceived)
    {
        SenderId = senderId ?? throw new ArgumentNullException(nameof(senderId));
        SenderName = senderName ?? throw new ArgumentNullException(nameof(senderName));
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}

/// <summary>
/// Event args for error events
/// </summary>
public class ErrorEventArgs : P2PEventArgs
{
    public string ErrorMessage { get; }

    public ErrorEventArgs(string errorMessage) : base(P2PEventType.Error)
    {
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    }
}

/// <summary>
/// Exception thrown by P2P operations
/// </summary>
public class P2PException : Exception
{
    public int ErrorCode { get; }

    public P2PException(int errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public P2PException(int errorCode, string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public static P2PException FromErrorCode(int errorCode)
    {
        return errorCode switch
        {
            NativeMethods.FFI_ERROR_INVALID_HANDLE => new P2PException(errorCode, "Invalid handle"),
            NativeMethods.FFI_ERROR_INVALID_PARAMETER => new P2PException(errorCode, "Invalid parameter"),
            NativeMethods.FFI_ERROR_NETWORK => new P2PException(errorCode, "Network error"),
            NativeMethods.FFI_ERROR_RUNTIME => new P2PException(errorCode, "Runtime error"),
            _ => new P2PException(errorCode, $"Unknown error: {errorCode}")
        };
    }
}