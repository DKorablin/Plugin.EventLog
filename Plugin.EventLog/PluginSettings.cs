using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using Plugin.EventLog.Data;
using Plugin.EventLog.UI;

namespace Plugin.EventLog
{
	/// <summary>Persistent configuration settings for the Event Log plugin, controlling which logs and machines are monitored and how the UI presents the data.</summary>
	public class PluginSettings
	{
		private Int32 _updateInterval;

		/// <summary>Gets or sets the display name of the Windows Event Log to monitor (e.g. <c>Application</c>, <c>System</c>).</summary>
		[Category("Data")]
		[Description("Gets or sets the display name of the Windows Event Log to monitor (e.g. Application, System).")]
		public String LogDisplayName { get; set; }

		/// <summary>Gets or sets a bitmask of <see cref="EventLogEntryType"/> values that determines which event types are displayed; 0 means all types are shown.</summary>
		[Category("Data")]
		[Description("Gets or sets a bitmask of EventLogEntryType values that determines which event types are displayed; 0 means all types are shown.")]
		[Editor(typeof(ColumnEditor<EventLogEntryType>), typeof(UITypeEditor))]
		[DefaultValue(0)]
		public UInt32 LogTypes { get; set; }

		/// <summary>Gets or sets the interval in minutes at which the event list is refreshed; set to 0 to disable automatic updates.</summary>
		[Category("UI")]
		[Description("Gets or sets the interval in minutes at which the event list is refreshed; set to 0 to disable automatic updates.")]
		[DefaultValue(0)]
		public Int32 UpdateInterval
		{
			get => this._updateInterval;
			set => this._updateInterval = value <= 0 ? 0 : value;
		}

		/// <summary>Gets or sets a serialized string that defines the display order of list columns; managed automatically by the UI.</summary>
		[Browsable(false)]
		public String ColumnOrder { get; set; }

		/// <summary>Gets or sets a serialized string that defines which list columns are visible; managed automatically by the UI.</summary>
		[Browsable(false)]
		public String ColumnVisible { get; set; }

		/// <summary>Gets or sets a newline-separated list of host names to collect events from; leave empty to use the local machine.</summary>
		/// <remarks>.NET 2.0 XML Serializer fix</remarks>
		[Category("Data")]
		[Description("Gets or sets a newline-separated list of host names to collect events from; leave empty to use the local machine.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public String MachineNames { get; set; }

		/// <summary>Get the types of events of interest</summary>
		/// <returns>Array of events of interest</returns>
		internal EventLogEntryType[] GetLogTypes()
		{
			EventLogEntryType[] arr = (EventLogEntryType[])Enum.GetValues(typeof(EventLogEntryType));
			if(this.LogTypes == 0)
				return arr;
			else
			{
				List<EventLogEntryType> result = new List<EventLogEntryType>();
				for(Int32 loop = 0; loop < arr.Length; loop++)
					if(Utils.IsBitSet(this.LogTypes, loop))
						result.Add(arr[loop]);
				return result.ToArray();
			}
		}

		/// <summary>Get the array of servers to collect events from</summary>
		/// <returns>Array of servers</returns>
		internal String[] GetMachineNames()
		{
			String[] machineNames = this.MachineNames == null
				? new String[] { }
				: this.MachineNames.Split(new String[] { Environment.NewLine, }, StringSplitOptions.RemoveEmptyEntries);

			return machineNames.Length == 0
				? new String[] { Environment.MachineName, }
				: machineNames;
		}

		/// <summary>Get the name of the log to collect events from</summary>
		/// <returns>Log type</returns>
		internal String GetLogDisplayName()
		{
			if(!String.IsNullOrEmpty(this.LogDisplayName))
				return this.LogDisplayName;
			else
			{
				System.Diagnostics.EventLog[] logs = null;
				try
				{
					logs = System.Diagnostics.EventLog.GetEventLogs(this.GetMachineNames()[0]);
					return logs[0].Log;
				} finally
				{
					if(logs != null)
						foreach(var log in logs)
							log.Dispose();
				}
			}
		}
	}
}