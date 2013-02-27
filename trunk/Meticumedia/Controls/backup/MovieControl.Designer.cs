﻿// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class MovieControl
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
            this.cmbFolders = new System.Windows.Forms.ComboBox();
            this.btnEditFolders = new System.Windows.Forms.Button();
            this.btnForceRefresh = new System.Windows.Forms.Button();
            this.btnEditMovie = new System.Windows.Forms.Button();
            this.cmbGenre = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnWatched = new System.Windows.Forms.Button();
            this.btnUnwatched = new System.Windows.Forms.Button();
            this.chkHideWatched = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkYearFilter = new System.Windows.Forms.CheckBox();
            this.numMinYear = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numMaxYear = new System.Windows.Forms.NumericUpDown();
            this.txtNameFilter = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lvMovieDirectory = new Meticumedia.ContentListView();
            this.colPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colYear = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colGenre = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOverview = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbInvalid = new Meticumedia.LegendBoxControl();
            this.lbUnwatched = new Meticumedia.LegendBoxControl();
            this.pbUpdating = new TextProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.numMinYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxYear)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbFolders
            // 
            this.cmbFolders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFolders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFolders.FormattingEnabled = true;
            this.cmbFolders.Location = new System.Drawing.Point(77, 3);
            this.cmbFolders.Name = "cmbFolders";
            this.cmbFolders.Size = new System.Drawing.Size(375, 21);
            this.cmbFolders.Sorted = true;
            this.cmbFolders.TabIndex = 8;
            this.cmbFolders.SelectedIndexChanged += new System.EventHandler(this.cmbFolders_SelectedIndexChanged);
            // 
            // btnEditFolders
            // 
            this.btnEditFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditFolders.Location = new System.Drawing.Point(458, 3);
            this.btnEditFolders.Name = "btnEditFolders";
            this.btnEditFolders.Size = new System.Drawing.Size(75, 23);
            this.btnEditFolders.TabIndex = 18;
            this.btnEditFolders.Text = "Edit List";
            this.btnEditFolders.UseVisualStyleBackColor = true;
            this.btnEditFolders.Click += new System.EventHandler(this.btnEditDir_Click);
            // 
            // btnForceRefresh
            // 
            this.btnForceRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnForceRefresh.Location = new System.Drawing.Point(539, 3);
            this.btnForceRefresh.Name = "btnForceRefresh";
            this.btnForceRefresh.Size = new System.Drawing.Size(89, 23);
            this.btnForceRefresh.TabIndex = 19;
            this.btnForceRefresh.Text = "Force Refresh";
            this.btnForceRefresh.UseVisualStyleBackColor = true;
            this.btnForceRefresh.Click += new System.EventHandler(this.btnForceRefresh_Click);
            // 
            // btnEditMovie
            // 
            this.btnEditMovie.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditMovie.Location = new System.Drawing.Point(553, 457);
            this.btnEditMovie.Name = "btnEditMovie";
            this.btnEditMovie.Size = new System.Drawing.Size(75, 24);
            this.btnEditMovie.TabIndex = 20;
            this.btnEditMovie.Text = "Edit";
            this.btnEditMovie.UseVisualStyleBackColor = true;
            this.btnEditMovie.Click += new System.EventHandler(this.btnEditMovie_Click);
            // 
            // cmbGenre
            // 
            this.cmbGenre.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbGenre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGenre.FormattingEnabled = true;
            this.cmbGenre.Location = new System.Drawing.Point(77, 30);
            this.cmbGenre.Name = "cmbGenre";
            this.cmbGenre.Size = new System.Drawing.Size(375, 21);
            this.cmbGenre.TabIndex = 22;
            this.cmbGenre.SelectedIndexChanged += new System.EventHandler(this.cmbGenre_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Genre";
            // 
            // btnWatched
            // 
            this.btnWatched.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWatched.Location = new System.Drawing.Point(472, 457);
            this.btnWatched.Name = "btnWatched";
            this.btnWatched.Size = new System.Drawing.Size(75, 24);
            this.btnWatched.TabIndex = 24;
            this.btnWatched.Text = "Watched";
            this.btnWatched.UseVisualStyleBackColor = true;
            this.btnWatched.Click += new System.EventHandler(this.btnWatchMod_Click);
            // 
            // btnUnwatched
            // 
            this.btnUnwatched.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnwatched.Location = new System.Drawing.Point(391, 457);
            this.btnUnwatched.Name = "btnUnwatched";
            this.btnUnwatched.Size = new System.Drawing.Size(75, 24);
            this.btnUnwatched.TabIndex = 25;
            this.btnUnwatched.Text = "Unwatched";
            this.btnUnwatched.UseVisualStyleBackColor = true;
            this.btnUnwatched.Click += new System.EventHandler(this.btnWatchMod_Click);
            // 
            // chkHideWatched
            // 
            this.chkHideWatched.AutoSize = true;
            this.chkHideWatched.Location = new System.Drawing.Point(7, 111);
            this.chkHideWatched.Name = "chkHideWatched";
            this.chkHideWatched.Size = new System.Drawing.Size(95, 17);
            this.chkHideWatched.TabIndex = 26;
            this.chkHideWatched.Text = "Hide Watched";
            this.chkHideWatched.UseVisualStyleBackColor = true;
            this.chkHideWatched.CheckedChanged += new System.EventHandler(this.chkHideWatched_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Movie Folder";
            // 
            // chkYearFilter
            // 
            this.chkYearFilter.AutoSize = true;
            this.chkYearFilter.Location = new System.Drawing.Point(7, 60);
            this.chkYearFilter.Name = "chkYearFilter";
            this.chkYearFilter.Size = new System.Drawing.Size(48, 17);
            this.chkYearFilter.TabIndex = 29;
            this.chkYearFilter.Text = "Year";
            this.chkYearFilter.UseVisualStyleBackColor = true;
            this.chkYearFilter.CheckedChanged += new System.EventHandler(this.chkYearFilter_CheckedChanged);
            // 
            // numMinYear
            // 
            this.numMinYear.Location = new System.Drawing.Point(77, 59);
            this.numMinYear.Maximum = new decimal(new int[] {
            2050,
            0,
            0,
            0});
            this.numMinYear.Minimum = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            this.numMinYear.Name = "numMinYear";
            this.numMinYear.Size = new System.Drawing.Size(53, 20);
            this.numMinYear.TabIndex = 30;
            this.numMinYear.Value = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            this.numMinYear.ValueChanged += new System.EventHandler(this.numMinYear_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(136, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "to";
            // 
            // numMaxYear
            // 
            this.numMaxYear.Location = new System.Drawing.Point(158, 59);
            this.numMaxYear.Maximum = new decimal(new int[] {
            2050,
            0,
            0,
            0});
            this.numMaxYear.Minimum = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            this.numMaxYear.Name = "numMaxYear";
            this.numMaxYear.Size = new System.Drawing.Size(53, 20);
            this.numMaxYear.TabIndex = 32;
            this.numMaxYear.Value = new decimal(new int[] {
            2013,
            0,
            0,
            0});
            this.numMaxYear.ValueChanged += new System.EventHandler(this.numMaxYear_ValueChanged);
            // 
            // txtNameFilter
            // 
            this.txtNameFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNameFilter.Location = new System.Drawing.Point(76, 85);
            this.txtNameFilter.Name = "txtNameFilter";
            this.txtNameFilter.Size = new System.Drawing.Size(551, 20);
            this.txtNameFilter.TabIndex = 33;
            this.txtNameFilter.TextChanged += new System.EventHandler(this.txtNameFilter_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "Name Filter";
            // 
            // lvMovieDirectory
            // 
            this.lvMovieDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvMovieDirectory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPath,
            this.colName,
            this.colYear,
            this.colGenre,
            this.colOverview});
            this.lvMovieDirectory.Contents = null;
            this.lvMovieDirectory.FullRowSelect = true;
            this.lvMovieDirectory.HideWatched = false;
            this.lvMovieDirectory.Location = new System.Drawing.Point(6, 134);
            this.lvMovieDirectory.Name = "lvMovieDirectory";
            this.lvMovieDirectory.Size = new System.Drawing.Size(622, 317);
            this.lvMovieDirectory.TabIndex = 1;
            this.lvMovieDirectory.UseCompatibleStateImageBehavior = false;
            this.lvMovieDirectory.View = System.Windows.Forms.View.Details;
            // 
            // colPath
            // 
            this.colPath.Text = "Folder Path";
            this.colPath.Width = 267;
            // 
            // colName
            // 
            this.colName.Text = "Movie Name";
            this.colName.Width = 164;
            // 
            // colYear
            // 
            this.colYear.Text = "Year";
            this.colYear.Width = 50;
            // 
            // colGenre
            // 
            this.colGenre.Text = "Genre";
            this.colGenre.Width = 200;
            // 
            // colOverview
            // 
            this.colOverview.Text = "Overview";
            this.colOverview.Width = 2000;
            // 
            // lbInvalid
            // 
            this.lbInvalid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbInvalid.AutoSize = true;
            this.lbInvalid.Color = System.Drawing.Color.LightGray;
            this.lbInvalid.Label = "Invalid ID";
            this.lbInvalid.Location = new System.Drawing.Point(98, 454);
            this.lbInvalid.Margin = new System.Windows.Forms.Padding(0);
            this.lbInvalid.Name = "lbInvalid";
            this.lbInvalid.Size = new System.Drawing.Size(92, 30);
            this.lbInvalid.TabIndex = 27;
            // 
            // lbUnwatched
            // 
            this.lbUnwatched.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbUnwatched.AutoSize = true;
            this.lbUnwatched.Color = System.Drawing.Color.LightGray;
            this.lbUnwatched.Label = "Watched";
            this.lbUnwatched.Location = new System.Drawing.Point(7, 454);
            this.lbUnwatched.Margin = new System.Windows.Forms.Padding(0);
            this.lbUnwatched.Name = "lbUnwatched";
            this.lbUnwatched.Size = new System.Drawing.Size(91, 30);
            this.lbUnwatched.TabIndex = 23;
            // 
            // pbUpdating
            // 
            this.pbUpdating.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbUpdating.Location = new System.Drawing.Point(5, 134);
            this.pbUpdating.Message = "Updating Folders";
            this.pbUpdating.Name = "pbUpdating";
            this.pbUpdating.Size = new System.Drawing.Size(621, 24);
            this.pbUpdating.TabIndex = 28;
            // 
            // MovieControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtNameFilter);
            this.Controls.Add(this.numMaxYear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numMinYear);
            this.Controls.Add(this.chkYearFilter);
            this.Controls.Add(this.lvMovieDirectory);
            this.Controls.Add(this.lbInvalid);
            this.Controls.Add(this.chkHideWatched);
            this.Controls.Add(this.btnUnwatched);
            this.Controls.Add(this.btnWatched);
            this.Controls.Add(this.lbUnwatched);
            this.Controls.Add(this.cmbGenre);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnEditMovie);
            this.Controls.Add(this.btnForceRefresh);
            this.Controls.Add(this.btnEditFolders);
            this.Controls.Add(this.cmbFolders);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pbUpdating);
            this.Name = "MovieControl";
            this.Size = new System.Drawing.Size(634, 484);
            ((System.ComponentModel.ISupportInitialize)(this.numMinYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxYear)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ContentListView lvMovieDirectory;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colPath;
        private System.Windows.Forms.ComboBox cmbFolders;
        private System.Windows.Forms.ColumnHeader colYear;
        private System.Windows.Forms.Button btnEditFolders;
        private System.Windows.Forms.Button btnForceRefresh;
        private System.Windows.Forms.ColumnHeader colGenre;
        private System.Windows.Forms.ColumnHeader colOverview;
        private System.Windows.Forms.Button btnEditMovie;
        private System.Windows.Forms.ComboBox cmbGenre;
        private System.Windows.Forms.Label label2;
        private LegendBoxControl lbUnwatched;
        private System.Windows.Forms.Button btnWatched;
        private System.Windows.Forms.Button btnUnwatched;
        private System.Windows.Forms.CheckBox chkHideWatched;
        private System.Windows.Forms.Label label3;
        private LegendBoxControl lbInvalid;
        private TextProgressBar pbUpdating;
        private System.Windows.Forms.CheckBox chkYearFilter;
        private System.Windows.Forms.NumericUpDown numMinYear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numMaxYear;
        private System.Windows.Forms.TextBox txtNameFilter;
        private System.Windows.Forms.Label label4;
    }
}