# 💬 MessagesWinUI - BRUTAL WinUI 3 Edition 🔥

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![WinUI](https://img.shields.io/badge/WinUI-3.0-BRUTAL-red)](https://docs.microsoft.com/en-us/windows/apps/winui/)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%2F11-green)](https://www.microsoft.com/windows/)
[![License](https://img.shields.io/badge/License-MIT-yellow)](LICENSE)
[![Features](https://img.shields.io/badge/Features-EPIC-orange)](README.md)

A **BRUTAL** modern **peer-to-peer messaging application** built with **WinUI 3** and **Rust**, featuring an **EPIC custom title bar**, **advanced interactive notifications**, and **professional MVVM architecture** that pushes the limits of what WinUI 3 can do! 🚀

## 🔥 BRUTAL Features

### 🎨 **EPIC Custom Title Bar & Advanced UI**
- **🔥 BRUTAL Custom Title Bar** - Draggable with app branding, live connection status, audio visualizer, theme toggle
- **🌈 Dynamic Theme System** - Smooth transitions, automatic title bar color adaptation  
- **💎 DesktopAcrylicBackdrop** - Professional glass morphism effects
- **⚡ Advanced Visual Effects** - Composition effects, reveal effects, particle systems ready
- **📱 Responsive Layout System** - Adaptive UI that responds to window size
- **🎬 Epic Animations** - Entrance effects, theme change transitions

### 🔔 **Interactive Toast Notifications**
- **💬 Advanced Message Notifications** - Quick reply, mute, open chat actions
- **🌐 Connection Status Notifications** - Visual feedback for peer connections/disconnections
- **📁 File Transfer Progress** - Live progress bars with pause/cancel/open actions
- **🎉 Celebration Notifications** - Special events, theme changes, party mode (F12)

### 🎮 **Advanced Input & Interaction**
- **⌨️ Epic Keyboard Shortcuts** - Ctrl+T (theme), Ctrl+D (discover), F12 (party mode)
- **👆 Touch Gesture Support** - Swipe detection, manipulation events ready
- **🖊️ Pen Input Support** - Surface device compatibility, handwriting ready
- **🎯 Multi-Device Input** - Mouse, touch, pen unified handling

### 🔗 **P2P Networking & Performance**
- **🚀 Zero-server architecture** - Direct peer-to-peer communication
- **⚡ High-performance Rust backend** - ArchSockRust integration
- **🌐 Real-time messaging** with instant delivery
- **📁 Advanced file transfer** with live progress notifications
- **👥 Multi-peer support** - Connect to multiple friends simultaneously
- **🧹 Safe disposal patterns** - No COM exceptions, clean shutdown

### 💬 **Professional Chat Experience**  
- **🎨 Professional UserControls** - ConversationPanel, EmojiPicker with MVVM
- **💾 Message persistence** - Conversations saved per peer
- **📜 Auto-scroll** to latest messages with smooth animations
- **⌨️ Epic shortcuts** - Enter-to-send, Ctrl+T theme toggle
- **📊 Live connection indicators** in custom title bar
- **😀 Advanced emoji support** with TeachingTip picker
- **🔔 Interactive notifications** - Reply directly from Windows notifications

### 🚀 **BRUTAL Performance & Architecture**
- **🏗️ Professional MVVM** - Separation of concerns, declarative UI
- **🧵 Thread-safe operations** - Proper DispatcherQueue usage
- **🎯 Event-driven architecture** - PropertyChanged reactive UI
- **🛡️ Defensive programming** - COM exception handling, safe disposal
- **📱 Responsive design** - Adaptive layouts for different window sizes
- **🌍 Full localization** - English/Spanish with .resw files

## 🏗️ BRUTAL Architecture

### 🔥 Advanced WinUI 3 + Rust Stack

```
┌─────────────────────────┐
│  🔥 EPIC Custom Title Bar │  ← Draggable, live status, audio viz, theme toggle
├─────────────────────────┤
│  🎨 WinUI 3 BRUTAL UI    │  ← DesktopAcrylicBackdrop, animations, effects
├─────────────────────────┤
│  🔔 Interactive Toasts   │  ← Quick reply, progress, celebrations
├─────────────────────────┤
│  🏗️ Professional MVVM    │  ← ViewModels, Commands, reactive data binding
├─────────────────────────┤
│  🛡️ Safe Interop Layer   │  ← COM exception handling, thread-safe P/Invoke
├─────────────────────────┤
│  ⚡ Rust FFI Bridge      │  ← High-performance Foreign Function Interface
├─────────────────────────┤
│  🚀 ArchSockRust Core    │  ← Blazing-fast P2P networking engine
└─────────────────────────┘
```

### 🎯 BRUTAL WinUI 3 Features Integration

```
🔥 Custom Title Bar
├── 🎨 App branding with "BRUTAL" badge
├── 🌐 Live connection status indicator  
├── 🎵 Audio visualizer (dancing rectangles)
├── 🌈 Theme toggle with celebrations
└── 🖱️ Draggable window regions

🔔 Advanced Notifications  
├── 💬 Message notifications → Quick reply, mute, open
├── 🌐 Connection notifications → Visual status feedback
├── 📁 File transfer → Live progress, pause/cancel
└── 🎉 Celebrations → Theme changes, special events

🎮 Epic Input System
├── ⌨️ Keyboard shortcuts → Ctrl+T, Ctrl+D, F12 party
├── 👆 Touch gestures → Swipe detection ready
├── 🖊️ Pen input → Surface device support
└── 🎯 Multi-device → Unified input handling
```

### Key MVVM Components

- **BaseViewModel** - INotifyPropertyChanged base class with helpers
- **MainWindowViewModel** - Main business logic, peer management, commands  
- **ConversationViewModel** - Individual chat session management
- **RelayCommand** - ICommand implementation for MVVM binding
- **Professional DataTemplates** - Declarative UI with converters
- **Localization System** - Resource-based multilingual support

## 🚀 Getting Started

### Prerequisites

- **Windows 10** (version 1809+) or **Windows 11**
- **Visual Studio 2022** with:
  - .NET 8.0 SDK
  - Windows App SDK 1.7+
  - WinUI 3 project templates
- **Rust P2P Library** ([rust_sockets](https://github.com/oriaj-nocrala/rust_sockets))

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/oriaj-nocrala/MessagesWinUI.git
   cd MessagesWinUI
   ```

2. **Set up the Rust P2P library**
   ```bash
   # Clone and build the Rust library separately
   git clone https://github.com/oriaj-nocrala/rust_sockets.git
   cd rust_sockets
   cargo build --lib --release
   
   # Copy the compiled library to the WinUI project
   cp target/release/archsockrust.dll ../MessagesWinUI/MessagesWinUI/MessagesWinUI/
   ```

3. **Build and run**
   ```bash
   # Open in Visual Studio 2022 or use Developer Command Prompt
   dotnet build
   dotnet run
   ```

## 🔧 Technical Details

### Professional MVVM Pattern

- **Separation of Concerns** - Views handle UI, ViewModels handle logic
- **Data Binding** - Declarative XAML with two-way binding
- **Commands Pattern** - Button clicks through ICommand implementations
- **Event-Driven Architecture** - PropertyChanged notifications for reactive UI
- **Resource Management** - Proper disposal patterns and memory management

### Localization System

```xml
<!-- XAML uses resource keys -->
<TextBlock Text="{Binding WelcomeTitleText}" />
```

```csharp
// ViewModel exposes localized properties
public string WelcomeTitleText => LocalizationHelper.GetString("WelcomeTitle");
```

```
Strings/
├── en/Resources.resw     # English resources
└── es/Resources.resw     # Spanish resources
```

### Message Protocol
- **Discovery**: UDP broadcast on port 6968 for peer detection
- **Communication**: TCP connections on port 6969 for message delivery  
- **Serialization**: Efficient binary protocol with Protocol Buffers
- **File Transfer**: Embedded in message protocol with progress tracking

## 🎯 Usage

1. **Launch the application** - MessagesWinUI starts with professional MVVM UI
2. **Language Detection** - Automatically detects system language (EN/ES)
3. **Peer discovery** - Click "Discover Peers" or automatic detection
4. **Connect to friends** - Click "Connect" next to discovered peers
5. **Start chatting** - Professional conversation panels with message history
6. **Send files** - Use file picker for sharing documents
7. **Emoji support** - Click emoji button for organized emoji picker

## 🛠️ Development

### Project Structure

```
MessagesWinUI/
├── MessagesWinUI/                    # Main WinUI 3 project  
│   ├── MainWindow.xaml               # Main declarative UI
│   ├── MainWindow.xaml.cs            # Minimal MVVM code-behind
│   ├── ViewModels/                   # MVVM ViewModels
│   │   ├── BaseViewModel.cs          # INotifyPropertyChanged base
│   │   ├── MainWindowViewModel.cs    # Main business logic
│   │   └── ConversationViewModel.cs  # Chat session logic
│   ├── Commands/                     # MVVM Commands
│   │   └── RelayCommand.cs           # ICommand implementation
│   ├── Controls/                     # Professional UserControls
│   │   ├── ConversationPanel.xaml    # Chat UI component
│   │   └── EmojiPicker.xaml          # Emoji selection control
│   ├── Models/                       # Data models
│   │   └── PeerInfo.cs               # Peer information model
│   ├── Converters/                   # XAML Value Converters
│   │   └── MessageConverters.cs      # UI binding converters
│   ├── Helpers/                      # Utility classes
│   │   └── LocalizationHelper.cs     # Multilingual support
│   ├── Resources/                    # XAML Resources
│   │   └── DataTemplates.xaml        # Declarative UI templates
│   ├── Strings/                      # Localization resources
│   │   ├── en/Resources.resw         # English strings
│   │   └── es/Resources.resw         # Spanish strings
│   ├── Interop/                      # P/Invoke wrapper classes
│   │   ├── P2PMessenger.cs           # High-level C# API
│   │   ├── NativeMethods.cs          # P/Invoke declarations  
│   │   └── P2PEventArgs.cs           # Event system
│   └── archsockrust.dll              # Rust P2P library (not in repo)
└── MessagesWinUI (Package)/          # MSIX packaging project
```

### Building from Source

```bash
# Debug build
dotnet build -c Debug

# Release build  
dotnet build -c Release

# Run with specific culture for testing localization
dotnet run -- --culture es

# Create package
msbuild "MessagesWinUI (Package)/MessagesWinUI (Package).wapproj" -p:Configuration=Release
```

### MVVM Development Guidelines

1. **ViewModels** - All business logic goes here, never in code-behind
2. **Commands** - Use RelayCommand for button clicks and user actions
3. **Data Binding** - Prefer declarative XAML over imperative code
4. **Localization** - Always use resource keys, never hardcode strings
5. **Converters** - Use for complex UI transformations in XAML

### Dependencies

The Rust library dependency is **not included** in this repository. You must:

1. Clone [rust_sockets](https://github.com/oriaj-nocrala/rust_sockets) separately
2. Build the library: `cargo build --lib --release`  
3. Copy `archsockrust.dll` to the WinUI project directory
4. The `.gitignore` explicitly excludes this library to keep repos separate

## 🌍 Localization

The app supports multiple languages through .resw resource files:

- **English (Default)** - `Strings/en/Resources.resw`
- **Spanish** - `Strings/es/Resources.resw`

To add a new language:
1. Create `Strings/{language-code}/Resources.resw`
2. Translate all string keys from English version
3. Test with `dotnet run -- --culture {language-code}`

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

**Development Setup:**
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow MVVM patterns and existing code style
4. Test both English and Spanish UI
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Related Projects

- **[rust_sockets](https://github.com/oriaj-nocrala/rust_sockets)** - The core P2P networking library written in Rust
- **[WinUI 3 Documentation](https://docs.microsoft.com/en-us/windows/apps/winui/)** - Official WinUI 3 documentation
- **[.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)** - Official .NET documentation

## 🙏 Acknowledgments

- **Microsoft WinUI team** for the excellent modern Windows UI framework
- **Rust community** for the powerful systems programming language
- **Protocol Buffers** for efficient cross-language serialization
- **MSN Messenger** for the UI inspiration
- **MVVM Pattern** for clean separation of concerns

---

⭐ **Star this repo if you find it useful!** ⭐

Made with ❤️, professional MVVM architecture, and modern Windows development tools