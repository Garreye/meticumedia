// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class FileNameFormatControl
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
            this.dgvNameFormat = new System.Windows.Forms.DataGridView();
            this.colHeader = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colFooter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWhitespace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCase = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbFormat = new System.Windows.Forms.GroupBox();
            this.gbEpFormat = new System.Windows.Forms.GroupBox();
            this.chkSeasonDoubleDigits = new System.Windows.Forms.CheckBox();
            this.chkEpisodeDoubleDigits = new System.Windows.Forms.CheckBox();
            this.chkHeaderPerEpisode = new System.Windows.Forms.CheckBox();
            this.chkSeasonFirst = new System.Windows.Forms.CheckBox();
            this.txtEpisodeHeader = new System.Windows.Forms.TextBox();
            this.txtSeasonHeader = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gbPreview = new System.Windows.Forms.GroupBox();
            this.txtFilePreview2 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lblExample2 = new System.Windows.Forms.Label();
            this.txtFilePreview1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lblExample1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNameFormat)).BeginInit();
            this.gbFormat.SuspendLayout();
            this.gbEpFormat.SuspendLayout();
            this.gbPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvNameFormat
            // 
            this.dgvNameFormat.AllowUserToOrderColumns = true;
            this.dgvNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvNameFormat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvNameFormat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNameFormat.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHeader,
            this.colType,
            this.colFooter,
            this.colWhitespace,
            this.colCase,
            this.colValue});
            this.dgvNameFormat.Location = new System.Drawing.Point(6, 19);
            this.dgvNameFormat.Name = "dgvNameFormat";
            this.dgvNameFormat.Size = new System.Drawing.Size(438, 113);
            this.dgvNameFormat.TabIndex = 0;
            this.dgvNameFormat.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNameFormat_CellValueChanged);
            this.dgvNameFormat.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNameFormat_CellValueChanged);
            // 
            // colHeader
            // 
            this.colHeader.FillWeight = 10F;
            this.colHeader.HeaderText = "Header";
            this.colHeader.Name = "colHeader";
            this.colHeader.Width = 50;
            // 
            // colType
            // 
            this.colType.HeaderText = "Type";
            this.colType.Name = "colType";
            this.colType.Width = 150;
            // 
            // colFooter
            // 
            this.colFooter.FillWeight = 10F;
            this.colFooter.HeaderText = "Footer";
            this.colFooter.Name = "colFooter";
            this.colFooter.Width = 50;
            // 
            // colWhitespace
            // 
            this.colWhitespace.HeaderText = "Whitespace";
            this.colWhitespace.Name = "colWhitespace";
            this.colWhitespace.Width = 70;
            // 
            // colCase
            // 
            this.colCase.HeaderText = "Case  Option";
            this.colCase.Name = "colCase";
            this.colCase.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colCase.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colValue
            // 
            this.colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colValue.HeaderText = "Custom String Entry";
            this.colValue.Name = "colValue";
            // 
            // gbFormat
            // 
            this.gbFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFormat.Controls.Add(this.dgvNameFormat);
            this.gbFormat.Location = new System.Drawing.Point(3, 3);
            this.gbFormat.Name = "gbFormat";
            this.gbFormat.Size = new System.Drawing.Size(450, 138);
            this.gbFormat.TabIndex = 1;
            this.gbFormat.TabStop = false;
            this.gbFormat.Text = "File Name Format";
            // 
            // gbEpFormat
            // 
            this.gbEpFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEpFormat.Controls.Add(this.chkSeasonDoubleDigits);
            this.gbEpFormat.Controls.Add(this.chkEpisodeDoubleDigits);
            this.gbEpFormat.Controls.Add(this.chkHeaderPerEpisode);
            this.gbEpFormat.Controls.Add(this.chkSeasonFirst);
            this.gbEpFormat.Controls.Add(this.txtEpisodeHeader);
            this.gbEpFormat.Controls.Add(this.txtSeasonHeader);
            this.gbEpFormat.Controls.Add(this.label2);
            this.gbEpFormat.Controls.Add(this.label1);
            this.gbEpFormat.Location = new System.Drawing.Point(3, 147);
            this.gbEpFormat.Name = "gbEpFormat";
            this.gbEpFormat.Size = new System.Drawing.Size(450, 78);
            this.gbEpFormat.TabIndex = 2;
            this.gbEpFormat.TabStop = false;
            this.gbEpFormat.Text = "Episode Number Format";
            // 
            // chkSeasonDoubleDigits
            // 
            this.chkSeasonDoubleDigits.AutoSize = true;
            this.chkSeasonDoubleDigits.Location = new System.Drawing.Point(279, 21);
            this.chkSeasonDoubleDigits.Name = "chkSeasonDoubleDigits";
            this.chkSeasonDoubleDigits.Size = new System.Drawing.Size(158, 17);
            this.chkSeasonDoubleDigits.TabIndex = 7;
            this.chkSeasonDoubleDigits.Text = "Force Season Double Digits";
            this.chkSeasonDoubleDigits.UseVisualStyleBackColor = true;
            this.chkSeasonDoubleDigits.CheckedChanged += new System.EventHandler(this.episodeFormatChanges);
            // 
            // chkEpisodeDoubleDigits
            // 
            this.chkEpisodeDoubleDigits.AutoSize = true;
            this.chkEpisodeDoubleDigits.Location = new System.Drawing.Point(279, 44);
            this.chkEpisodeDoubleDigits.Name = "chkEpisodeDoubleDigits";
            this.chkEpisodeDoubleDigits.Size = new System.Drawing.Size(160, 17);
            this.chkEpisodeDoubleDigits.TabIndex = 6;
            this.chkEpisodeDoubleDigits.Text = "Force Episode Double Digits";
            this.chkEpisodeDoubleDigits.UseVisualStyleBackColor = true;
            this.chkEpisodeDoubleDigits.CheckedChanged += new System.EventHandler(this.episodeFormatChanges);
            // 
            // chkHeaderPerEpisode
            // 
            this.chkHeaderPerEpisode.AutoSize = true;
            this.chkHeaderPerEpisode.Location = new System.Drawing.Point(151, 44);
            this.chkHeaderPerEpisode.Name = "chkHeaderPerEpisode";
            this.chkHeaderPerEpisode.Size = new System.Drawing.Size(121, 17);
            this.chkHeaderPerEpisode.TabIndex = 5;
            this.chkHeaderPerEpisode.Text = "Header Per Epiosde";
            this.chkHeaderPerEpisode.UseVisualStyleBackColor = true;
            this.chkHeaderPerEpisode.CheckedChanged += new System.EventHandler(this.episodeFormatChanges);
            // 
            // chkSeasonFirst
            // 
            this.chkSeasonFirst.AutoSize = true;
            this.chkSeasonFirst.Location = new System.Drawing.Point(151, 21);
            this.chkSeasonFirst.Name = "chkSeasonFirst";
            this.chkSeasonFirst.Size = new System.Drawing.Size(84, 17);
            this.chkSeasonFirst.TabIndex = 4;
            this.chkSeasonFirst.Text = "Season First";
            this.chkSeasonFirst.UseVisualStyleBackColor = true;
            this.chkSeasonFirst.CheckedChanged += new System.EventHandler(this.episodeFormatChanges);
            // 
            // txtEpisodeHeader
            // 
            this.txtEpisodeHeader.Location = new System.Drawing.Point(95, 45);
            this.txtEpisodeHeader.Name = "txtEpisodeHeader";
            this.txtEpisodeHeader.Size = new System.Drawing.Size(50, 20);
            this.txtEpisodeHeader.TabIndex = 3;
            this.txtEpisodeHeader.TextChanged += new System.EventHandler(this.episodeFormatChanges);
            // 
            // txtSeasonHeader
            // 
            this.txtSeasonHeader.Location = new System.Drawing.Point(95, 19);
            this.txtSeasonHeader.Name = "txtSeasonHeader";
            this.txtSeasonHeader.Size = new System.Drawing.Size(50, 20);
            this.txtSeasonHeader.TabIndex = 2;
            this.txtSeasonHeader.TextChanged += new System.EventHandler(this.episodeFormatChanges);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Episode Header";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Season Header";
            // 
            // gbPreview
            // 
            this.gbPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPreview.Controls.Add(this.txtFilePreview2);
            this.gbPreview.Controls.Add(this.label9);
            this.gbPreview.Controls.Add(this.lblExample2);
            this.gbPreview.Controls.Add(this.txtFilePreview1);
            this.gbPreview.Controls.Add(this.label8);
            this.gbPreview.Controls.Add(this.lblExample1);
            this.gbPreview.Location = new System.Drawing.Point(3, 231);
            this.gbPreview.Name = "gbPreview";
            this.gbPreview.Size = new System.Drawing.Size(450, 114);
            this.gbPreview.TabIndex = 10;
            this.gbPreview.TabStop = false;
            this.gbPreview.Text = "Preview";
            // 
            // txtFilePreview2
            // 
            this.txtFilePreview2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilePreview2.Location = new System.Drawing.Point(104, 82);
            this.txtFilePreview2.Name = "txtFilePreview2";
            this.txtFilePreview2.ReadOnly = true;
            this.txtFilePreview2.Size = new System.Drawing.Size(340, 20);
            this.txtFilePreview2.TabIndex = 11;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 85);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "File Name Results";
            // 
            // lblExample2
            // 
            this.lblExample2.AutoSize = true;
            this.lblExample2.Location = new System.Drawing.Point(6, 66);
            this.lblExample2.Name = "lblExample2";
            this.lblExample2.Size = new System.Drawing.Size(402, 13);
            this.lblExample2.TabIndex = 12;
            this.lblExample2.Text = "Example 2 - Double Episode: Episode 23 and 24 of season 9 of the show \"Seinfeld\"";
            // 
            // txtFilePreview1
            // 
            this.txtFilePreview1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilePreview1.Location = new System.Drawing.Point(104, 38);
            this.txtFilePreview1.Name = "txtFilePreview1";
            this.txtFilePreview1.ReadOnly = true;
            this.txtFilePreview1.Size = new System.Drawing.Size(340, 20);
            this.txtFilePreview1.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "File Name Results";
            // 
            // lblExample1
            // 
            this.lblExample1.AutoSize = true;
            this.lblExample1.Location = new System.Drawing.Point(6, 22);
            this.lblExample1.Name = "lblExample1";
            this.lblExample1.Size = new System.Drawing.Size(419, 13);
            this.lblExample1.TabIndex = 9;
            this.lblExample1.Text = "Example 1 - Single Episode: Episode 5 of season 1 of the show \'Arrested Developme" +
    "nt\'.";
            // 
            // FileNameFormatControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbPreview);
            this.Controls.Add(this.gbEpFormat);
            this.Controls.Add(this.gbFormat);
            this.Name = "FileNameFormatControl";
            this.Size = new System.Drawing.Size(456, 349);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNameFormat)).EndInit();
            this.gbFormat.ResumeLayout(false);
            this.gbEpFormat.ResumeLayout(false);
            this.gbEpFormat.PerformLayout();
            this.gbPreview.ResumeLayout(false);
            this.gbPreview.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvNameFormat;
        private System.Windows.Forms.GroupBox gbFormat;
        private System.Windows.Forms.GroupBox gbEpFormat;
        private System.Windows.Forms.CheckBox chkHeaderPerEpisode;
        private System.Windows.Forms.CheckBox chkSeasonFirst;
        private System.Windows.Forms.TextBox txtEpisodeHeader;
        private System.Windows.Forms.TextBox txtSeasonHeader;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbPreview;
        private System.Windows.Forms.TextBox txtFilePreview2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblExample2;
        private System.Windows.Forms.TextBox txtFilePreview1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblExample1;
        private System.Windows.Forms.CheckBox chkSeasonDoubleDigits;
        private System.Windows.Forms.CheckBox chkEpisodeDoubleDigits;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHeader;
        private System.Windows.Forms.DataGridViewComboBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFooter;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWhitespace;
        private System.Windows.Forms.DataGridViewComboBoxColumn colCase;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
    }
}
