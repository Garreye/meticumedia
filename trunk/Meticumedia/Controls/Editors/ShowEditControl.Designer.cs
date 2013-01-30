// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia.Controls
{
    partial class ShowEditControl
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
            this.chkDoRenaming = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.chkInlcudeScan = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDescr = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numId = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numYear = new System.Windows.Forms.NumericUpDown();
            this.gbOnlineSearch = new System.Windows.Forms.GroupBox();
            this.cntrlSearch = new Meticumedia.SearchControl();
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.chkDoMissing = new System.Windows.Forms.CheckBox();
            this.btnDbSearch = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYear)).BeginInit();
            this.gbOnlineSearch.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkDoRenaming
            // 
            this.chkDoRenaming.AutoSize = true;
            this.chkDoRenaming.Location = new System.Drawing.Point(7, 197);
            this.chkDoRenaming.Name = "chkDoRenaming";
            this.chkDoRenaming.Size = new System.Drawing.Size(91, 17);
            this.chkDoRenaming.TabIndex = 26;
            this.chkDoRenaming.Text = "Do Renaming";
            this.chkDoRenaming.UseVisualStyleBackColor = true;
            this.chkDoRenaming.CheckedChanged += new System.EventHandler(this.chkDoRenaming_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(72, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(216, 20);
            this.txtName.TabIndex = 24;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // chkInlcudeScan
            // 
            this.chkInlcudeScan.AutoSize = true;
            this.chkInlcudeScan.Location = new System.Drawing.Point(7, 174);
            this.chkInlcudeScan.Name = "chkInlcudeScan";
            this.chkInlcudeScan.Size = new System.Drawing.Size(120, 17);
            this.chkInlcudeScan.TabIndex = 23;
            this.chkInlcudeScan.Text = "Include in Scanning";
            this.chkInlcudeScan.UseVisualStyleBackColor = true;
            this.chkInlcudeScan.CheckedChanged += new System.EventHandler(this.chkInlcudeScan_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Description";
            // 
            // txtDescr
            // 
            this.txtDescr.Location = new System.Drawing.Point(72, 48);
            this.txtDescr.Multiline = true;
            this.txtDescr.Name = "txtDescr";
            this.txtDescr.Size = new System.Drawing.Size(216, 66);
            this.txtDescr.TabIndex = 21;
            this.txtDescr.TextChanged += new System.EventHandler(this.txtDescr_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "ID";
            // 
            // numId
            // 
            this.numId.Enabled = false;
            this.numId.Location = new System.Drawing.Point(72, 146);
            this.numId.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numId.Name = "numId";
            this.numId.Size = new System.Drawing.Size(120, 20);
            this.numId.TabIndex = 28;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Year";
            // 
            // numYear
            // 
            this.numYear.Location = new System.Drawing.Point(72, 120);
            this.numYear.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numYear.Name = "numYear";
            this.numYear.Size = new System.Drawing.Size(120, 20);
            this.numYear.TabIndex = 31;
            this.numYear.ValueChanged += new System.EventHandler(this.numYear_ValueChanged);
            // 
            // gbOnlineSearch
            // 
            this.gbOnlineSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbOnlineSearch.Controls.Add(this.cntrlSearch);
            this.gbOnlineSearch.Location = new System.Drawing.Point(6, 3);
            this.gbOnlineSearch.Name = "gbOnlineSearch";
            this.gbOnlineSearch.Size = new System.Drawing.Size(442, 241);
            this.gbOnlineSearch.TabIndex = 34;
            this.gbOnlineSearch.TabStop = false;
            this.gbOnlineSearch.Text = "Online Database";
            // 
            // cntrlSearch
            // 
            this.cntrlSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlSearch.IsShow = true;
            this.cntrlSearch.Location = new System.Drawing.Point(6, 19);
            this.cntrlSearch.Name = "cntrlSearch";
            this.cntrlSearch.Size = new System.Drawing.Size(430, 216);
            this.cntrlSearch.TabIndex = 33;
            this.cntrlSearch.SearchResultsSelected += new System.EventHandler<Meticumedia.SearchControl.SearchResultsSelectedArgs>(this.cntrlSearch_SearchResultsSelected);
            // 
            // gbProperties
            // 
            this.gbProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProperties.Controls.Add(this.chkDoMissing);
            this.gbProperties.Controls.Add(this.btnDbSearch);
            this.gbProperties.Controls.Add(this.label7);
            this.gbProperties.Controls.Add(this.txtDescr);
            this.gbProperties.Controls.Add(this.label3);
            this.gbProperties.Controls.Add(this.label1);
            this.gbProperties.Controls.Add(this.numYear);
            this.gbProperties.Controls.Add(this.chkInlcudeScan);
            this.gbProperties.Controls.Add(this.numId);
            this.gbProperties.Controls.Add(this.txtName);
            this.gbProperties.Controls.Add(this.label2);
            this.gbProperties.Controls.Add(this.chkDoRenaming);
            this.gbProperties.Location = new System.Drawing.Point(6, 3);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(448, 241);
            this.gbProperties.TabIndex = 35;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "Show Properties";
            // 
            // chkDoMissing
            // 
            this.chkDoMissing.AutoSize = true;
            this.chkDoMissing.Location = new System.Drawing.Point(6, 218);
            this.chkDoMissing.Name = "chkDoMissing";
            this.chkDoMissing.Size = new System.Drawing.Size(112, 17);
            this.chkDoMissing.TabIndex = 35;
            this.chkDoMissing.Text = "Do Missing Check";
            this.chkDoMissing.UseVisualStyleBackColor = true;
            this.chkDoMissing.CheckedChanged += new System.EventHandler(this.chkDoMissing_CheckedChanged);
            // 
            // btnDbSearch
            // 
            this.btnDbSearch.Location = new System.Drawing.Point(198, 143);
            this.btnDbSearch.Name = "btnDbSearch";
            this.btnDbSearch.Size = new System.Drawing.Size(90, 23);
            this.btnDbSearch.TabIndex = 34;
            this.btnDbSearch.Text = "Change";
            this.btnDbSearch.UseVisualStyleBackColor = true;
            this.btnDbSearch.Click += new System.EventHandler(this.btnDbSearch_Click);
            // 
            // ShowEditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbProperties);
            this.Controls.Add(this.gbOnlineSearch);
            this.Name = "ShowEditControl";
            this.Size = new System.Drawing.Size(457, 252);
            ((System.ComponentModel.ISupportInitialize)(this.numId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYear)).EndInit();
            this.gbOnlineSearch.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.gbProperties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkDoRenaming;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.CheckBox chkInlcudeScan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDescr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numYear;
        private SearchControl cntrlSearch;
        private System.Windows.Forms.GroupBox gbOnlineSearch;
        private System.Windows.Forms.GroupBox gbProperties;
        private System.Windows.Forms.Button btnDbSearch;
        private System.Windows.Forms.CheckBox chkDoMissing;
    }
}
