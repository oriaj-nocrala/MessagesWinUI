# MessagesWinUI - P2P Messenger with WinUI 3

## Project Overview

This is a WinUI 3 desktop application that provides a modern MSN Messenger-style interface for the Rust P2P messaging library (ArchSockRust). The application enables peer-to-peer communication on local networks without requiring central servers.

## Project Structure

```
MessagesWinUI/
├── MessagesWinUI/
│   ├── MessagesWinUI/                    # Main WinUI 3 project
│   │   ├── MainWindow.xaml               # MSN Messenger-style UI
│   │   ├── MainWindow.xaml.cs            # Main window logic
│   │   ├── App.xaml                      # Application resources
│   │   ├── App.xaml.cs                   # Application startup
│   │   ├── Converters.cs                 # XAML value converters
│   │   ├── archsockrust.dll              # Rust P2P library (compiled)
│   │   ├── Models/
│   │   │   └── PeerInfo.cs              # Data models for peers and messages
│   │   └── Interop/                     # P/Invoke wrapper classes
│   │       ├── P2PMessenger.cs          # High-level C# API
│   │       ├── NativeMethods.cs         # P/Invoke declarations
│   │       └── P2PEventArgs.cs          # Event arguments
│   └── MessagesWinUI (Package)/         # Packaging project
└── rust_sockets/                        # Rust P2P library source
    ├── target/release/archsockrust.dll  # Compiled DLL
    └── ArchSockRust.NET/                # C# interop examples
```

## Features Implemented

### 🎨 MSN Messenger-Style UI
- **Top Bar**: User info with profile picture and online status
- **Left Panel**: Live peer list with connection status
- **Right Panel**: Chat area with message history and input
- **Bottom Status Bar**: Connection status and peer count

### 🔧 P2P Integration
- **Automatic Peer Discovery**: UDP broadcast every 5 seconds
- **Direct P2P Connections**: TCP connections between peers
- **Real-time Messaging**: Instant text message delivery
- **File Transfer**: Send files to connected peers
- **Event-Driven UI**: Live updates for peer discovery/connection

### 💬 Chat Features
- **Message Bubbles**: MSN-style chat bubbles (blue for sent, gray for received)
- **Timestamps**: Message time display
- **Sender Names**: Clear message attribution
- **Auto-scroll**: Automatic scroll to latest messages
- **Enter to Send**: Keyboard shortcut support

## Build Instructions

### Prerequisites
You need Visual Studio with the following workloads:
- **.NET Multi-platform App UI development**
- **Windows application development**

Or use the **Developer Command Prompt for Visual Studio**.

### Build Steps
```bash
# 1. Build the Rust library first
cd rust_sockets
cargo build --lib --release

# 2. Copy DLL to WinUI project (already done)
# cp target/release/archsockrust.dll ../MessagesWinUI/MessagesWinUI/archsockrust.dll

# 3. Build from Developer Command Prompt
cd MessagesWinUI/MessagesWinUI
dotnet build

# 4. Or build the entire solution
cd MessagesWinUI
dotnet build
```

### Known Issues & Solutions

#### Build Errors
- **Missing Microsoft.Build.Packaging.Pri.Tasks.dll**: Use Developer Command Prompt for Visual Studio
- **Missing workloads**: Install WinUI 3 workloads in Visual Studio Installer
- **XAML compilation errors**: Ensure all converters are properly registered in App.xaml

#### Runtime Requirements
- **Windows 10 version 1809 or later**
- **.NET 8.0 runtime**
- **Visual C++ Redistributable** (for Rust DLL)

## Usage

1. **Start Application**: Launch MessagesWinUI
2. **Peer Discovery**: Click "Discover Peers" or wait for automatic discovery
3. **Connect**: Click "Connect" next to discovered peers
4. **Chat**: Select connected peer and start messaging
5. **File Transfer**: Use 📁 button to send files

## Technical Details

### Interop Architecture
```
┌─────────────────────┐
│   WinUI 3 App       │  ← MainWindow.xaml.cs
├─────────────────────┤
│  C# High-Level API  │  ← P2PMessenger.cs
├─────────────────────┤  
│   P/Invoke Layer    │  ← NativeMethods.cs
├─────────────────────┤
│   C FFI Exports     │  ← Rust src/ffi.rs
├─────────────────────┤
│  Rust Core Library  │  ← ArchSockRust P2P engine
└─────────────────────┘
```

