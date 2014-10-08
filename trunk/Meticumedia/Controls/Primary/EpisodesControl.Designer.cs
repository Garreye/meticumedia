// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class EpisodesControl
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
            this.gbEpisodes = new System.Windows.Forms.GroupBox();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
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
            this.label3 = new System.Windows.Forms.Label();
            this.cmbEpFilter = new System.Windows.Forms.ComboBox();
            this.gbEpisodes.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbEpisodes
            // 
            this.gbEpisodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEpisodes.Controls.Add(this.btnMoveDown);
            this.gbEpisodes.Controls.Add(this.btnMoveUp);
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
            this.gbEpisodes.Size = new System.Drawing.Size(610, 296);
            this.gbEpisodes.TabIndex = 15;
            this.gbEpisodes.TabStop = false;
            this.gbEpisodes.Text = "Episodes";
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDown.Enabled = false;
            this.btnMoveDown.Location = new System.Drawing.Point(579, 18);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(25, 23);
            this.btnMoveDown.TabIndex = 26;
            this.btnMoveDown.Text = "\\/";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUp.Enabled = false;
            this.btnMoveUp.Location = new System.Drawing.Point(548, 18);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(25, 23);
            this.btnMoveUp.TabIndex = 25;
            this.btnMoveUp.Text = "/\\";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // lbScanDir
            // 
            this.lbScanDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbScanDir.AutoSize = true;
            this.lbScanDir.Color = System.Drawing.SystemColors.Control;
            this.lbScanDir.Label = "In Scan Dir.";
            this.lbScanDir.Location = new System.Drawing.Point(91, 262);
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
            this.lbIgnored.Location = new System.Drawing.Point(278, 262);
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
            this.lbUnaired.Location = new System.Drawing.Point(194, 262);
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
            this.lbMissing.Location = new System.Drawing.Point(9, 262);
            this.lbMissing.Margin = new System.Windows.Forms.Padding(0);
            this.lbMissing.Name = "lbMissing";
            this.lbMissing.Size = new System.Drawing.Size(82, 30);
            this.lbMissing.TabIndex = 20;
            // 
            // btnUnignore
            // 
            this.btnUnignore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnignore.Location = new System.Drawing.Point(448, 265);
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
            this.btnIgnore.Location = new System.Drawing.Point(529, 265);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(75, 23);
            this.btnIgnore.TabIndex = 13;
            this.btnIgnore.Text = "Ignore";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // chkDisplayIgnored
            // 
            this.chkDisplayIgnored.AutoSize = true;
            this.chkDisplayIgnored.Location = new System.Drawing.Point(273, 22);
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
            this.lvEpisodes.Size = new System.Drawing.Size(595, 213);
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
            // EpisodesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbEpisodes);
            this.Name = "EpisodesControl";
            this.Size = new System.Drawing.Size(616, 302);
            this.gbEpisodes.ResumeLayout(false);
            this.gbEpisodes.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbEpisodes;
        private LegendBoxControl lbScanDir;
        private LegendBoxControl lbIgnored;
        private LegendBoxControl lbUnaired;
        private LegendBoxControl lbMissing;
        private System.Windows.Forms.Button btnUnignore;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.CheckBox chkDisplayIgnored;
        private System.Windows.Forms.ListView lvEpisodes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbEpFilter;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
    }
}
