namespace Plugin.EventLog
{
	partial class PanelLogs
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStrip tsMain;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelLogs));
			System.Windows.Forms.ImageList ilLogIcons;
			this.tsbnDateFilter = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsbnTimer = new System.Windows.Forms.ToolStripButton();
			this.splitMain = new System.Windows.Forms.SplitContainer();
			this.lvData = new Plugin.EventLog.UI.LogListView();
			this.tabInfo = new System.Windows.Forms.TabControl();
			this.tabPageMessage = new System.Windows.Forms.TabPage();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.tabPageGrid = new System.Windows.Forms.TabPage();
			this.pgInfo = new System.Windows.Forms.PropertyGrid();
			this.tabPageBinary = new System.Windows.Forms.TabPage();
			this.bvBytes = new System.ComponentModel.Design.ByteViewer();
			this.refreshTimer = new System.Timers.Timer();
			tsMain = new System.Windows.Forms.ToolStrip();
			ilLogIcons = new System.Windows.Forms.ImageList(this.components);
			tsMain.SuspendLayout();
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			this.tabInfo.SuspendLayout();
			this.tabPageMessage.SuspendLayout();
			this.tabPageGrid.SuspendLayout();
			this.tabPageBinary.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.refreshTimer)).BeginInit();
			this.SuspendLayout();
			// 
			// tsMain
			// 
			tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbnDateFilter,
            this.tsbnTimer});
			tsMain.Location = new System.Drawing.Point(0, 0);
			tsMain.Name = "tsMain";
			tsMain.Size = new System.Drawing.Size(150, 25);
			tsMain.TabIndex = 0;
			// 
			// tsbnDateFilter
			// 
			this.tsbnDateFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsbnDateFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnDateFilter.Name = "tsbnDateFilter";
			this.tsbnDateFilter.Size = new System.Drawing.Size(13, 22);
			// 
			// tsbnTimer
			// 
			this.tsbnTimer.CheckOnClick = true;
			this.tsbnTimer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnTimer.Image = ((System.Drawing.Image)(resources.GetObject("tsbnTimer.Image")));
			this.tsbnTimer.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnTimer.Name = "tsbnTimer";
			this.tsbnTimer.Size = new System.Drawing.Size(23, 22);
			this.tsbnTimer.ToolTipText = "Timer";
			this.tsbnTimer.Click += new System.EventHandler(this.tsbnTimer_Click);
			// 
			// ilLogIcons
			// 
			ilLogIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilLogIcons.ImageStream")));
			ilLogIcons.TransparentColor = System.Drawing.Color.Magenta;
			ilLogIcons.Images.SetKeyName(0, "itemInformation.bmp");
			ilLogIcons.Images.SetKeyName(1, "itemWarning.bmp");
			ilLogIcons.Images.SetKeyName(2, "itemError.bmp");
			// 
			// splitMain
			// 
			this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitMain.Location = new System.Drawing.Point(0, 25);
			this.splitMain.Name = "splitMain";
			this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitMain.Panel1
			// 
			this.splitMain.Panel1.Controls.Add(this.lvData);
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add(this.tabInfo);
			this.splitMain.Size = new System.Drawing.Size(150, 125);
			this.splitMain.SplitterDistance = 74;
			this.splitMain.TabIndex = 2;
			this.splitMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.splitMain_MouseDoubleClick);
			// 
			// lvData
			// 
			this.lvData.AllowColumnReorder = true;
			this.lvData.DefaultImage = 0;
			this.lvData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvData.FullRowSelect = true;
			this.lvData.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvData.HideSelection = false;
			this.lvData.Location = new System.Drawing.Point(0, 0);
			this.lvData.Name = "lvData";
			this.lvData.Plugin = null;
			this.lvData.Size = new System.Drawing.Size(150, 74);
			this.lvData.StateImageList = ilLogIcons;
			this.lvData.TabIndex = 1;
			this.lvData.UseCompatibleStateImageBehavior = false;
			this.lvData.View = System.Windows.Forms.View.Details;
			this.lvData.SelectedIndexChanged += new System.EventHandler(this.lvData_SelectedIndexChanged);
			this.lvData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvData_KeyDown);
			// 
			// tabInfo
			// 
			this.tabInfo.Controls.Add(this.tabPageMessage);
			this.tabInfo.Controls.Add(this.tabPageGrid);
			this.tabInfo.Controls.Add(this.tabPageBinary);
			this.tabInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabInfo.Location = new System.Drawing.Point(0, 0);
			this.tabInfo.Name = "tabInfo";
			this.tabInfo.SelectedIndex = 0;
			this.tabInfo.Size = new System.Drawing.Size(150, 47);
			this.tabInfo.TabIndex = 1;
			// 
			// tabPageMessage
			// 
			this.tabPageMessage.Controls.Add(this.txtMessage);
			this.tabPageMessage.Location = new System.Drawing.Point(4, 22);
			this.tabPageMessage.Name = "tabPageMessage";
			this.tabPageMessage.Size = new System.Drawing.Size(142, 21);
			this.tabPageMessage.TabIndex = 2;
			this.tabPageMessage.Text = "Message";
			this.tabPageMessage.UseVisualStyleBackColor = true;
			// 
			// txtMessage
			// 
			this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtMessage.Location = new System.Drawing.Point(0, 0);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.ReadOnly = true;
			this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtMessage.Size = new System.Drawing.Size(142, 21);
			this.txtMessage.TabIndex = 0;
			// 
			// tabPageGrid
			// 
			this.tabPageGrid.Controls.Add(this.pgInfo);
			this.tabPageGrid.Location = new System.Drawing.Point(4, 22);
			this.tabPageGrid.Name = "tabPageGrid";
			this.tabPageGrid.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageGrid.Size = new System.Drawing.Size(142, 21);
			this.tabPageGrid.TabIndex = 0;
			this.tabPageGrid.Text = "Grid";
			this.tabPageGrid.UseVisualStyleBackColor = true;
			// 
			// pgInfo
			// 
			this.pgInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgInfo.HelpVisible = false;
			this.pgInfo.Location = new System.Drawing.Point(3, 3);
			this.pgInfo.Name = "pgInfo";
			this.pgInfo.Size = new System.Drawing.Size(136, 15);
			this.pgInfo.TabIndex = 0;
			this.pgInfo.ToolbarVisible = false;
			// 
			// tabPageBinary
			// 
			this.tabPageBinary.Controls.Add(this.bvBytes);
			this.tabPageBinary.Location = new System.Drawing.Point(4, 22);
			this.tabPageBinary.Name = "tabPageBinary";
			this.tabPageBinary.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageBinary.Size = new System.Drawing.Size(142, 21);
			this.tabPageBinary.TabIndex = 1;
			this.tabPageBinary.Text = "Binary";
			this.tabPageBinary.UseVisualStyleBackColor = true;
			// 
			// bvBytes
			// 
			this.bvBytes.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
			this.bvBytes.ColumnCount = 1;
			this.bvBytes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.bvBytes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.bvBytes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.bvBytes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.bvBytes.Location = new System.Drawing.Point(3, 3);
			this.bvBytes.Name = "bvBytes";
			this.bvBytes.RowCount = 1;
			this.bvBytes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.bvBytes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.bvBytes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.bvBytes.Size = new System.Drawing.Size(136, 15);
			this.bvBytes.TabIndex = 0;
			// 
			// refreshTimer
			// 
			this.refreshTimer.SynchronizingObject = this;
			this.refreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.refreshTimer_Elapsed);
			// 
			// PanelLogs
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitMain);
			this.Controls.Add(tsMain);
			this.Name = "PanelLogs";
			tsMain.ResumeLayout(false);
			tsMain.PerformLayout();
			this.splitMain.Panel1.ResumeLayout(false);
			this.splitMain.Panel2.ResumeLayout(false);
			this.splitMain.ResumeLayout(false);
			this.tabInfo.ResumeLayout(false);
			this.tabPageMessage.ResumeLayout(false);
			this.tabPageMessage.PerformLayout();
			this.tabPageGrid.ResumeLayout(false);
			this.tabPageBinary.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.refreshTimer)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Plugin.EventLog.UI.LogListView lvData;
		private System.Windows.Forms.SplitContainer splitMain;
		private System.Windows.Forms.PropertyGrid pgInfo;
		private System.Windows.Forms.ToolStripDropDownButton tsbnDateFilter;
		private System.Timers.Timer refreshTimer;
		private System.Windows.Forms.ToolStripButton tsbnTimer;
		private System.Windows.Forms.TabControl tabInfo;
		private System.Windows.Forms.TabPage tabPageGrid;
		private System.Windows.Forms.TabPage tabPageBinary;
		private System.ComponentModel.Design.ByteViewer bvBytes;
		private System.Windows.Forms.TabPage tabPageMessage;
		private System.Windows.Forms.TextBox txtMessage;
	}
}
