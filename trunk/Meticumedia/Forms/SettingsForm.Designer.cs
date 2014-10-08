// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class SettingsForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tcSetting = new System.Windows.Forms.TabControl();
            this.tpScanFolders = new System.Windows.Forms.TabPage();
            this.cntrlScanFolders = new Meticumedia.ScanFolderControl();
            this.tpTvFolders = new System.Windows.Forms.TabPage();
            this.cntrlTvFolders = new Meticumedia.ContentFoldersControl(ContentType.TvShow);
            this.tpTvFile = new System.Windows.Forms.TabPage();
            this.cntrlTvFileNameFormat = new Meticumedia.FileNameFormatControl();
            this.tpMovieFolders = new System.Windows.Forms.TabPage();
            this.cntrlMovieFolders = new Meticumedia.ContentFoldersControl(ContentType.Movie);
            this.tpMovieFileName = new System.Windows.Forms.TabPage();
            this.cntrlMovieFileNameFormat = new Meticumedia.FileNameFormatControl();
            this.tpFileExt = new System.Windows.Forms.TabPage();
            this.cntrlFileTypes = new Meticumedia.FileTypesControl();
            this.tcSetting.SuspendLayout();
            this.tpScanFolders.SuspendLayout();
            this.tpTvFolders.SuspendLayout();
            this.tpTvFile.SuspendLayout();
            this.tpMovieFolders.SuspendLayout();
            this.tpMovieFileName.SuspendLayout();
            this.tpFileExt.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(625, 468);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(540, 468);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // tcSetting
            // 
            this.tcSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcSetting.Controls.Add(this.tpScanFolders);
            this.tcSetting.Controls.Add(this.tpTvFolders);
            this.tcSetting.Controls.Add(this.tpTvFile);
            this.tcSetting.Controls.Add(this.tpMovieFolders);
            this.tcSetting.Controls.Add(this.tpMovieFileName);
            this.tcSetting.Controls.Add(this.tpFileExt);
            this.tcSetting.Location = new System.Drawing.Point(12, 12);
            this.tcSetting.Name = "tcSetting";
            this.tcSetting.SelectedIndex = 0;
            this.tcSetting.Size = new System.Drawing.Size(688, 450);
            this.tcSetting.TabIndex = 2;
            // 
            // tpScanFolders
            // 
            this.tpScanFolders.Controls.Add(this.cntrlScanFolders);
            this.tpScanFolders.Location = new System.Drawing.Point(4, 22);
            this.tpScanFolders.Name = "tpScanFolders";
            this.tpScanFolders.Padding = new System.Windows.Forms.Padding(3);
            this.tpScanFolders.Size = new System.Drawing.Size(680, 424);
            this.tpScanFolders.TabIndex = 0;
            this.tpScanFolders.Text = "Scan Folders";
            this.tpScanFolders.UseVisualStyleBackColor = true;
            // 
            // cntrlScanFolders
            // 
            this.cntrlScanFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlScanFolders.Location = new System.Drawing.Point(3, 6);
            this.cntrlScanFolders.Name = "cntrlScanFolders";
            this.cntrlScanFolders.Size = new System.Drawing.Size(671, 405);
            this.cntrlScanFolders.TabIndex = 0;
            // 
            // tpTvFolders
            // 
            this.tpTvFolders.Controls.Add(this.cntrlTvFolders);
            this.tpTvFolders.Location = new System.Drawing.Point(4, 22);
            this.tpTvFolders.Name = "tpTvFolders";
            this.tpTvFolders.Size = new System.Drawing.Size(680, 424);
            this.tpTvFolders.TabIndex = 3;
            this.tpTvFolders.Text = "TV Root Folders";
            this.tpTvFolders.UseVisualStyleBackColor = true;
            // 
            // cntrlTvFolders
            // 
            this.cntrlTvFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlTvFolders.Location = new System.Drawing.Point(3, 3);
            this.cntrlTvFolders.Name = "cntrlTvFolders";
            this.cntrlTvFolders.Size = new System.Drawing.Size(674, 418);
            this.cntrlTvFolders.TabIndex = 0;
            // 
            // tpTvFile
            // 
            this.tpTvFile.Controls.Add(this.cntrlTvFileNameFormat);
            this.tpTvFile.Location = new System.Drawing.Point(4, 22);
            this.tpTvFile.Name = "tpTvFile";
            this.tpTvFile.Padding = new System.Windows.Forms.Padding(3);
            this.tpTvFile.Size = new System.Drawing.Size(680, 424);
            this.tpTvFile.TabIndex = 1;
            this.tpTvFile.Text = "TV File Name";
            this.tpTvFile.UseVisualStyleBackColor = true;
            // 
            // cntrlTvFileNameFormat
            // 
            this.cntrlTvFileNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlTvFileNameFormat.Location = new System.Drawing.Point(6, 6);
            this.cntrlTvFileNameFormat.Name = "cntrlTvFileNameFormat";
            this.cntrlTvFileNameFormat.Size = new System.Drawing.Size(668, 412);
            this.cntrlTvFileNameFormat.TabIndex = 0;
            // 
            // tpMovieFolders
            // 
            this.tpMovieFolders.Controls.Add(this.cntrlMovieFolders);
            this.tpMovieFolders.Location = new System.Drawing.Point(4, 22);
            this.tpMovieFolders.Name = "tpMovieFolders";
            this.tpMovieFolders.Padding = new System.Windows.Forms.Padding(3);
            this.tpMovieFolders.Size = new System.Drawing.Size(680, 424);
            this.tpMovieFolders.TabIndex = 2;
            this.tpMovieFolders.Text = "Movie Root Folders";
            this.tpMovieFolders.UseVisualStyleBackColor = true;
            // 
            // cntrlMovieFolders
            // 
            this.cntrlMovieFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlMovieFolders.Location = new System.Drawing.Point(6, 6);
            this.cntrlMovieFolders.Name = "cntrlMovieFolders";
            this.cntrlMovieFolders.Size = new System.Drawing.Size(668, 412);
            this.cntrlMovieFolders.TabIndex = 0;
            // 
            // tpMovieFileName
            // 
            this.tpMovieFileName.Controls.Add(this.cntrlMovieFileNameFormat);
            this.tpMovieFileName.Location = new System.Drawing.Point(4, 22);
            this.tpMovieFileName.Name = "tpMovieFileName";
            this.tpMovieFileName.Padding = new System.Windows.Forms.Padding(3);
            this.tpMovieFileName.Size = new System.Drawing.Size(680, 424);
            this.tpMovieFileName.TabIndex = 5;
            this.tpMovieFileName.Text = "Movie File Name";
            this.tpMovieFileName.UseVisualStyleBackColor = true;
            // 
            // cntrlMovieFileNameFormat
            // 
            this.cntrlMovieFileNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlMovieFileNameFormat.Location = new System.Drawing.Point(6, 6);
            this.cntrlMovieFileNameFormat.Name = "cntrlMovieFileNameFormat";
            this.cntrlMovieFileNameFormat.Size = new System.Drawing.Size(671, 412);
            this.cntrlMovieFileNameFormat.TabIndex = 0;
            // 
            // tpFileExt
            // 
            this.tpFileExt.Controls.Add(this.cntrlFileTypes);
            this.tpFileExt.Location = new System.Drawing.Point(4, 22);
            this.tpFileExt.Name = "tpFileExt";
            this.tpFileExt.Padding = new System.Windows.Forms.Padding(3);
            this.tpFileExt.Size = new System.Drawing.Size(680, 424);
            this.tpFileExt.TabIndex = 4;
            this.tpFileExt.Text = "File Extensions";
            this.tpFileExt.UseVisualStyleBackColor = true;
            // 
            // cntrlFileTypes
            // 
            this.cntrlFileTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cntrlFileTypes.Location = new System.Drawing.Point(6, 6);
            this.cntrlFileTypes.Name = "cntrlFileTypes";
            this.cntrlFileTypes.Size = new System.Drawing.Size(668, 415);
            this.cntrlFileTypes.TabIndex = 0;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 503);
            this.Controls.Add(this.tcSetting);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.tcSetting.ResumeLayout(false);
            this.tpScanFolders.ResumeLayout(false);
            this.tpTvFolders.ResumeLayout(false);
            this.tpTvFile.ResumeLayout(false);
            this.tpMovieFolders.ResumeLayout(false);
            this.tpMovieFileName.ResumeLayout(false);
            this.tpFileExt.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private ContentFoldersControl cntrlMovieFolders;
        private System.Windows.Forms.TabControl tcSetting;
        private System.Windows.Forms.TabPage tpScanFolders;
        private System.Windows.Forms.TabPage tpTvFile;
        private System.Windows.Forms.TabPage tpMovieFolders;
        private ScanFolderControl cntrlScanFolders;
        private System.Windows.Forms.TabPage tpTvFolders;
        private ContentFoldersControl cntrlTvFolders;
        private System.Windows.Forms.TabPage tpFileExt;
        private FileTypesControl cntrlFileTypes;
        private FileNameFormatControl cntrlTvFileNameFormat;
        private System.Windows.Forms.TabPage tpMovieFileName;
        private FileNameFormatControl cntrlMovieFileNameFormat;
    }
}