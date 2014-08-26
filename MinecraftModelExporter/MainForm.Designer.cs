namespace MinecraftModelExporter
{
    partial class MainForm
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
            this.srcOpenButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.srcFileTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dimsLbl = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.selectTrgBtn = new System.Windows.Forms.Button();
            this.trgFileTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.processBox = new System.Windows.Forms.GroupBox();
            this.exportUV = new System.Windows.Forms.CheckBox();
            this.exportNormals = new System.Windows.Forms.CheckBox();
            this.exportTextures = new System.Windows.Forms.CheckBox();
            this.texDataPanel = new System.Windows.Forms.Panel();
            this.defaultTexResButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.texOutFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.interiorOnly = new System.Windows.Forms.CheckBox();
            this.noOuterFaces = new System.Windows.Forms.CheckBox();
            this.centerObject = new System.Windows.Forms.CheckBox();
            this.exportMaterials = new System.Windows.Forms.CheckBox();
            this.optimizeModel = new System.Windows.Forms.CheckBox();
            this.exportButton = new System.Windows.Forms.Button();
            this.mainTip = new System.Windows.Forms.ToolTip(this.components);
            this.loadCfg = new System.Windows.Forms.Button();
            this.saveCfg = new System.Windows.Forms.Button();
            this.cp = new System.Windows.Forms.Label();
            this.fileSelect1 = new MinecraftModelExporter.FileSelect();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.processBox.SuspendLayout();
            this.texDataPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // srcOpenButton
            // 
            this.srcOpenButton.Location = new System.Drawing.Point(6, 21);
            this.srcOpenButton.Name = "srcOpenButton";
            this.srcOpenButton.Size = new System.Drawing.Size(127, 32);
            this.srcOpenButton.TabIndex = 0;
            this.srcOpenButton.Text = "Open source";
            this.srcOpenButton.UseVisualStyleBackColor = true;
            this.srcOpenButton.Click += new System.EventHandler(this.srcOpenButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "File path:";
            // 
            // srcFileTextBox
            // 
            this.srcFileTextBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.srcFileTextBox.Location = new System.Drawing.Point(71, 55);
            this.srcFileTextBox.Name = "srcFileTextBox";
            this.srcFileTextBox.Size = new System.Drawing.Size(311, 22);
            this.srcFileTextBox.TabIndex = 2;
            this.srcFileTextBox.Text = "No file opened";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dimsLbl);
            this.groupBox1.Controls.Add(this.srcOpenButton);
            this.groupBox1.Controls.Add(this.srcFileTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(388, 105);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source file";
            // 
            // dimsLbl
            // 
            this.dimsLbl.AutoSize = true;
            this.dimsLbl.Location = new System.Drawing.Point(8, 80);
            this.dimsLbl.Name = "dimsLbl";
            this.dimsLbl.Size = new System.Drawing.Size(109, 16);
            this.dimsLbl.TabIndex = 3;
            this.dimsLbl.Tag = "Dimensions: {0}";
            this.dimsLbl.Text = "Dimensions: -x-x-";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.selectTrgBtn);
            this.groupBox2.Controls.Add(this.trgFileTextBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(406, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(388, 105);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target file";
            // 
            // selectTrgBtn
            // 
            this.selectTrgBtn.Location = new System.Drawing.Point(6, 21);
            this.selectTrgBtn.Name = "selectTrgBtn";
            this.selectTrgBtn.Size = new System.Drawing.Size(127, 32);
            this.selectTrgBtn.TabIndex = 0;
            this.selectTrgBtn.Text = "Select file";
            this.selectTrgBtn.UseVisualStyleBackColor = true;
            this.selectTrgBtn.Click += new System.EventHandler(this.selectTrgBtn_Click);
            // 
            // trgFileTextBox
            // 
            this.trgFileTextBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.trgFileTextBox.Location = new System.Drawing.Point(71, 55);
            this.trgFileTextBox.Name = "trgFileTextBox";
            this.trgFileTextBox.Size = new System.Drawing.Size(311, 22);
            this.trgFileTextBox.TabIndex = 2;
            this.trgFileTextBox.Text = "No file selected";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "File path:";
            // 
            // processBox
            // 
            this.processBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.processBox.Controls.Add(this.exportUV);
            this.processBox.Controls.Add(this.exportNormals);
            this.processBox.Controls.Add(this.exportTextures);
            this.processBox.Controls.Add(this.texDataPanel);
            this.processBox.Controls.Add(this.interiorOnly);
            this.processBox.Controls.Add(this.noOuterFaces);
            this.processBox.Controls.Add(this.centerObject);
            this.processBox.Controls.Add(this.exportMaterials);
            this.processBox.Controls.Add(this.optimizeModel);
            this.processBox.Controls.Add(this.exportButton);
            this.processBox.Enabled = false;
            this.processBox.Location = new System.Drawing.Point(12, 123);
            this.processBox.Name = "processBox";
            this.processBox.Size = new System.Drawing.Size(672, 271);
            this.processBox.TabIndex = 5;
            this.processBox.TabStop = false;
            this.processBox.Text = "Processor settings";
            // 
            // exportUV
            // 
            this.exportUV.AutoSize = true;
            this.exportUV.Checked = true;
            this.exportUV.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportUV.Location = new System.Drawing.Point(264, 48);
            this.exportUV.Name = "exportUV";
            this.exportUV.Size = new System.Drawing.Size(94, 20);
            this.exportUV.TabIndex = 9;
            this.exportUV.Text = "Export UVs";
            this.exportUV.UseVisualStyleBackColor = true;
            // 
            // exportNormals
            // 
            this.exportNormals.AutoSize = true;
            this.exportNormals.Checked = true;
            this.exportNormals.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportNormals.Location = new System.Drawing.Point(264, 22);
            this.exportNormals.Name = "exportNormals";
            this.exportNormals.Size = new System.Drawing.Size(116, 20);
            this.exportNormals.TabIndex = 8;
            this.exportNormals.Text = "Export normals";
            this.exportNormals.UseVisualStyleBackColor = true;
            // 
            // exportTextures
            // 
            this.exportTextures.AutoSize = true;
            this.exportTextures.Location = new System.Drawing.Point(11, 152);
            this.exportTextures.Name = "exportTextures";
            this.exportTextures.Size = new System.Drawing.Size(114, 20);
            this.exportTextures.TabIndex = 6;
            this.exportTextures.Text = "Export textures";
            this.exportTextures.UseVisualStyleBackColor = true;
            this.exportTextures.CheckedChanged += new System.EventHandler(this.exportTextures_CheckedChanged);
            // 
            // texDataPanel
            // 
            this.texDataPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.texDataPanel.Controls.Add(this.defaultTexResButton);
            this.texDataPanel.Controls.Add(this.label4);
            this.texDataPanel.Controls.Add(this.fileSelect1);
            this.texDataPanel.Controls.Add(this.texOutFolder);
            this.texDataPanel.Controls.Add(this.label2);
            this.texDataPanel.Enabled = false;
            this.texDataPanel.Location = new System.Drawing.Point(11, 178);
            this.texDataPanel.Name = "texDataPanel";
            this.texDataPanel.Size = new System.Drawing.Size(248, 84);
            this.texDataPanel.TabIndex = 7;
            // 
            // defaultTexResButton
            // 
            this.defaultTexResButton.Location = new System.Drawing.Point(168, 30);
            this.defaultTexResButton.Name = "defaultTexResButton";
            this.defaultTexResButton.Size = new System.Drawing.Size(75, 23);
            this.defaultTexResButton.TabIndex = 4;
            this.defaultTexResButton.Text = "Default";
            this.defaultTexResButton.UseVisualStyleBackColor = true;
            this.defaultTexResButton.Click += new System.EventHandler(this.defaultTexResButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Resource pack:";
            // 
            // texOutFolder
            // 
            this.texOutFolder.Location = new System.Drawing.Point(90, 2);
            this.texOutFolder.Name = "texOutFolder";
            this.texOutFolder.Size = new System.Drawing.Size(153, 22);
            this.texOutFolder.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Output folder:";
            // 
            // interiorOnly
            // 
            this.interiorOnly.AutoSize = true;
            this.interiorOnly.Location = new System.Drawing.Point(11, 126);
            this.interiorOnly.Name = "interiorOnly";
            this.interiorOnly.Size = new System.Drawing.Size(136, 20);
            this.interiorOnly.TabIndex = 5;
            this.interiorOnly.Text = "Export interior only";
            this.interiorOnly.UseVisualStyleBackColor = true;
            // 
            // noOuterFaces
            // 
            this.noOuterFaces.AutoSize = true;
            this.noOuterFaces.Location = new System.Drawing.Point(11, 100);
            this.noOuterFaces.Name = "noOuterFaces";
            this.noOuterFaces.Size = new System.Drawing.Size(146, 20);
            this.noOuterFaces.TabIndex = 4;
            this.noOuterFaces.Text = "Exclude edge faces";
            this.noOuterFaces.UseVisualStyleBackColor = true;
            // 
            // centerObject
            // 
            this.centerObject.AutoSize = true;
            this.centerObject.Checked = true;
            this.centerObject.CheckState = System.Windows.Forms.CheckState.Checked;
            this.centerObject.Location = new System.Drawing.Point(11, 74);
            this.centerObject.Name = "centerObject";
            this.centerObject.Size = new System.Drawing.Size(107, 20);
            this.centerObject.TabIndex = 3;
            this.centerObject.Text = "Center model";
            this.centerObject.UseVisualStyleBackColor = true;
            // 
            // exportMaterials
            // 
            this.exportMaterials.AutoSize = true;
            this.exportMaterials.Checked = true;
            this.exportMaterials.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportMaterials.Location = new System.Drawing.Point(11, 48);
            this.exportMaterials.Name = "exportMaterials";
            this.exportMaterials.Size = new System.Drawing.Size(208, 20);
            this.exportMaterials.TabIndex = 2;
            this.exportMaterials.Text = "Export materials (MTL for OBJ)";
            this.exportMaterials.UseVisualStyleBackColor = true;
            // 
            // optimizeModel
            // 
            this.optimizeModel.AutoSize = true;
            this.optimizeModel.Checked = true;
            this.optimizeModel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optimizeModel.Location = new System.Drawing.Point(11, 22);
            this.optimizeModel.Name = "optimizeModel";
            this.optimizeModel.Size = new System.Drawing.Size(120, 20);
            this.optimizeModel.TabIndex = 1;
            this.optimizeModel.Text = "Optimize model";
            this.optimizeModel.UseVisualStyleBackColor = true;
            // 
            // exportButton
            // 
            this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exportButton.Location = new System.Drawing.Point(523, 231);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(146, 37);
            this.exportButton.TabIndex = 0;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // loadCfg
            // 
            this.loadCfg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loadCfg.Location = new System.Drawing.Point(690, 129);
            this.loadCfg.Name = "loadCfg";
            this.loadCfg.Size = new System.Drawing.Size(104, 32);
            this.loadCfg.TabIndex = 8;
            this.loadCfg.Text = "Load config";
            this.loadCfg.UseVisualStyleBackColor = true;
            this.loadCfg.Click += new System.EventHandler(this.loadCfg_Click);
            // 
            // saveCfg
            // 
            this.saveCfg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveCfg.Location = new System.Drawing.Point(690, 167);
            this.saveCfg.Name = "saveCfg";
            this.saveCfg.Size = new System.Drawing.Size(104, 32);
            this.saveCfg.TabIndex = 9;
            this.saveCfg.Text = "Save config";
            this.saveCfg.UseVisualStyleBackColor = true;
            this.saveCfg.Click += new System.EventHandler(this.saveCfg_Click);
            // 
            // cp
            // 
            this.cp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cp.AutoSize = true;
            this.cp.ForeColor = System.Drawing.Color.Silver;
            this.cp.Location = new System.Drawing.Point(730, 384);
            this.cp.Name = "cp";
            this.cp.Size = new System.Drawing.Size(73, 16);
            this.cp.TabIndex = 10;
            this.cp.Text = "Robot9706";
            // 
            // fileSelect1
            // 
            this.fileSelect1.FilePath = "";
            this.fileSelect1.Filter = "Zip files|*.zip";
            this.fileSelect1.Location = new System.Drawing.Point(8, 53);
            this.fileSelect1.Margin = new System.Windows.Forms.Padding(4);
            this.fileSelect1.Name = "fileSelect1";
            this.fileSelect1.Size = new System.Drawing.Size(236, 26);
            this.fileSelect1.TabIndex = 2;
            this.fileSelect1.OnFileSelected += new MinecraftModelExporter.FileSelect.OnFileSelectedHandler(this.fileSelect1_OnFileSelected);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(803, 401);
            this.Controls.Add(this.cp);
            this.Controls.Add(this.saveCfg);
            this.Controls.Add(this.processBox);
            this.Controls.Add(this.loadCfg);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Minecraft world model exporter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.processBox.ResumeLayout(false);
            this.processBox.PerformLayout();
            this.texDataPanel.ResumeLayout(false);
            this.texDataPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button srcOpenButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox srcFileTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label dimsLbl;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button selectTrgBtn;
        private System.Windows.Forms.TextBox trgFileTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox processBox;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.CheckBox optimizeModel;
        private System.Windows.Forms.CheckBox exportMaterials;
        private System.Windows.Forms.CheckBox centerObject;
        private System.Windows.Forms.CheckBox noOuterFaces;
        private System.Windows.Forms.ToolTip mainTip;
        private System.Windows.Forms.CheckBox interiorOnly;
        private System.Windows.Forms.CheckBox exportTextures;
        private System.Windows.Forms.Panel texDataPanel;
        private System.Windows.Forms.TextBox texOutFolder;
        private System.Windows.Forms.Label label2;
        private FileSelect fileSelect1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button saveCfg;
        private System.Windows.Forms.Button loadCfg;
        private System.Windows.Forms.Button defaultTexResButton;
        private System.Windows.Forms.CheckBox exportUV;
        private System.Windows.Forms.CheckBox exportNormals;
        private System.Windows.Forms.Label cp;
    }
}

