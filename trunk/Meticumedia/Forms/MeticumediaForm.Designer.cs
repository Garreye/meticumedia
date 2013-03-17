// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class MeticumediaForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MeticumediaForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tcTv = new System.Windows.Forms.TabControl();
            this.tpShows = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cntrlShows = new Meticumedia.ShowsControl();
            this.cntrlEpisodes = new Meticumedia.EpisodesControl();
            this.linkTvRage = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.tbSchedule = new System.Windows.Forms.TabPage();
            this.linkTvRage2 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.cntrlSched = new Meticumedia.ScheduleControl();
            this.tpMovies = new System.Windows.Forms.TabPage();
            this.cntrlMovies = new Meticumedia.MovieControl();
            this.linkMovieDb = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.tpScan = new System.Windows.Forms.TabPage();
            this.cntrlScan = new Meticumedia.ScanControl();
            this.tpQueue = new System.Windows.Forms.TabPage();
            this.cntrlQueue = new Meticumedia.QueueControl();
            this.tpLog = new System.Windows.Forms.TabPage();
            this.cntrlLog = new Meticumedia.LogControl();
            this.menuStrip1.SuspendLayout();
            this.tcTv.SuspendLayout();
            this.tpShows.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tbSchedule.SuspendLayout();
            this.tpMovies.SuspendLayout();
            this.tpScan.SuspendLayout();
            this.tpQueue.SuspendLayout();
            this.tpLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(807, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.optionsToolStripMenuItem.Text = "Tools";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.donateToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // donateToolStripMenuItem
            // 
            this.donateToolStripMenuItem.Name = "donateToolStripMenuItem";
            this.donateToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.donateToolStripMenuItem.Text = "Donate";
            this.donateToolStripMenuItem.Click += new System.EventHandler(this.donateToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tcTv
            // 
            this.tcTv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcTv.Controls.Add(this.tpShows);
            this.tcTv.Controls.Add(this.tbSchedule);
            this.tcTv.Controls.Add(this.tpMovies);
            this.tcTv.Controls.Add(this.tpScan);
            this.tcTv.Controls.Add(this.tpQueue);
            this.tcTv.Controls.Add(this.tpLog);
            this.tcTv.Location = new System.Drawing.Point(12, 27);
            this.tcTv.Name = "tcTv";
            this.tcTv.SelectedIndex = 0;
            this.tcTv.Size = new System.Drawing.Size(783, 602);
            this.tcTv.TabIndex = 1;
            // 
            // tpShows
            // 
            this.tpShows.Controls.Add(this.splitContainer1);
            this.tpShows.Controls.Add(this.linkTvRage);
            this.tpShows.Controls.Add(this.label1);
            this.tpShows.Location = new System.Drawing.Point(4, 22);
            this.tpShows.Name = "tpShows";
            this.tpShows.Padding = new System.Windows.Forms.Padding(3);
            this.tpShows.Size = new System.Drawing.Size(775, 576);
            this.tpShows.TabIndex = 0;
            this.tpShows.Text = "TV Shows";
            this.tpShows.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Location = new System.Drawing.Point(9, 6);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cntrlShows);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cntrlEpisodes);
            this.splitContainer1.Size = new System.Drawing.Size(760, 551);
            this.splitContainer1.SplitterDistance = 275;
            this.splitContainer1.TabIndex = 3;
            // 
            // cntrlShows
            // 
            this.cntrlShows.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlShows.ContentType = Meticumedia.ContentType.TvShow;
            this.cntrlShows.Location = new System.Drawing.Point(3, 3);
            this.cntrlShows.Name = "cntrlShows";
            this.cntrlShows.Size = new System.Drawing.Size(755, 267);
            this.cntrlShows.TabIndex = 0;
            // 
            // cntrlEpisodes
            // 
            this.cntrlEpisodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlEpisodes.Location = new System.Drawing.Point(3, 3);
            this.cntrlEpisodes.Name = "cntrlEpisodes";
            this.cntrlEpisodes.Size = new System.Drawing.Size(752, 264);
            this.cntrlEpisodes.TabIndex = 0;
            this.cntrlEpisodes.TvShow = null;
            // 
            // linkTvRage
            // 
            this.linkTvRage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkTvRage.AutoSize = true;
            this.linkTvRage.Location = new System.Drawing.Point(193, 560);
            this.linkTvRage.Name = "linkTvRage";
            this.linkTvRage.Size = new System.Drawing.Size(47, 13);
            this.linkTvRage.TabIndex = 2;
            this.linkTvRage.TabStop = true;
            this.linkTvRage.Text = "TVRage";
            this.linkTvRage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.dbLink_LinkClicked);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 560);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "TV Show and Episode Information from";
            // 
            // tbSchedule
            // 
            this.tbSchedule.Controls.Add(this.linkTvRage2);
            this.tbSchedule.Controls.Add(this.label2);
            this.tbSchedule.Controls.Add(this.cntrlSched);
            this.tbSchedule.Location = new System.Drawing.Point(4, 22);
            this.tbSchedule.Name = "tbSchedule";
            this.tbSchedule.Size = new System.Drawing.Size(775, 576);
            this.tbSchedule.TabIndex = 4;
            this.tbSchedule.Text = "TV Schedule";
            this.tbSchedule.UseVisualStyleBackColor = true;
            // 
            // linkTvRage2
            // 
            this.linkTvRage2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkTvRage2.AutoSize = true;
            this.linkTvRage2.Location = new System.Drawing.Point(193, 560);
            this.linkTvRage2.Name = "linkTvRage2";
            this.linkTvRage2.Size = new System.Drawing.Size(47, 13);
            this.linkTvRage2.TabIndex = 4;
            this.linkTvRage2.TabStop = true;
            this.linkTvRage2.Text = "TVRage";
            this.linkTvRage2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.dbLink_LinkClicked);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 560);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(191, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "TV Show and Episode Information from";
            // 
            // cntrlSched
            // 
            this.cntrlSched.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlSched.Location = new System.Drawing.Point(3, 3);
            this.cntrlSched.Name = "cntrlSched";
            this.cntrlSched.Size = new System.Drawing.Size(769, 554);
            this.cntrlSched.TabIndex = 0;
            // 
            // tpMovies
            // 
            this.tpMovies.Controls.Add(this.cntrlMovies);
            this.tpMovies.Controls.Add(this.linkMovieDb);
            this.tpMovies.Controls.Add(this.label3);
            this.tpMovies.Location = new System.Drawing.Point(4, 22);
            this.tpMovies.Name = "tpMovies";
            this.tpMovies.Size = new System.Drawing.Size(775, 576);
            this.tpMovies.TabIndex = 5;
            this.tpMovies.Text = "Movies";
            this.tpMovies.UseVisualStyleBackColor = true;
            // 
            // cntrlMovies
            // 
            this.cntrlMovies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlMovies.ContentType = Meticumedia.ContentType.Movie;
            this.cntrlMovies.Location = new System.Drawing.Point(3, 3);
            this.cntrlMovies.Name = "cntrlMovies";
            this.cntrlMovies.Size = new System.Drawing.Size(769, 554);
            this.cntrlMovies.TabIndex = 7;
            // 
            // linkMovieDb
            // 
            this.linkMovieDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkMovieDb.AutoSize = true;
            this.linkMovieDb.Location = new System.Drawing.Point(118, 560);
            this.linkMovieDb.Name = "linkMovieDb";
            this.linkMovieDb.Size = new System.Drawing.Size(69, 13);
            this.linkMovieDb.TabIndex = 6;
            this.linkMovieDb.TabStop = true;
            this.linkMovieDb.Text = "TheMovieDb";
            this.linkMovieDb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.dbLink_LinkClicked);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 560);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Movie Information from";
            // 
            // tpScan
            // 
            this.tpScan.Controls.Add(this.cntrlScan);
            this.tpScan.Location = new System.Drawing.Point(4, 22);
            this.tpScan.Name = "tpScan";
            this.tpScan.Padding = new System.Windows.Forms.Padding(3);
            this.tpScan.Size = new System.Drawing.Size(775, 576);
            this.tpScan.TabIndex = 1;
            this.tpScan.Text = "Scan";
            this.tpScan.UseVisualStyleBackColor = true;
            // 
            // cntrlScan
            // 
            this.cntrlScan.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlScan.Location = new System.Drawing.Point(3, 3);
            this.cntrlScan.Name = "cntrlScan";
            this.cntrlScan.Size = new System.Drawing.Size(769, 570);
            this.cntrlScan.TabIndex = 0;
            // 
            // tpQueue
            // 
            this.tpQueue.Controls.Add(this.cntrlQueue);
            this.tpQueue.Location = new System.Drawing.Point(4, 22);
            this.tpQueue.Name = "tpQueue";
            this.tpQueue.Padding = new System.Windows.Forms.Padding(3);
            this.tpQueue.Size = new System.Drawing.Size(775, 576);
            this.tpQueue.TabIndex = 2;
            this.tpQueue.Text = "Queue";
            this.tpQueue.UseVisualStyleBackColor = true;
            // 
            // cntrlQueue
            // 
            this.cntrlQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlQueue.Location = new System.Drawing.Point(3, 3);
            this.cntrlQueue.Name = "cntrlQueue";
            this.cntrlQueue.Size = new System.Drawing.Size(769, 570);
            this.cntrlQueue.TabIndex = 0;
            // 
            // tpLog
            // 
            this.tpLog.Controls.Add(this.cntrlLog);
            this.tpLog.Location = new System.Drawing.Point(4, 22);
            this.tpLog.Name = "tpLog";
            this.tpLog.Padding = new System.Windows.Forms.Padding(3);
            this.tpLog.Size = new System.Drawing.Size(775, 576);
            this.tpLog.TabIndex = 3;
            this.tpLog.Text = "Log";
            this.tpLog.UseVisualStyleBackColor = true;
            // 
            // cntrlLog
            // 
            this.cntrlLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlLog.Location = new System.Drawing.Point(3, 0);
            this.cntrlLog.Name = "cntrlLog";
            this.cntrlLog.Size = new System.Drawing.Size(769, 573);
            this.cntrlLog.TabIndex = 0;
            // 
            // MeticumediaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 641);
            this.Controls.Add(this.tcTv);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MeticumediaForm";
            this.Text = "meticumedia";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MeticumediaForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tcTv.ResumeLayout(false);
            this.tpShows.ResumeLayout(false);
            this.tpShows.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tbSchedule.ResumeLayout(false);
            this.tbSchedule.PerformLayout();
            this.tpMovies.ResumeLayout(false);
            this.tpMovies.PerformLayout();
            this.tpScan.ResumeLayout(false);
            this.tpQueue.ResumeLayout(false);
            this.tpLog.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.TabControl tcTv;
        private System.Windows.Forms.TabPage tpShows;
        private System.Windows.Forms.TabPage tpScan;
        private System.Windows.Forms.TabPage tpQueue;
        private System.Windows.Forms.TabPage tpLog;
        private ScanControl cntrlScan;
        private QueueControl cntrlQueue;
        private LogControl cntrlLog;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.TabPage tbSchedule;
        private ScheduleControl cntrlSched;
        private System.Windows.Forms.TabPage tpMovies;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem donateToolStripMenuItem;
        private System.Windows.Forms.LinkLabel linkTvRage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkTvRage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkMovieDb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private EpisodesControl cntrlEpisodes;
        private MovieControl cntrlMovies;
        private ShowsControl cntrlShows;
    }
}

