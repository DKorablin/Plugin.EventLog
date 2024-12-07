using System;
using System.Diagnostics;

namespace Plugin.EventLog.Data
{
	internal class LogEntry
	{
		public String Category { get; private set; }
		public Int16 CategoryNumber { get; private set; }
		public Byte[] Data { get; private set; }
		public EventLogEntryType EntryType { get; private set; }
		public Int64 InstanceId { get; private set; }
		public String MachineName { get; private set; }
		public String Message { get; private set; }
		public String[] ReplacementStrings { get; private set; }
		public String Source { get; private set; }
		public DateTime TimeGenerated { get; private set; }
		public DateTime TimeWritten { get; private set; }
		public String UserName { get; private set; }

		public LogEntry(EventLogEntry entry)
		{
			this.Category = entry.Category;
			this.CategoryNumber = entry.CategoryNumber;
			this.Data = entry.Data;
			this.EntryType = entry.EntryType;
			this.InstanceId = entry.InstanceId;
			this.MachineName = entry.MachineName;
			this.Message = entry.Message;
			this.ReplacementStrings = entry.ReplacementStrings;
			this.Source = entry.Source;
			this.TimeGenerated = entry.TimeGenerated;
			this.TimeWritten = entry.TimeWritten;
			this.UserName = entry.UserName;
		}
	}
}