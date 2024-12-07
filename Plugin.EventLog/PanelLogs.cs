using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Plugin.EventLog.Data;
using Plugin.EventLog.Properties;
using Plugin.EventLog.Threading;
using Plugin.EventLog.UI;
using SAL.Windows;

namespace Plugin.EventLog
{
	public partial class PanelLogs : UserControl
	{
		private static readonly Color NewColor = Color.Green;
		private Object _syncLock = new Object();
		private volatile Int32 _threadCount = 0;
		private volatile Boolean _dataRecieved = false;
		private DateTime? _lastEventDate;
		private DateSelectorHost _dateSelector;

		private const String Caption = "Event Viewer";
		private PluginWindows Plugin => (PluginWindows)this.Window.Plugin;
		private IWindow Window => (IWindow)base.Parent;

		public PanelLogs()
			=> this.InitializeComponent();

		private void SetCaption(Int32 count, Int32? selectedItems)
		{
			String caption = selectedItems == null
				? $"{PanelLogs.Caption} ({count:n0})"
				: $"{PanelLogs.Caption} ({count:n0}/{selectedItems.Value:n0})";

			this.SetCaption(caption);
		}

		private void SetLoadingCaption(Int32 loadingThreads)
		{
			String caption = $"{PanelLogs.Caption} Loading... (Threads: {loadingThreads:n0})";
			this.SetCaption(caption);
		}

		private void SetCaption(String caption)
		{
			if(base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate { this.SetCaption(caption); });
				return;
			}

			this.Window.Caption = caption;
		}

		protected override void OnCreateControl()
		{
			this.Window.Caption = PanelLogs.Caption;
			this.Window.SetTabPicture(Resources.EventLog_Icon);
			this.Window.Shown += new EventHandler(Window_Shown);
			lvData.Plugin = this.Plugin;

			this._dateSelector = new DateSelectorHost(DateTime.Today, DateTime.Today, true);
			this._dateSelector.Control.DateRangeSelected += new EventHandler<DateRangeEventArgs>(Control_DateRangeSelected);
			tsbnDateFilter.DropDownItems.Add(this._dateSelector);

			tabInfo.TabPages.Remove(tabPageBinary);
			
			base.OnCreateControl();
		}

		private void Window_Shown(Object sender, EventArgs e)
		{
			this.Control_DateRangeSelected(this._dateSelector, new DateRangeEventArgs(DateTime.Today.AddDays(-1), DateTime.Today));

			/*gridSearch.Visible = false;

			Boolean isValidConnection = false;
			using(var logger = this.Plugin.GetExceptionLoggerInstance())
				isValidConnection = logger != null;
			tsMain.Enabled = isValidConnection && !String.IsNullOrEmpty(this.Plugin.SettingsInternal.UserName) && !String.IsNullOrEmpty(this.Plugin.SettingsInternal.Password);*/
		}

		private void Control_DateRangeSelected(Object sender, DateRangeEventArgs e)
		{
			tsbnDateFilter.Text = this._dateSelector.GetDateFilterText();
			tsbnDateFilter.HideDropDown();

			this.GetEvents();
		}

