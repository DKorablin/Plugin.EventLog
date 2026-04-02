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
	public class PluginSettings
	{
		private Int32 _updateInterval;

		[Category("Data")]
		[Description("Log type")]
		public String LogDisplayName { get; set; }

		[Category("Data")]
		[Description("Required events")]
		[Editor(typeof(ColumnEditor<EventLogEntryType>), typeof(UITypeEditor))]
		[DefaultValue(0)]
		public UInt32 LogTypes { get; set; }

		/// <summary>Events update interval</summary>
		[Category("UI")]
		[Description("Events update interval (min). 0 - Off")]
		[DefaultValue(0)]
		public Int32 UpdateInterval
		{
			get => this._updateInterval;
			set => this._updateInterval = value <= 0 ? 0 : value;
		}

		/// <summary>Column order when selecting an application</summary>
		[Browsable(false)]
		public String ColumnOrder { get; set; }

		/// <summary>Columns visible in the list</summary>
		[Browsable(false)]
		public String ColumnVisible { get; set; }

		/// <remarks>.NET 2.0 XML Serializer fix</remarks>
		[Category("Data")]
		[Description("Target servers with log entries")]
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