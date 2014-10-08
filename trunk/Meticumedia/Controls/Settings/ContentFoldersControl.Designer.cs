// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class ContentFoldersControl
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
            this.tvFolders = new System.Windows.Forms.TreeView();
            this.gbMfProperties = new System.Windows.Forms.GroupBox();
            this.btnAddAllSubs = new System.Windows.Forms.Button();
            this.btnMfAddSubfolder = new System.Windows.Forms.Button();
            this.chkMfDefault = new System.Windows.Forms.CheckBox();
            this.chkMfAllowOrg = new System.Windows.Forms.CheckBox();
            this.btnMfUpdate = new System.Windows.Forms.Button();
            this.btMfClear = new System.Windows.Forms.Button();
            this.btnMfRemove = new System.Windows.Forms.Button();
            this.btnMfAdd = new System.Windows.Forms.Button();
            this.gbMfProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvFolders
            // 
            this.tvFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvFolders.HideSelection = false;
            this.tvFolders.Location = new System.Drawing.Point(6, 32);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.Size = new System.Drawing.Size(624, 234);
            this.tvFolders.TabIndex = 15;
            this.tvFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFolders_AfterSelect);
            // 
            // gbMfProperties
            // 
            this.gbMfProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMfProperties.Controls.Add(this.btnAddAllSubs);
            this.gbMfProperties.Controls.Add(this.btnMfAddSubfolder);
            this.gbMfProperties.Controls.Add(this.chkMfDefault);
            this.gbMfProperties.Controls.Add(this.chkMfAllowOrg);
            this.gbMfProperties.Controls.Add(this.btnMfUpdate);
            this.gbMfProperties.Enabled = false;
            this.gbMfProperties.Location = new System.Drawing.Point(6, 272);
            this.gbMfProperties.Name = "gbMfProperties";
            this.gbMfProperties.Size = new System.Drawing.Size(624, 95);
            this.gbMfProperties.TabIndex = 11;
            this.gbMfProperties.TabStop = false;
            this.gbMfProperties.Text = "Properties";
            // 
            // btnAddAllSubs
            // 
            this.btnAddAllSubs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddAllSubs.Location = new System.Drawing.Point(212, 65);
            this.btnAddAllSubs.Name = "btnAddAllSubs";
            this.btnAddAllSubs.Size = new System.Drawing.Size(206, 23);
            this.btnAddAllSubs.TabIndex = 9;
            this.btnAddAllSubs.Text = "Set All Subfolders as Child Root Folders";
            this.btnAddAllSubs.UseVisualStyleBackColor = true;
            this.btnAddAllSubs.Click += new System.EventHandler(this.btnAddAllSubs_Click);
            // 
            // btnMfAddSubfolder
            // 
            this.btnMfAddSubfolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMfAddSubfolder.Location = new System.Drawing.Point(6, 66);
            this.btnMfAddSubfolder.Name = "btnMfAddSubfolder";
            this.btnMfAddSubfolder.Size = new System.Drawing.Size(200, 23);
            this.btnMfAddSubfolder.TabIndex = 8;
            this.btnMfAddSubfolder.Text = "Set a Subfolder as a Child Root Folder";
            this.btnMfAddSubfolder.UseVisualStyleBackColor = true;
            this.btnMfAddSubfolder.Click += new System.EventHandler(this.btnAddSubfolder_Click);
            // 
            // chkMfDefault
            // 
            this.chkMfDefault.AutoSize = true;
            this.chkMfDefault.Location = new System.Drawing.Point(6, 42);
            this.chkMfDefault.Name = "chkMfDefault";
            this.chkMfDefault.Size = new System.Drawing.Size(346, 17);
            this.chkMfDefault.TabIndex = 7;
            this.chkMfDefault.Text = "Default (Directory scan will move/copy new items to here by default)";
            this.chkMfDefault.UseVisualStyleBackColor = true;
            this.chkMfDefault.CheckedChanged += new System.EventHandler(this.chkDefault_CheckedChanged);
            // 
            // chkMfAllowOrg
            // 
            this.chkMfAllowOrg.AutoSize = true;
            this.chkMfAllowOrg.Location = new System.Drawing.Point(6, 19);
            this.chkMfAllowOrg.Name = "chkMfAllowOrg";
            this.chkMfAllowOrg.Size = new System.Drawing.Size(236, 17);
            this.chkMfAllowOrg.TabIndex = 6;
            this.chkMfAllowOrg.Text = "Allow meticumedia to rename content folders";
            this.chkMfAllowOrg.UseVisualStyleBackColor = true;
            this.chkMfAllowOrg.CheckedChanged += new System.EventHandler(this.chkAllowOrg_CheckedChanged);
            // 
            // btnMfUpdate
            // 
            this.btnMfUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMfUpdate.Enabled = false;
            this.btnMfUpdate.Location = new System.Drawing.Point(543, 66);
            this.btnMfUpdate.Name = "btnMfUpdate";
            this.btnMfUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnMfUpdate.TabIndex = 5;
            this.btnMfUpdate.Text = "Update";
            this.btnMfUpdate.UseVisualStyleBackColor = true;
            this.btnMfUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btMfClear
            // 
            this.btMfClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btMfClear.Location = new System.Drawing.Point(555, 3);
            this.btMfClear.Name = "btMfClear";
            this.btMfClear.Size = new System.Drawing.Size(75, 23);
            this.btMfClear.TabIndex = 14;
            this.btMfClear.Text = "Clear";
            this.btMfClear.UseVisualStyleBackColor = true;
            this.btMfClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnMfRemove
            // 
            this.btnMfRemove.Location = new System.Drawing.Point(84, 3);
            this.btnMfRemove.Name = "btnMfRemove";
            this.btnMfRemove.Size = new System.Drawing.Size(75, 23);
            this.btnMfRemove.TabIndex = 13;
            this.btnMfRemove.Text = "Remove";
            this.btnMfRemove.UseVisualStyleBackColor = true;
            this.btnMfRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnMfAdd
            // 
            this.btnMfAdd.Location = new System.Drawing.Point(3, 3);
            this.btnMfAdd.Name = "btnMfAdd";
            this.btnMfAdd.Size = new System.Drawing.Size(75, 23);
            this.btnMfAdd.TabIndex = 12;
            this.btnMfAdd.Text = "Add";
            this.btnMfAdd.UseVisualStyleBackColor = true;
            this.btnMfAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // ContentFoldersControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvFolders);
            this.Controls.Add(this.gbMfProperties);
            this.Controls.Add(this.btMfClear);
            this.Controls.Add(this.btnMfRemove);
            this.Controls.Add(this.btnMfAdd);
            this.Name = "ContentFoldersControl";
            this.Size = new System.Drawing.Size(633, 370);
            this.gbMfProperties.ResumeLayout(false);
            this.gbMfProperties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvFolders;
        private System.Windows.Forms.GroupBox gbMfProperties;
        private System.Windows.Forms.Button btnMfAddSubfolder;
        private System.Windows.Forms.CheckBox chkMfDefault;
        private System.Windows.Forms.CheckBox chkMfAllowOrg;
        private System.Windows.Forms.Button btnMfUpdate;
        private System.Windows.Forms.Button btMfClear;
        private System.Windows.Forms.Button btnMfRemove;
        private System.Windows.Forms.Button btnMfAdd;
        private System.Windows.Forms.Button btnAddAllSubs;
    }
}
