// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class FileTypesControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileTypesControl));
            this.ftcVideo = new Meticumedia.FileTypeControl();
            this.ftcDelete = new Meticumedia.FileTypeControl();
            this.SuspendLayout();
            // 
            // ftcVideo
            // 
            this.ftcVideo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ftcVideo.FileTypes = ((System.Collections.Generic.List<string>)(resources.GetObject("ftcVideo.FileTypes")));
            this.ftcVideo.Location = new System.Drawing.Point(3, 3);
            this.ftcVideo.Name = "ftcVideo";
            this.ftcVideo.Size = new System.Drawing.Size(216, 431);
            this.ftcVideo.TabIndex = 0;
            this.ftcVideo.Title = "Video File Types";
            // 
            // ftcDelete
            // 
            this.ftcDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ftcDelete.FileTypes = ((System.Collections.Generic.List<string>)(resources.GetObject("ftcDelete.FileTypes")));
            this.ftcDelete.Location = new System.Drawing.Point(225, 6);
            this.ftcDelete.Name = "ftcDelete";
            this.ftcDelete.Size = new System.Drawing.Size(216, 431);
            this.ftcDelete.TabIndex = 1;
            this.ftcDelete.Title = "Delete File Types";
            // 
            // FileTypesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ftcDelete);
            this.Controls.Add(this.ftcVideo);
            this.Name = "FileTypesControl";
            this.Size = new System.Drawing.Size(491, 437);
            this.ResumeLayout(false);

        }

        #endregion

        private FileTypeControl ftcVideo;
        private FileTypeControl ftcDelete;

    }
}
