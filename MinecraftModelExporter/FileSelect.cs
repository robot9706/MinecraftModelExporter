using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MinecraftModelExporter
{
    [DefaultEvent("OnFileSelected")]
    public partial class FileSelect : UserControl
    {
        private string _fileFilter = string.Empty;
        public string Filter
        {
            get 
            {
                return _opf.Filter; 
            }
            set
            {
                _opf.Filter = value;
            }
        }

        public string FilePath
        {
            get
            {
                return _opf.FileName; 
            }
            set
            {
                _opf.FileName = value;
                pathTb.Text = value;
            }
        }

        private OpenFileDialog _opf;

        public delegate void OnFileSelectedHandler(string newFile);
        public event OnFileSelectedHandler OnFileSelected;

        public FileSelect()
        {
            InitializeComponent();

            _opf = new OpenFileDialog();
        }

        private void selectFile_Click(object sender, EventArgs e)
        {
            if (_opf.ShowDialog() == DialogResult.OK)
            {
                pathTb.Text = _opf.FileName;

                if (OnFileSelected != null)
                    OnFileSelected(_opf.FileName);
            }
        }
    }
}
