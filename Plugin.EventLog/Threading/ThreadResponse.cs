using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.EventLog.Data;

namespace Plugin.EventLog.Threading
{
	internal class ThreadResponse
	{
		public LogEntry[] Entries { get; set; }
		public Exception Exception { get; set; }

		public ThreadResponse(IEnumerable<LogEntry> entries)
			=> this.Entries = entries.ToArray();

		public ThreadResponse(Exception exc)
			=> this.Exception = exc;
	}
}