### Event Flow
1. **Rust Library** → **C FFI** → **P/Invoke** → **C# Events** → **UI Updates**
2. **UI Actions** → **C# Methods** → **P/Invoke** → **C FFI** → **Rust Library**

### Message Protocol
- **Discovery**: UDP broadcast on port 6968
- **Messaging**: TCP connections on port 6969
- **Serialization**: Binary format with size prefixes
- **File Transfer**: Embedded in message protocol

## Next Steps

### To Complete Setup
1. Open **Developer Command Prompt for Visual Studio**
2. Navigate to project directory
3. Run `dotnet build`
4. Test with multiple instances

### Potential Improvements
- **Contact List Persistence**: Save known peers
- **Message History**: Store chat history
- **Notifications**: Windows toast notifications
- **Themes**: Dark/light theme support
- **Emoji Support**: Rich text messaging
- **Group Chat**: Multi-peer conversations

## Debugging Tips

### Common Issues
- **DLL Not Found**: Ensure `archsockrust.dll` is in output directory
- **Connection Failed**: Check Windows Firewall settings
- **No Peers Found**: See "Peer Discovery Issues" below
- **Build Fails**: Use Developer Command Prompt

### Peer Discovery Issues
- **UDP Broadcast Blocked**: Windows may block UDP broadcast to 255.255.255.255
- **Firewall Restrictions**: Even with permissive settings, UDP broadcast may fail
- **Network Topology**: Different subnets (192.168.1.x vs 192.168.2.x) won't see each other
- **Port Conflicts**: Multiple instances on same discovery port cause binding errors

#### Identified Root Cause
The current discovery uses hardcoded `BROADCAST_ADDR = "255.255.255.255"` which is:
- Blocked by Windows Defender Firewall in many configurations
- Not cross-platform compatible
- Doesn't work across different subnets

#### Proposed Solutions
1. **Dynamic Network Interface Detection**: Auto-detect broadcast address for each network interface
2. **Multicast Instead of Broadcast**: Use 224.0.0.x multicast addresses (industry standard)
3. **Multi-Strategy Discovery**: Combine localhost + interface broadcasts + multicast
4. **Fallback Methods**: Direct IP connection when broadcast fails

#### Current Discovery Implementation
```rust
// In src/discovery/mod.rs lines 106-111:
let discovery_ports = [6968, 6970, 6972, 6974, 6976, 6978];
for port in discovery_ports {
    let addr = format!("{}:{}", BROADCAST_ADDR, port);  // 255.255.255.255
    let _ = socket.send_to(&buf, &addr);
}
```

#### Testing Results
- **Local Instances**: Don't discover each other even on same machine
- **Remote Instances**: Can see local broadcasts but not vice versa  
- **Windows 10 vs Windows 11**: Windows 10 allows broadcast, Windows 11 more restrictive
- **Port 6968 TCP**: Accessible, confirming it's UDP broadcast specific issue

### Testing
- Run multiple instances with different names
- Test on same machine with different ports
- Test across network between different machines

## Files Modified/Created

### New Files Created
- `MainWindow.xaml` - MSN Messenger-style interface
- `MainWindow.xaml.cs` - UI logic and P2P integration
- `Converters.cs` - XAML value converters
- `Models/PeerInfo.cs` - Data models
- `Interop/P2PMessenger.cs` - High-level C# wrapper
- `Interop/NativeMethods.cs` - P/Invoke declarations
- `Interop/P2PEventArgs.cs` - Event system

### Modified Files
- `MessagesWinUI.csproj` - Added DLL content reference
- `App.xaml` - Added converter resources

## Status
✅ Rust library compiled to DLL  
✅ Interop classes copied and adapted  
✅ MSN Messenger-style UI designed  
✅ P2P integration implemented  
✅ Chat functionality complete  
✅ Build from Developer Command Prompt working  
✅ WinUI application launches successfully  
⚠️ **Current Issue**: Peer discovery not working due to UDP broadcast limitations  
⏳ **Next**: Implement dynamic network discovery solution  

The application builds and runs successfully, but peer discovery needs to be enhanced for reliable cross-platform and cross-network functionality.