using System;
using Plugin.EventLog.Data;
using System.Diagnostics;

namespace Plugin.EventLog.UI
{
	internal class LogListView : DataListView
	{
		public override Int32 GetImageIndex(Object item)
		{
			LogEntry entry = (LogEntry)item;
			LogImageList result;
			switch(entry.EntryType)
			{
			case EventLogEntryType.Information:
				result = LogImageList.Information;
				break;
			case EventLogEntryType.Warning:
				result = LogImageList.Warning;
				break;
			case EventLogEntryType.Error:
				result = Data.LogImageList.Error;
				break;
			default:
				result = LogImageList.Information;
				break;
			}
			return (Int32)result;
		}
	}
}