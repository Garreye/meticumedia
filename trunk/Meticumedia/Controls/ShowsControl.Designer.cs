// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class ShowsControl
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
            this.label3 = new System.Windows.Forms.Label();
            this.cmbEpFilter = new System.Windows.Forms.ComboBox();
            this.gbEpisodes = new System.Windows.Forms.GroupBox();
            this.lbScanDir = new Meticumedia.LegendBoxControl();
            this.lbIgnored = new Meticumedia.LegendBoxControl();
            this.lbUnaired = new Meticumedia.LegendBoxControl();
            this.lbMissing = new Meticumedia.LegendBoxControl();
            this.btnUnignore = new System.Windows.Forms.Button();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.chkDisplayIgnored = new System.Windows.Forms.CheckBox();
            this.lvEpisodes = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnForceRefresh = new System.Windows.Forms.Button();
            this.btnEditFolders = new System.Windows.Forms.Button();
            this.cmbFolders = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lvShows = new Meticumedia.ContentListView();
            this.colPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colYear = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOverview = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnEditMovie = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNameFilter = new System.Windows.Forms.TextBox();
            this.lbInvalid = new Meticumedia.LegendBoxControl();
            this.pbUpdating = new TextProgressBar();
            this.gbEpisodes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Filter";
            // 
            // cmbEpFilter
            // 
            this.cmbEpFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEpFilter.FormattingEnabled = true;
            this.cmbEpFilter.Location = new System.Drawing.Point(41, 19);
            this.cmbEpFilter.Name = "cmbEpFilter";
            this.cmbEpFilter.Size = new System.Drawing.Size(226, 21);
            this.cmbEpFilter.TabIndex = 9;
            this.cmbEpFilter.SelectedIndexChanged += new System.EventHandler(this.cmbEpFilter_SelectedIndexChanged);
            // 
            // gbEpisodes
            // 
            this.gbEpisodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEpisodes.Controls.Add(this.lbScanDir);
            this.gbEpisodes.Controls.Add(this.lbIgnored);
            this.gbEpisodes.Controls.Add(this.lbUnaired);
            this.gbEpisodes.Controls.Add(this.lbMissing);
            this.gbEpisodes.Controls.Add(this.btnUnignore);
            this.gbEpisodes.Controls.Add(this.btnIgnore);
            this.gbEpisodes.Controls.Add(this.chkDisplayIgnored);
            this.gbEpisodes.Controls.Add(this.lvEpisodes);
            this.gbEpisodes.Controls.Add(this.label3);
            this.gbEpisodes.Controls.Add(this.cmbEpFilter);
            this.gbEpisodes.Location = new System.Drawing.Point(3, 3);
            this.gbEpisodes.Name = "gbEpisodes";
            this.gbEpisodes.Size = new System.Drawing.Size(621, 290);
            this.gbEpisodes.TabIndex = 14;
            this.gbEpisodes.TabStop = false;
            this.gbEpisodes.Text = "Episodes";
            // 
            // lbScanDir
            // 
            this.lbScanDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbScanDir.AutoSize = true;
            this.lbScanDir.Color = System.Drawing.SystemColors.Control;
            this.lbScanDir.Label = "In Scan Dir.";
            this.lbScanDir.Location = new System.Drawing.Point(91, 256);
            this.lbScanDir.Margin = new System.Windows.Forms.Padding(0);
            this.lbScanDir.Name = "lbScanDir";
            this.lbScanDir.Size = new System.Drawing.Size(103, 30);
            this.lbScanDir.TabIndex = 23;
            // 
            // lbIgnored
            // 
            this.lbIgnored.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbIgnored.AutoSize = true;
            this.lbIgnored.Color = System.Drawing.SystemColors.Control;
            this.lbIgnored.Label = "Ignored";
            this.lbIgnored.Location = new System.Drawing.Point(278, 256);
            this.lbIgnored.Margin = new System.Windows.Forms.Padding(0);
            this.lbIgnored.Name = "lbIgnored";
            this.lbIgnored.Size = new System.Drawing.Size(83, 30);
            this.lbIgnored.TabIndex = 22;
            this.lbIgnored.Visible = false;
            // 
            // lbUnaired
            // 
            this.lbUnaired.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbUnaired.AutoSize = true;
            this.lbUnaired.Color = System.Drawing.SystemColors.Control;
            this.lbUnaired.Label = "Unaired";
            this.lbUnaired.Location = new System.Drawing.Point(194, 256);
            this.lbUnaired.Margin = new System.Windows.Forms.Padding(0);
            this.lbUnaired.Name = "lbUnaired";
            this.lbUnaired.Size = new System.Drawing.Size(84, 30);
            this.lbUnaired.TabIndex = 21;
            // 
            // lbMissing
            // 
            this.lbMissing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbMissing.AutoSize = true;
            this.lbMissing.Color = System.Drawing.SystemColors.Control;
            this.lbMissing.Label = "Missing";
            this.lbMissing.Location = new System.Drawing.Point(9, 256);
            this.lbMissing.Margin = new System.Windows.Forms.Padding(0);
            this.lbMissing.Name = "lbMissing";
            this.lbMissing.Size = new System.Drawing.Size(82, 30);
            this.lbMissing.TabIndex = 20;
            // 
            // btnUnignore
            // 
            this.btnUnignore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnignore.Location = new System.Drawing.Point(459, 259);
            this.btnUnignore.Name = "btnUnignore";
            this.btnUnignore.Size = new System.Drawing.Size(75, 23);
            this.btnUnignore.TabIndex = 18;
            this.btnUnignore.Text = "Unignore";
            this.btnUnignore.UseVisualStyleBackColor = true;
            this.btnUnignore.Visible = false;
            this.btnUnignore.Click += new System.EventHandler(this.btnUnignore_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIgnore.Location = new System.Drawing.Point(540, 259);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(75, 23);
            this.btnIgnore.TabIndex = 13;
            this.btnIgnore.Text = "Ignore";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // chkDisplayIgnored
            // 
            this.chkDisplayIgnored.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDisplayIgnored.AutoSize = true;
            this.chkDisplayIgnored.Location = new System.Drawing.Point(516, 22);
            this.chkDisplayIgnored.Name = "chkDisplayIgnored";
            this.chkDisplayIgnored.Size = new System.Drawing.Size(99, 17);
            this.chkDisplayIgnored.TabIndex = 12;
            this.chkDisplayIgnored.Text = "Display Ignored";
            this.chkDisplayIgnored.UseVisualStyleBackColor = true;
            this.chkDisplayIgnored.CheckedChanged += new System.EventHandler(this.chkDisplayIgnored_CheckedChanged);
            // 
            // lvEpisodes
            // 
            this.lvEpisodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvEpisodes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvEpisodes.FullRowSelect = true;
            this.lvEpisodes.HideSelection = false;
            this.lvEpisodes.Location = new System.Drawing.Point(9, 46);
            this.lvEpisodes.Name = "lvEpisodes";
            this.lvEpisodes.Size = new System.Drawing.Size(606, 207);
            this.lvEpisodes.TabIndex = 11;
            this.lvEpisodes.UseCompatibleStateImageBehavior = false;
            this.lvEpisodes.View = System.Windows.Forms.View.Details;
            this.lvEpisodes.DoubleClick += new System.EventHandler(this.lvEpisodes_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Season";
            this.columnHeader1.Width = 52;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Episode";
            this.columnHeader2.Width = 57;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 184;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Air Date";
            this.columnHeader4.Width = 102;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Overview";
            this.columnHeader5.Width = 318;
            // 
            // btnForceRefresh
            // 
            this.btnForceRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnForceRefresh.Location = new System.Drawing.Point(529, 3);
            this.btnForceRefresh.Name = "btnForceRefresh";
            this.btnForceRefresh.Size = new System.Drawing.Size(89, 23);
            this.btnForceRefresh.TabIndex = 23;
            this.btnForceRefresh.Text = "Force Refresh";
            this.btnForceRefresh.UseVisualStyleBackColor = true;
            this.btnForceRefresh.Click += new System.EventHandler(this.btnForceRefresh_Click);
            // 
            // btnEditFolders
            // 
            this.btnEditFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditFolders.Location = new System.Drawing.Point(448, 3);
            this.btnEditFolders.Name = "btnEditFolders";
            this.btnEditFolders.Size = new System.Drawing.Size(75, 23);
            this.btnEditFolders.TabIndex = 22;
            this.btnEditFolders.Text = "Edit List";
            this.btnEditFolders.UseVisualStyleBackColor = true;
            // 
            // cmbFolders
            // 
            this.cmbFolders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFolders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFolders.FormattingEnabled = true;
            this.cmbFolders.Location = new System.Drawing.Point(75, 3);
            this.cmbFolders.Name = "cmbFolders";
            this.cmbFolders.Size = new System.Drawing.Size(367, 21);
            this.cmbFolders.Sorted = true;
            this.cmbFolders.TabIndex = 21;
            this.cmbFolders.SelectedIndexChanged += new System.EventHandler(this.cmbFolders_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "TV Folder";
            // 
            // lvShows
            // 
            this.lvShows.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvShows.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPath,
            this.colName,
            this.colYear,
            this.colOverview});
            this.lvShows.Contents = null;
            this.lvShows.DisplayGenres = true;
            this.lvShows.FullRowSelect = true;
            this.lvShows.HideSelection = false;
            this.lvShows.HideWatched = false;
            this.lvShows.Location = new System.Drawing.Point(3, 56);
            this.lvShows.MultiSelect = false;
            this.lvShows.Name = "lvShows";
            this.lvShows.Size = new System.Drawing.Size(615, 145);
            this.lvShows.TabIndex = 24;
            this.lvShows.UseCompatibleStateImageBehavior = false;
            this.lvShows.View = System.Windows.Forms.View.Details;
            this.lvShows.SelectedIndexChanged += new System.EventHandler(this.lvShows_SelectedIndexChanged);
            // 
            // colPath
            // 
            this.colPath.Text = "Folder Path";
            this.colPath.Width = 267;
            // 
            // colName
            // 
            this.colName.Text = "Show Name";
            this.colName.Width = 164;
            // 
            // colYear
            // 
            this.colYear.Text = "Year";
            this.colYear.Width = 200;
            // 
            // colOverview
            // 
            this.colOverview.Text = "Overview";
            this.colOverview.Width = 2000;
            // 
            // btnEditMovie
            // 
            this.btnEditMovie.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditMovie.Location = new System.Drawing.Point(543, 207);
            this.btnEditMovie.Name = "btnEditMovie";
            this.btnEditMovie.Size = new System.Drawing.Size(75, 24);
            this.btnEditMovie.TabIndex = 25;
            this.btnEditMovie.Text = "Edit";
            this.btnEditMovie.UseVisualStyleBackColor = true;
            this.btnEditMovie.Click += new System.EventHandler(this.btnEditShow_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.txtNameFilter);
            this.splitContainer1.Panel1.Controls.Add(this.lvShows);
            this.splitContainer1.Panel1.Controls.Add(this.lbInvalid);
            this.splitContainer1.Panel1.Controls.Add(this.pbUpdating);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btnEditMovie);
            this.splitContainer1.Panel1.Controls.Add(this.cmbFolders);
            this.splitContainer1.Panel1.Controls.Add(this.btnEditFolders);
            this.splitContainer1.Panel1.Controls.Add(this.btnForceRefresh);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gbEpisodes);
            this.splitContainer1.Size = new System.Drawing.Size(627, 542);
            this.splitContainer1.SplitterDistance = 240;
            this.splitContainer1.TabIndex = 31;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = "Name Filter";
            // 
            // txtNameFilter
            // 
            this.txtNameFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNameFilter.Location = new System.Drawing.Point(75, 30);
            this.txtNameFilter.Name = "txtNameFilter";
            this.txtNameFilter.Size = new System.Drawing.Size(543, 20);
            this.txtNameFilter.TabIndex = 35;
            this.txtNameFilter.TextChanged += new System.EventHandler(this.txtNameFilter_TextChanged);
            // 
            // lbInvalid
            // 
            this.lbInvalid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbInvalid.AutoSize = true;
            this.lbInvalid.Color = System.Drawing.Color.LightGray;
            this.lbInvalid.Label = "Invalid ID";
            this.lbInvalid.Location = new System.Drawing.Point(3, 204);
            this.lbInvalid.Margin = new System.Windows.Forms.Padding(0);
            this.lbInvalid.Name = "lbInvalid";
            this.lbInvalid.Size = new System.Drawing.Size(102, 30);
            this.lbInvalid.TabIndex = 29;
            // 
            // pbUpdating
            // 
            this.pbUpdating.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbUpdating.Location = new System.Drawing.Point(3, 56);
            this.pbUpdating.Message = "Updating Folders";
            this.pbUpdating.Name = "pbUpdating";
            this.pbUpdating.Size = new System.Drawing.Size(615, 23);
            this.pbUpdating.TabIndex = 30;
            // 
            // ShowsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ShowsControl";
            this.Size = new System.Drawing.Size(632, 550);
            this.gbEpisodes.ResumeLayout(false);
            this.gbEpisodes.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbEpFilter;
        private System.Windows.Forms.GroupBox gbEpisodes;
        private System.Windows.Forms.ListView lvEpisodes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.CheckBox chkDisplayIgnored;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Button btnUnignore;
        private LegendBoxControl lbIgnored;
        private LegendBoxControl lbUnaired;
        private LegendBoxControl lbMissing;
        private System.Windows.Forms.Button btnForceRefresh;
        private System.Windows.Forms.Button btnEditFolders;
        private System.Windows.Forms.ComboBox cmbFolders;
        private System.Windows.Forms.Label label1;
        private ContentListView lvShows;
        private System.Windows.Forms.ColumnHeader colPath;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colYear;
        private System.Windows.Forms.ColumnHeader colOverview;
        private System.Windows.Forms.Button btnEditMovie;
        private LegendBoxControl lbInvalid;
        private TextProgressBar pbUpdating;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private LegendBoxControl lbScanDir;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNameFilter;
    }
}
