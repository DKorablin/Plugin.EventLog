using System;
using System.Windows.Forms;
using Plugin.EventLog.Properties;

namespace Plugin.EventLog.UI
{
	public class DateSelectorHost : ToolStripControlHost
	{
		internal new DateSelectorPanel Control => (DateSelectorPanel)base.Control;

		public DateSelectorHost(DateTime start, DateTime end, Boolean single)
			: this(start, end, single, null)
		{ }

		public DateSelectorHost(DateTime start, DateTime end, Boolean single, String name)
			: base(new DateSelectorPanel(start, end, single), name)
		{
			base.Padding = Padding.Empty;
			base.Width = this.Control.Width;
		}

		public String GetDateFilterText()
		{
			DateTime startDate = this.Control.StartDate;
			DateTime endDate = this.Control.EndDate;
			String format = startDate == endDate ? "{1}" : "{0}-{1}";
			return String.Format(format, startDate.ToShortDateString(), (endDate - DateTime.Today).Days == 0 ? Resources.msgToday : endDate.ToShortDateString());
		}

		public void GetDateFilter(String filterText, out DateTime start, out DateTime end)
		{
			start = this.Control.StartDate;
			end = this.Control.EndDate;
			DateTime today = DateTime.Today;
			String[] array = filterText.Split('-');

			Boolean flag = ((array.Length == 1) ? array[0] : array[1]) == Resources.msgToday;
			if(flag && (today - end).Days > 0)
			{
				this.Control.EndDate = end = today;
				if(array.Length == 1)
					this.Control.StartDate = start = today;
			}
			end = end.Add(new TimeSpan(23, 59, 59));

			/*DateTime tdStart = this.Control.StartDate;
			DateTime tdEnd = this.Control.EndDate;

			String[] text = filterText.Split('-');
			if(text[1] == Resources.msgToday && (tdEnd - DateTime.Today).Days > 0)
				this.Control.EndDate = DateTime.Today;

			start = this.Control.StartDate;
			end = DateTime.Parse(this.Control.EndDate.ToShortDateString() + " 23:59:59");*/
		}
	}
}