// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class EpisodeEditControl
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
            this.label7 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.chkIgnored = new System.Windows.Forms.CheckBox();
            this.numSeason = new System.Windows.Forms.NumericUpDown();
            this.lblSeason = new System.Windows.Forms.Label();
            this.lblEpisode = new System.Windows.Forms.Label();
            this.numEpisode = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numYear = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numMonth = new System.Windows.Forms.NumericUpDown();
            this.numDay = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOverview = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkDisableDatabase = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numSeason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEpisode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMonth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDay)).BeginInit();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(62, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(186, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // chkIgnored
            // 
            this.chkIgnored.AutoSize = true;
            this.chkIgnored.Location = new System.Drawing.Point(254, 6);
            this.chkIgnored.Name = "chkIgnored";
            this.chkIgnored.Size = new System.Drawing.Size(62, 17);
            this.chkIgnored.TabIndex = 23;
            this.chkIgnored.Text = "Ignored";
            this.chkIgnored.UseVisualStyleBackColor = true;
            this.chkIgnored.CheckedChanged += new System.EventHandler(this.chkIgnored_CheckedChanged);
            // 
            // numSeason
            // 
            this.numSeason.Enabled = false;
            this.numSeason.Location = new System.Drawing.Point(62, 29);
            this.numSeason.Name = "numSeason";
            this.numSeason.Size = new System.Drawing.Size(62, 20);
            this.numSeason.TabIndex = 26;
            this.numSeason.ValueChanged += new System.EventHandler(this.numSeason_ValueChanged);
            // 
            // lblSeason
            // 
            this.lblSeason.AutoSize = true;
            this.lblSeason.Enabled = false;
            this.lblSeason.Location = new System.Drawing.Point(3, 31);
            this.lblSeason.Name = "lblSeason";
            this.lblSeason.Size = new System.Drawing.Size(43, 13);
            this.lblSeason.TabIndex = 27;
            this.lblSeason.Text = "Season";
            // 
            // lblEpisode
            // 
            this.lblEpisode.AutoSize = true;
            this.lblEpisode.Enabled = false;
            this.lblEpisode.Location = new System.Drawing.Point(139, 31);
            this.lblEpisode.Name = "lblEpisode";
            this.lblEpisode.Size = new System.Drawing.Size(45, 13);
            this.lblEpisode.TabIndex = 29;
            this.lblEpisode.Text = "Episode";
            // 
            // numEpisode
            // 
            this.numEpisode.Enabled = false;
            this.numEpisode.Location = new System.Drawing.Point(190, 29);
            this.numEpisode.Name = "numEpisode";
            this.numEpisode.Size = new System.Drawing.Size(62, 20);
            this.numEpisode.TabIndex = 28;
            this.numEpisode.ValueChanged += new System.EventHandler(this.numEpisode_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Date (Y/M/D)";
            // 
            // numYear
            // 
            this.numYear.Location = new System.Drawing.Point(82, 55);
            this.numYear.Maximum = new decimal(new int[] {
            2100,
            0,
            0,
            0});
            this.numYear.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numYear.Name = "numYear";
            this.numYear.Size = new System.Drawing.Size(62, 20);
            this.numYear.TabIndex = 31;
            this.numYear.Value = new decimal(new int[] {
            2013,
            0,
            0,
            0});
            this.numYear.ValueChanged += new System.EventHandler(this.numDate_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "/";
            // 
            // numMonth
            // 
            this.numMonth.Location = new System.Drawing.Point(168, 57);
            this.numMonth.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numMonth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMonth.Name = "numMonth";
            this.numMonth.Size = new System.Drawing.Size(62, 20);
            this.numMonth.TabIndex = 33;
            this.numMonth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMonth.ValueChanged += new System.EventHandler(this.numDate_ValueChanged);
            // 
            // numDay
            // 
            this.numDay.Location = new System.Drawing.Point(254, 57);
            this.numDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDay.Name = "numDay";
            this.numDay.Size = new System.Drawing.Size(62, 20);
            this.numDay.TabIndex = 34;
            this.numDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDay.ValueChanged += new System.EventHandler(this.numDate_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(236, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "/";
            // 
            // txtOverview
            // 
            this.txtOverview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOverview.Location = new System.Drawing.Point(82, 83);
            this.txtOverview.Multiline = true;
            this.txtOverview.Name = "txtOverview";
            this.txtOverview.Size = new System.Drawing.Size(234, 77);
            this.txtOverview.TabIndex = 36;
            this.txtOverview.TextChanged += new System.EventHandler(this.txtOverview_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "Overview";
            // 
            // chkDisableDatabase
            // 
            this.chkDisableDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkDisableDatabase.AutoSize = true;
            this.chkDisableDatabase.Location = new System.Drawing.Point(4, 166);
            this.chkDisableDatabase.Name = "chkDisableDatabase";
            this.chkDisableDatabase.Size = new System.Drawing.Size(232, 17);
            this.chkDisableDatabase.TabIndex = 38;
            this.chkDisableDatabase.Text = "Disable Updating Properties From Database";
            this.chkDisableDatabase.UseVisualStyleBackColor = true;
            this.chkDisableDatabase.CheckedChanged += new System.EventHandler(this.chkDisableDatabase_CheckedChanged);
            // 
            // EpisodeEditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkDisableDatabase);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOverview);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numDay);
            this.Controls.Add(this.numMonth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numYear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblEpisode);
            this.Controls.Add(this.numEpisode);
            this.Controls.Add(this.lblSeason);
            this.Controls.Add(this.numSeason);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.chkIgnored);
            this.Name = "EpisodeEditControl";
            this.Size = new System.Drawing.Size(322, 190);
            ((System.ComponentModel.ISupportInitialize)(this.numSeason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEpisode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMonth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.CheckBox chkIgnored;
        private System.Windows.Forms.NumericUpDown numSeason;
        private System.Windows.Forms.Label lblSeason;
        private System.Windows.Forms.Label lblEpisode;
        private System.Windows.Forms.NumericUpDown numEpisode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numYear;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numMonth;
        private System.Windows.Forms.NumericUpDown numDay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOverview;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkDisableDatabase;
    }
}
