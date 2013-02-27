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
            this.cmbShows = new System.Windows.Forms.ComboBox();
            this.btnQueueSelected = new System.Windows.Forms.Button();
            this.cmbDirectory = new System.Windows.Forms.ComboBox();
            this.btnEditScanDirs = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnEditMovieFolders = new System.Windows.Forms.Button();
            this.rbMovieFolderCheck = new System.Windows.Forms.RadioButton();
            this.cmbMovieFolders = new System.Windows.Forms.ComboBox();
            this.rbMissingCheck = new System.Windows.Forms.RadioButton();
            this.rbDirCheck = new System.Windows.Forms.RadioButton();
            this.btnRun = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkQueueFilter = new System.Windows.Forms.CheckBox();
            this.chkFolderCatFilter = new System.Windows.Forms.CheckBox();
            this.chkTrashCatFilter = new System.Windows.Forms.CheckBox();
            this.chkUnknownCatFilter = new System.Windows.Forms.CheckBox();
            this.chkCustomCatFilter = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkNonTvCatFilter = new System.Windows.Forms.CheckBox();
            this.chkTvCatFilter = new System.Windows.Forms.CheckBox();
            this.chkRenameFilter = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkDelFilter = new System.Windows.Forms.CheckBox();
            this.chkMoveCopyFilter = new System.Windows.Forms.CheckBox();
            this.chkNoneFilter = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkIgnoreCatFilter = new System.Windows.Forms.CheckBox();
            this.btnModAction = new System.Windows.Forms.Button();
            this.chkRenameDelete = new System.Windows.Forms.CheckBox();
            this.chkMoveCopy = new System.Windows.Forms.CheckBox();
            this.lvResults = new Meticumedia.DoubleBufferedListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pbScanProgress = new TextProgressBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbShows
            // 
            this.cmbShows.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbShows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShows.Enabled = false;
            this.cmbShows.FormattingEnabled = true;
            this.cmbShows.Location = new System.Drawing.Point(134, 48);
            this.cmbShows.Name = "cmbShows";
            this.cmbShows.Size = new System.Drawing.Size(546, 21);
            this.cmbShows.TabIndex = 9;
            // 
            // btnQueueSelected
            // 
            this.btnQueueSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQueueSelected.Enabled = false;
            this.btnQueueSelected.Location = new System.Drawing.Point(515, 355);
            this.btnQueueSelected.Name = "btnQueueSelected";
            this.btnQueueSelected.Size = new System.Drawing.Size(158, 23);
            this.btnQueueSelected.TabIndex = 13;
            this.btnQueueSelected.Text = "Add Checked To Queue";
            this.btnQueueSelected.UseVisualStyleBackColor = true;
            this.btnQueueSelected.Click += new System.EventHandler(this.btnQueueSelected_Click);
            // 
            // cmbDirectory
            // 
            this.cmbDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirectory.FormattingEnabled = true;
            this.cmbDirectory.Location = new System.Drawing.Point(134, 19);
            this.cmbDirectory.Name = "cmbDirectory";
            this.cmbDirectory.Size = new System.Drawing.Size(465, 21);
            this.cmbDirectory.Sorted = true;
            this.cmbDirectory.TabIndex = 15;
            // 
            // btnEditScanDirs
            // 
            this.btnEditScanDirs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditScanDirs.Location = new System.Drawing.Point(605, 17);
            this.btnEditScanDirs.Name = "btnEditScanDirs";
            this.btnEditScanDirs.Size = new System.Drawing.Size(75, 23);
            this.btnEditScanDirs.TabIndex = 17;
            this.btnEditScanDirs.Text = "Edit List";
            this.btnEditScanDirs.UseVisualStyleBackColor = true;
            this.btnEditScanDirs.Click += new System.EventHandler(this.btnEditScanDirs_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pbScanProgress);
            this.groupBox1.Controls.Add(this.btnEditMovieFolders);
            this.groupBox1.Controls.Add(this.rbMovieFolderCheck);
            this.groupBox1.Controls.Add(this.cmbMovieFolders);
            this.groupBox1.Controls.Add(this.rbMissingCheck);
            this.groupBox1.Controls.Add(this.rbDirCheck);
            this.groupBox1.Controls.Add(this.btnRun);
            this.groupBox1.Controls.Add(this.cmbDirectory);
            this.groupBox1.Controls.Add(this.cmbShows);
            this.groupBox1.Controls.Add(this.btnEditScanDirs);
            this.groupBox1.Location = new System.Drawing.Point(3, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(686, 133);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setup";
            // 
            // btnEditMovieFolders
            // 
            this.btnEditMovieFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditMovieFolders.Location = new System.Drawing.Point(605, 75);
            this.btnEditMovieFolders.Name = "btnEditMovieFolders";
            this.btnEditMovieFolders.Size = new System.Drawing.Size(75, 23);
            this.btnEditMovieFolders.TabIndex = 27;
            this.btnEditMovieFolders.Text = "Edit List";
            this.btnEditMovieFolders.UseVisualStyleBackColor = true;
            this.btnEditMovieFolders.Click += new System.EventHandler(this.btnEditMovieFolders_Click);
            // 
            // rbMovieFolderCheck
            // 
            this.rbMovieFolderCheck.AutoSize = true;
            this.rbMovieFolderCheck.Location = new System.Drawing.Point(6, 75);
            this.rbMovieFolderCheck.Name = "rbMovieFolderCheck";
            this.rbMovieFolderCheck.Size = new System.Drawing.Size(120, 17);
            this.rbMovieFolderCheck.TabIndex = 26;
            this.rbMovieFolderCheck.Text = "Movie Folder Check";
            this.rbMovieFolderCheck.UseVisualStyleBackColor = true;
            this.rbMovieFolderCheck.CheckedChanged += new System.EventHandler(this.rbScanType_CheckedChanged);
            // 
            // cmbMovieFolders
            // 
            this.cmbMovieFolders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMovieFolders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMovieFolders.Enabled = false;
            this.cmbMovieFolders.FormattingEnabled = true;
            this.cmbMovieFolders.Location = new System.Drawing.Point(134, 75);
            this.cmbMovieFolders.Name = "cmbMovieFolders";
            this.cmbMovieFolders.Size = new System.Drawing.Size(465, 21);
            this.cmbMovieFolders.TabIndex = 25;
            // 
            // rbMissingCheck
            // 
            this.rbMissingCheck.AutoSize = true;
            this.rbMissingCheck.Location = new System.Drawing.Point(6, 48);
            this.rbMissingCheck.Name = "rbMissingCheck";
            this.rbMissingCheck.Size = new System.Drawing.Size(122, 17);
            this.rbMissingCheck.TabIndex = 24;
            this.rbMissingCheck.Text = "TV Missing/Rename";
            this.rbMissingCheck.UseVisualStyleBackColor = true;
            this.rbMissingCheck.CheckedChanged += new System.EventHandler(this.rbScanType_CheckedChanged);
            // 
            // rbDirCheck
            // 
            this.rbDirCheck.AutoSize = true;
            this.rbDirCheck.Checked = true;
            this.rbDirCheck.Location = new System.Drawing.Point(6, 20);
            this.rbDirCheck.Name = "rbDirCheck";
            this.rbDirCheck.Size = new System.Drawing.Size(101, 17);
            this.rbDirCheck.TabIndex = 23;
            this.rbDirCheck.TabStop = true;
            this.rbDirCheck.Text = "Directory Check";
            this.rbDirCheck.UseVisualStyleBackColor = true;
            this.rbDirCheck.CheckedChanged += new System.EventHandler(this.rbScanType_CheckedChanged);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(7, 102);
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
            this.groupBox2.Controls.Add(this.chkQueueFilter);
            this.groupBox2.Controls.Add(this.chkFolderCatFilter);
            this.groupBox2.Controls.Add(this.chkTrashCatFilter);
            this.groupBox2.Controls.Add(this.chkUnknownCatFilter);
            this.groupBox2.Controls.Add(this.chkCustomCatFilter);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.chkNonTvCatFilter);
            this.groupBox2.Controls.Add(this.chkTvCatFilter);
            this.groupBox2.Controls.Add(this.chkRenameFilter);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.chkDelFilter);
            this.groupBox2.Controls.Add(this.chkMoveCopyFilter);
            this.groupBox2.Controls.Add(this.chkNoneFilter);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.chkIgnoreCatFilter);
            this.groupBox2.Controls.Add(this.btnModAction);
            this.groupBox2.Controls.Add(this.chkRenameDelete);
            this.groupBox2.Controls.Add(this.chkMoveCopy);
            this.groupBox2.Controls.Add(this.lvResults);
            this.groupBox2.Controls.Add(this.btnQueueSelected);
            this.groupBox2.Location = new System.Drawing.Point(4, 145);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(679, 384);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Scan";
            // 
            // chkQueueFilter
            // 
            this.chkQueueFilter.AutoSize = true;
            this.chkQueueFilter.Location = new System.Drawing.Point(363, 22);
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
            this.chkFolderCatFilter.Location = new System.Drawing.Point(468, 45);
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
            this.chkTrashCatFilter.Location = new System.Drawing.Point(331, 45);
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
            this.chkUnknownCatFilter.Location = new System.Drawing.Point(390, 45);
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
            this.chkCustomCatFilter.Location = new System.Drawing.Point(264, 45);
            this.chkCustomCatFilter.Name = "chkCustomCatFilter";
            this.chkCustomCatFilter.Size = new System.Drawing.Size(61, 17);
            this.chkCustomCatFilter.TabIndex = 31;
            this.chkCustomCatFilter.Text = "Custom";
            this.chkCustomCatFilter.UseVisualStyleBackColor = true;
            this.chkCustomCatFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Category Filter:";
            // 
            // chkNonTvCatFilter
            // 
            this.chkNonTvCatFilter.AutoSize = true;
            this.chkNonTvCatFilter.Checked = true;
            this.chkNonTvCatFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNonTvCatFilter.Location = new System.Drawing.Point(165, 45);
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
            this.chkTvCatFilter.Location = new System.Drawing.Point(89, 44);
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
            this.chkRenameFilter.Location = new System.Drawing.Point(228, 21);
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
            this.label2.Location = new System.Drawing.Point(6, 359);
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
            this.chkDelFilter.Location = new System.Drawing.Point(300, 21);
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
            this.chkMoveCopyFilter.Location = new System.Drawing.Point(140, 21);
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
            this.chkNoneFilter.Location = new System.Drawing.Point(82, 21);
            this.chkNoneFilter.Name = "chkNoneFilter";
            this.chkNoneFilter.Size = new System.Drawing.Size(52, 17);
            this.chkNoneFilter.TabIndex = 19;
            this.chkNoneFilter.Text = "None";
            this.chkNoneFilter.UseVisualStyleBackColor = true;
            this.chkNoneFilter.CheckedChanged += new System.EventHandler(this.chkFilter_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Action Filter:";
            // 
            // chkIgnoreCatFilter
            // 
            this.chkIgnoreCatFilter.AutoSize = true;
            this.chkIgnoreCatFilter.Location = new System.Drawing.Point(529, 45);
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
            this.btnModAction.Location = new System.Drawing.Point(392, 355);
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
            this.chkRenameDelete.Location = new System.Drawing.Point(177, 358);
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
            this.chkMoveCopy.Location = new System.Drawing.Point(89, 358);
            this.chkMoveCopy.Name = "chkMoveCopy";
            this.chkMoveCopy.Size = new System.Drawing.Size(82, 17);
            this.chkMoveCopy.TabIndex = 14;
            this.chkMoveCopy.Text = "Move/Copy";
            this.chkMoveCopy.UseVisualStyleBackColor = true;
            this.chkMoveCopy.CheckedChanged += new System.EventHandler(this.chkMoveCopy_CheckedChanged);
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
            this.lvResults.Location = new System.Drawing.Point(6, 68);
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new System.Drawing.Size(667, 281);
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
            this.pbScanProgress.Location = new System.Drawing.Point(134, 102);
            this.pbScanProgress.Message = null;
            this.pbScanProgress.Name = "pbScanProgress";
            this.pbScanProgress.Size = new System.Drawing.Size(546, 23);
            this.pbScanProgress.TabIndex = 28;
            // 
            // ScanControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ScanControl";
            this.Size = new System.Drawing.Size(692, 532);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbShows;
        private DoubleBufferedListView lvResults;
        private System.Windows.Forms.Button btnQueueSelected;
        private System.Windows.Forms.ComboBox cmbDirectory;
        private System.Windows.Forms.Button btnEditScanDirs;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.RadioButton rbMissingCheck;
        private System.Windows.Forms.RadioButton rbDirCheck;
        private System.Windows.Forms.CheckBox chkRenameDelete;
        private System.Windows.Forms.CheckBox chkMoveCopy;
        private System.Windows.Forms.Button btnModAction;
        private System.Windows.Forms.RadioButton rbMovieFolderCheck;
        private System.Windows.Forms.ComboBox cmbMovieFolders;
        private System.Windows.Forms.Button btnEditMovieFolders;
        private TextProgressBar pbScanProgress;
        private System.Windows.Forms.CheckBox chkIgnoreCatFilter;
        private System.Windows.Forms.CheckBox chkDelFilter;
        private System.Windows.Forms.CheckBox chkMoveCopyFilter;
        private System.Windows.Forms.CheckBox chkNoneFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkRenameFilter;
        private System.Windows.Forms.CheckBox chkTvCatFilter;
        private System.Windows.Forms.CheckBox chkNonTvCatFilter;
        private System.Windows.Forms.CheckBox chkCustomCatFilter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkTrashCatFilter;
        private System.Windows.Forms.CheckBox chkUnknownCatFilter;
        private System.Windows.Forms.CheckBox chkFolderCatFilter;
        private System.Windows.Forms.CheckBox chkQueueFilter;
    }
}
