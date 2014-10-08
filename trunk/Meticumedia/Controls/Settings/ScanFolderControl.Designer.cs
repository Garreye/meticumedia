// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class ScanFolderControl
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
            this.gbScanProperties = new System.Windows.Forms.GroupBox();
            this.chkAutoDeleteFolders = new System.Windows.Forms.CheckBox();
            this.chkAllowDeleting = new System.Windows.Forms.CheckBox();
            this.chkRecursive = new System.Windows.Forms.CheckBox();
            this.btnScanUpdate = new System.Windows.Forms.Button();
            this.rbScanMoveFrom = new System.Windows.Forms.RadioButton();
            this.rbScanCopyFrom = new System.Windows.Forms.RadioButton();
            this.btnScanDirSel = new System.Windows.Forms.Button();
            this.txtScanDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lvScanFolders = new System.Windows.Forms.ListView();
            this.colScanPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colScanMove = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colScanRecursive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colScanAlowDeleting = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colScanAutoDelete = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnScanClear = new System.Windows.Forms.Button();
            this.btnScanRemove = new System.Windows.Forms.Button();
            this.bntScanAdd = new System.Windows.Forms.Button();
            this.gbScanProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbScanProperties
            // 
            this.gbScanProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbScanProperties.Controls.Add(this.chkAutoDeleteFolders);
            this.gbScanProperties.Controls.Add(this.chkAllowDeleting);
            this.gbScanProperties.Controls.Add(this.chkRecursive);
            this.gbScanProperties.Controls.Add(this.btnScanUpdate);
            this.gbScanProperties.Controls.Add(this.rbScanMoveFrom);
            this.gbScanProperties.Controls.Add(this.rbScanCopyFrom);
            this.gbScanProperties.Controls.Add(this.btnScanDirSel);
            this.gbScanProperties.Controls.Add(this.txtScanDir);
            this.gbScanProperties.Controls.Add(this.label1);
            this.gbScanProperties.Enabled = false;
            this.gbScanProperties.Location = new System.Drawing.Point(3, 237);
            this.gbScanProperties.Name = "gbScanProperties";
            this.gbScanProperties.Size = new System.Drawing.Size(617, 88);
            this.gbScanProperties.TabIndex = 5;
            this.gbScanProperties.TabStop = false;
            this.gbScanProperties.Text = "Properties";
            // 
            // chkAutoDeleteFolders
            // 
            this.chkAutoDeleteFolders.AutoSize = true;
            this.chkAutoDeleteFolders.Location = new System.Drawing.Point(234, 63);
            this.chkAutoDeleteFolders.Name = "chkAutoDeleteFolders";
            this.chkAutoDeleteFolders.Size = new System.Drawing.Size(191, 17);
            this.chkAutoDeleteFolders.TabIndex = 8;
            this.chkAutoDeleteFolders.Text = "Automatically Delete Empty Folders";
            this.chkAutoDeleteFolders.UseVisualStyleBackColor = true;
            this.chkAutoDeleteFolders.CheckedChanged += new System.EventHandler(this.chkAutoDeleteFolders_CheckedChanged);
            // 
            // chkAllowDeleting
            // 
            this.chkAllowDeleting.AutoSize = true;
            this.chkAllowDeleting.Checked = true;
            this.chkAllowDeleting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAllowDeleting.Location = new System.Drawing.Point(86, 65);
            this.chkAllowDeleting.Name = "chkAllowDeleting";
            this.chkAllowDeleting.Size = new System.Drawing.Size(142, 17);
            this.chkAllowDeleting.TabIndex = 7;
            this.chkAllowDeleting.Text = "Allow Files to be Deleted";
            this.chkAllowDeleting.UseVisualStyleBackColor = true;
            this.chkAllowDeleting.CheckedChanged += new System.EventHandler(this.chkAllowDeleting_CheckedChanged);
            // 
            // chkRecursive
            // 
            this.chkRecursive.AutoSize = true;
            this.chkRecursive.Location = new System.Drawing.Point(6, 67);
            this.chkRecursive.Name = "chkRecursive";
            this.chkRecursive.Size = new System.Drawing.Size(74, 17);
            this.chkRecursive.TabIndex = 6;
            this.chkRecursive.Text = "Recursive";
            this.chkRecursive.UseVisualStyleBackColor = true;
            this.chkRecursive.CheckedChanged += new System.EventHandler(this.chkRecursive_CheckedChanged);
            // 
            // btnScanUpdate
            // 
            this.btnScanUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScanUpdate.Enabled = false;
            this.btnScanUpdate.Location = new System.Drawing.Point(536, 59);
            this.btnScanUpdate.Name = "btnScanUpdate";
            this.btnScanUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnScanUpdate.TabIndex = 5;
            this.btnScanUpdate.Text = "Update";
            this.btnScanUpdate.UseVisualStyleBackColor = true;
            this.btnScanUpdate.Click += new System.EventHandler(this.btnScanUpdate_Click);
            // 
            // rbScanMoveFrom
            // 
            this.rbScanMoveFrom.AutoSize = true;
            this.rbScanMoveFrom.Checked = true;
            this.rbScanMoveFrom.Location = new System.Drawing.Point(9, 44);
            this.rbScanMoveFrom.Name = "rbScanMoveFrom";
            this.rbScanMoveFrom.Size = new System.Drawing.Size(102, 17);
            this.rbScanMoveFrom.TabIndex = 4;
            this.rbScanMoveFrom.TabStop = true;
            this.rbScanMoveFrom.Text = "Move Files From";
            this.rbScanMoveFrom.UseVisualStyleBackColor = true;
            this.rbScanMoveFrom.CheckedChanged += new System.EventHandler(this.rbScanMoveFrom_CheckedChanged);
            // 
            // rbScanCopyFrom
            // 
            this.rbScanCopyFrom.AutoSize = true;
            this.rbScanCopyFrom.Location = new System.Drawing.Point(117, 44);
            this.rbScanCopyFrom.Name = "rbScanCopyFrom";
            this.rbScanCopyFrom.Size = new System.Drawing.Size(99, 17);
            this.rbScanCopyFrom.TabIndex = 3;
            this.rbScanCopyFrom.Text = "Copy Files From";
            this.rbScanCopyFrom.UseVisualStyleBackColor = true;
            // 
            // btnScanDirSel
            // 
            this.btnScanDirSel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScanDirSel.Location = new System.Drawing.Point(274, 16);
            this.btnScanDirSel.Name = "btnScanDirSel";
            this.btnScanDirSel.Size = new System.Drawing.Size(43, 23);
            this.btnScanDirSel.TabIndex = 2;
            this.btnScanDirSel.Text = "..";
            this.btnScanDirSel.UseVisualStyleBackColor = true;
            this.btnScanDirSel.Click += new System.EventHandler(this.btnScanDirSel_Click);
            // 
            // txtScanDir
            // 
            this.txtScanDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScanDir.Location = new System.Drawing.Point(61, 18);
            this.txtScanDir.Name = "txtScanDir";
            this.txtScanDir.Size = new System.Drawing.Size(207, 20);
            this.txtScanDir.TabIndex = 1;
            this.txtScanDir.TextChanged += new System.EventHandler(this.txtScanDir_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Directory";
            // 
            // lvScanFolders
            // 
            this.lvScanFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvScanFolders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colScanPath,
            this.colScanMove,
            this.colScanRecursive,
            this.colScanAlowDeleting,
            this.colScanAutoDelete});
            this.lvScanFolders.FullRowSelect = true;
            this.lvScanFolders.Location = new System.Drawing.Point(3, 32);
            this.lvScanFolders.MultiSelect = false;
            this.lvScanFolders.Name = "lvScanFolders";
            this.lvScanFolders.Size = new System.Drawing.Size(620, 199);
            this.lvScanFolders.TabIndex = 6;
            this.lvScanFolders.UseCompatibleStateImageBehavior = false;
            this.lvScanFolders.View = System.Windows.Forms.View.Details;
            this.lvScanFolders.SelectedIndexChanged += new System.EventHandler(this.lvScanFolders_SelectedIndexChanged);
            // 
            // colScanPath
            // 
            this.colScanPath.Text = "Path";
            this.colScanPath.Width = 389;
            // 
            // colScanMove
            // 
            this.colScanMove.Text = "Copy/Move";
            this.colScanMove.Width = 79;
            // 
            // colScanRecursive
            // 
            this.colScanRecursive.Text = "Recursive";
            this.colScanRecursive.Width = 64;
            // 
            // colScanAlowDeleting
            // 
            this.colScanAlowDeleting.Text = "Deleting Enabled";
            this.colScanAlowDeleting.Width = 91;
            // 
            // colScanAutoDelete
            // 
            this.colScanAutoDelete.Text = "Auto Del Empty Folders";
            // 
            // btnScanClear
            // 
            this.btnScanClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScanClear.Location = new System.Drawing.Point(548, 3);
            this.btnScanClear.Name = "btnScanClear";
            this.btnScanClear.Size = new System.Drawing.Size(75, 23);
            this.btnScanClear.TabIndex = 9;
            this.btnScanClear.Text = "Clear";
            this.btnScanClear.UseVisualStyleBackColor = true;
            this.btnScanClear.Click += new System.EventHandler(this.btnScanClear_Click);
            // 
            // btnScanRemove
            // 
            this.btnScanRemove.Location = new System.Drawing.Point(84, 3);
            this.btnScanRemove.Name = "btnScanRemove";
            this.btnScanRemove.Size = new System.Drawing.Size(75, 23);
            this.btnScanRemove.TabIndex = 8;
            this.btnScanRemove.Text = "Remove";
            this.btnScanRemove.UseVisualStyleBackColor = true;
            this.btnScanRemove.Click += new System.EventHandler(this.btnScanRemove_Click);
            // 
            // bntScanAdd
            // 
            this.bntScanAdd.Location = new System.Drawing.Point(3, 3);
            this.bntScanAdd.Name = "bntScanAdd";
            this.bntScanAdd.Size = new System.Drawing.Size(75, 23);
            this.bntScanAdd.TabIndex = 7;
            this.bntScanAdd.Text = "Add";
            this.bntScanAdd.UseVisualStyleBackColor = true;
            this.bntScanAdd.Click += new System.EventHandler(this.bntScanAdd_Click);
            // 
            // ScanFolderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbScanProperties);
            this.Controls.Add(this.lvScanFolders);
            this.Controls.Add(this.btnScanClear);
            this.Controls.Add(this.btnScanRemove);
            this.Controls.Add(this.bntScanAdd);
            this.Name = "ScanFolderControl";
            this.Size = new System.Drawing.Size(626, 328);
            this.gbScanProperties.ResumeLayout(false);
            this.gbScanProperties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbScanProperties;
        private System.Windows.Forms.CheckBox chkAllowDeleting;
        private System.Windows.Forms.CheckBox chkRecursive;
        private System.Windows.Forms.Button btnScanUpdate;
        private System.Windows.Forms.RadioButton rbScanMoveFrom;
        private System.Windows.Forms.RadioButton rbScanCopyFrom;
        private System.Windows.Forms.Button btnScanDirSel;
        private System.Windows.Forms.TextBox txtScanDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvScanFolders;
        private System.Windows.Forms.ColumnHeader colScanPath;
        private System.Windows.Forms.ColumnHeader colScanMove;
        private System.Windows.Forms.ColumnHeader colScanRecursive;
        private System.Windows.Forms.ColumnHeader colScanAlowDeleting;
        private System.Windows.Forms.Button btnScanClear;
        private System.Windows.Forms.Button btnScanRemove;
        private System.Windows.Forms.Button bntScanAdd;
        private System.Windows.Forms.CheckBox chkAutoDeleteFolders;
        private System.Windows.Forms.ColumnHeader colScanAutoDelete;
    }
}
