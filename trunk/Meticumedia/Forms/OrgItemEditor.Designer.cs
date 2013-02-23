// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class OrgItemEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtSourceFile = new System.Windows.Forms.TextBox();
            this.gbTv = new System.Windows.Forms.GroupBox();
            this.btnNewShow = new System.Windows.Forms.Button();
            this.txtEpisodeName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numEpisode2 = new System.Windows.Forms.NumericUpDown();
            this.chkEpisode2 = new System.Windows.Forms.CheckBox();
            this.numEpisode1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numSeason = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbShows = new System.Windows.Forms.ComboBox();
            this.lblSHow = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbDelete = new System.Windows.Forms.RadioButton();
            this.rbRename = new System.Windows.Forms.RadioButton();
            this.rbCopy = new System.Windows.Forms.RadioButton();
            this.rbMove = new System.Windows.Forms.RadioButton();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.gbCategory = new System.Windows.Forms.GroupBox();
            this.rbCustom = new System.Windows.Forms.RadioButton();
            this.rbMovie = new System.Windows.Forms.RadioButton();
            this.rbTv = new System.Windows.Forms.RadioButton();
            this.gbMovie = new System.Windows.Forms.GroupBox();
            this.cntrlMovieEdit = new Meticumedia.ContentEditControl();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDestination = new System.Windows.Forms.Button();
            this.gbTv.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEpisode2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEpisode1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSeason)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.gbCategory.SuspendLayout();
            this.gbMovie.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source";
            // 
            // txtSourceFile
            // 
            this.txtSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceFile.Location = new System.Drawing.Point(60, 12);
            this.txtSourceFile.Name = "txtSourceFile";
            this.txtSourceFile.ReadOnly = true;
            this.txtSourceFile.Size = new System.Drawing.Size(282, 20);
            this.txtSourceFile.TabIndex = 1;
            // 
            // gbTv
            // 
            this.gbTv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTv.Controls.Add(this.btnNewShow);
            this.gbTv.Controls.Add(this.txtEpisodeName);
            this.gbTv.Controls.Add(this.label4);
            this.gbTv.Controls.Add(this.numEpisode2);
            this.gbTv.Controls.Add(this.chkEpisode2);
            this.gbTv.Controls.Add(this.numEpisode1);
            this.gbTv.Controls.Add(this.label3);
            this.gbTv.Controls.Add(this.numSeason);
            this.gbTv.Controls.Add(this.label2);
            this.gbTv.Controls.Add(this.cmbShows);
            this.gbTv.Controls.Add(this.lblSHow);
            this.gbTv.Location = new System.Drawing.Point(6, 146);
            this.gbTv.Name = "gbTv";
            this.gbTv.Size = new System.Drawing.Size(336, 155);
            this.gbTv.TabIndex = 3;
            this.gbTv.TabStop = false;
            this.gbTv.Text = "TV ";
            // 
            // btnNewShow
            // 
            this.btnNewShow.Location = new System.Drawing.Point(255, 17);
            this.btnNewShow.Name = "btnNewShow";
            this.btnNewShow.Size = new System.Drawing.Size(75, 23);
            this.btnNewShow.TabIndex = 10;
            this.btnNewShow.Text = "New..";
            this.btnNewShow.UseVisualStyleBackColor = true;
            this.btnNewShow.Click += new System.EventHandler(this.btnNewShow_Click);
            // 
            // txtEpisodeName
            // 
            this.txtEpisodeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEpisodeName.Location = new System.Drawing.Point(87, 124);
            this.txtEpisodeName.Name = "txtEpisodeName";
            this.txtEpisodeName.ReadOnly = true;
            this.txtEpisodeName.Size = new System.Drawing.Size(239, 20);
            this.txtEpisodeName.TabIndex = 9;
            this.txtEpisodeName.TextChanged += new System.EventHandler(this.tvFieldChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Episode Name";
            // 
            // numEpisode2
            // 
            this.numEpisode2.Location = new System.Drawing.Point(87, 98);
            this.numEpisode2.Name = "numEpisode2";
            this.numEpisode2.Size = new System.Drawing.Size(73, 20);
            this.numEpisode2.TabIndex = 7;
            this.numEpisode2.ValueChanged += new System.EventHandler(this.tvFieldChanged);
            // 
            // chkEpisode2
            // 
            this.chkEpisode2.AutoSize = true;
            this.chkEpisode2.Location = new System.Drawing.Point(8, 98);
            this.chkEpisode2.Name = "chkEpisode2";
            this.chkEpisode2.Size = new System.Drawing.Size(73, 17);
            this.chkEpisode2.TabIndex = 6;
            this.chkEpisode2.Text = "Episode 2";
            this.chkEpisode2.UseVisualStyleBackColor = true;
            this.chkEpisode2.CheckedChanged += new System.EventHandler(this.tvFieldChanged);
            // 
            // numEpisode1
            // 
            this.numEpisode1.Location = new System.Drawing.Point(87, 72);
            this.numEpisode1.Name = "numEpisode1";
            this.numEpisode1.Size = new System.Drawing.Size(73, 20);
            this.numEpisode1.TabIndex = 5;
            this.numEpisode1.ValueChanged += new System.EventHandler(this.tvFieldChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Episode";
            // 
            // numSeason
            // 
            this.numSeason.Location = new System.Drawing.Point(87, 46);
            this.numSeason.Name = "numSeason";
            this.numSeason.Size = new System.Drawing.Size(73, 20);
            this.numSeason.TabIndex = 3;
            this.numSeason.ValueChanged += new System.EventHandler(this.tvFieldChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Season";
            // 
            // cmbShows
            // 
            this.cmbShows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShows.FormattingEnabled = true;
            this.cmbShows.Location = new System.Drawing.Point(87, 19);
            this.cmbShows.Name = "cmbShows";
            this.cmbShows.Size = new System.Drawing.Size(162, 21);
            this.cmbShows.TabIndex = 1;
            this.cmbShows.SelectedIndexChanged += new System.EventHandler(this.tvFieldChanged);
            // 
            // lblSHow
            // 
            this.lblSHow.AutoSize = true;
            this.lblSHow.Location = new System.Drawing.Point(6, 22);
            this.lblSHow.Name = "lblSHow";
            this.lblSHow.Size = new System.Drawing.Size(34, 13);
            this.lblSHow.TabIndex = 0;
            this.lblSHow.Text = "Show";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.rbDelete);
            this.groupBox2.Controls.Add(this.rbRename);
            this.groupBox2.Controls.Add(this.rbCopy);
            this.groupBox2.Controls.Add(this.rbMove);
            this.groupBox2.Controls.Add(this.rbNone);
            this.groupBox2.Location = new System.Drawing.Point(6, 38);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(336, 48);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Action";
            // 
            // rbDelete
            // 
            this.rbDelete.AutoSize = true;
            this.rbDelete.Location = new System.Drawing.Point(250, 19);
            this.rbDelete.Name = "rbDelete";
            this.rbDelete.Size = new System.Drawing.Size(56, 17);
            this.rbDelete.TabIndex = 4;
            this.rbDelete.TabStop = true;
            this.rbDelete.Text = "Delete";
            this.rbDelete.UseVisualStyleBackColor = true;
            this.rbDelete.CheckedChanged += new System.EventHandler(this.actionCheckedChanged);
            // 
            // rbRename
            // 
            this.rbRename.AutoSize = true;
            this.rbRename.Location = new System.Drawing.Point(179, 19);
            this.rbRename.Name = "rbRename";
            this.rbRename.Size = new System.Drawing.Size(65, 17);
            this.rbRename.TabIndex = 3;
            this.rbRename.TabStop = true;
            this.rbRename.Text = "Rename";
            this.rbRename.UseVisualStyleBackColor = true;
            this.rbRename.CheckedChanged += new System.EventHandler(this.actionCheckedChanged);
            // 
            // rbCopy
            // 
            this.rbCopy.AutoSize = true;
            this.rbCopy.Location = new System.Drawing.Point(124, 19);
            this.rbCopy.Name = "rbCopy";
            this.rbCopy.Size = new System.Drawing.Size(49, 17);
            this.rbCopy.TabIndex = 2;
            this.rbCopy.TabStop = true;
            this.rbCopy.Text = "Copy";
            this.rbCopy.UseVisualStyleBackColor = true;
            this.rbCopy.CheckedChanged += new System.EventHandler(this.actionCheckedChanged);
            // 
            // rbMove
            // 
            this.rbMove.AutoSize = true;
            this.rbMove.Location = new System.Drawing.Point(66, 19);
            this.rbMove.Name = "rbMove";
            this.rbMove.Size = new System.Drawing.Size(52, 17);
            this.rbMove.TabIndex = 1;
            this.rbMove.TabStop = true;
            this.rbMove.Text = "Move";
            this.rbMove.UseVisualStyleBackColor = true;
            this.rbMove.CheckedChanged += new System.EventHandler(this.actionCheckedChanged);
            // 
            // rbNone
            // 
            this.rbNone.AutoSize = true;
            this.rbNone.Location = new System.Drawing.Point(9, 19);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(51, 17);
            this.rbNone.TabIndex = 0;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "None";
            this.rbNone.UseVisualStyleBackColor = true;
            this.rbNone.CheckedChanged += new System.EventHandler(this.actionCheckedChanged);
            // 
            // gbCategory
            // 
            this.gbCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbCategory.Controls.Add(this.rbCustom);
            this.gbCategory.Controls.Add(this.rbMovie);
            this.gbCategory.Controls.Add(this.rbTv);
            this.gbCategory.Location = new System.Drawing.Point(6, 92);
            this.gbCategory.Name = "gbCategory";
            this.gbCategory.Size = new System.Drawing.Size(336, 48);
            this.gbCategory.TabIndex = 5;
            this.gbCategory.TabStop = false;
            this.gbCategory.Text = "Category";
            // 
            // rbCustom
            // 
            this.rbCustom.AutoSize = true;
            this.rbCustom.Location = new System.Drawing.Point(114, 19);
            this.rbCustom.Name = "rbCustom";
            this.rbCustom.Size = new System.Drawing.Size(60, 17);
            this.rbCustom.TabIndex = 2;
            this.rbCustom.TabStop = true;
            this.rbCustom.Text = "Custom";
            this.rbCustom.UseVisualStyleBackColor = true;
            this.rbCustom.CheckedChanged += new System.EventHandler(this.categoryCheckedChanged);
            // 
            // rbMovie
            // 
            this.rbMovie.AutoSize = true;
            this.rbMovie.Location = new System.Drawing.Point(54, 19);
            this.rbMovie.Name = "rbMovie";
            this.rbMovie.Size = new System.Drawing.Size(54, 17);
            this.rbMovie.TabIndex = 1;
            this.rbMovie.TabStop = true;
            this.rbMovie.Text = "Movie";
            this.rbMovie.UseVisualStyleBackColor = true;
            this.rbMovie.CheckedChanged += new System.EventHandler(this.categoryCheckedChanged);
            // 
            // rbTv
            // 
            this.rbTv.AutoSize = true;
            this.rbTv.Location = new System.Drawing.Point(9, 19);
            this.rbTv.Name = "rbTv";
            this.rbTv.Size = new System.Drawing.Size(39, 17);
            this.rbTv.TabIndex = 0;
            this.rbTv.TabStop = true;
            this.rbTv.Text = "TV";
            this.rbTv.UseVisualStyleBackColor = true;
            this.rbTv.CheckedChanged += new System.EventHandler(this.categoryCheckedChanged);
            // 
            // gbMovie
            // 
            this.gbMovie.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMovie.Controls.Add(this.cntrlMovieEdit);
            this.gbMovie.Location = new System.Drawing.Point(6, 307);
            this.gbMovie.Name = "gbMovie";
            this.gbMovie.Size = new System.Drawing.Size(336, 210);
            this.gbMovie.TabIndex = 6;
            this.gbMovie.TabStop = false;
            this.gbMovie.Text = "Movie";
            // 
            // cntrlMovieEdit
            // 
            this.cntrlMovieEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlMovieEdit.Location = new System.Drawing.Point(6, 13);
            this.cntrlMovieEdit.Content = null;
            this.cntrlMovieEdit.ContentType = ContentType.Movie;
            this.cntrlMovieEdit.Name = "cntrlMovieEdit";
            this.cntrlMovieEdit.Size = new System.Drawing.Size(320, 191);
            this.cntrlMovieEdit.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(186, 552);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(267, 552);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtDestination
            // 
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.Location = new System.Drawing.Point(78, 526);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.ReadOnly = true;
            this.txtDestination.Size = new System.Drawing.Size(223, 20);
            this.txtDestination.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 529);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Destination";
            // 
            // btnDestination
            // 
            this.btnDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDestination.Location = new System.Drawing.Point(307, 523);
            this.btnDestination.Name = "btnDestination";
            this.btnDestination.Size = new System.Drawing.Size(35, 23);
            this.btnDestination.TabIndex = 11;
            this.btnDestination.Text = "...";
            this.btnDestination.UseVisualStyleBackColor = true;
            this.btnDestination.Click += new System.EventHandler(this.btnDestination_Click);
            // 
            // OrgItemEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 582);
            this.Controls.Add(this.btnDestination);
            this.Controls.Add(this.txtDestination);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.gbMovie);
            this.Controls.Add(this.gbCategory);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gbTv);
            this.Controls.Add(this.txtSourceFile);
            this.Controls.Add(this.label1);
            this.Name = "OrgItemEditor";
            this.Text = "Scan Item Modifier";
            this.gbTv.ResumeLayout(false);
            this.gbTv.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEpisode2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEpisode1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSeason)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbCategory.ResumeLayout(false);
            this.gbCategory.PerformLayout();
            this.gbMovie.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSourceFile;
        private System.Windows.Forms.GroupBox gbTv;
        private System.Windows.Forms.ComboBox cmbShows;
        private System.Windows.Forms.Label lblSHow;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbNone;
        private System.Windows.Forms.RadioButton rbDelete;
        private System.Windows.Forms.RadioButton rbRename;
        private System.Windows.Forms.RadioButton rbCopy;
        private System.Windows.Forms.RadioButton rbMove;
        private System.Windows.Forms.GroupBox gbCategory;
        private System.Windows.Forms.RadioButton rbMovie;
        private System.Windows.Forms.RadioButton rbTv;
        private System.Windows.Forms.NumericUpDown numEpisode1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numSeason;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtEpisodeName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numEpisode2;
        private System.Windows.Forms.CheckBox chkEpisode2;
        private System.Windows.Forms.GroupBox gbMovie;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rbCustom;
        private System.Windows.Forms.Button btnDestination;
        private ContentEditControl cntrlMovieEdit;
        private System.Windows.Forms.Button btnNewShow;
    }
}