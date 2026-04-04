using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
		private DateTime? _lastEventDate;
		private DateSelectorHost _dateSelector;

		private CancellationTokenSource _cts;

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

		private void SetCaption(String caption)
		{
			if(base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate { this.SetCaption(caption); });
				return;
			}

			this.Window.Caption = caption;
		}

		private void SetLoadingCaption(Int32 loadingThreads)
		{
			String caption = $"{PanelLogs.Caption} Loading... (Threads: {loadingThreads:n0})";
			this.SetCaption(caption);
		}

		protected override void OnCreateControl()
		{
			this.Window.Caption = PanelLogs.Caption;
			this.Window.SetTabPicture(Resources.EventLog_Icon);
			this.Window.Shown += new EventHandler(this.Window_Shown);
			lvData.Plugin = this.Plugin;

			this._dateSelector = new DateSelectorHost(DateTime.Today, DateTime.Today, true);
			this._dateSelector.Control.DateRangeSelected += new EventHandler<DateRangeEventArgs>(this.Control_DateRangeSelected);
			tsbnDateFilter.DropDownItems.Add(this._dateSelector);

			this.InitializeLogTypeFilter();

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

		private void InitializeLogTypeFilter()
		{
			EventLogEntryType[] logTypes = (EventLogEntryType[])Enum.GetValues(typeof(EventLogEntryType));
			UInt32 currentLogTypes = this.Plugin.Settings.LogTypes;

			for(Int32 index = 0; index < logTypes.Length; index++)
			{
				ToolStripMenuItem item = new ToolStripMenuItem(logTypes[index].ToString())
				{
					CheckOnClick = true,
					Checked = currentLogTypes == 0 || Utils.IsBitSet(currentLogTypes, index),
				};

				item.CheckedChanged += new EventHandler(this.LogTypeItem_CheckedChanged);
				this.tsbnLogType.DropDownItems.Add(item);
			}

			this.UpdateLogTypeCaption();
		}

		private void LogTypeItem_CheckedChanged(Object sender, EventArgs e)
		{
			ToolStripMenuItem changed = (ToolStripMenuItem)sender;
			if(!changed.Checked)
			{
				Boolean anyOtherChecked = false;
				foreach(ToolStripMenuItem menuItem in this.tsbnLogType.DropDownItems)
					if(menuItem != changed && menuItem.Checked)
					{
						anyOtherChecked = true;
						break;
					}

				if(!anyOtherChecked)
				{
					changed.Checked = true;
					return;
				}
			}

			Boolean allChecked = true;
			UInt32 logTypes = 0;

			for(Int32 index = 0; index < this.tsbnLogType.DropDownItems.Count; index++)
			{
				ToolStripMenuItem menuItem = (ToolStripMenuItem)this.tsbnLogType.DropDownItems[index];
				if(!menuItem.Checked)
					allChecked = false;
				else
					logTypes |= (UInt32)(1 << index);
			}

			this.Plugin.Settings.LogTypes = allChecked ? 0 : logTypes;
			this.UpdateLogTypeCaption();
		}

		private void UpdateLogTypeCaption()
		{
			List<String> selected = new List<String>();
			if(this.Plugin.Settings.LogTypes != 0)
				foreach(ToolStripMenuItem item in this.tsbnLogType.DropDownItems)
					if(item.Checked)
						selected.Add(item.Text);

			this.tsbnLogType.Text = this.Plugin.Settings.LogTypes == 0 ? "All" : (selected.Count == 0 ? "None" : String.Join(", ", selected));
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
			{
				txtMessage.Text = String.Empty;
				pgInfo.SelectedObject = null;
			}
		}

		private void refreshTimer_Elapsed(Object sender, System.Timers.ElapsedEventArgs e)
		{
			this.Plugin.Trace.TraceInformation("Updating Events ({0})...", DateTime.Now.ToShortTimeString());
			this.GetEvents();
		}

		private void GetEvents()
		{
			if(base.InvokeRequired)
			{
				base.BeginInvoke((MethodInvoker)delegate { this.GetEvents(); });
				return;
			}

			this._cts?.Cancel();

			foreach(ListViewItem item in lvData.Items)
			{//Highlight new events
				LogEntry log = (LogEntry)item.Tag;
				if(this._lastEventDate == null || this._lastEventDate < log.TimeGenerated)
					this._lastEventDate = log.TimeGenerated;
			}

			this._cts = new CancellationTokenSource();
			this.LockControls();
			this.GetDateFilter(out DateTime timeStart, out DateTime timeEnd);

			try
			{
				String logDisplayName = this.Plugin.Settings.GetLogDisplayName();
				String[] machineNames = this.Plugin.Settings.GetMachineNames();
				EventLogEntryType[] logTypes = this.Plugin.Settings.GetLogTypes();

				_ = this.LoadEventsAsync(machineNames, logDisplayName, logTypes, timeStart, timeEnd, this._cts.Token);
			} catch(Exception exc)
			{
				this.Plugin.Trace.TraceData(TraceEventType.Error, 10, exc);
				this.SetCaption(PanelLogs.Caption);
				this.UnlockControls(DateTime.Now);
			}
		}

		private void GetDateFilter(out DateTime start, out DateTime end)
			=> this._dateSelector.GetDateFilter(tsbnDateFilter.Text, out start, out end);

		private async Task LoadEventsAsync(String[] machineNames, String logDisplayName, EventLogEntryType[] logTypes, DateTime timeStart, DateTime timeEnd, CancellationToken cancellationToken)
		{
			this.SetLoadingCaption(machineNames.Length);

			List<Task<ThreadResponse>> tasks = new List<Task<ThreadResponse>>();
			foreach(String machineName in machineNames)
			{
				ThreadRequest request = new ThreadRequest(machineName, logDisplayName, logTypes, timeStart, timeEnd, cancellationToken);
				tasks.Add(Task.Run(() => PanelLogs.GetEventsCore(request), cancellationToken));
			}

			ThreadResponse[] responses;
			try
			{
				responses = await Task.WhenAll(tasks);
			} catch(OperationCanceledException)
			{
				return;
			}

			if(cancellationToken.IsCancellationRequested || this.IsDisposed)
				return;

			lvData.Clear();

			foreach(ThreadResponse response in responses)
			{
				if(response == null)
					continue;
				if(response.Entries != null)
					lvData.FillList(response.Entries);
				else if(response.Exception != null)
					this.Plugin.Trace.TraceData(TraceEventType.Error, 10, response.Exception);
			}

			this.UnlockControls(timeEnd);
			this.SetCaption(lvData.ItemsCount, null);

			if(this._lastEventDate != null)//Highlight new events since the last update date
				foreach(ListViewItem item in lvData.Items)
				{
					LogEntry log = (LogEntry)item.Tag;
					if(log.TimeGenerated > this._lastEventDate)
						item.ForeColor = PanelLogs.NewColor;
				}
		}

		private static ThreadResponse GetEventsCore(ThreadRequest request)
		{
			try
			{
				LogEntry[] entries = PluginWindows.GetEvents(request);
				return new ThreadResponse(entries);
			} catch(Exception exc)
			{
				exc.Data.Add(nameof(request.LogDisplayName), request.LogDisplayName);
				exc.Data.Add(nameof(request.MachineName), request.MachineName);
				return new ThreadResponse(exc);
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
				this.GetDateFilter(out _, out DateTime end);
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