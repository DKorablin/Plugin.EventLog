# EventLog Viewer Plugin

A plugin for SAL host application that provides a Windows Event Log viewer panel inside a SAL-hosted application.

## Features

- Browse Windows Event Log entries in a dockable panel
- Filter events by **date range** using an interactive date selector
- Filter events by **event type** (Error, Warning, Information, etc.)
- Query events from **remote machines** (configurable via settings)
- Select a specific **Event Log** source (Application, System, Security, etc.)
- Auto-refresh on a configurable **update interval**
- Customizable **column visibility and order**
- New entries since last refresh are highlighted in green
- Exposes a public API (`GetEvents`) for use by other plugins or the host

## Requirements

- Windows (WinForms)
- .NET Framework 4.8 **or** .NET 8.0 (Windows)
- A SAL-compatible host application referencing `SAL.Windows`

## Installation

1. Download the release archive (.zip or .nupkg).
2. Place the plugin assembly into the host application plugin directory (SAL / host supporting Windows environment):
	- [Flatbed.Dialog](https://dkorablin.github.io/Flatbed-Dialog/)
	- [Flatbed.Dialog (Lite)](https://dkorablin.github.io/Flatbed-Dialog-Lite)
	- [Flatbed.MDI](https://dkorablin.github.io/Flatbed-MDI)
	- [Flatbed.MDI (WPF)](https://dkorablin.github.io/Flatbed-MDI-Avalon)
3. Restart the host application; Plugin.McpBridge should appear in the plugin list (Tools -> EventLog).

## Usage

The plugin registers itself automatically when loaded by the SAL host.
It adds an **EventLog** entry under the **Tools** menu.
Clicking it opens the Event Viewer panel docked to the left side of the host window.

### Configurable Settings

| Setting | Description |
|---|---|
| `LogDisplayName` | The Event Log to read from (e.g. `Application`, `System`) |
| `LogTypes` | Bitmask of `EventLogEntryType` values to include |
| `UpdateInterval` | Auto-refresh interval in minutes (`0` = disabled) |
| `MachineNames` | Newline-separated list of remote machine names to query |
| `ColumnOrder` | Saved column order for the log list |
| `ColumnVisible` | Saved column visibility for the log list |

### Programmatic API

Other plugins or the SAL host can call `GetEvents` directly:

```csharp
LogEntry[] entries = plugin.GetEvents(dateFrom, dateTo, new[] { "Error", "Warning" });
```

## Building

```shell
git clone --recurse-submodules https://github.com/DKorablin/Plugin.EventLog.git
cd Plugin.EventLog
dotnet build
```