namespace MinecraftModelExporter
{
    partial class TaskForm
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
            this.components = new System.ComponentModel.Container();
            this.taskLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.cancelButton = new System.Windows.Forms.Button();
            this.worker = new MinecraftModelExporter.AbortableBackgroundWorker();
            this.pLbl = new System.Windows.Forms.Label();
            this.uiUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // taskLabel
            // 
            this.taskLabel.Location = new System.Drawing.Point(13, 9);
            this.taskLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.taskLabel.Name = "taskLabel";
            this.taskLabel.Size = new System.Drawing.Size(537, 27);
            this.taskLabel.TabIndex = 0;
            this.taskLabel.Text = "Task";
            this.taskLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(16, 39);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(486, 23);
            this.progressBar.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(234, 68);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(92, 33);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancel_Click);
            // 
            // worker
            // 
            this.worker.DoWork += new System.EventHandler(this.worker_DoWork);
            this.worker.RunWorkerCompleted += new System.EventHandler(this.worker_RunWorkerCompleted);
            // 
            // pLbl
            // 
            this.pLbl.Location = new System.Drawing.Point(508, 39);
            this.pLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.pLbl.Name = "pLbl";
            this.pLbl.Size = new System.Drawing.Size(42, 23);
            this.pLbl.TabIndex = 5;
            this.pLbl.Text = "100%";
            this.pLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiUpdate
            // 
            this.uiUpdate.Tick += new System.EventHandler(this.uiUpdate_Tick);
            // 
            // TaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 112);
            this.Controls.Add(this.pLbl);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.taskLabel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TaskForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TaskForm";
            this.Shown += new System.EventHandler(this.TaskForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label taskLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button cancelButton;
        private AbortableBackgroundWorker worker;
        private System.Windows.Forms.Label pLbl;
        private System.Windows.Forms.Timer uiUpdate;
    }
}