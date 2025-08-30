using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MessagesWinUI.ViewModels;

/// <summary>
/// Base class for ViewModels implementing INotifyPropertyChanged with helper methods
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises PropertyChanged event for the specified property
    /// </summary>
    /// <param name="propertyName">Property name (auto-filled by CallerMemberName)</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets property value and raises PropertyChanged if value changed
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    /// <param name="field">Backing field reference</param>
    /// <param name="value">New value</param>
    /// <param name="propertyName">Property name (auto-filled)</param>
    /// <returns>True if value changed</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Sets property value, raises PropertyChanged, and executes callback if value changed
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, Action onChanged, [CallerMemberName] string? propertyName = null)
    {
        if (SetProperty(ref field, value, propertyName))
        {
            onChanged?.Invoke();
            return true;
        }
        return false;
    }
}