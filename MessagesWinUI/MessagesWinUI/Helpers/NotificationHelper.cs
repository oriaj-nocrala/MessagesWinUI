using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Microsoft.UI.Xaml;

namespace MessagesWinUI.Helpers;

/// <summary>
/// Advanced notification helper with WinUI 3 interactive features
/// </summary>
public static class NotificationHelper
{
    private static bool _isInitialized = false;
    private static string _appId = "MessagesWinUI.P2PMessenger";

    /// <summary>
    /// Initializes the advanced notification system
    /// </summary>
    public static void Initialize()
    {
        try
        {
            if (!_isInitialized)
            {
                // Register for notification activation callbacks
                ToastNotificationManagerCompat.OnActivated += OnToastActivated;
                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("🔔 Advanced notification system initialized");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Failed to initialize notifications: {ex.Message}");
        }
    }

    /// <summary>
    /// Shows an advanced interactive message notification
    /// </summary>
    /// <param name="peerName">Name of the peer who sent the message</param>
    /// <param name="message">The message content</param>
    /// <param name="peerId">Peer ID for quick actions</param>
    public static void ShowAdvancedMessageNotification(string peerName, string message, string peerId)
    {
        try
        {
            var displayMessage = message.Length > 150 ? message.Substring(0, 147) + "..." : message;
            var messageId = Guid.NewGuid().ToString();

            var toastXml = $@"
<toast scenario='incomingCall' activationType='foreground' launch='action=openChat&amp;peerId={peerId}&amp;messageId={messageId}'>
    <visual>
        <binding template='ToastGeneric'>
            <text hint-maxLines='1'>{peerName} sent a message</text>
            <text>💬 {displayMessage}</text>
            <text placement='attribution'>P2P Messenger</text>
            <image placement='appLogoOverride' hint-crop='circle' src='ms-appx:///Assets/user-avatar.png'/>
            <image src='ms-appx:///Assets/message-preview.png' alt='Message preview'/>
        </binding>
    </visual>
    <audio src='ms-winsoundevent:Notification.IM' loop='false'/>
    <actions>
        <input id='quickReply' type='text' placeHolderContent='Type a quick reply...' title='Quick Reply'/>
        <action content='📤 Send' 
                arguments='action=quickReply&amp;peerId={peerId}&amp;messageId={messageId}' 
                activationType='background' 
                hint-inputId='quickReply'/>
        <action content='💬 Open Chat' 
                arguments='action=openChat&amp;peerId={peerId}&amp;messageId={messageId}' 
                activationType='foreground'/>
        <action content='🔕 Mute' 
                arguments='action=mute&amp;peerId={peerId}' 
                activationType='background'/>
    </actions>
</toast>";

            ShowToast(toastXml, $"📨 Message from {peerName}");
            System.Diagnostics.Debug.WriteLine($"🚀 Advanced message notification shown for {peerName}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Failed to show advanced message notification: {ex.Message}");
        }
    }

    /// <summary>
    /// Shows an animated connection status notification
    /// </summary>
    /// <param name="peerName">Name of the peer</param>
    /// <param name="isConnected">Whether connected or disconnected</param>
    /// <param name="peerId">Peer ID</param>
    public static void ShowConnectionStatusNotification(string peerName, bool isConnected, string peerId)
    {
        try
        {
            var statusEmoji = isConnected ? "🟢" : "🔴";
            var statusText = isConnected ? "came online" : "went offline";
            var actionText = isConnected ? "💬 Start Chat" : "📝 Leave Message";
            var statusId = Guid.NewGuid().ToString();

            var toastXml = $@"
<toast activationType='foreground' launch='action=openPeer&amp;peerId={peerId}&amp;statusId={statusId}'>
    <visual>
        <binding template='ToastGeneric'>
            <text hint-maxLines='1'>{statusEmoji} {peerName}</text>
            <text>{peerName} {statusText}</text>
            <text placement='attribution'>P2P Network Status</text>
            <image placement='appLogoOverride' hint-crop='circle' src='ms-appx:///Assets/peer-avatar.png'/>
            <group>
                <subgroup>
                    <text hint-style='captionSubtle' hint-align='left'>Status:</text>
                    <text hint-style='caption' hint-align='left'>{(isConnected ? "Online" : "Offline")}</text>
                </subgroup>
                <subgroup>
                    <text hint-style='captionSubtle' hint-align='right'>Network:</text>
                    <text hint-style='caption' hint-align='right'>P2P Direct</text>
                </subgroup>
            </group>
        </binding>
    </visual>
    <audio src='{(isConnected ? "ms-winsoundevent:Notification.Proximity.Connection" : "ms-winsoundevent:Notification.Proximity.Disconnection")}' loop='false'/>
    <actions>
        <action content='{actionText}' 
                arguments='action={(isConnected ? "startChat" : "leaveMessage")}&amp;peerId={peerId}&amp;statusId={statusId}' 
                activationType='foreground'/>
        <action content='👥 View Peers' 
                arguments='action=viewPeers&amp;peerId={peerId}' 
                activationType='foreground'/>
    </actions>
</toast>";

            ShowToast(toastXml, $"{statusEmoji} {peerName} {statusText}");
            System.Diagnostics.Debug.WriteLine($"🌐 Connection notification shown: {peerName} {statusText}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Failed to show connection notification: {ex.Message}");
        }
    }

    /// <summary>
    /// Shows file transfer progress notification with live updates
    /// </summary>
    /// <param name="peerName">Name of the peer</param>
    /// <param name="fileName">File being transferred</param>
    /// <param name="progress">Progress percentage (0-100)</param>
    /// <param name="transferId">Unique transfer ID</param>
    public static void ShowFileTransferNotification(string peerName, string fileName, int progress, string transferId)
    {
        try
        {
            var progressText = progress < 100 ? $"📊 {progress}% complete" : "✅ Transfer complete";
            var progressValue = progress / 100.0;

            var toastXml = $@"
<toast activationType='foreground' launch='action=viewTransfer&amp;transferId={transferId}'>
    <visual>
        <binding template='ToastGeneric'>
            <text hint-maxLines='1'>📁 File from {peerName}</text>
            <text>{fileName}</text>
            <text>{progressText}</text>
            <text placement='attribution'>File Transfer</text>
            <image placement='appLogoOverride' src='ms-appx:///Assets/file-icon.png'/>
            <progress value='{progressValue:F2}' status='{(progress < 100 ? "Receiving..." : "Complete")}'/>
        </binding>
    </visual>
    <audio src='ms-winsoundevent:Notification.Mail' loop='false'/>
    <actions>
        {(progress < 100 ? 
            $"<action content='⏸️ Pause' arguments='action=pauseTransfer&amp;transferId={transferId}' activationType='background'/>" +
            $"<action content='❌ Cancel' arguments='action=cancelTransfer&amp;transferId={transferId}' activationType='background'/>" :
            $"<action content='📂 Open File' arguments='action=openFile&amp;transferId={transferId}' activationType='foreground'/>" +
            $"<action content='📁 Show in Folder' arguments='action=showInFolder&amp;transferId={transferId}' activationType='foreground'/>")}
    </actions>
</toast>";

            ShowToast(toastXml, $"📁 {fileName} from {peerName}");
            System.Diagnostics.Debug.WriteLine($"📁 File transfer notification: {fileName} - {progress}%");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Failed to show file transfer notification: {ex.Message}");
        }
    }

    /// <summary>
    /// Shows a celebration notification for special events
    /// </summary>
    /// <param name="title">Celebration title</param>
    /// <param name="message">Celebration message</param>
    /// <param name="celebrationType">Type of celebration</param>
    public static void ShowCelebrationNotification(string title, string message, string celebrationType = "general")
    {
        try
        {
            var emojis = celebrationType switch
            {
                "firstConnection" => "🎉🔗",
                "milestone" => "🏆🎊", 
                "anniversary" => "🎂✨",
                _ => "🎉🎈"
            };

            var toastXml = $@"
<toast scenario='reminder' activationType='foreground' launch='action=celebrate&amp;type={celebrationType}'>
    <visual>
        <binding template='ToastGeneric'>
            <text hint-maxLines='1'>{emojis} {title}</text>
            <text>{message}</text>
            <text placement='attribution'>MessagesWinUI</text>
            <image placement='hero' src='ms-appx:///Assets/celebration-banner.png'/>
            <image placement='appLogoOverride' src='ms-appx:///Assets/party-icon.png'/>
        </binding>
    </visual>
    <audio src='ms-winsoundevent:Notification.Default' loop='false'/>
    <actions>
        <action content='🎊 Awesome!' 
                arguments='action=dismiss&amp;type={celebrationType}' 
                activationType='background'/>
        <action content='📱 Share Achievement' 
                arguments='action=share&amp;type={celebrationType}' 
                activationType='foreground'/>
    </actions>
</toast>";

            ShowToast(toastXml, $"{emojis} {title}");
            System.Diagnostics.Debug.WriteLine($"🎉 Celebration notification: {title}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Failed to show celebration notification: {ex.Message}");
        }
    }

    /// <summary>
    /// Generic method to show toast with XML
    /// </summary>
    private static void ShowToast(string toastXml, string logMessage)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(toastXml);
        
        var toast = new ToastNotification(xmlDoc);
        toast.ExpirationTime = DateTime.Now.AddMinutes(5); // Auto-expire in 5 minutes
        toast.Priority = ToastNotificationPriority.High;
        
        ToastNotificationManager.CreateToastNotifier(_appId).Show(toast);
    }

