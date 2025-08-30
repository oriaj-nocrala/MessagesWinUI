using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using MessagesWinUI.Models;

namespace MessagesWinUI.Converters;

/// <summary>
/// Converter for message bubble styles based on sender
/// </summary>
public class MessageBubbleStyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isFromMe)
        {
            return Application.Current.Resources[isFromMe ? "MyMessageBubbleStyle" : "OtherMessageBubbleStyle"];
        }
        return Application.Current.Resources["OtherMessageBubbleStyle"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter for message text color based on sender
/// </summary>
public class MessageForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isFromMe && isFromMe)
        {
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); // White for my messages
        }
        
        return (Application.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush) ?? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter for message timestamp color based on sender
/// </summary>
public class MessageTimeForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isFromMe && isFromMe)
        {
            return new SolidColorBrush(Windows.UI.Color.FromArgb(180, 255, 255, 255)); // Semi-transparent white
        }
        
        return (Application.Current.Resources["TextFillColorSecondaryBrush"] as SolidColorBrush) ?? new SolidColorBrush(Windows.UI.Color.FromArgb(180, 0, 0, 0));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Template selector for different message types
/// </summary>
public class MessageTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextMessageTemplate { get; set; }
    public DataTemplate? SystemMessageTemplate { get; set; }
    public DataTemplate? FileMessageTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is MessageInfo message)
        {
            return message.MessageType switch
            {
                MessageType.Text => TextMessageTemplate,
                MessageType.System => SystemMessageTemplate,
                MessageType.File or MessageType.Image => FileMessageTemplate,
                _ => TextMessageTemplate
            } ?? TextMessageTemplate!;
        }
        
        return TextMessageTemplate!;
    }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        return SelectTemplateCore(item);
    }
}