		private void lvData_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.F5:
				this.GetEvents();
				e.Handled = true;
				break;
			}
		}

		private void lvData_SelectedIndexChanged(Object sender, EventArgs e)
		{
			this.SetCaption(lvData.ItemsCount, lvData.SelectedIndices.Count);

			if(lvData.SelectedIndices.Count == 1)
			{
				LogEntry entry = (LogEntry)lvData.SelectedObject;
				if(lvData.SelectedItems[0].ForeColor == PanelLogs.NewColor)
					lvData.SelectedItems[0].ForeColor = Color.Empty;
				pgInfo.SelectedObject = entry;

				if(entry.Data.Length > 0)
				{
					bvBytes.SetBytes(entry.Data);
					if(tabPageBinary.Parent == null)
						tabInfo.TabPages.Add(tabPageBinary);
				} else if(tabPageBinary.Parent != null)
					tabInfo.TabPages.Remove(tabPageBinary);

				txtMessage.Text = entry.Message;

				splitMain.Panel2Collapsed = false;
			} else
				pgInfo.SelectedObject = null;
		}

		private void refreshTimer_Elapsed(Object sender, System.Timers.ElapsedEventArgs e)
		{
			this.Plugin.Trace.TraceInformation("Updating Events ({0})...", DateTime.Now.ToShortTimeString());
			this.GetEvents();
		}

		private void GetEvents()
		{
			if(this._threadCount > 0)
				return;//Потоки всё ещё в процессе выполнения

			foreach(ListViewItem item in lvData.Items)
			{//Подсветка новых событий
				LogEntry log = (LogEntry)item.Tag;
				if(this._lastEventDate == null || this._lastEventDate < log.TimeGenerated)
					this._lastEventDate = log.TimeGenerated;
			}

			this.LockControls();
			DateTime timeStart, timeEnd;
			this.GetDateFilter(out timeStart, out timeEnd);

			try
			{
				String logDisplayName = this.Plugin.Settings.GetLogDisplayName();
				String[] machineNames = this.Plugin.Settings.GetMachineNames();
				EventLogEntryType[] logTypes = this.Plugin.Settings.GetLogTypes();

				this._dataRecieved = false;
				foreach(String machineName in machineNames)
				{
					ThreadRequest info = new ThreadRequest(machineName, logDisplayName, logTypes, this, timeStart, timeEnd);
					this._threadCount++;
					ThreadPool.QueueUserWorkItem(new WaitCallback(GetEventsAsync), info);
				}
				this.SetLoadingCaption(this._threadCount);
			} catch(Exception exc)
			{
				this.Plugin.Trace.TraceData(TraceEventType.Error, 10, exc);
				this.SetCaption(PanelLogs.Caption);
				this.UnlockControls(DateTime.Now);
			}
		}

		private void GetDateFilter(out DateTime start, out DateTime end)
			=> this._dateSelector.GetDateFilter(tsbnDateFilter.Text, out start, out end);

		private static void GetEventsAsync(Object data)
		{
			ThreadRequest info = (ThreadRequest)data;
			PanelLogs.GetEvents(info);
		}

		private static void GetEvents(ThreadRequest request)
		{
			LogEntry[] entries;
			ThreadResponse response;
			try
			{
				using(System.Diagnostics.EventLog evt = new System.Diagnostics.EventLog(request.LogDisplayName, request.MachineName))
				{
					entries = evt.Entries
						.Cast<System.Diagnostics.EventLogEntry>()
						.Where(p => p.TimeGenerated > request.TimeStart
							&& p.TimeGenerated < request.TimeEnd
							&& request.LogTypes.Contains(p.EntryType))
						.Select(p => new LogEntry(p))
						.ToArray();
				}

				response = new ThreadResponse(entries);

			} catch(Exception exc)
			{
				if(request.Ctrl.IsDisposed)
					return;

				exc.Data.Add("LogDisplayName", request.LogDisplayName);
				exc.Data.Add("MachineName", request.MachineName);
				response = new ThreadResponse(exc);
			}

			if(!request.Ctrl.IsDisposed)
				request.Ctrl.FillList(request, response);
		}

		private void FillList(ThreadRequest request, ThreadResponse response)
		{
			lock(_syncLock)
			{
				this._threadCount--;
				if(!this._dataRecieved)
				{
					lvData.Clear();
					this._dataRecieved = true;
				}

				if(response.Entries != null)
					lvData.FillList(response.Entries);
				else if(response.Exception != null)
					this.Plugin.Trace.TraceData(TraceEventType.Error, 10, response.Exception);

				if(this._threadCount == 0)
				{
					this.UnlockControls(request.TimeEnd);

					this.SetCaption(lvData.ItemsCount, null);

					if(this._lastEventDate!=null)//Подсветка новых событий с даты последнего обновления
						foreach(ListViewItem item in lvData.Items)
						{
							LogEntry log = (LogEntry)item.Tag;
							if(log.TimeGenerated > this._lastEventDate)
								item.ForeColor = PanelLogs.NewColor;
						}
				} else
					this.SetLoadingCaption(this._threadCount);
			}
		}

		private void LockControls()
		{
			refreshTimer.Stop();
			base.Cursor = Cursors.WaitCursor;
			tsbnDateFilter.Enabled = false;
			tsbnTimer.Enabled = false;
			tsbnTimer.ToolTipText = "Timer (Update in progress...)";
		}

		private void UnlockControls(DateTime current)
		{
			if(base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate { UnlockControls(current); });
				return;
			}

			base.Cursor = Cursors.Default;
			tsbnDateFilter.Enabled = true;

			if(this.Plugin.Settings.UpdateInterval > 0 && current.Date == DateTime.Today.Date)
			{
				refreshTimer.Interval = this.Plugin.Settings.UpdateInterval * 1000 * 60;
				refreshTimer.Start();
				
				tsbnTimer.ToolTipText = "Timer (On)";
				tsbnTimer.Enabled = tsbnTimer.Checked = true;
			} else
			{
				if(this.Plugin.Settings.UpdateInterval <= 0)
					tsbnTimer.ToolTipText = "Timer (UpdateInterval turned off)";
				else if(current.Date != DateTime.Today.Date)
					tsbnTimer.ToolTipText = "Timer (Update timer only for today)";
				tsbnTimer.Enabled = tsbnTimer.Checked = false;
			}
		}

		private void tsbnTimer_Click(Object sender, EventArgs e)
		{
			if(tsbnTimer.Checked)
			{
				DateTime start, end;
				this.GetDateFilter(out start, out end);
				this.UnlockControls(end);
			} else
			{
				tsbnTimer.ToolTipText = "Timer (Off)";
				refreshTimer.Stop();
			}
		}

		private void splitMain_MouseDoubleClick(Object sender, MouseEventArgs e)
		{
			if(splitMain.SplitterRectangle.Contains(e.Location))
				splitMain.Panel2Collapsed = true;
		}
	}
}