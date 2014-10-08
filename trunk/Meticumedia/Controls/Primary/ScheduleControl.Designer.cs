// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class ScheduleControl
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
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numDays = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.lblLastNext = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbShows = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkDisplayOverview = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbEpFilter = new System.Windows.Forms.ComboBox();
            this.lbScanDir = new Meticumedia.LegendBoxControl();
            this.lbIgnored = new Meticumedia.LegendBoxControl();
            this.lbUnaired = new Meticumedia.LegendBoxControl();
            this.lbMissing = new Meticumedia.LegendBoxControl();
            this.lvResults = new Meticumedia.DoubleBufferedListView();
            this.colShow = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSeason = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEpisode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOverview = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDays)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(7, 19);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(182, 21);
            this.cmbType.Sorted = true;
            this.cmbType.TabIndex = 15;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.numDays);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblLastNext);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbType);
            this.groupBox1.Controls.Add(this.cmbShows);
            this.groupBox1.Location = new System.Drawing.Point(3, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(686, 53);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setup";
            // 
            // numDays
            // 
            this.numDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numDays.Location = new System.Drawing.Point(561, 20);
            this.numDays.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDays.Name = "numDays";
            this.numDays.Size = new System.Drawing.Size(84, 20);
            this.numDays.TabIndex = 27;
            this.numDays.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.numDays.ValueChanged += new System.EventHandler(this.numDays_ValueChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(651, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "days";
            // 
            // lblLastNext
            // 
            this.lblLastNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastNext.AutoSize = true;
            this.lblLastNext.Location = new System.Drawing.Point(509, 22);
            this.lblLastNext.Name = "lblLastNext";
            this.lblLastNext.Size = new System.Drawing.Size(46, 13);
            this.lblLastNext.TabIndex = 24;
            this.lblLastNext.Text = "from last";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(195, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "episodes for";
            // 
            // cmbShows
            // 
            this.cmbShows.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbShows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShows.FormattingEnabled = true;
            this.cmbShows.Location = new System.Drawing.Point(265, 19);
            this.cmbShows.Name = "cmbShows";
            this.cmbShows.Size = new System.Drawing.Size(238, 21);
            this.cmbShows.TabIndex = 9;
            this.cmbShows.SelectedIndexChanged += new System.EventHandler(this.cmbShows_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmbEpFilter);
            this.groupBox2.Controls.Add(this.lbScanDir);
            this.groupBox2.Controls.Add(this.lbIgnored);
            this.groupBox2.Controls.Add(this.lbUnaired);
            this.groupBox2.Controls.Add(this.lbMissing);
            this.groupBox2.Controls.Add(this.chkDisplayOverview);
            this.groupBox2.Controls.Add(this.lvResults);
            this.groupBox2.Location = new System.Drawing.Point(3, 65);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(686, 464);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Results";
            // 
            // chkDisplayOverview
            // 
            this.chkDisplayOverview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDisplayOverview.AutoSize = true;
            this.chkDisplayOverview.Location = new System.Drawing.Point(498, 19);
            this.chkDisplayOverview.Name = "chkDisplayOverview";
            this.chkDisplayOverview.Size = new System.Drawing.Size(176, 17);
            this.chkDisplayOverview.TabIndex = 15;
            this.chkDisplayOverview.Text = "Display Overview (Spoiler Alert!)";
            this.chkDisplayOverview.UseVisualStyleBackColor = true;
            this.chkDisplayOverview.CheckedChanged += new System.EventHandler(this.chkDisplayOverview_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Filter";
            // 
            // cmbEpFilter
            // 
            this.cmbEpFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEpFilter.FormattingEnabled = true;
            this.cmbEpFilter.Location = new System.Drawing.Point(41, 15);
            this.cmbEpFilter.Name = "cmbEpFilter";
            this.cmbEpFilter.Size = new System.Drawing.Size(226, 21);
            this.cmbEpFilter.TabIndex = 30;
            // 
            // lbScanDir
            // 
            this.lbScanDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbScanDir.AutoSize = true;
            this.lbScanDir.Color = System.Drawing.SystemColors.Control;
            this.lbScanDir.Label = "In Scan Dir.";
            this.lbScanDir.Location = new System.Drawing.Point(89, 431);
            this.lbScanDir.Margin = new System.Windows.Forms.Padding(0);
            this.lbScanDir.Name = "lbScanDir";
            this.lbScanDir.Size = new System.Drawing.Size(103, 30);
            this.lbScanDir.TabIndex = 29;
            // 
            // lbIgnored
            // 
            this.lbIgnored.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbIgnored.AutoSize = true;
            this.lbIgnored.Color = System.Drawing.SystemColors.Control;
            this.lbIgnored.Label = "Ignored";
            this.lbIgnored.Location = new System.Drawing.Point(276, 431);
            this.lbIgnored.Margin = new System.Windows.Forms.Padding(0);
            this.lbIgnored.Name = "lbIgnored";
            this.lbIgnored.Size = new System.Drawing.Size(83, 30);
            this.lbIgnored.TabIndex = 28;
            this.lbIgnored.Visible = false;
            // 
            // lbUnaired
            // 
            this.lbUnaired.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbUnaired.AutoSize = true;
            this.lbUnaired.Color = System.Drawing.SystemColors.Control;
            this.lbUnaired.Label = "Unaired";
            this.lbUnaired.Location = new System.Drawing.Point(192, 431);
            this.lbUnaired.Margin = new System.Windows.Forms.Padding(0);
            this.lbUnaired.Name = "lbUnaired";
            this.lbUnaired.Size = new System.Drawing.Size(84, 30);
            this.lbUnaired.TabIndex = 27;
            // 
            // lbMissing
            // 
            this.lbMissing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbMissing.AutoSize = true;
            this.lbMissing.Color = System.Drawing.SystemColors.Control;
            this.lbMissing.Label = "Missing";
            this.lbMissing.Location = new System.Drawing.Point(7, 431);
            this.lbMissing.Margin = new System.Windows.Forms.Padding(0);
            this.lbMissing.Name = "lbMissing";
            this.lbMissing.Size = new System.Drawing.Size(82, 30);
            this.lbMissing.TabIndex = 26;
            // 
            // lvResults
            // 
            this.lvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colShow,
            this.colSeason,
            this.colEpisode,
            this.colName,
            this.colDate,
            this.colOverview});
            this.lvResults.FullRowSelect = true;
            this.lvResults.Location = new System.Drawing.Point(6, 42);
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new System.Drawing.Size(668, 386);
            this.lvResults.TabIndex = 11;
            this.lvResults.UseCompatibleStateImageBehavior = false;
            this.lvResults.View = System.Windows.Forms.View.Details;
            this.lvResults.DoubleClick += new System.EventHandler(this.lvResults_DoubleClick);
            // 
            // colShow
            // 
            this.colShow.Text = "Show";
            this.colShow.Width = 200;
            // 
            // colSeason
            // 
            this.colSeason.Text = "Season";
            // 
            // colEpisode
            // 
            this.colEpisode.Text = "Episode";
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 170;
            // 
            // colDate
            // 
            this.colDate.Text = "Air Date";
            this.colDate.Width = 80;
            // 
            // colOverview
            // 
            this.colOverview.Text = "Overview";
            this.colOverview.Width = 200;
            // 
            // ScheduleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ScheduleControl";
            this.Size = new System.Drawing.Size(692, 532);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDays)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufferedListView lvResults;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.ColumnHeader colShow;
        private System.Windows.Forms.ColumnHeader colSeason;
        private System.Windows.Forms.ColumnHeader colEpisode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numDays;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblLastNext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbShows;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colOverview;
        private System.Windows.Forms.CheckBox chkDisplayOverview;
        private System.Windows.Forms.ColumnHeader colDate;
        private LegendBoxControl lbIgnored;
        private LegendBoxControl lbUnaired;
        private LegendBoxControl lbMissing;
        private LegendBoxControl lbScanDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbEpFilter;
    }
}
