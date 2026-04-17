using System;
using System.Diagnostics;

namespace Plugin.EventLog.Data
{
	public class LogEntry
	{
		public String Category { get; }

		public Int16 CategoryNumber { get; }

		public Byte[] Data { get; }

		public EventLogEntryType EntryType { get; }

		public Int64 InstanceId { get; }

		public String MachineName { get; }

		public String Message { get; }

		public String[] ReplacementStrings { get; }

		public String Source { get; }

		public DateTime TimeGenerated { get; }

		public DateTime TimeWritten { get; }

		public String UserName { get; }

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