using System;
using System.Windows.Input;

namespace MessagesWinUI.Commands;

/// <summary>
/// Generic command implementation for MVVM pattern
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    /// <summary>
    /// Initializes a new instance of RelayCommand
    /// </summary>
    /// <param name="execute">Action to execute when command is invoked</param>
    /// <param name="canExecute">Function to determine if command can execute</param>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Determines whether the command can execute
    /// </summary>
    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    /// <summary>
    /// Executes the command
    /// </summary>
    public void Execute(object? parameter) => _execute();

    /// <summary>
    /// Raises CanExecuteChanged event to update UI command state
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// Generic command implementation with parameter support
/// </summary>
/// <typeparam name="T">Parameter type</typeparam>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Predicate<T?>? _canExecute;

    /// <summary>
    /// Initializes a new instance of RelayCommand with parameter
    /// </summary>
    /// <param name="execute">Action to execute when command is invoked</param>
    /// <param name="canExecute">Function to determine if command can execute</param>
    public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Determines whether the command can execute
    /// </summary>
    public bool CanExecute(object? parameter)
    {
        if (_canExecute == null)
            return true;

        if (parameter is T typedParameter)
            return _canExecute(typedParameter);

        if (parameter == null && default(T) == null)
            return _canExecute(default(T));

        return false;
    }

    /// <summary>
    /// Executes the command
    /// </summary>
    public void Execute(object? parameter)
    {
        if (parameter is T typedParameter)
        {
            _execute(typedParameter);
        }
        else if (parameter == null && default(T) == null)
        {
            _execute(default(T));
        }
    }

    /// <summary>
    /// Raises CanExecuteChanged event to update UI command state
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}