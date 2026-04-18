using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.EventLog.Data;

namespace Plugin.EventLog.Threading
{
	internal class ThreadResponse
	{
		/// <summary>The log entries retrieved from the event log</summary>
		public LogEntry[] Entries { get; set; }

		/// <summary>The exception that occurred during the log reading operation</summary>
		public Exception Exception { get; set; }

		/// <summary>Creates a successful response with the retrieved log entries</summary>
		public ThreadResponse(IEnumerable<LogEntry> entries)
			=> this.Entries = entries.ToArray();

		/// <summary>Creates a faulted response with the exception that was thrown</summary>
		public ThreadResponse(Exception exc)
			=> this.Exception = exc;
	}
}