using System;
using Plugin.EventLog.Data;

namespace Plugin.EventLog.Threading
{
	internal class ThreadResponse
	{
		public LogEntry[] Entries { get; set; }
		public Exception Exception { get; set; }

		public ThreadResponse(LogEntry[] entries)
			=> this.Entries = entries;

		public ThreadResponse(Exception exc)
			=> this.Exception = exc;
	}
}