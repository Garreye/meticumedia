namespace Meticumedia.Controls.Primary
{
    partial class NewContentControl
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
            this.chkTvEpisodes = new System.Windows.Forms.CheckBox();
            this.chkMovies = new System.Windows.Forms.CheckBox();
            this.lvContent = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // chkTvEpisodes
            // 
            this.chkTvEpisodes.AutoSize = true;
            this.chkTvEpisodes.Checked = true;
            this.chkTvEpisodes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTvEpisodes.Location = new System.Drawing.Point(3, 3);
            this.chkTvEpisodes.Name = "chkTvEpisodes";
            this.chkTvEpisodes.Size = new System.Drawing.Size(86, 17);
            this.chkTvEpisodes.TabIndex = 1;
            this.chkTvEpisodes.Text = "TV Episodes";
            this.chkTvEpisodes.UseVisualStyleBackColor = true;
            // 
            // chkMovies
            // 
            this.chkMovies.AutoSize = true;
            this.chkMovies.Checked = true;
            this.chkMovies.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMovies.Location = new System.Drawing.Point(95, 3);
            this.chkMovies.Name = "chkMovies";
            this.chkMovies.Size = new System.Drawing.Size(60, 17);
            this.chkMovies.TabIndex = 2;
            this.chkMovies.Text = "Movies";
            this.chkMovies.UseVisualStyleBackColor = true;
            // 
            // lvContent
            // 
            this.lvContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvContent.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader1,
            this.columnHeader2});
            this.lvContent.Location = new System.Drawing.Point(3, 26);
            this.lvContent.Name = "lvContent";
            this.lvContent.Size = new System.Drawing.Size(536, 425);
            this.lvContent.TabIndex = 3;
            this.lvContent.UseCompatibleStateImageBehavior = false;
            this.lvContent.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Added";
            this.columnHeader3.Width = 129;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Filename";
            this.columnHeader1.Width = 178;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Content";
            this.columnHeader2.Width = 252;
            // 
            // NewContentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvContent);
            this.Controls.Add(this.chkMovies);
            this.Controls.Add(this.chkTvEpisodes);
            this.Name = "NewContentControl";
            this.Size = new System.Drawing.Size(542, 454);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkTvEpisodes;
        private System.Windows.Forms.CheckBox chkMovies;
        private System.Windows.Forms.ListView lvContent;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}
