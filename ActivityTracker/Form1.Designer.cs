
namespace ActivityTracker
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lstViewEntries = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnDuration = new System.Windows.Forms.ColumnHeader();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstViewEntries
            // 
            this.lstViewEntries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnDuration});
            this.lstViewEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstViewEntries.FullRowSelect = true;
            this.lstViewEntries.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstViewEntries.HideSelection = false;
            this.lstViewEntries.Location = new System.Drawing.Point(0, 0);
            this.lstViewEntries.Name = "lstViewEntries";
            this.lstViewEntries.OwnerDraw = true;
            this.lstViewEntries.Size = new System.Drawing.Size(474, 384);
            this.lstViewEntries.TabIndex = 0;
            this.lstViewEntries.UseCompatibleStateImageBehavior = false;
            this.lstViewEntries.View = System.Windows.Forms.View.Details;
            this.lstViewEntries.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lstViewEntries_DrawColumnHeader);
            this.lstViewEntries.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lstViewEntries_DrawItem);
            // 
            // columnName
            // 
            this.columnName.Name = "columnName";
            this.columnName.Text = "Name";
            this.columnName.Width = 160;
            // 
            // columnDuration
            // 
            this.columnDuration.Name = "columnDuration";
            this.columnDuration.Text = "Duration";
            this.columnDuration.Width = 250;
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "notifyIcon";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemExit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(94, 26);
            // 
            // toolStripMenuItemExit
            // 
            this.toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            this.toolStripMenuItemExit.Size = new System.Drawing.Size(93, 22);
            this.toolStripMenuItemExit.Text = "Exit";
            this.toolStripMenuItemExit.Click += new System.EventHandler(this.toolStripMenuItemExit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 384);
            this.Controls.Add(this.lstViewEntries);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(490, 423);
            this.Name = "MainForm";
            this.Text = "Activity Tracker";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstViewEntries;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnDuration;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExit;
    }
}

