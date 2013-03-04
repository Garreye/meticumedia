// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class ContentEditControl
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
            this.label4 = new System.Windows.Forms.Label();
            this.btnRemoveGenre = new System.Windows.Forms.Button();
            this.btnAddGenre = new System.Windows.Forms.Button();
            this.numId = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbGenres = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numYear = new System.Windows.Forms.NumericUpDown();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDbSearch = new System.Windows.Forms.Button();
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.gbAltMatchNames = new System.Windows.Forms.GroupBox();
            this.lbAltNames = new System.Windows.Forms.ListBox();
            this.btnRemoveMatch = new System.Windows.Forms.Button();
            this.btnAddMatch = new System.Windows.Forms.Button();
            this.chkIncludeInSchedule = new System.Windows.Forms.CheckBox();
            this.chkDoMissing = new System.Windows.Forms.CheckBox();
            this.chkDoRenaming = new System.Windows.Forms.CheckBox();
            this.gbOnlineSearch = new System.Windows.Forms.GroupBox();
            this.cntrlSearch = new Meticumedia.SearchControl();
            ((System.ComponentModel.ISupportInitialize)(this.numId)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYear)).BeginInit();
            this.gbProperties.SuspendLayout();
            this.gbAltMatchNames.SuspendLayout();
            this.gbOnlineSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = " ID";
            // 
            // btnRemoveGenre
            // 
            this.btnRemoveGenre.Location = new System.Drawing.Point(6, 48);
            this.btnRemoveGenre.Name = "btnRemoveGenre";
            this.btnRemoveGenre.Size = new System.Drawing.Size(35, 23);
            this.btnRemoveGenre.TabIndex = 2;
            this.btnRemoveGenre.Text = "-";
            this.btnRemoveGenre.UseVisualStyleBackColor = true;
            this.btnRemoveGenre.Click += new System.EventHandler(this.btnRemoveGenre_Click);
            // 
            // btnAddGenre
            // 
            this.btnAddGenre.Location = new System.Drawing.Point(6, 19);
            this.btnAddGenre.Name = "btnAddGenre";
            this.btnAddGenre.Size = new System.Drawing.Size(35, 23);
            this.btnAddGenre.TabIndex = 1;
            this.btnAddGenre.Text = "+";
            this.btnAddGenre.UseVisualStyleBackColor = true;
            this.btnAddGenre.Click += new System.EventHandler(this.btnAddGenre_Click);
            // 
            // numId
            // 
            this.numId.Enabled = false;
            this.numId.Location = new System.Drawing.Point(47, 70);
            this.numId.Maximum = new decimal(new int[] {
            -1530494976,
            232830,
            0,
            0});
            this.numId.Name = "numId";
            this.numId.Size = new System.Drawing.Size(90, 20);
            this.numId.TabIndex = 25;
            this.numId.ValueChanged += new System.EventHandler(this.numId_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lbGenres);
            this.groupBox2.Controls.Add(this.btnRemoveGenre);
            this.groupBox2.Controls.Add(this.btnAddGenre);
            this.groupBox2.Location = new System.Drawing.Point(10, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(214, 109);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Genres";
            // 
            // lbGenres
            // 
            this.lbGenres.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbGenres.FormattingEnabled = true;
            this.lbGenres.Location = new System.Drawing.Point(47, 19);
            this.lbGenres.Name = "lbGenres";
            this.lbGenres.Size = new System.Drawing.Size(161, 82);
            this.lbGenres.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Year";
            // 
            // numYear
            // 
            this.numYear.Location = new System.Drawing.Point(47, 46);
            this.numYear.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numYear.Name = "numYear";
            this.numYear.Size = new System.Drawing.Size(90, 20);
            this.numYear.TabIndex = 22;
            this.numYear.ValueChanged += new System.EventHandler(this.numYear_ValueChanged);
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(47, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(366, 20);
            this.txtName.TabIndex = 21;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Name";
            // 
            // btnDbSearch
            // 
            this.btnDbSearch.Location = new System.Drawing.Point(143, 67);
            this.btnDbSearch.Name = "btnDbSearch";
            this.btnDbSearch.Size = new System.Drawing.Size(81, 23);
            this.btnDbSearch.TabIndex = 35;
            this.btnDbSearch.Text = "Change";
            this.btnDbSearch.UseVisualStyleBackColor = true;
            this.btnDbSearch.Click += new System.EventHandler(this.btnDbSearch_Click);
            // 
            // gbProperties
            // 
            this.gbProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProperties.Controls.Add(this.gbAltMatchNames);
            this.gbProperties.Controls.Add(this.chkIncludeInSchedule);
            this.gbProperties.Controls.Add(this.chkDoMissing);
            this.gbProperties.Controls.Add(this.chkDoRenaming);
            this.gbProperties.Controls.Add(this.label2);
            this.gbProperties.Controls.Add(this.btnDbSearch);
            this.gbProperties.Controls.Add(this.txtName);
            this.gbProperties.Controls.Add(this.label4);
            this.gbProperties.Controls.Add(this.numYear);
            this.gbProperties.Controls.Add(this.numId);
            this.gbProperties.Controls.Add(this.label3);
            this.gbProperties.Controls.Add(this.groupBox2);
            this.gbProperties.Location = new System.Drawing.Point(3, 3);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(425, 211);
            this.gbProperties.TabIndex = 36;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "Properties";
            // 
            // gbAltMatchNames
            // 
            this.gbAltMatchNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAltMatchNames.Controls.Add(this.lbAltNames);
            this.gbAltMatchNames.Controls.Add(this.btnRemoveMatch);
            this.gbAltMatchNames.Controls.Add(this.btnAddMatch);
            this.gbAltMatchNames.Location = new System.Drawing.Point(244, 119);
            this.gbAltMatchNames.Name = "gbAltMatchNames";
            this.gbAltMatchNames.Size = new System.Drawing.Size(175, 86);
            this.gbAltMatchNames.TabIndex = 40;
            this.gbAltMatchNames.TabStop = false;
            this.gbAltMatchNames.Text = "Alternative Match Names";
            this.gbAltMatchNames.Visible = false;
            // 
            // lbAltNames
            // 
            this.lbAltNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbAltNames.FormattingEnabled = true;
            this.lbAltNames.Location = new System.Drawing.Point(47, 19);
            this.lbAltNames.Name = "lbAltNames";
            this.lbAltNames.Size = new System.Drawing.Size(122, 56);
            this.lbAltNames.TabIndex = 0;
            // 
            // btnRemoveMatch
            // 
            this.btnRemoveMatch.Location = new System.Drawing.Point(6, 48);
            this.btnRemoveMatch.Name = "btnRemoveMatch";
            this.btnRemoveMatch.Size = new System.Drawing.Size(35, 23);
            this.btnRemoveMatch.TabIndex = 2;
            this.btnRemoveMatch.Text = "-";
            this.btnRemoveMatch.UseVisualStyleBackColor = true;
            this.btnRemoveMatch.Click += new System.EventHandler(this.btnRemoveMatch_Click);
            // 
            // btnAddMatch
            // 
            this.btnAddMatch.Location = new System.Drawing.Point(6, 19);
            this.btnAddMatch.Name = "btnAddMatch";
            this.btnAddMatch.Size = new System.Drawing.Size(35, 23);
            this.btnAddMatch.TabIndex = 1;
            this.btnAddMatch.Text = "+";
            this.btnAddMatch.UseVisualStyleBackColor = true;
            this.btnAddMatch.Click += new System.EventHandler(this.btnAddMatch_Click);
            // 
            // chkIncludeInSchedule
            // 
            this.chkIncludeInSchedule.AutoSize = true;
            this.chkIncludeInSchedule.Location = new System.Drawing.Point(244, 96);
            this.chkIncludeInSchedule.Name = "chkIncludeInSchedule";
            this.chkIncludeInSchedule.Size = new System.Drawing.Size(120, 17);
            this.chkIncludeInSchedule.TabIndex = 39;
            this.chkIncludeInSchedule.Text = "Include in Schedule";
            this.chkIncludeInSchedule.UseVisualStyleBackColor = true;
            this.chkIncludeInSchedule.Visible = false;
            this.chkIncludeInSchedule.CheckedChanged += new System.EventHandler(this.chkIncludeInSchedule_CheckedChanged);
            // 
            // chkDoMissing
            // 
            this.chkDoMissing.AutoSize = true;
            this.chkDoMissing.Location = new System.Drawing.Point(244, 73);
            this.chkDoMissing.Name = "chkDoMissing";
            this.chkDoMissing.Size = new System.Drawing.Size(112, 17);
            this.chkDoMissing.TabIndex = 38;
            this.chkDoMissing.Text = "Do Missing Check";
            this.chkDoMissing.UseVisualStyleBackColor = true;
            this.chkDoMissing.Visible = false;
            this.chkDoMissing.CheckedChanged += new System.EventHandler(this.chkDoMissing_CheckedChanged);
            // 
            // chkDoRenaming
            // 
            this.chkDoRenaming.AutoSize = true;
            this.chkDoRenaming.Location = new System.Drawing.Point(244, 49);
            this.chkDoRenaming.Name = "chkDoRenaming";
            this.chkDoRenaming.Size = new System.Drawing.Size(91, 17);
            this.chkDoRenaming.TabIndex = 37;
            this.chkDoRenaming.Text = "Do Renaming";
            this.chkDoRenaming.UseVisualStyleBackColor = true;
            this.chkDoRenaming.CheckedChanged += new System.EventHandler(this.chkDoRenaming_CheckedChanged);
            // 
            // gbOnlineSearch
            // 
            this.gbOnlineSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbOnlineSearch.Controls.Add(this.cntrlSearch);
            this.gbOnlineSearch.Location = new System.Drawing.Point(3, 3);
            this.gbOnlineSearch.Name = "gbOnlineSearch";
            this.gbOnlineSearch.Size = new System.Drawing.Size(425, 211);
            this.gbOnlineSearch.TabIndex = 37;
            this.gbOnlineSearch.TabStop = false;
            this.gbOnlineSearch.Text = "Online Database";
            // 
            // cntrlSearch
            // 
            this.cntrlSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlSearch.ContentType = Meticumedia.ContentType.TvShow;
            this.cntrlSearch.Location = new System.Drawing.Point(6, 19);
            this.cntrlSearch.Name = "cntrlSearch";
            this.cntrlSearch.Size = new System.Drawing.Size(413, 186);
            this.cntrlSearch.TabIndex = 33;
            this.cntrlSearch.SearchResultsSelected += new System.EventHandler<Meticumedia.SearchControl.SearchResultsSelectedArgs>(this.cntrlSearch_SearchResultsSelected);
            // 
            // ContentEditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbProperties);
            this.Controls.Add(this.gbOnlineSearch);
            this.Name = "ContentEditControl";
            this.Size = new System.Drawing.Size(431, 217);
            ((System.ComponentModel.ISupportInitialize)(this.numId)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numYear)).EndInit();
            this.gbProperties.ResumeLayout(false);
            this.gbProperties.PerformLayout();
            this.gbAltMatchNames.ResumeLayout(false);
            this.gbOnlineSearch.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRemoveGenre;
        private System.Windows.Forms.Button btnAddGenre;
        private System.Windows.Forms.NumericUpDown numId;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lbGenres;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numYear;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDbSearch;
        private System.Windows.Forms.GroupBox gbProperties;
        private System.Windows.Forms.GroupBox gbOnlineSearch;
        private SearchControl cntrlSearch;
        private System.Windows.Forms.CheckBox chkDoMissing;
        private System.Windows.Forms.CheckBox chkDoRenaming;
        private System.Windows.Forms.CheckBox chkIncludeInSchedule;
        private System.Windows.Forms.GroupBox gbAltMatchNames;
        private System.Windows.Forms.ListBox lbAltNames;
        private System.Windows.Forms.Button btnRemoveMatch;
        private System.Windows.Forms.Button btnAddMatch;
    }
}
