using System;
using System.Diagnostics;
using System.Threading;

namespace Plugin.EventLog.Threading
{
	internal class ThreadRequest
	{
		public String MachineName { get; }

		public String LogDisplayName { get; }

		public EventLogEntryType[] LogTypes { get; }

		public DateTime TimeStart { get; }

		public DateTime TimeEnd { get; }

		public CancellationToken CancellationToken { get; }

		public ThreadRequest(String machineName, String logDisplayName, EventLogEntryType[] logTypes, DateTime timeStart, DateTime timeEnd, CancellationToken cancellationToken = default)
		{
			this.MachineName = machineName;
			this.LogDisplayName = logDisplayName;
			this.LogTypes = logTypes;
			this.TimeStart = timeStart;
			this.TimeEnd = timeEnd;
			this.CancellationToken = cancellationToken;
		}
	}
}