using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AlphaOmega.Windows.Forms;
using Plugin.EventLog.Data;

namespace Plugin.EventLog.UI
{
	internal class DataListView : DbListView
	{
		public class LateBoundEventArgs : EventArgs
		{
			public Int32 PageNumber { get; private set; }
			public Int32 PageSize { get; set; }
			public IList Data { get; set; }

			public LateBoundEventArgs(Int32 pageNumber, Int32 pageSize)
			{
				this.PageNumber = pageNumber;
				this.PageSize = pageSize;
			}

			public override String ToString()
				=> String.Format("{0} : {{Size: {1} Page: {2}}}", base.GetType().Name, this.PageSize, this.PageNumber);
		}

		#region Fields
		private PluginWindows _plugin;
		private ContextMenuStrip _cmsAction;
		private ToolStripMenuItem _tsmiCopy;
		private ToolStripMenuItem _tsmiGroupBy;
		private ToolStripMenuItem _tsmiRemove;
		private ToolStripMenuItem _tsmiRemoveSelected;
		private ToolStripMenuItem _tsmiSelect;
		private ContextMenuStrip _cmsHeader;
		public event EventHandler<EventArgs> RemoveSelectedItems;
		public event EventHandler<LateBoundEventArgs> LoadVirtualItems;
		#endregion Fields

		#region Properties
		private IList VirtualDataArray { get; set; }

		public PluginWindows Plugin
		{
			get => this._plugin;
			set
			{
				if(this._plugin != null)
					throw new InvalidOperationException("Plugin already set");
				this._plugin = value;
			}
		}

		public Object SelectedObject
		{
			get
			{
				Object result = null;
				if(base.SelectedIndices.Count > 0)
					result = base.VirtualMode
						? this.VirtualDataArray[base.SelectedIndices[0]]
						: base.SelectedItems[0].Tag;
				return result;
			}
		}

		public Int32 ItemsCount
		{
			get
			{
				if(!base.VirtualMode)
					return base.Items.Count;
				if(this.VirtualDataArray != null)
					return this.VirtualDataArray.Count;
				return 0;
			}
		}

		public Int32 DefaultImage { get; set; }
		#endregion Properties

		public virtual Int32 GetImageIndex(Object item)
			=> this.DefaultImage;

		public DataListView()
		{
			base.KeyDown += new KeyEventHandler(this.DataListView_KeyDown);
			base.ColumnReordered += new ColumnReorderedEventHandler(this.DataListView_ColumnReordered);
			base.VirtualItemsSelectionRangeChanged += new ListViewVirtualItemsSelectionRangeChangedEventHandler(this.DataListView_VirtualItemsSelectionRangeChanged);
			base.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(this.DataListView_RetrieveVirtualItem);
			this.InitializeComponent();
		}

		public new void Clear()
		{
			if(base.InvokeRequired)
			{
				base.Invoke(new MethodInvoker(delegate { this.Clear(); }));
				return;
			}

			base.SuspendLayout();
			try
			{
				this.VirtualDataArray = null;
				base.Clear();
				base.SelectedIndices.Clear();
				base.Groups.Clear();
				base.Columns.Clear();
			} finally
			{
				base.ResumeLayout();
			}
		}

