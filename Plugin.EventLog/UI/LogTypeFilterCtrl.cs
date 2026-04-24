using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Plugin.EventLog.UI
{
	/// <summary>A <see cref="ToolStripDropDownButton"/> that manages the log-type filter, keeping the dropdown open during item selection and raising <see cref="SelectionChanged"/> when closed with a modified selection.</summary>
	internal class LogTypeFilterCtrl : ToolStripDropDownButton
	{
		private EventLogEntryType[] _allTypes;
		private Boolean _pendingRefresh;

		/// <summary>Raised when the dropdown closes and the selected log types have been modified.</summary>
		public event EventHandler SelectionChanged;

		/// <summary>Gets or sets the currently selected event log entry types; setting this value updates the dropdown items without raising <see cref="SelectionChanged"/>.</summary>
		public EventLogEntryType[] SelectedLogTypes
		{
			get
			{
				List<EventLogEntryType> result = new List<EventLogEntryType>();
				for(Int32 index = 0; index < base.DropDownItems.Count; index++)
					if(((ToolStripMenuItem)base.DropDownItems[index]).Checked)
						result.Add(this._allTypes[index]);
				return result.ToArray();
			}
			set
			{
				for(Int32 index = 0; index < base.DropDownItems.Count; index++)
				{
					ToolStripMenuItem item = (ToolStripMenuItem)base.DropDownItems[index];
					Boolean isChecked = value == null || Array.IndexOf(value, this._allTypes[index]) >= 0;
					item.CheckedChanged -= new EventHandler(this.Item_CheckedChanged);
					item.Checked = isChecked;
					item.CheckedChanged += new EventHandler(this.Item_CheckedChanged);
				}
				this.UpdateCaption();
			}
		}

		/// <summary>Initializes a new instance of <see cref="LogTypeFilterCtrl"/>.</summary>
		public LogTypeFilterCtrl()
			: base() { }

		/// <summary>Populates the dropdown with one item per <see cref="EventLogEntryType"/> value and wires up all event handlers.</summary>
		/// <param name="selectedTypes">The initially selected types; pass <see langword="null"/> to select all.</param>
		public void Initialize(EventLogEntryType[] selectedTypes)
		{
			this._allTypes = (EventLogEntryType[])Enum.GetValues(typeof(EventLogEntryType));

			for(Int32 index = 0; index < this._allTypes.Length; index++)
			{
				ToolStripMenuItem item = new ToolStripMenuItem(this._allTypes[index].ToString())
				{
					CheckOnClick = true,
					Checked = selectedTypes == null || Array.IndexOf(selectedTypes, this._allTypes[index]) >= 0,
				};
				item.CheckedChanged += new EventHandler(this.Item_CheckedChanged);
				base.DropDownItems.Add(item);
			}

			base.DropDown.Closing += new ToolStripDropDownClosingEventHandler(this.DropDown_Closing);
			base.DropDownClosed += new EventHandler(this.Button_DropDownClosed);

			this.UpdateCaption();
		}

		private void Item_CheckedChanged(Object sender, EventArgs e)
		{
			ToolStripMenuItem changed = (ToolStripMenuItem)sender;
			if(!changed.Checked)
			{
				Boolean anyOtherChecked = false;
				foreach(ToolStripMenuItem menuItem in base.DropDownItems)
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
			this.UpdateCaption();
			this._pendingRefresh = true;
		}

		private void UpdateCaption()
		{
			List<String> selected = new List<String>();
			Boolean allChecked = true;
			foreach(ToolStripMenuItem item in base.DropDownItems)
				if(item.Checked)
					selected.Add(item.Text);
				else
					allChecked = false;

			base.Text = allChecked
				? "All"
				: (selected.Count == 0 ? "None" : String.Join(", ", selected));
		}

		private void DropDown_Closing(Object sender, ToolStripDropDownClosingEventArgs e)
			=> e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked;

		private void Button_DropDownClosed(Object sender, EventArgs e)
		{
			if(!this._pendingRefresh)
				return;
			this._pendingRefresh = false;
			this.SelectionChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
