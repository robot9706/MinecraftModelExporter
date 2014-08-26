namespace MinecraftModelExporter
{
    partial class FileSelect
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
            this.pathTb = new System.Windows.Forms.TextBox();
            this.selectFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pathTb
            // 
            this.pathTb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathTb.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pathTb.Location = new System.Drawing.Point(2, 2);
            this.pathTb.Name = "pathTb";
            this.pathTb.ReadOnly = true;
            this.pathTb.Size = new System.Drawing.Size(131, 20);
            this.pathTb.TabIndex = 0;
            // 
            // selectFile
            // 
            this.selectFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectFile.Location = new System.Drawing.Point(138, 1);
            this.selectFile.Name = "selectFile";
            this.selectFile.Size = new System.Drawing.Size(75, 23);
            this.selectFile.TabIndex = 1;
            this.selectFile.Text = "Browse";
            this.selectFile.UseVisualStyleBackColor = true;
            this.selectFile.Click += new System.EventHandler(this.selectFile_Click);
            // 
            // FileSelect
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.selectFile);
            this.Controls.Add(this.pathTb);
            this.Name = "FileSelect";
            this.Size = new System.Drawing.Size(214, 24);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pathTb;
        private System.Windows.Forms.Button selectFile;
    }
}
