namespace MinecraftModelExporter
{
    partial class PartProgressTaskForm
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
            this.progress1 = new System.Windows.Forms.ProgressBar();
            this.cancelButton = new System.Windows.Forms.Button();
            this.worker = new MinecraftModelExporter.AbortableBackgroundWorker();
            this.progress2 = new System.Windows.Forms.ProgressBar();
            this.pLbl1 = new System.Windows.Forms.Label();
            this.pLbl2 = new System.Windows.Forms.Label();
            this.uiTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // taskLabel
            // 
            this.taskLabel.Location = new System.Drawing.Point(13, 9);
            this.taskLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.taskLabel.Name = "taskLabel";
            this.taskLabel.Size = new System.Drawing.Size(526, 27);
            this.taskLabel.TabIndex = 0;
            this.taskLabel.Text = "Task";
            this.taskLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progress1
            // 
            this.progress1.Location = new System.Drawing.Point(16, 39);
            this.progress1.Name = "progress1";
            this.progress1.Size = new System.Drawing.Size(486, 23);
            this.progress1.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(229, 95);
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
            // progress2
            // 
            this.progress2.Location = new System.Drawing.Point(16, 66);
            this.progress2.Name = "progress2";
            this.progress2.Size = new System.Drawing.Size(486, 23);
            this.progress2.TabIndex = 3;
            // 
            // pLbl1
            // 
            this.pLbl1.Location = new System.Drawing.Point(509, 39);
            this.pLbl1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.pLbl1.Name = "pLbl1";
            this.pLbl1.Size = new System.Drawing.Size(42, 23);
            this.pLbl1.TabIndex = 4;
            this.pLbl1.Text = "100%";
            this.pLbl1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pLbl2
            // 
            this.pLbl2.Location = new System.Drawing.Point(509, 66);
            this.pLbl2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.pLbl2.Name = "pLbl2";
            this.pLbl2.Size = new System.Drawing.Size(42, 23);
            this.pLbl2.TabIndex = 5;
            this.pLbl2.Text = "100%";
            this.pLbl2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTimer
            // 
            this.uiTimer.Tick += new System.EventHandler(this.uiTimer_Tick);
            // 
            // PartProgressTaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 138);
            this.Controls.Add(this.pLbl2);
            this.Controls.Add(this.pLbl1);
            this.Controls.Add(this.progress2);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.progress1);
            this.Controls.Add(this.taskLabel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PartProgressTaskForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TaskForm";
            this.Shown += new System.EventHandler(this.PartProgressTaskForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label taskLabel;
        private System.Windows.Forms.ProgressBar progress1;
        private System.Windows.Forms.Button cancelButton;
        private AbortableBackgroundWorker worker;
        private System.Windows.Forms.ProgressBar progress2;
        private System.Windows.Forms.Label pLbl1;
        private System.Windows.Forms.Label pLbl2;
        private System.Windows.Forms.Timer uiTimer;
    }
}