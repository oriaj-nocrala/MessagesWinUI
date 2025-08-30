using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MessagesWinUI.Controls;

/// <summary>
/// Professional emoji picker control with categories and search
/// </summary>
public sealed partial class EmojiPicker : UserControl
{
    /// <summary>
    /// Event fired when an emoji is selected
    /// </summary>
    public event EventHandler<string>? EmojiSelected;

    /// <summary>
    /// Collection of emojis to display
    /// </summary>
    public ObservableCollection<string> Emojis { get; }

    public EmojiPicker()
    {
        this.InitializeComponent();
        
        // Initialize with common emojis organized by category
        Emojis = new ObservableCollection<string>
        {
            // Smileys & Emotion
            "😀", "😃", "😄", "😁", "😆", "😅", "😂", "🤣", "😊", "😇",
            "🙂", "🙃", "😉", "😌", "😍", "🥰", "😘", "😗", "😙", "😚",
            "😋", "😛", "😝", "😜", "🤪", "🤨", "🧐", "🤓", "😎", "🤩",
            
            // Gestures & Body Parts  
            "👍", "👎", "👌", "✌️", "🤞", "🤟", "🤘", "🤙", "👈", "👉",
            "👆", "🖕", "👇", "☝️", "👋", "🤚", "🖐️", "✋", "🖖", "👏",
            
            // Hearts & Symbols
            "❤️", "🧡", "💛", "💚", "💙", "💜", "🖤", "🤍", "🤎", "💔",
            "❣️", "💕", "💞", "💓", "💗", "💖", "💘", "💝", "💟", "♥️",
            
            // Activities & Objects
            "⚽", "🏀", "🏈", "⚾", "🥎", "🎾", "🏐", "🏉", "🎱", "🪀",
            "🏓", "🏸", "🏒", "🏑", "🥍", "🏏", "🪃", "🥅", "⛳", "🪁"
        };
        
        EmojiRepeater.ItemsSource = Emojis;
    }

    /// <summary>
    /// Handles emoji button clicks
    /// </summary>
    private void EmojiButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string emoji)
        {
            EmojiSelected?.Invoke(this, emoji);
        }
    }

    /// <summary>
    /// Adds a category of emojis
    /// </summary>
    /// <param name="emojis">Emojis to add</param>
    public void AddEmojiCategory(IEnumerable<string> emojis)
    {
        foreach (var emoji in emojis)
        {
            Emojis.Add(emoji);
        }
    }

    /// <summary>
    /// Clears and sets new emoji collection
    /// </summary>
    /// <param name="emojis">New emojis to display</param>
    public void SetEmojis(IEnumerable<string> emojis)
    {
        Emojis.Clear();
        foreach (var emoji in emojis)
        {
            Emojis.Add(emoji);
        }
    }
}