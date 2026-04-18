using System;
using System.Diagnostics;
using System.Threading;

namespace Plugin.EventLog.Threading
{
	internal class ThreadRequest
	{
		/// <summary>The name of the machine to query events from</summary>
		public String MachineName { get; }

		/// <summary>The display name of the event log to read</summary>
		public String LogDisplayName { get; }

		/// <summary>The event entry types to include in the query</summary>
		public EventLogEntryType[] LogTypes { get; }

		/// <summary>The start of the date range to query</summary>
		public DateTime TimeStart { get; }

		/// <summary>The end of the date range to query</summary>
		public DateTime TimeEnd { get; }

		/// <summary>Token used to cancel the log reading operation</summary>
		public CancellationToken CancellationToken { get; }

		/// <summary>Creates a new thread request for querying event log entries</summary>
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