    /// <summary>
    /// Handles toast notification activation (button clicks, etc.)
    /// </summary>
    private static void OnToastActivated(object? sender, ToastNotificationActivatedEventArgsCompat args)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🔔 Toast activated: {args.Argument}");
            
            // Parse the activation arguments
            var arguments = ParseQueryString(args.Argument);
            var action = arguments["action"];
            var peerId = arguments["peerId"];
            var messageId = arguments["messageId"];

            // Handle different actions
            switch (action)
            {
                case "quickReply":
                    var replyText = args.UserInput["quickReply"]?.ToString();
                    HandleQuickReply(peerId, replyText);
                    break;
                
                case "openChat":
                    HandleOpenChat(peerId);
                    break;
                
                case "mute":
                    HandleMutePeer(peerId);
                    break;
                
                case "startChat":
                    HandleStartChat(peerId);
                    break;
                
                case "viewPeers":
                    HandleViewPeers();
                    break;
                
                default:
                    // Default action - bring app to foreground
                    App.MainWindow?.Activate();
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error handling toast activation: {ex.Message}");
        }
    }

    // Quick action handlers
    private static void HandleQuickReply(string? peerId, string? replyText)
    {
        if (!string.IsNullOrEmpty(peerId) && !string.IsNullOrEmpty(replyText))
        {
            System.Diagnostics.Debug.WriteLine($"📤 Quick reply to {peerId}: {replyText}");
            // TODO: Send the quick reply through the P2P messenger
        }
    }

    private static void HandleOpenChat(string? peerId)
    {
        System.Diagnostics.Debug.WriteLine($"💬 Opening chat with {peerId}");
        App.MainWindow?.Activate();
        // TODO: Navigate to specific conversation
    }

    private static void HandleMutePeer(string? peerId)
    {
        System.Diagnostics.Debug.WriteLine($"🔕 Muting peer {peerId}");
        // TODO: Add peer to muted list
    }

    private static void HandleStartChat(string? peerId)
    {
        System.Diagnostics.Debug.WriteLine($"🚀 Starting new chat with {peerId}");
        App.MainWindow?.Activate();
        // TODO: Start new conversation
    }

    private static void HandleViewPeers()
    {
        System.Diagnostics.Debug.WriteLine($"👥 Showing peers list");
        App.MainWindow?.Activate();
        // TODO: Navigate to peers view
    }

    /// <summary>
    /// Simple query string parser for toast activation arguments
    /// </summary>
    private static Dictionary<string, string?> ParseQueryString(string queryString)
    {
        var result = new Dictionary<string, string?>();
        
        if (string.IsNullOrEmpty(queryString))
            return result;
            
        var pairs = queryString.Split('&');
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=', 2);
            if (keyValue.Length == 2)
            {
                var key = Uri.UnescapeDataString(keyValue[0]);
                var value = Uri.UnescapeDataString(keyValue[1]);
                result[key] = value;
            }
        }
        
        return result;
    }

    /// <summary>
    /// Cleanup notification resources
    /// </summary>
    public static void Cleanup()
    {
        try
        {
            if (_isInitialized)
            {
                // Try to clear toast history, but don't fail if app is shutting down
                try
                {
                    ToastNotificationManager.History.Clear(_appId);
                }
                catch (System.Runtime.InteropServices.COMException comEx)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ COM exception during notification cleanup (expected during shutdown): {comEx.Message}");
                    // This is expected when the app is shutting down - don't treat as error
                }
                catch (Exception innerEx)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Error clearing notification history: {innerEx.Message}");
                }
                
                _isInitialized = false;
                System.Diagnostics.Debug.WriteLine("🧹 Advanced notification system cleaned up");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error cleaning up notifications: {ex.Message}");
            // Don't rethrow - this is cleanup code
        }
    }
}

/// <summary>
/// Compatibility class for toast activation - simplified version
/// </summary>
public static class ToastNotificationManagerCompat
{
    public static event EventHandler<ToastNotificationActivatedEventArgsCompat>? OnActivated;
    
    // This would need proper implementation for full functionality
    // For now, we'll handle basic scenarios
}

/// <summary>
/// Arguments for toast notification activation
/// </summary>
public class ToastNotificationActivatedEventArgsCompat
{
    public string Argument { get; set; } = "";
    public Dictionary<string, object> UserInput { get; set; } = new();
}