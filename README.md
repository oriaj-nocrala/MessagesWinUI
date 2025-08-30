# 💬 MessagesWinUI

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![WinUI](https://img.shields.io/badge/WinUI-3.0-blue)](https://docs.microsoft.com/en-us/windows/apps/winui/)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%2F11-green)](https://www.microsoft.com/windows/)
[![License](https://img.shields.io/badge/License-MIT-yellow)](LICENSE)

A modern **peer-to-peer messaging application** built with **WinUI 3** and **Rust**, featuring a sleek MSN Messenger-inspired interface with **professional MVVM architecture** for seamless local network communication.

## ✨ Features

### 🎨 Modern UI/UX
- **MSN Messenger-style interface** with contemporary WinUI 3 design
- **Professional MVVM architecture** - ViewModels, Commands, declarative XAML
- **Multilingual support** - English/Spanish localization with .resw files
- **Real-time peer discovery** with live connection status
- **Dark/Light theme support** following system preferences
- **Message bubbles** with sender identification and timestamps

### 🔗 P2P Networking  
- **Zero-server architecture** - Direct peer-to-peer communication
- **Automatic peer discovery** via UDP broadcast on local networks
- **Real-time messaging** with instant delivery
- **File transfer capabilities** with progress tracking
- **Multi-peer support** - Connect to multiple friends simultaneously

### 📱 Chat Experience
- **Professional UserControls** - ConversationPanel, EmojiPicker
- **Message persistence** - Conversations saved per peer
- **Auto-scroll** to latest messages with smooth animations
- **Enter-to-send** keyboard shortcuts
- **Connection status indicators**
- **Emoji support** with organized picker

## 🏗️ Architecture

### Modern MVVM Implementation

```
┌─────────────────────────┐
│   WinUI 3 XAML Views    │  ← Declarative UI with data binding
├─────────────────────────┤
│   ViewModels Layer      │  ← Business logic, Commands, INotifyPropertyChanged
├─────────────────────────┤
│   Models & Services     │  ← Data models, P2P integration
├─────────────────────────┤  
│   C# P/Invoke Layer     │  ← NativeMethods.cs - FFI bridge
├─────────────────────────┤
│   Rust C FFI Exports    │  ← Foreign Function Interface
├─────────────────────────┤
│   Rust P2P Core         │  ← High-performance networking engine
└─────────────────────────┘
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