using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MessagesWinUI.ViewModels;
using MessagesWinUI.Models;
using MessagesWinUI.Helpers;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Windows.UI.ViewManagement;
using Windows.Storage;
using System;
using System.Threading.Tasks;

namespace MessagesWinUI;

/// <summary>
/// Main window with MVVM architecture and declarative UI
/// </summary>
public sealed partial class MainWindow : Window
{
    /// <summary>
    /// Main ViewModel for this window
    /// </summary>
    public MainWindowViewModel ViewModel { get; }

    public MainWindow()
    {
        InitializeComponent();
        
        // Initialize ViewModel
        ViewModel = new MainWindowViewModel();
        
        // Configure title bar for theme support
        ConfigureTitleBar();
        
        // Initialize localization
        LocalizationHelper.Initialize();
        
        // Setup window events
        Closed += MainWindow_Closed;
        
        // Initialize the ViewModel asynchronously
        _ = InitializeAsync();
    }

    /// <summary>
    /// Initializes the application asynchronously
    /// </summary>
    private async Task InitializeAsync()
    {
        try
        {
            await ViewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to initialize application: {ex.Message}");
        }
    }

    #region UI Event Handlers (MVVM Bridge)

    /// <summary>
    /// Handles peer list item clicks to start conversations
    /// </summary>
    private void PeersListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is PeerInfo peer && peer.IsConnected)
        {
            ViewModel.StartConversationCommand.Execute(peer.Id);
        }
    }

    /// <summary>
    /// Handles conversation tab close requests
    /// </summary>
    private void ConversationTabView_TabCloseRequested(TabView _, TabViewTabCloseRequestedEventArgs args)
    {
        if (args.Item is ConversationViewModel conversation)
        {
            ViewModel.CloseConversationCommand.Execute(conversation.PeerId);
        }
    }

    /// <summary>
    /// Handles message sending from conversation panels
    /// </summary>
    private void ConversationPanel_MessageSent(object _, string message)
    {
        if (ViewModel.SelectedConversation != null)
        {
            ViewModel.SendMessageCommand.Execute((ViewModel.SelectedConversation.PeerId, message));
        }
    }

    /// <summary>
    /// Handles file sending from conversation panels
    /// </summary>
    private void ConversationPanel_FileSent(object _, StorageFile file)
    {
        if (ViewModel.SelectedConversation != null)
        {
            ViewModel.SendFileCommand.Execute((ViewModel.SelectedConversation.PeerId, file));
        }
    }

    /// <summary>
    /// Handles connect button clicks from DataTemplate
    /// </summary>
    private void ConnectButton_Click(object sender, RoutedEventArgs _)
    {
        if (sender is Button button && button.CommandParameter is string peerId)
        {
            ViewModel.ConnectToPeerCommand.Execute(peerId);
        }
    }

    #endregion

    #region Title Bar Configuration

    /// <summary>
    /// Configures title bar colors for dark/light theme support
    /// </summary>
    private void ConfigureTitleBar()
    {
        try
        {
            // Get the app window
            var windowHandle = WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow?.TitleBar != null && AppWindowTitleBar.IsCustomizationSupported())
            {
                var titleBar = appWindow.TitleBar;
                
                // Check system theme
                var uiSettings = new UISettings();
                var background = uiSettings.GetColorValue(UIColorType.Background);
                var isDarkTheme = background.R < 128; // Dark if background is closer to black than white
                
                if (isDarkTheme)
                {
                    // Windows 11 Dark Theme colors (proper gray, not black)
                    titleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 32, 32, 32);      // #202020 - Dark gray
                    titleBar.ForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);  // White text
                    titleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 24, 24, 24);  // Slightly darker when inactive
                    titleBar.InactiveForegroundColor = Windows.UI.Color.FromArgb(255, 160, 160, 160); // Gray text when inactive
                    
                    // Button colors - gray background with white icons
                    titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 32, 32, 32);  // Same as title bar background
                    titleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);  // White icons
                    titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 51, 51, 51);  // Slightly lighter on hover
                    titleBar.ButtonHoverForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);  // White icons on hover
                    titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(255, 77, 77, 77);  // Light gray pressed
                    titleBar.ButtonPressedForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);  // White icons when pressed
                    
                    // Inactive button colors
                    titleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 24, 24, 24);  // Same as inactive title bar
                    titleBar.ButtonInactiveForegroundColor = Windows.UI.Color.FromArgb(255, 160, 160, 160);  // Dimmed white when inactive
                }
                else
                {
                    // Light theme - use system defaults by setting to transparent to let system handle it
                    titleBar.BackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                    titleBar.ForegroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                    titleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                    titleBar.InactiveForegroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                    
                    // Let system handle button colors in light mode
                    titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                    titleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                    titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                    titleBar.ButtonHoverForegroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                    titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                    titleBar.ButtonPressedForegroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                    titleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                    titleBar.ButtonInactiveForegroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                }
                
                System.Diagnostics.Debug.WriteLine($"Title bar configured for {(isDarkTheme ? "dark" : "light")} theme");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Title bar customization not supported or failed to get title bar");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to configure title bar: {ex.Message}");
        }
    }

    #endregion

    #region Window Lifecycle

    /// <summary>
    /// Handles window closing to clean up resources
    /// </summary>
    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        ViewModel?.Dispose();
    }

    #endregion
}