		private void InitializeComponent()
		{
			this._cmsAction = new ContextMenuStrip();
			this._cmsAction.SuspendLayout();
			this._cmsHeader = new ContextMenuStrip();
			this._cmsHeader.SuspendLayout();
			this._tsmiCopy = new ToolStripMenuItem();
			this._tsmiGroupBy = new ToolStripMenuItem();
			this._tsmiRemove = new ToolStripMenuItem();
			this._tsmiRemoveSelected = new ToolStripMenuItem();
			this._tsmiSelect = new ToolStripMenuItem();
			this._cmsHeader.Name = "cmsHeader";
			this._cmsHeader.Size = new Size(61, 4);
			this._cmsHeader.ItemClicked += new ToolStripItemClickedEventHandler(this.cmsHeader_ItemClicked);
			this._cmsAction.Items.AddRange(new ToolStripItem[]
			{
				this._tsmiCopy,
				this._tsmiGroupBy,
				this._tsmiRemove,
				this._tsmiSelect
			});
			this._cmsAction.Name = "cmsAction";
			this._cmsAction.Size = new Size(124, 114);
			this._cmsAction.ItemClicked += new ToolStripItemClickedEventHandler(this._cmsAction_ItemClicked);
			this._cmsAction.Opening += new CancelEventHandler(this._cmsAction_Opening);
			this._tsmiCopy.Name = "tsmiCopy";
			this._tsmiCopy.Size = new Size(123, 22);
			this._tsmiCopy.Text = "Copy";
			this._tsmiCopy.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.tsmiCopy_DropDownItemClicked);
			this._tsmiGroupBy.Name = "tsmiGroupBy";
			this._tsmiGroupBy.Size = new Size(123, 22);
			this._tsmiGroupBy.Text = "Group By";
			this._tsmiGroupBy.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.tsmiGroupBy_DropDownItemClicked);
			this._tsmiRemoveSelected.Name = "tsmiRemoveSelected";
			this._tsmiRemoveSelected.Size = new Size(118, 22);
			this._tsmiRemoveSelected.Text = "&Selected";
			this._tsmiRemove.DropDownItems.AddRange(new ToolStripItem[]
			{
				this._tsmiRemoveSelected
			});
			this._tsmiRemove.Name = "tsmiRemove";
			this._tsmiRemove.Size = new Size(123, 22);
			this._tsmiRemove.Text = "&Remove";
			this._tsmiRemove.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.tsmiRemove_DropDownItemClicked);
			this._tsmiSelect.Name = "tsmiSelect";
			this._tsmiSelect.Size = new Size(123, 22);
			this._tsmiSelect.Text = "&Select";
			this._tsmiSelect.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.tsmiSelect_DropDownItemClicked);
			base.ContextMenuStrip = this._cmsAction;
			this._cmsAction.ResumeLayout(false);
			this._cmsHeader.ResumeLayout(false);
		}

		private void cmsHeader_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			this._cmsHeader.Visible = false;

			ToolStripMenuItem clickedItem = (ToolStripMenuItem)e.ClickedItem;
			if(clickedItem.Checked)
			{
				foreach(ColumnHeader column in base.Columns)
					if(column.Text == e.ClickedItem.Text)
					{
						base.Columns.Remove(column);
						break;
					}
			} else
			{
				ColumnHeader column = new ColumnHeader
				{
					Text = e.ClickedItem.Text
				};
				base.Columns.Add(column);
				if(!base.VirtualMode)
					foreach(ListViewItem item in base.Items)
					{
						if(item.SubItems.Count <= column.Index)
							item.SubItems.Add(String.Empty);
						item.SubItems[column.Index].Text = DataListView.GetReflectedText(item.Tag, column.Text);
					}
				column.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			}

			String[] results = base.Columns.Count == this.GetItem(0).GetType().GetProperties().Length
				? new String[] { }
				: new String[base.Columns.Count];

			for(Int32 loop = 0; loop < results.Length; loop++)
				results[loop] = base.Columns[loop].Text;

			this.Plugin.Settings.ColumnVisible = ObjectPropertyParser.SetPropertiesToString(this.GetItem(0).GetType(), results, this.Plugin.Settings.ColumnVisible);
			this.Plugin.HostWindows.Plugins.Settings(this.Plugin).SaveAssemblyParameters();
		}

		private void _cmsAction_Opening(Object sender, CancelEventArgs e)
		{
			IntPtr hHeader = Native.SendMessage(base.Handle, Native.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
			Native.RECT rect;
			if(Native.GetWindowRect(new HandleRef(this, hHeader), out rect))
			{
				Rectangle rectHeader = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
				if(rectHeader.Contains(Control.MousePosition))
				{
					e.Cancel = true;
					this._cmsHeader.Items.Clear();
					if(this.ItemsCount > 0)
					{
						Type type = this.GetItem(0).GetType();
						PropertyInfo[] properties = type.GetProperties();
						for(Int32 i = 0; i < properties.Length; i++)
						{
							PropertyInfo property = properties[i];
							Boolean isVisible = false;
							foreach(ColumnHeader column in base.Columns)
								if(column.Text == property.Name)
								{
									isVisible = true;
									break;
								}

							this._cmsHeader.Items.Add(new ToolStripMenuItem(property.Name)
							{
								CheckOnClick = true,
								Checked = isVisible,
								Tag = type
							});
						}
					}
					this._cmsHeader.Show(Control.MousePosition);
					return;
				}
			}
			this._tsmiRemove.Visible = base.SelectedIndices.Count > 0;
			this._tsmiSelect.Visible = base.SelectedIndices.Count == 1;
			this._tsmiCopy.Visible = base.SelectedIndices.Count == 1;

			if(base.SelectedIndices.Count == 1)
			{
				this._tsmiSelect.DropDownItems.Clear();
				this._tsmiCopy.DropDownItems.Clear();
				Object row = this.SelectedObject;
				foreach(ColumnHeader column2 in base.Columns)
				{
					String text = column2.Text;
					
					/*PropertyInfo property2 = row.GetType().GetProperty(column2.Text);
					Object dummy = property2.GetValue(row, null);
					text = (dummy == null) ? "<null>" : dummy.ToString();
					if(text.Length > 33)
						text = text.Substring(0, 15) + "..." + text.Substring(text.Length - 15);*/

					this._tsmiSelect.DropDownItems.Add(new ToolStripMenuItem(text)
					{
						Tag = column2.Index
					});
					this._tsmiCopy.DropDownItems.Add(new ToolStripMenuItem(text)
					{
						Tag = column2.Index
					});
				}
			}
			this._tsmiGroupBy.Enabled = (base.Columns.Count > 0);
		}

		private void _cmsAction_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == this._tsmiSelect)
				return;
			if(e.ClickedItem == this._tsmiCopy)
				return;
			if(e.ClickedItem == this._tsmiRemove)
				return;
			if(e.ClickedItem == this._tsmiGroupBy)
				return;
			throw new NotImplementedException();
		}

		private void tsmiSelect_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			this._cmsAction.Visible = false;
			Int32 columnIndex = (Int32)e.ClickedItem.Tag;
			String column = base.Columns[columnIndex].Text;
			Object text = this.SelectedObject.GetType().InvokeMember(column, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, this.SelectedObject, null);
			base.SelectedIndices.Clear();
			if(text != null)
			{
				for(Int32 loop = 0; loop < this.ItemsCount; loop++)
				{
					Object item = this.GetItem(loop);
					if(text.Equals(item.GetType().InvokeMember(column, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, item, null)))
						base.SelectedIndices.Add(loop);
				}
			}
		}

		private void tsmiCopy_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			this._cmsAction.Visible = false;
			Int32 columnIndex = (Int32)e.ClickedItem.Tag;
			String column = base.Columns[columnIndex].Text;
			Object text = this.SelectedObject.GetType().InvokeMember(column, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, this.SelectedObject, null);
			base.SelectedIndices.Clear();
			if(text != null)
				Clipboard.SetText(text.ToString());
		}

		private void tsmiGroupBy_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			this._cmsAction.Visible = false;
			base.Groups.Clear();
			ToolStripMenuItem clickedItem = (ToolStripMenuItem)e.ClickedItem;
			if(clickedItem.Checked)
			{
				if(!base.VirtualMode)
					foreach(ListViewItem item in base.Items)
						item.Group = null;

				base.Groups.Clear();
				clickedItem.Checked = false;
			} else
			{
				foreach(ToolStripMenuItem item2 in this._tsmiGroupBy.DropDownItems)
					item2.Checked = (item2 == e.ClickedItem);

				/*ColumnHeader column = null;
				foreach(ColumnHeader columnItem in base.Columns)
					if(columnItem.Text == e.ClickedItem.Text)
					{
						column = columnItem;
						break;
					}*/

				if(/*column != null && */!base.VirtualMode)
					for(Int32 loop = 0; loop < this.ItemsCount; loop++)
					{
						Object row = this.GetItem(loop);
						base.Items[loop].Group = this.GetOrCreateGroup(row, e.ClickedItem.Text);
					}
			}
		}

		private void tsmiRemove_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			this._cmsAction.Visible = false;
			if(e.ClickedItem != this._tsmiRemoveSelected)
				throw new NotImplementedException(e.ClickedItem.ToString());

			this.RemoveSelectedItems?.Invoke(this, e);
		}

		private void DataListView_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.Back:
			case Keys.Delete:
				if(base.SelectedIndices.Count > 0)
				{
					this.tsmiRemove_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(this._tsmiRemoveSelected));
					e.Handled = true;
				}
				break;
			case Keys.A | Keys.Control:
				for(Int32 loop = 0; loop < base.Items.Count; loop++)
					base.Items[loop].Selected = true;
				e.Handled = true;
				break;
			}
		}

		private void DataListView_VirtualItemsSelectionRangeChanged(Object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
		{
			if(e.IsSelected)
				base.OnSelectedIndexChanged(e);
		}

		private void DataListView_ColumnReordered(Object sender, ColumnReorderedEventArgs e)
		{
			if(this.ItemsCount > 0 && e.NewDisplayIndex != e.OldDisplayIndex)
			{
				String[] results = new String[base.Columns.Count];
				foreach(ColumnHeader column in base.Columns)
				{
					Int32 index = column.DisplayIndex;
					if(index == e.OldDisplayIndex)
						index = e.NewDisplayIndex;
					else if(e.NewDisplayIndex > e.OldDisplayIndex)
					{
						if(index <= e.NewDisplayIndex && index > e.OldDisplayIndex)
							index--;
					} else if(e.NewDisplayIndex < e.OldDisplayIndex && index >= e.NewDisplayIndex && index < e.OldDisplayIndex)
						index++;

					results[index] = column.Text;
				}
				this.Plugin.Settings.ColumnOrder = ObjectPropertyParser.SetPropertiesToString(this.GetItem(0).GetType(), results, this.Plugin.Settings.ColumnOrder);
				this.Plugin.HostWindows.Plugins.Settings(this.Plugin).SaveAssemblyParameters();
			}
		}

		private void DataListView_RetrieveVirtualItem(Object sender, RetrieveVirtualItemEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException("e");

			Object row = this.GetItem(e.ItemIndex);
			if(row == null)
			{
				if(this.LoadVirtualItems == null)
					throw new ArgumentNullException("e.ItemIndex", "LoadVirtualItems event not attached");
				if(this.VirtualDataArray == null)
				{
					e.Item = new ListViewItem();
					return;
				}
				Int32 itemIndex = e.ItemIndex;
				Int32 total = this.ItemsCount;
				Int32 rowsCount = base.GetVisibleRowsCount();
				Int32 pageSize = rowsCount * 2;
				while(itemIndex > 0 && itemIndex > e.ItemIndex - rowsCount && this.GetItem(itemIndex - 1) == null)
					itemIndex--;

				Int32 pageNumber = itemIndex / pageSize;
				LateBoundEventArgs args = new LateBoundEventArgs(pageNumber, pageSize);
				this.Plugin.Trace.TraceEvent(TraceEventType.Verbose, 1, args.ToString() + " Index: " + e.ItemIndex);
				this.LoadVirtualItems(sender, args);

				Int32 loop = 0;
				while(loop < args.Data.Count && itemIndex + loop < total)
				{
					this.SetItem(itemIndex + loop, args.Data[loop]);
					loop++;
				}
				row = this.GetItem(e.ItemIndex);
				if(row == null)
					this.Plugin.Trace.TraceData(TraceEventType.Error, 10, new ArgumentNullException("e.ItemIndex", String.Format("Row {0} not loaded", e.ItemIndex)));
			}
			e.Item = this.CreateListItem(row);
		}

		public Object GetItem(Int32 index)
			=> base.VirtualMode
				? (this.VirtualDataArray == null ? null : this.VirtualDataArray[index])
				: base.Items[index].Tag;

		public void SetItem(Int32 index, Object value)
		{
			if(base.VirtualMode)
				this.VirtualDataArray[index] = value;
			else
				base.Items[index].Tag = value;
		}

		public void RemoveSelectedFromList()
		{
			if(base.VirtualMode)
			{
				ArrayList arr = new ArrayList(this.VirtualDataArray);
				foreach(Int32 index in base.SelectedIndices)
					arr.RemoveAt(index);

				this.FillList(arr.ToArray());
			} else
			{
				while(base.SelectedItems.Count > 0)
					base.SelectedItems[0].Remove();
			}
		}

		public void FillList(IList items)
		{
			if(base.InvokeRequired)
			{
				base.Invoke(new MethodInvoker(delegate { this.FillList(items); }));
				return;
			}

			base.SuspendLayout();

			if(items.Count > 0 && base.Columns.Count == 0)
			{
				Type objectType = items[0].GetType();
				String[] columns = ObjectPropertyParser.GetPropertiesFromString(objectType, this.Plugin.Settings.ColumnOrder);
				if(columns.Length > 0)
				{
					foreach(String column in columns)
						base.Columns.Add(column);
				} else
				{
					PropertyInfo[] properties = objectType.GetProperties();
					foreach(PropertyInfo property in properties)
						base.Columns.Add(property.Name);
				}

				columns = ObjectPropertyParser.GetPropertiesFromString(objectType, this.Plugin.Settings.ColumnVisible);
				if(columns.Length > 0)
				{
					foreach(ColumnHeader column in base.Columns)
					{
						Boolean isVisible = false;
						foreach(String columnName in columns)
							if(column.Text == columnName)
							{
								isVisible = true;
								break;
							}
						if(!isVisible)
							base.Columns.Remove(column);
					}
				}

				if(!objectType.Name.Equals(this._tsmiGroupBy.Tag))
				{//Проверка на необходимость переписать свойства у меню группировки
					this._tsmiGroupBy.DropDownItems.Clear();

					foreach(PropertyInfo property in objectType.GetProperties())
						this._tsmiGroupBy.DropDownItems.Add(new ToolStripMenuItem(property.Name) { Name = objectType.Name, });
					this._tsmiGroupBy.Tag = objectType.Name;
				}
			}

			if(base.VirtualMode)
			{
				if(this.VirtualDataArray == null)
				{
					this.VirtualDataArray = items;
					base.VirtualListSize = items.Count;
				} else
				{
					Object[] arrItems = new Object[this.VirtualDataArray.Count + items.Count];
					items.CopyTo(arrItems, 0);
					this.VirtualDataArray.CopyTo(arrItems, items.Count);

					this.VirtualDataArray = arrItems;
					base.VirtualListSize = arrItems.Length;
				}
			} else
			{
				//List<ListViewItem> itemsToAdd = new List<ListViewItem>();
				foreach(Object row in items)
				{
					ListViewItem item = this.CreateListItem(row);
					//itemsToAdd.Add(item);
					base.Items.Insert(0, item);
				}
				//base.Items.AddRange(itemsToAdd.ToArray());
			}

			if(items.Count > 0)
				base.EnsureVisible(0);
			base.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}

		private ListViewItem CreateListItem(Object row)
		{
			ListViewItem result = new ListViewItem
			{
				Tag = row,
			};
			result.ImageIndex = result.StateImageIndex = this.GetImageIndex(row);

			String[] subItems = Array.ConvertAll<String, String>(new String[base.Columns.Count], a => String.Empty);
			result.SubItems.AddRange(subItems);

			foreach(ColumnHeader column in base.Columns)
				result.SubItems[column.Index].Text = DataListView.GetReflectedText(row, column.Text);

			String groupProperty = null;
			foreach(ToolStripMenuItem groupItem in this._tsmiGroupBy.DropDownItems)
			{
				if(groupItem.Checked)
				{
					groupProperty = groupItem.Text;
					break;
				}
			}

			if(groupProperty != null)
				foreach(ColumnHeader column in base.Columns)
					if(column.Text == groupProperty)
					{
						result.Group = this.GetOrCreateGroup(row, groupProperty);
						break;
					}
			return result;
		}

		private static String GetReflectedText(Object row, String propertyName)
		{
			PropertyInfo property = row.GetType().GetProperty(propertyName);
			if(!property.CanRead)
				return null;//TODO: Необходимо разобраться с пропертями из которых нельзя читать
			Object val = property.GetValue(row, null);
			if(val == null)
				return String.Empty;
			else if(property.PropertyType.IsPrimitive && val is IFormattable)
			{
				String text = ((IFormattable)val).ToString("#,##0", CultureInfo.CurrentUICulture);
				return text.Length == 0 ? val.ToString() : text;
			} else
				return val.ToString();
		}

		private ListViewGroup GetOrCreateGroup(Object row, String propertyName)
		{
			Object value = row.GetType().InvokeMember(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, row, null);
			String groupName = (value == null) ? "<null>" : value.ToString();

			foreach(ListViewGroup groupItem in base.Groups)
				if(groupItem.Header == groupName)
					return groupItem;

			ListViewGroup group = new ListViewGroup(groupName);
			base.Groups.Add(group);
			return group;
		}
	}
}