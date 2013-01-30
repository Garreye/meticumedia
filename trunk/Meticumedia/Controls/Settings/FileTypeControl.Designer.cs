// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
namespace Meticumedia
{
    partial class FileTypeControl
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
            this.gbTypes = new System.Windows.Forms.GroupBox();
            this.btnRemType = new System.Windows.Forms.Button();
            this.txtNewType = new System.Windows.Forms.TextBox();
            this.btnAddType = new System.Windows.Forms.Button();
            this.lvTypes = new System.Windows.Forms.ListView();
            this.gbTypes.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbTypes
            // 
            this.gbTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbTypes.Controls.Add(this.btnRemType);
            this.gbTypes.Controls.Add(this.txtNewType);
            this.gbTypes.Controls.Add(this.btnAddType);
            this.gbTypes.Controls.Add(this.lvTypes);
            this.gbTypes.Location = new System.Drawing.Point(3, 3);
            this.gbTypes.Name = "gbTypes";
            this.gbTypes.Size = new System.Drawing.Size(210, 325);
            this.gbTypes.TabIndex = 1;
            this.gbTypes.TabStop = false;
            this.gbTypes.Text = "Files Types";
            // 
            // btnRemType
            // 
            this.btnRemType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemType.Location = new System.Drawing.Point(174, 19);
            this.btnRemType.Name = "btnRemType";
            this.btnRemType.Size = new System.Drawing.Size(30, 23);
            this.btnRemType.TabIndex = 3;
            this.btnRemType.Text = "-";
            this.btnRemType.UseVisualStyleBackColor = true;
            this.btnRemType.Click += new System.EventHandler(this.btnRemType_Click);
            // 
            // txtNewType
            // 
            this.txtNewType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewType.Location = new System.Drawing.Point(6, 19);
            this.txtNewType.Name = "txtNewType";
            this.txtNewType.Size = new System.Drawing.Size(126, 20);
            this.txtNewType.TabIndex = 2;
            // 
            // btnAddType
            // 
            this.btnAddType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddType.Location = new System.Drawing.Point(138, 19);
            this.btnAddType.Name = "btnAddType";
            this.btnAddType.Size = new System.Drawing.Size(30, 23);
            this.btnAddType.TabIndex = 1;
            this.btnAddType.Text = "+";
            this.btnAddType.UseVisualStyleBackColor = true;
            this.btnAddType.Click += new System.EventHandler(this.btnAddType_Click);
            // 
            // lvTypes
            // 
            this.lvTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTypes.Location = new System.Drawing.Point(6, 48);
            this.lvTypes.Name = "lvTypes";
            this.lvTypes.Size = new System.Drawing.Size(198, 271);
            this.lvTypes.TabIndex = 0;
            this.lvTypes.UseCompatibleStateImageBehavior = false;
            // 
            // FileTypeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbTypes);
            this.Name = "FileTypeControl";
            this.Size = new System.Drawing.Size(216, 331);
            this.gbTypes.ResumeLayout(false);
            this.gbTypes.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbTypes;
        private System.Windows.Forms.Button btnRemType;
        private System.Windows.Forms.TextBox txtNewType;
        private System.Windows.Forms.Button btnAddType;
        private System.Windows.Forms.ListView lvTypes;
    }
}
