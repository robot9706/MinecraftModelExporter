using MinecraftModelExporter.Builtins;
using MinecraftModelExporter.Data;
using MinecraftModelExporter.GeometryProcessor;
using MinecraftModelExporter.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MinecraftModelExporter
{
    struct ImportFileSturct
    {
        public string File;
        public FileReader Reader;
    }

    public partial class MainForm : Form
    {
        private PluginManager _plugins;

        private OpenFileDialog _openFile;
        private SaveFileDialog _saveFile;

        private OpenFileDialog _cfgLoad;
        private SaveFileDialog _cfgSave;

        private ImportedData _sourceData;
        private FileWriter _writer;

        private bool _resourcePackOk = false;

        public MainForm()
        {
            Block.Init();

            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mainTip.SetToolTip(optimizeModel, "If checked big areas will be connected.");
            mainTip.SetToolTip(noOuterFaces, "If checked block faces on the edge of the model will be excluded.");
            mainTip.SetToolTip(saveCfg, "Saves every option, selected files too.");

            _plugins = new PluginManager(Application.StartupPath);
            _plugins.AddBuiltinFileReader(new MCEditSchematicReader());
            _plugins.AddBuiltinFileWriter(new OBJFileWriter());

            _cfgSave = new SaveFileDialog();
            _cfgSave.Filter = "Config file|*.cfg";

            _cfgLoad = new OpenFileDialog();
            _cfgLoad.Filter = _cfgSave.Filter;

            //Open file
            {
                string srcAllFiles = "All supported formats|";
                string srcFles = "";
                foreach (LoadedFileReader lfr in _plugins.Readers)
                {
                    string files = "";
                    foreach (string ext in lfr.Info.SupportedFiles)
                    {
                        files += "*." + ext + ";";
                    }
                    srcAllFiles += files;
                    files = files.Substring(0, files.Length - 1);

                    srcFles += lfr.Info.Name + "|" + files + "|";
                }

                _openFile = new OpenFileDialog();
                if (string.IsNullOrEmpty(srcFles))
                {
                    _openFile.Filter = "All files|*.*";
                }
                else
                {
                    srcAllFiles = srcAllFiles.Substring(0, srcAllFiles.Length - 1);
                    srcFles = srcFles.Substring(0, srcFles.Length - 1);

                    _openFile.Filter = srcAllFiles + "|" + srcFles + "|All files|*.*";
                }
            }

            //Save file
            {
                string trgFles = "";
                foreach (LoadedFileWriter lfw in _plugins.Writers)
                {
                    trgFles += lfw.Info.Name + "|*." + lfw.Info.OutputFormat + "|";
                }
                trgFles = trgFles.Substring(0, trgFles.Length - 1);

                _saveFile = new SaveFileDialog();
                if (string.IsNullOrEmpty(trgFles))
                {
                    _saveFile.Filter = "All files|*.*";
                }
                else
                {
                    _saveFile.Filter = trgFles;
                }
            }
        }

        private void srcOpenButton_Click(object sender, EventArgs e)
        {
            if (_openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetInputFile(_openFile.FileName);
            }
        }

        private void SetInputFile(string file)
        {
            dimsLbl.Text = "";

            string fileExt = Path.GetExtension(file);
            fileExt = fileExt.Substring(1, fileExt.Length - 1).ToLower();

            FileReader reader = null;
            foreach (LoadedFileReader fr in _plugins.Readers)
            {
                foreach (string ext in fr.Info.SupportedFiles)
                {
                    if (ext.ToLower() == fileExt)
                    {
                        reader = fr.Reader;
                        break;
                    }
                }
            }

            if (reader == null)
            {
                MessageBox.Show("Unsupported input format.");
            }
            else
            {
                ImportFileSturct data = new ImportFileSturct();
                data.File = file;
                data.Reader = reader;

                TaskForm task = new TaskForm(ImportDataTask, data);
                task.ShowDialog(this);
                if (task.Success)
                    srcFileTextBox.Text = file;

                dimsLbl.Text = String.Format((string)dimsLbl.Tag, _sourceData.ModelSize.X.ToString() + "x" + _sourceData.ModelSize.Y.ToString() + "x" + _sourceData.ModelSize.Z.ToString());
            }

            processBox.Enabled = (Path.IsPathRooted(srcFileTextBox.Text) && Path.IsPathRooted(srcFileTextBox.Text));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _plugins.CloseLogger();
        }

        private bool ImportDataTask(object data, TaskProgressReport report)
        {
            try
            {
                report.Report(0, "Loading file");
                ImportFileSturct info = (ImportFileSturct)data;
                _sourceData = info.Reader.ReadFile(info.File, report);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void selectTrgBtn_Click(object sender, EventArgs e)
        {
            if (_saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetOutFile(_saveFile.FileName);
            }

            processBox.Enabled = (Path.IsPathRooted(srcFileTextBox.Text) && Path.IsPathRooted(srcFileTextBox.Text));
        }

        public void SetOutFile(string file)
        {
            string ext = Path.GetExtension(file);
            if (ext.Length > 1)
                ext = ext.Substring(1, ext.Length - 1).ToLower();

            FileWriter writer = null;

            foreach (LoadedFileWriter lfw in _plugins.Writers)
            {
                if (ext == lfw.Info.OutputFormat.ToLower())
                {
                    writer = lfw.Writer;
                    break;
                }
            }

            if (writer == null)
            {
                trgFileTextBox.Text = string.Empty;
                MessageBox.Show("Unsupported output format.");
            }
            else
            {
                trgFileTextBox.Text = file;

                _writer = writer;
            }

            processBox.Enabled = (Path.IsPathRooted(srcFileTextBox.Text) && Path.IsPathRooted(srcFileTextBox.Text));
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (exportTextures.Checked && (!string.IsNullOrEmpty(fileSelect1.FilePath) && !File.Exists(fileSelect1.FilePath) && !_resourcePackOk))
            {
                MessageBox.Show("Invalid resource pack!");
                return;
            }

            ExportConfig conf = new ExportConfig();
            conf.OptimizeModel = optimizeModel.Checked;
            conf.ExportMaterials = exportMaterials.Checked;
            conf.CenterObject = centerObject.Checked;
            conf.DontExportOuterFaces = noOuterFaces.Checked;
            conf.InteriorOnly = interiorOnly.Checked;

            conf.ExportTextures = exportTextures.Checked;
            conf.TextureOutputFolder = texOutFolder.Text;
            conf.ResourcePack = fileSelect1.FilePath;

            conf.ExportNormals = exportNormals.Checked;
            conf.ExportUVs = exportUV.Checked;

            ExportTask task = new ExportTask(_sourceData, _writer, _saveFile.FileName, conf);

            PartProgressTaskForm taskForm = new PartProgressTaskForm(task.Export, null);
            taskForm.ShowDialog();

            if (taskForm.Success)
            {
                MessageBox.Show("File saved: " + _saveFile.FileName + "\nVertices: " + task.ExportedVertices.ToString() + ", Triangles: " + task.ExportedTriangles.ToString());
            }
        }

        private void exportTextures_CheckedChanged(object sender, EventArgs e)
        {
            texDataPanel.Enabled = exportTextures.Checked;
        }

        private void fileSelect1_OnFileSelected(string newFile)
        {
            if (!string.IsNullOrEmpty(newFile))
            {
                if (!Path.IsPathRooted(newFile))
                {
                    MessageBox.Show("Invalid path!");
                    return;
                }
                if (!File.Exists(newFile))
                {
                    MessageBox.Show("File not found!");
                    return;
                }

                ResourcePack rs = new ResourcePack(newFile);
                _resourcePackOk = rs.Check();
                if (!_resourcePackOk)
                {
                    fileSelect1.FilePath = string.Empty;
                    MessageBox.Show("Invalid resource pack!");
                }
            }
        }

        private void saveCfg_Click(object sender, EventArgs e)
        {
            if (_cfgSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    using (FileStream fs = File.OpenWrite(_cfgSave.FileName))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8))
                        {
                            bw.Write(_openFile.FileName);
                            bw.Write(_saveFile.FileName);

                            bw.Write(optimizeModel.Checked);
                            bw.Write(exportMaterials.Checked);
                            bw.Write(centerObject.Checked);
                            bw.Write(noOuterFaces.Checked);
                            bw.Write(interiorOnly.Checked);

                            bw.Write(exportNormals.Checked);
                            bw.Write(exportUV.Checked);

                            bw.Write(exportTextures.Checked);
                            if (exportTextures.Checked)
                            {
                                bw.Write(texOutFolder.Text);
                                bw.Write(fileSelect1.FilePath);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save config file: " + ex.Message);
                }
            }
        }

        private void loadCfg_Click(object sender, EventArgs e)
        {
            if (_cfgLoad.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    using (FileStream fs = File.OpenRead(_cfgLoad.FileName))
                    {
                        using (BinaryReader br = new BinaryReader(fs, Encoding.UTF8))
                        {
                            string infile = br.ReadString();
                            string outfile = br.ReadString();

                            optimizeModel.Checked = br.ReadBoolean();
                            exportMaterials.Checked = br.ReadBoolean();
                            centerObject.Checked = br.ReadBoolean();
                            noOuterFaces.Checked = br.ReadBoolean();
                            interiorOnly.Checked = br.ReadBoolean();

                            exportNormals.Checked = br.ReadBoolean();
                            exportUV.Checked = br.ReadBoolean();

                            exportTextures.Checked = br.ReadBoolean();
                            if (exportTextures.Checked)
                            {
                                texOutFolder.Text = br.ReadString();
                                fileSelect1.FilePath = br.ReadString();
                            }

                            if (!string.IsNullOrEmpty(infile))
                            {
                                _openFile.FileName = infile;
                                SetInputFile(infile);
                            }
                            if (!string.IsNullOrEmpty(outfile))
                            {
                                _saveFile.FileName = outfile;
                                SetOutFile(outfile);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid or corrupt config file: " + ex.Message);
                }
            }
        }

        private void defaultTexResButton_Click(object sender, EventArgs e)
        {
            fileSelect1.FilePath = string.Empty;
        }
    }
}
