using System;
using System.Windows.Forms;

namespace Plugin.EventLog.UI
{
	internal class DateSelectorPanel : TableLayoutPanel
	{
		private readonly MonthCalendar _calStart;

		private readonly MonthCalendar _calEnd;

		/// <summary>Отображать один календарь вместо 2х</summary>
		private readonly Boolean _singleCalendar;

		public DateTime StartDate
		{
			get => this._calStart.SelectionStart.Date;
			set
			{
				if(this._calStart.InvokeRequired)
					this._calStart.Invoke((MethodInvoker)delegate { this._calStart.SelectionStart = value; });
				else
					this._calStart.SelectionStart = value;
			}
		}

		public DateTime EndDate
		{
			get => (this._singleCalendar
					? this._calStart.SelectionEnd
					: this._calEnd.SelectionStart).Date;
			set
			{
				if(this._singleCalendar)
					if(this._calStart.InvokeRequired)
						this._calStart.Invoke((MethodInvoker)delegate { this._calStart.SelectionEnd = value; });
					else
						this._calStart.SelectionEnd = value;
				else
					if(this._calEnd.InvokeRequired)
						this._calEnd.Invoke((MethodInvoker)delegate { this._calEnd.SelectionStart = value; });
					else
						this._calEnd.SelectionStart = value;
			}
		}

		public event EventHandler<DateRangeEventArgs> DateRangeSelected;

		public DateSelectorPanel(DateTime start, DateTime end, Boolean single)
		{
			this._singleCalendar = single;
			if(this._singleCalendar)
			{
				base.ColumnCount = 1;
				base.RowCount = 1;

				this._calStart = new MonthCalendar
				{
					MaxSelectionCount = 10
				};
			} else
			{
				base.ColumnCount = 2;
				base.RowCount = 1;

				this._calStart = new MonthCalendar();
				this._calEnd = new MonthCalendar
				{
					MaxSelectionCount = _calStart.MaxSelectionCount = 1
				};
			}
			this.SetDateFilter(start, end);

			if(this._singleCalendar)
			{
				this._calStart.DateSelected += new DateRangeEventHandler(this.calStart_DateSelected);

				base.Controls.AddRange(new Control[] { this._calStart, });
				//base.Width = this._calStart.Width;
			} else
			{
				this._calStart.DateSelected += new DateRangeEventHandler(this.calStart_DateSelected);
				this._calEnd.DateSelected += new DateRangeEventHandler(this.calEnd_DateSelected);

				base.Controls.AddRange(new Control[] { this._calStart, this._calEnd, });
				base.Width = this._calStart.Width + this._calEnd.Width;
			}
		}

		private void calStart_DateSelected(Object sender, DateRangeEventArgs e)
		{
			DateTime start = e.Start;

			this.OnDateSelected(start, this.EndDate);
		}

		private void calEnd_DateSelected(Object sender, DateRangeEventArgs e)
		{
			DateTime end = e.Start;

			this.OnDateSelected(this.StartDate, end);
		}

		private void OnDateSelected(DateTime start, DateTime end)
		{
			this.SetDateFilter(start, end);

			this.DateRangeSelected?.Invoke(this, new DateRangeEventArgs(start, end));
		}

		public void SetDateFilter(DateTime start, DateTime end)
		{
			this.StartDate = start;
			this.EndDate = end;

			if(!this._singleCalendar)
			{
				this._calStart.MaxDate = end;//.AddDays(-1);
				this._calEnd.MinDate = start;//.AddDays(1);
			}
		}
	}
}