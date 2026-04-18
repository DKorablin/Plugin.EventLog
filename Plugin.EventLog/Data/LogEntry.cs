using System;
using System.Diagnostics;

namespace Plugin.EventLog.Data
{
	/// <summary>Represents a snapshot of a single Windows Event Log entry.</summary>
	public class LogEntry
	{
		/// <summary>Gets the text associated with the category of the entry.</summary>
		public String Category { get; }

		/// <summary>Gets the application-specific category number for the entry.</summary>
		public Int16 CategoryNumber { get; }

		/// <summary>Gets the binary data associated with the entry.</summary>
		public Byte[] Data { get; }

		/// <summary>Gets the event type of the entry.</summary>
		public EventLogEntryType EntryType { get; }

		/// <summary>Gets the resource identifier that designates the message text of the entry.</summary>
		public Int64 InstanceId { get; }

		/// <summary>Gets the name of the computer on which the entry was generated.</summary>
		public String MachineName { get; }

		/// <summary>Gets the localized message associated with the entry.</summary>
		public String Message { get; }

		/// <summary>Gets the replacement strings used in the formatted message of the entry.</summary>
		public String[] ReplacementStrings { get; }

		/// <summary>Gets the name of the application that generated the entry.</summary>
		public String Source { get; }

		/// <summary>Gets the local time at which the entry was generated.</summary>
		public DateTime TimeGenerated { get; }

		/// <summary>Gets the local time at which the entry was written to the log.</summary>
		public DateTime TimeWritten { get; }

		/// <summary>Gets the user name of the account associated with the entry.</summary>
		public String UserName { get; }

		/// <summary>Initializes a new instance of <see cref="LogEntry"/> from an <see cref="EventLogEntry"/>.</summary>
		/// <param name="entry">The source event log entry to copy data from.</param>
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