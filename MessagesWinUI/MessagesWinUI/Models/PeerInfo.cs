using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MessagesWinUI.Models;

public class PeerInfo : INotifyPropertyChanged
{
    private bool _isConnected = false;
    private string _status = "Discovered";

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            if (_isConnected != value)
            {
                _isConnected = value;
                Status = value ? "Connected" : "Discovered";
                OnPropertyChanged();
                OnPropertyChanged(nameof(Status));
            }
        }
    }

    public string Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class MessageInfo
{
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public bool IsFromMe { get; set; } = false;
}