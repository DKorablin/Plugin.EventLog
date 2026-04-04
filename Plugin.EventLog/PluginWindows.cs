using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugin.EventLog.Data;
using Plugin.EventLog.Threading;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.EventLog
{
	public class PluginWindows : IPlugin, IPluginSettings<PluginSettings>
	{
		private TraceSource _trace;
		private PluginSettings _settings;
		private Dictionary<String, DockState> _documentTypes;

		internal TraceSource Trace => this._trace ?? (this._trace = PluginWindows.CreateTraceSource<PluginWindows>());

		internal IHostWindows HostWindows { get; }
		private IMenuItem PluginMenu { get; set; }

		/// <summary>Settings for interaction from the host</summary>
		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Settings for interaction from the plugin</summary>
		public PluginSettings Settings
		{
			get
			{
				if(this._settings == null)
				{
					this._settings = new PluginSettings();
					this.HostWindows.Plugins.Settings(this).LoadAssemblyParameters(this._settings);
				}
				return this._settings;
			}
		}

		private Dictionary<String, DockState> DocumentTypes
		{
			get
			{
				if(this._documentTypes == null)
					this._documentTypes = new Dictionary<String, DockState>()
					{
						{ typeof(PanelLogs).ToString(), DockState.DockLeft },
					};
				return this._documentTypes;
			}
		}

		public PluginWindows(IHostWindows hostWindows)
			=> this.HostWindows = hostWindows ?? throw new ArgumentNullException(nameof(hostWindows));

		public IWindow GetPluginControl(String typeName, Object args)
			=> this.CreateWindow(typeName, false, args);

		public LogEntry[] GetEvents(DateTime timeStart, DateTime timeEnd, EventLogEntryType[] eventTypes)
		{
			List<LogEntry> result = new List<LogEntry>();
			foreach(var machineName in this.Settings.GetMachineNames())
			{
				try
				{
					var logEntries = PluginWindows.GetEvents(new ThreadRequest(machineName, this.Settings.LogDisplayName, eventTypes, timeStart, timeEnd));
					result.AddRange(logEntries);
				}
				catch(Exception exc)
				{
					this.Trace.TraceData(TraceEventType.Error, 20, exc);
					throw;
				}
			}

			return result.ToArray();
		}

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			IMenuItem menuTools = this.HostWindows.MainMenu.FindMenuItem("Tools");
			if(menuTools == null)
			{
				this.Trace.TraceEvent(TraceEventType.Error, 10, "Menu item 'Tools' not found");
				return false;
			}

			this.PluginMenu = menuTools.Create("&EventLog");
			this.PluginMenu.Name = "tsmiToolsEventLog";
			this.PluginMenu.Click += (sender, e) => this.CreateWindow(typeof(PanelLogs).ToString(), false);
			menuTools.Items.Insert(0, this.PluginMenu);
			return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			this.HostWindows?.MainMenu.Items.Remove(this.PluginMenu);
			return true;
		}

		private IWindow CreateWindow(String typeName, Boolean searchForOpened, Object args = null)
			=> this.DocumentTypes.TryGetValue(typeName, out DockState state)
				? this.HostWindows.Windows.CreateWindow(this, typeName, searchForOpened, state, args)
				: null;

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}

		internal static LogEntry[] GetEvents(ThreadRequest request)
		{
			var list = new List<LogEntry>();
			using(System.Diagnostics.EventLog evt = new System.Diagnostics.EventLog(request.LogDisplayName, request.MachineName))
			{
				foreach(System.Diagnostics.EventLogEntry entry in evt.Entries)
				{
					if(request.CancellationToken.IsCancellationRequested)
						return list.ToArray();

					if(entry.TimeGenerated >= request.TimeEnd)
						continue;// Too new — may still find matches, keep scanning
					if(entry.TimeGenerated <= request.TimeStart)
						continue;// Too old — but newer entries still ahead, keep scanning

					if(request.LogTypes.Contains(entry.EntryType))
						list.Add(new LogEntry(entry));
				}
			}

			return list.ToArray();
		}
	}
}