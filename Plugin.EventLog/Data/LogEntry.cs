using System;
using System.Diagnostics;

namespace Plugin.EventLog.Data
{
	public class LogEntry
	{
		private EventLogEntry _entry;
		private String _message;

		public String Category { get; }

		public Int16 CategoryNumber { get; }

		public Byte[] Data { get; }

		public EventLogEntryType EntryType { get; }

		public Int64 InstanceId { get; }

		public String MachineName { get; }

		public String Message
		{
			get
			{
				if(this._message == null)
				{
					this._message = this._entry.Message;
					this._entry = null;// Release unmanaged buffer; no longer needed
				}
				return this._message;
			}
		}

		public String[] ReplacementStrings { get; }

		public String Source { get; }

		public DateTime TimeGenerated { get; }

		public DateTime TimeWritten { get; }

		public String UserName { get; }

		public LogEntry(EventLogEntry entry)
		{
			this._entry = entry;
			this.Category = entry.Category;
			this.CategoryNumber = entry.CategoryNumber;
			this.Data = entry.Data;
			this.EntryType = entry.EntryType;
			this.InstanceId = entry.InstanceId;
			this.MachineName = entry.MachineName;
			this.ReplacementStrings = entry.ReplacementStrings;
			this.Source = entry.Source;
			this.TimeGenerated = entry.TimeGenerated;
			this.TimeWritten = entry.TimeWritten;
			this.UserName = entry.UserName;
		}
	}
}