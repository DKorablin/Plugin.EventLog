using System;
using System.Diagnostics;

namespace Plugin.EventLog.Threading
{
	internal class ThreadRequest
	{
			public String MachineName { get; private set; }
			public String LogDisplayName { get; private set; }
			public EventLogEntryType[] LogTypes { get; private set; }
			public PanelLogs Ctrl { get; private set; }
			public DateTime TimeStart { get; private set; }
			public DateTime TimeEnd { get; private set; }

			public ThreadRequest(String machineName, String logDisplayName, EventLogEntryType[] logTypes, PanelLogs ctrl, DateTime timeStart, DateTime timeEnd)
			{
				this.MachineName = machineName;
				this.LogDisplayName = logDisplayName;
				this.LogTypes = logTypes;
				this.Ctrl = ctrl;
				this.TimeStart = timeStart;
				this.TimeEnd = timeEnd;
			}
		}
}
