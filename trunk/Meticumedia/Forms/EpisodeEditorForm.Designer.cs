// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class EpisodeEditorForm
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
            Meticumedia.TvEpisode tvEpisode1 = new Meticumedia.TvEpisode();
            Meticumedia.TvFile tvFile1 = new Meticumedia.TvFile();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cntrlEp = new Meticumedia.EpisodeEditControl();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(179, 203);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(260, 203);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cntrlEp
            // 
            this.cntrlEp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            tvEpisode1.AirDate = new System.DateTime(((long)(0)));
            tvEpisode1.DataBaseName = "";
            tvEpisode1.File = tvFile1;
            tvEpisode1.Ignored = false;
            tvEpisode1.InDatabase = false;
            tvEpisode1.Missing = Meticumedia.TvEpisode.MissingStatus.Missing;
            tvEpisode1.Name = "";
            tvEpisode1.NameIsUserSet = false;
            tvEpisode1.Number = -1;
            tvEpisode1.Overview = "";
            tvEpisode1.PreventDatabaseUpdates = false;
            tvEpisode1.Season = -1;
            tvEpisode1.Show = "";
            tvEpisode1.UserDefined = false;
            tvEpisode1.Watched = false;
            this.cntrlEp.Episode = tvEpisode1;
            this.cntrlEp.Location = new System.Drawing.Point(12, 12);
            this.cntrlEp.Name = "cntrlEp";
            this.cntrlEp.Size = new System.Drawing.Size(323, 185);
            this.cntrlEp.TabIndex = 0;
            // 
            // EpisodeEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 238);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cntrlEp);
            this.Name = "EpisodeEditorForm";
            this.Text = "Episode Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private EpisodeEditControl cntrlEp;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}