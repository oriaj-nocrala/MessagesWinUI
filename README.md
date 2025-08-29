# 💬 MessagesWinUI

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![WinUI](https://img.shields.io/badge/WinUI-3.0-blue)](https://docs.microsoft.com/en-us/windows/apps/winui/)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%2F11-green)](https://www.microsoft.com/windows/)
[![License](https://img.shields.io/badge/License-MIT-yellow)](LICENSE)

A modern **peer-to-peer messaging application** built with **WinUI 3** and **Rust**, featuring a sleek MSN Messenger-inspired interface for seamless local network communication.

## ✨ Features

### 🎨 Modern UI/UX
- **MSN Messenger-style interface** with contemporary WinUI 3 design
- **Tabbed conversations** - Each peer gets its own tab for intuitive multi-chat management  
- **Message bubbles** with sender identification and timestamps
- **Real-time peer discovery** with live connection status
- **Dark/Light theme support** following system preferences

### 🔗 P2P Networking  
- **Zero-server architecture** - Direct peer-to-peer communication
- **Automatic peer discovery** via UDP broadcast on local networks
- **Real-time messaging** with instant delivery
- **File transfer capabilities** with progress tracking
- **Multi-peer support** - Connect to multiple friends simultaneously

### 📱 Chat Experience
- **Message persistence** - Conversations saved per peer
- **Auto-scroll** to latest messages with smooth animations
- **Enter-to-send** keyboard shortcuts
- **Connection status indicators** 
- **Typing notifications** and delivery confirmations

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

### 📦 Creating Installation Package

To create an MSIX installer:

1. **Generate self-signed certificate** (first time only):
   ```powershell
   New-SelfSignedCertificate -Type Custom -Subject "CN=MessagesWinUI" -KeyUsage DigitalSignature -FriendlyName "MessagesWinUI Certificate" -CertStoreLocation "Cert:\CurrentUser\My" -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")
   ```

2. **Build packaging project**:
   ```bash
   # Build the package project
   msbuild "MessagesWinUI (Package)/MessagesWinUI (Package).wapproj" -p:Configuration=Release -p:Platform=x64
   ```

## 🏗️ Architecture

```
┌─────────────────────┐
│   WinUI 3 App       │  ← Modern Windows UI with tabs, themes
├─────────────────────┤
│  C# High-Level API  │  ← P2PMessenger.cs - Event-driven wrapper
├─────────────────────┤  
│   P/Invoke Layer    │  ← NativeMethods.cs - FFI bridge
├─────────────────────┤
│   Rust C FFI       │  ← Foreign Function Interface exports
├─────────────────────┤
│  Rust P2P Core      │  ← High-performance networking engine
└─────────────────────┘
```

### Key Components

- **MainWindow.xaml** - MSN-style UI with peer list and tabbed chat areas
- **P2PMessenger.cs** - High-level C# API wrapping the Rust library
- **NativeMethods.cs** - P/Invoke declarations for Rust FFI
- **rust_sockets library** - Core P2P networking, discovery, and messaging

## 🔧 Technical Details

### Message Protocol
- **Discovery**: UDP broadcast on port 6968 for peer detection
- **Communication**: TCP connections on port 6969 for message delivery  
- **Serialization**: Efficient binary protocol with Protocol Buffers
- **File Transfer**: Embedded in message protocol with progress tracking

### Network Discovery
- **Dynamic broadcast detection** - Automatically detects network interfaces
- **Multi-strategy discovery** - Localhost + interface broadcast + multicast fallback
- **Cross-subnet support** - Works across different network segments
- **Firewall-friendly** - Uses standard ports with proper error handling

### Performance
- **Async/await architecture** - Non-blocking UI with background networking
- **Memory management** - Automatic cleanup of peer connections and message history
- **Resource limits** - Configurable message history limits (1000 messages per peer)

## 🎯 Usage

1. **Launch the application** - MessagesWinUI starts automatically
2. **Peer discovery** - Click "Discover Peers" or wait for automatic detection
3. **Connect to friends** - Click "Connect" next to discovered peers
4. **Start chatting** - Each connected peer gets their own tab
5. **Send files** - Use the 📁 button to share files
6. **Multi-chat** - Switch between conversation tabs seamlessly

### Network Configuration

For optimal peer discovery across networks:
- **Local network**: Works automatically with default settings
- **Corporate networks**: May require firewall configuration for UDP broadcast
- **VPN environments**: Ensure UDP ports 6968 and TCP 6969 are accessible

## 🛠️ Development

### Project Structure

```
MessagesWinUI/
├── MessagesWinUI/                    # Main WinUI 3 project  
│   ├── MainWindow.xaml               # MSN Messenger-style UI
│   ├── MainWindow.xaml.cs            # UI logic and P2P integration
│   ├── Models/PeerInfo.cs            # Data models
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

# Run tests (when available)
dotnet test

# Create package
msbuild "MessagesWinUI (Package)/MessagesWinUI (Package).wapproj" -p:Configuration=Release
```

### Dependencies

The Rust library dependency is **not included** in this repository. You must:

1. Clone [rust_sockets](https://github.com/oriaj-nocrala/rust_sockets) separately
2. Build the library: `cargo build --lib --release`  
3. Copy `archsockrust.dll` to the WinUI project directory
4. The `.gitignore` explicitly excludes this library to keep repos separate

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

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

---

⭐ **Star this repo if you find it useful!** ⭐

Made with ❤️ and modern Windows development tools