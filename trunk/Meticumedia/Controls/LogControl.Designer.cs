// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class LogControl
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
            if (disposing && (components != null))
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
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.chkDelFilter = new System.Windows.Forms.CheckBox();
            this.chkMoveCopyFilter = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkRenameFilter = new System.Windows.Forms.CheckBox();
            this.lvLog = new Meticumedia.OrgItemListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(512, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(593, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 14;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chkDelFilter
            // 
            this.chkDelFilter.AutoSize = true;
            this.chkDelFilter.Checked = true;
            this.chkDelFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDelFilter.Location = new System.Drawing.Point(238, 9);
            this.chkDelFilter.Name = "chkDelFilter";
            this.chkDelFilter.Size = new System.Drawing.Size(57, 17);
            this.chkDelFilter.TabIndex = 25;
            this.chkDelFilter.Text = "Delete";
            this.chkDelFilter.UseVisualStyleBackColor = true;
            this.chkDelFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // chkMoveCopyFilter
            // 
            this.chkMoveCopyFilter.AutoSize = true;
            this.chkMoveCopyFilter.Checked = true;
            this.chkMoveCopyFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMoveCopyFilter.Location = new System.Drawing.Point(78, 9);
            this.chkMoveCopyFilter.Name = "chkMoveCopyFilter";
            this.chkMoveCopyFilter.Size = new System.Drawing.Size(82, 17);
            this.chkMoveCopyFilter.TabIndex = 24;
            this.chkMoveCopyFilter.Text = "Move/Copy";
            this.chkMoveCopyFilter.UseVisualStyleBackColor = true;
            this.chkMoveCopyFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Display Filter:";
            // 
            // chkRenameFilter
            // 
            this.chkRenameFilter.AutoSize = true;
            this.chkRenameFilter.Checked = true;
            this.chkRenameFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRenameFilter.Location = new System.Drawing.Point(166, 9);
            this.chkRenameFilter.Name = "chkRenameFilter";
            this.chkRenameFilter.Size = new System.Drawing.Size(66, 17);
            this.chkRenameFilter.TabIndex = 26;
            this.chkRenameFilter.Text = "Rename";
            this.chkRenameFilter.UseVisualStyleBackColor = true;
            this.chkRenameFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // lvLog
            // 
            this.lvLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvLog.ColumnSort = Meticumedia.OrgColumnType.Source_Folder;
            this.lvLog.FullRowSelect = true;
            this.lvLog.Location = new System.Drawing.Point(6, 32);
            this.lvLog.Name = "lvLog";
            this.lvLog.Size = new System.Drawing.Size(662, 509);
            this.lvLog.TabIndex = 12;
            this.lvLog.UseCompatibleStateImageBehavior = false;
            this.lvLog.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "File";
            this.columnHeader1.Width = 249;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Action";
            this.columnHeader2.Width = 98;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Destination";
            this.columnHeader3.Width = 284;
            // 
            // LogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkRenameFilter);
            this.Controls.Add(this.chkDelFilter);
            this.Controls.Add(this.chkMoveCopyFilter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lvLog);
            this.Name = "LogControl";
            this.Size = new System.Drawing.Size(671, 544);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OrgItemListView lvLog;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.CheckBox chkDelFilter;
        private System.Windows.Forms.CheckBox chkMoveCopyFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkRenameFilter;
    }
}
