
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
            this.lstViewEntries = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnDuration = new System.Windows.Forms.ColumnHeader();
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 384);
            this.Controls.Add(this.lstViewEntries);
            this.MinimumSize = new System.Drawing.Size(490, 423);
            this.Name = "MainForm";
            this.Text = "Activity Tracker";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstViewEntries;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnDuration;
    }
}

