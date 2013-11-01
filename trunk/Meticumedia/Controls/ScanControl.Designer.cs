// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class ScanControl
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
            this.btnQueueSelected = new System.Windows.Forms.Button();
            this.cmbScanSelection = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkFast = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cmbScanType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkQueueFilter = new System.Windows.Forms.CheckBox();
            this.chkFolderCatFilter = new System.Windows.Forms.CheckBox();
            this.chkTrashCatFilter = new System.Windows.Forms.CheckBox();
            this.chkUnknownCatFilter = new System.Windows.Forms.CheckBox();
            this.chkCustomCatFilter = new System.Windows.Forms.CheckBox();
            this.chkNonTvCatFilter = new System.Windows.Forms.CheckBox();
            this.chkTvCatFilter = new System.Windows.Forms.CheckBox();
            this.chkRenameFilter = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkDelFilter = new System.Windows.Forms.CheckBox();
            this.chkMoveCopyFilter = new System.Windows.Forms.CheckBox();
            this.chkNoneFilter = new System.Windows.Forms.CheckBox();
            this.chkIgnoreCatFilter = new System.Windows.Forms.CheckBox();
            this.btnModAction = new System.Windows.Forms.Button();
            this.chkRenameDelete = new System.Windows.Forms.CheckBox();
            this.chkMoveCopy = new System.Windows.Forms.CheckBox();
            this.gbActionFilter = new System.Windows.Forms.GroupBox();
            this.gbCategoryFilter = new System.Windows.Forms.GroupBox();
            this.lvResults = new Meticumedia.DoubleBufferedListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pbScanProgress = new TextProgressBar();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbActionFilter.SuspendLayout();
            this.gbCategoryFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnQueueSelected
            // 
            this.btnQueueSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQueueSelected.Enabled = false;
            this.btnQueueSelected.Location = new System.Drawing.Point(817, 405);
            this.btnQueueSelected.Name = "btnQueueSelected";
            this.btnQueueSelected.Size = new System.Drawing.Size(158, 23);
            this.btnQueueSelected.TabIndex = 13;
            this.btnQueueSelected.Text = "Add Checked To Queue";
            this.btnQueueSelected.UseVisualStyleBackColor = true;
            this.btnQueueSelected.Click += new System.EventHandler(this.btnQueueSelected_Click);
            // 
            // cmbScanSelection
            // 
            this.cmbScanSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbScanSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScanSelection.FormattingEnabled = true;
            this.cmbScanSelection.Location = new System.Drawing.Point(24, 3);
            this.cmbScanSelection.Name = "cmbScanSelection";
            this.cmbScanSelection.Size = new System.Drawing.Size(500, 21);
            this.cmbScanSelection.TabIndex = 15;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkFast);
            this.groupBox1.Controls.Add(this.splitContainer1);
            this.groupBox1.Controls.Add(this.pbScanProgress);
            this.groupBox1.Controls.Add(this.btnRun);
            this.groupBox1.Location = new System.Drawing.Point(3, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(988, 83);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setup";
            // 
            // chkFast
            // 
            this.chkFast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkFast.AutoSize = true;
            this.chkFast.Location = new System.Drawing.Point(908, 24);
            this.chkFast.Name = "chkFast";
            this.chkFast.Size = new System.Drawing.Size(74, 17);
            this.chkFast.TabIndex = 38;
            this.chkFast.Text = "Fast Scan";
            this.chkFast.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 19);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cmbScanType);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cmbScanSelection);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Size = new System.Drawing.Size(903, 27);
            this.splitContainer1.SplitterDistance = 372;
            this.splitContainer1.TabIndex = 37;
            // 
            // cmbScanType
            // 
            this.cmbScanType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbScanType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScanType.FormattingEnabled = true;
            this.cmbScanType.Location = new System.Drawing.Point(6, 3);
            this.cmbScanType.Name = "cmbScanType";
            this.cmbScanType.Size = new System.Drawing.Size(363, 21);
            this.cmbScanType.TabIndex = 29;
            this.cmbScanType.SelectedIndexChanged += new System.EventHandler(this.cmbScanType_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-1, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = "on";
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(7, 49);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(121, 23);
            this.btnRun.TabIndex = 22;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.gbCategoryFilter);
            this.groupBox2.Controls.Add(this.gbActionFilter);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnModAction);
            this.groupBox2.Controls.Add(this.chkRenameDelete);
            this.groupBox2.Controls.Add(this.chkMoveCopy);
            this.groupBox2.Controls.Add(this.lvResults);
            this.groupBox2.Controls.Add(this.btnQueueSelected);
            this.groupBox2.Location = new System.Drawing.Point(4, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(981, 434);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Scan";
            // 
            // chkQueueFilter
            // 
            this.chkQueueFilter.AutoSize = true;
            this.chkQueueFilter.Location = new System.Drawing.Point(290, 20);
            this.chkQueueFilter.Name = "chkQueueFilter";
            this.chkQueueFilter.Size = new System.Drawing.Size(64, 17);
            this.chkQueueFilter.TabIndex = 35;
            this.chkQueueFilter.Text = "Queued";
            this.chkQueueFilter.UseVisualStyleBackColor = true;
            this.chkQueueFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // chkFolderCatFilter
            // 
            this.chkFolderCatFilter.AutoSize = true;
            this.chkFolderCatFilter.Checked = true;
            this.chkFolderCatFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFolderCatFilter.Location = new System.Drawing.Point(388, 20);
            this.chkFolderCatFilter.Name = "chkFolderCatFilter";
            this.chkFolderCatFilter.Size = new System.Drawing.Size(55, 17);
            this.chkFolderCatFilter.TabIndex = 34;
            this.chkFolderCatFilter.Text = "Folder";
            this.chkFolderCatFilter.UseVisualStyleBackColor = true;
            this.chkFolderCatFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // chkTrashCatFilter
            // 
            this.chkTrashCatFilter.AutoSize = true;
            this.chkTrashCatFilter.Checked = true;
            this.chkTrashCatFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTrashCatFilter.Location = new System.Drawing.Point(251, 20);
            this.chkTrashCatFilter.Name = "chkTrashCatFilter";
            this.chkTrashCatFilter.Size = new System.Drawing.Size(53, 17);
            this.chkTrashCatFilter.TabIndex = 33;
            this.chkTrashCatFilter.Text = "Trash";
            this.chkTrashCatFilter.UseVisualStyleBackColor = true;
            this.chkTrashCatFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // chkUnknownCatFilter
            // 
            this.chkUnknownCatFilter.AutoSize = true;
            this.chkUnknownCatFilter.Checked = true;
            this.chkUnknownCatFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnknownCatFilter.Location = new System.Drawing.Point(310, 20);
            this.chkUnknownCatFilter.Name = "chkUnknownCatFilter";
            this.chkUnknownCatFilter.Size = new System.Drawing.Size(72, 17);
            this.chkUnknownCatFilter.TabIndex = 32;
            this.chkUnknownCatFilter.Text = "Unknown";
            this.chkUnknownCatFilter.UseVisualStyleBackColor = true;
            this.chkUnknownCatFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // chkCustomCatFilter
            // 
            this.chkCustomCatFilter.AutoSize = true;
            this.chkCustomCatFilter.Checked = true;
            this.chkCustomCatFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCustomCatFilter.Location = new System.Drawing.Point(184, 20);
            this.chkCustomCatFilter.Name = "chkCustomCatFilter";
            this.chkCustomCatFilter.Size = new System.Drawing.Size(61, 17);
            this.chkCustomCatFilter.TabIndex = 31;
            this.chkCustomCatFilter.Text = "Custom";
            this.chkCustomCatFilter.UseVisualStyleBackColor = true;
            this.chkCustomCatFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // chkNonTvCatFilter
            // 
            this.chkNonTvCatFilter.AutoSize = true;
            this.chkNonTvCatFilter.Checked = true;
            this.chkNonTvCatFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNonTvCatFilter.Location = new System.Drawing.Point(85, 20);
            this.chkNonTvCatFilter.Name = "chkNonTvCatFilter";
            this.chkNonTvCatFilter.Size = new System.Drawing.Size(93, 17);
            this.chkNonTvCatFilter.TabIndex = 29;
            this.chkNonTvCatFilter.Text = "Non-TV Video";
            this.chkNonTvCatFilter.UseVisualStyleBackColor = true;
            this.chkNonTvCatFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // chkTvCatFilter
            // 
            this.chkTvCatFilter.AutoSize = true;
            this.chkTvCatFilter.Checked = true;
            this.chkTvCatFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTvCatFilter.Location = new System.Drawing.Point(9, 19);
            this.chkTvCatFilter.Name = "chkTvCatFilter";
            this.chkTvCatFilter.Size = new System.Drawing.Size(70, 17);
            this.chkTvCatFilter.TabIndex = 28;
            this.chkTvCatFilter.Text = "TV Video";
            this.chkTvCatFilter.UseVisualStyleBackColor = true;
            this.chkTvCatFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // chkRenameFilter
            // 
            this.chkRenameFilter.AutoSize = true;
            this.chkRenameFilter.Checked = true;
            this.chkRenameFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRenameFilter.Location = new System.Drawing.Point(155, 19);
            this.chkRenameFilter.Name = "chkRenameFilter";
            this.chkRenameFilter.Size = new System.Drawing.Size(66, 17);
            this.chkRenameFilter.TabIndex = 27;
            this.chkRenameFilter.Text = "Rename";
            this.chkRenameFilter.UseVisualStyleBackColor = true;
            this.chkRenameFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 409);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Check Control:";
            // 
            // chkDelFilter
            // 
            this.chkDelFilter.AutoSize = true;
            this.chkDelFilter.Checked = true;
            this.chkDelFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDelFilter.Location = new System.Drawing.Point(227, 19);
            this.chkDelFilter.Name = "chkDelFilter";
            this.chkDelFilter.Size = new System.Drawing.Size(57, 17);
            this.chkDelFilter.TabIndex = 21;
            this.chkDelFilter.Text = "Delete";
            this.chkDelFilter.UseVisualStyleBackColor = true;
            this.chkDelFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // chkMoveCopyFilter
            // 
            this.chkMoveCopyFilter.AutoSize = true;
            this.chkMoveCopyFilter.Checked = true;
            this.chkMoveCopyFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMoveCopyFilter.Location = new System.Drawing.Point(64, 19);
            this.chkMoveCopyFilter.Name = "chkMoveCopyFilter";
            this.chkMoveCopyFilter.Size = new System.Drawing.Size(82, 17);
            this.chkMoveCopyFilter.TabIndex = 20;
            this.chkMoveCopyFilter.Text = "Move/Copy";
            this.chkMoveCopyFilter.UseVisualStyleBackColor = true;
            this.chkMoveCopyFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // chkNoneFilter
            // 
            this.chkNoneFilter.AutoSize = true;
            this.chkNoneFilter.Checked = true;
            this.chkNoneFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNoneFilter.Location = new System.Drawing.Point(6, 19);
            this.chkNoneFilter.Name = "chkNoneFilter";
            this.chkNoneFilter.Size = new System.Drawing.Size(52, 17);
            this.chkNoneFilter.TabIndex = 19;
            this.chkNoneFilter.Text = "None";
            this.chkNoneFilter.UseVisualStyleBackColor = true;
            this.chkNoneFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // chkIgnoreCatFilter
            // 
            this.chkIgnoreCatFilter.AutoSize = true;
            this.chkIgnoreCatFilter.Location = new System.Drawing.Point(449, 20);
            this.chkIgnoreCatFilter.Name = "chkIgnoreCatFilter";
            this.chkIgnoreCatFilter.Size = new System.Drawing.Size(62, 17);
            this.chkIgnoreCatFilter.TabIndex = 17;
            this.chkIgnoreCatFilter.Text = "Ignored";
            this.chkIgnoreCatFilter.UseVisualStyleBackColor = true;
            this.chkIgnoreCatFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // btnModAction
            // 
            this.btnModAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnModAction.Enabled = false;
            this.btnModAction.Location = new System.Drawing.Point(694, 405);
            this.btnModAction.Name = "btnModAction";
            this.btnModAction.Size = new System.Drawing.Size(117, 23);
            this.btnModAction.TabIndex = 16;
            this.btnModAction.Text = "Modify Action";
            this.btnModAction.UseVisualStyleBackColor = true;
            this.btnModAction.Click += new System.EventHandler(this.btnModAction_Click);
            // 
            // chkRenameDelete
            // 
            this.chkRenameDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkRenameDelete.AutoSize = true;
            this.chkRenameDelete.Location = new System.Drawing.Point(177, 408);
            this.chkRenameDelete.Name = "chkRenameDelete";
            this.chkRenameDelete.Size = new System.Drawing.Size(57, 17);
            this.chkRenameDelete.TabIndex = 15;
            this.chkRenameDelete.Text = "Delete";
            this.chkRenameDelete.UseVisualStyleBackColor = true;
            this.chkRenameDelete.CheckedChanged += new System.EventHandler(this.chkRenameDelete_CheckedChanged);
            // 
            // chkMoveCopy
            // 
            this.chkMoveCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkMoveCopy.AutoSize = true;
            this.chkMoveCopy.Location = new System.Drawing.Point(89, 408);
            this.chkMoveCopy.Name = "chkMoveCopy";
            this.chkMoveCopy.Size = new System.Drawing.Size(82, 17);
            this.chkMoveCopy.TabIndex = 14;
            this.chkMoveCopy.Text = "Move/Copy";
            this.chkMoveCopy.UseVisualStyleBackColor = true;
            this.chkMoveCopy.CheckedChanged += new System.EventHandler(this.chkMoveCopy_CheckedChanged);
            // 
            // gbActionFilter
            // 
            this.gbActionFilter.Controls.Add(this.chkNoneFilter);
            this.gbActionFilter.Controls.Add(this.chkQueueFilter);
            this.gbActionFilter.Controls.Add(this.chkMoveCopyFilter);
            this.gbActionFilter.Controls.Add(this.chkDelFilter);
            this.gbActionFilter.Controls.Add(this.chkRenameFilter);
            this.gbActionFilter.Location = new System.Drawing.Point(9, 19);
            this.gbActionFilter.Name = "gbActionFilter";
            this.gbActionFilter.Size = new System.Drawing.Size(362, 47);
            this.gbActionFilter.TabIndex = 36;
            this.gbActionFilter.TabStop = false;
            this.gbActionFilter.Text = "Action Filter";
            // 
            // gbCategoryFilter
            // 
            this.gbCategoryFilter.Controls.Add(this.chkTvCatFilter);
            this.gbCategoryFilter.Controls.Add(this.chkIgnoreCatFilter);
            this.gbCategoryFilter.Controls.Add(this.chkFolderCatFilter);
            this.gbCategoryFilter.Controls.Add(this.chkNonTvCatFilter);
            this.gbCategoryFilter.Controls.Add(this.chkTrashCatFilter);
            this.gbCategoryFilter.Controls.Add(this.chkCustomCatFilter);
            this.gbCategoryFilter.Controls.Add(this.chkUnknownCatFilter);
            this.gbCategoryFilter.Location = new System.Drawing.Point(377, 19);
            this.gbCategoryFilter.Name = "gbCategoryFilter";
            this.gbCategoryFilter.Size = new System.Drawing.Size(522, 47);
            this.gbCategoryFilter.TabIndex = 37;
            this.gbCategoryFilter.TabStop = false;
            this.gbCategoryFilter.Text = "Category Filter";
            // 
            // lvResults
            // 
            this.lvResults.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.lvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvResults.CheckBoxes = true;
            this.lvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvResults.FullRowSelect = true;
            this.lvResults.Location = new System.Drawing.Point(9, 72);
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new System.Drawing.Size(966, 327);
            this.lvResults.TabIndex = 11;
            this.lvResults.UseCompatibleStateImageBehavior = false;
            this.lvResults.View = System.Windows.Forms.View.Details;
            this.lvResults.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvResults_ColumnClick);
            this.lvResults.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvResults_ItemChecked);
            this.lvResults.SelectedIndexChanged += new System.EventHandler(this.lvResults_SelectedIndexChanged);
            this.lvResults.DoubleClick += new System.EventHandler(this.lvResults_DoubleClick);
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
            // pbScanProgress
            // 
            this.pbScanProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbScanProgress.Location = new System.Drawing.Point(134, 49);
            this.pbScanProgress.Message = null;
            this.pbScanProgress.Name = "pbScanProgress";
            this.pbScanProgress.Size = new System.Drawing.Size(848, 23);
            this.pbScanProgress.TabIndex = 28;
            // 
            // ScanControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ScanControl";
            this.Size = new System.Drawing.Size(994, 532);
            this.Load += new System.EventHandler(this.ScanControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbActionFilter.ResumeLayout(false);
            this.gbActionFilter.PerformLayout();
            this.gbCategoryFilter.ResumeLayout(false);
            this.gbCategoryFilter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufferedListView lvResults;
        private System.Windows.Forms.Button btnQueueSelected;
        private System.Windows.Forms.ComboBox cmbScanSelection;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.CheckBox chkRenameDelete;
        private System.Windows.Forms.CheckBox chkMoveCopy;
        private System.Windows.Forms.Button btnModAction;
        private TextProgressBar pbScanProgress;
        private System.Windows.Forms.CheckBox chkIgnoreCatFilter;
        private System.Windows.Forms.CheckBox chkDelFilter;
        private System.Windows.Forms.CheckBox chkMoveCopyFilter;
        private System.Windows.Forms.CheckBox chkNoneFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkRenameFilter;
        private System.Windows.Forms.CheckBox chkTvCatFilter;
        private System.Windows.Forms.CheckBox chkNonTvCatFilter;
        private System.Windows.Forms.CheckBox chkCustomCatFilter;
        private System.Windows.Forms.CheckBox chkTrashCatFilter;
        private System.Windows.Forms.CheckBox chkUnknownCatFilter;
        private System.Windows.Forms.CheckBox chkFolderCatFilter;
        private System.Windows.Forms.CheckBox chkQueueFilter;
        private System.Windows.Forms.ComboBox cmbScanType;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkFast;
        private System.Windows.Forms.GroupBox gbCategoryFilter;
        private System.Windows.Forms.GroupBox gbActionFilter;
    }
}
