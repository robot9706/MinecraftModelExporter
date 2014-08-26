using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MinecraftModelExporter
{
    public partial class TaskForm : Form
    {
        private bool _success = false;
        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        private Func<object, TaskProgressReport, bool> _taskFunction;
        private object _arg;

        private Exception _ex;

        private TaskProgressReport _report;

        public TaskForm(Func<object, TaskProgressReport, bool> task, object arg)
        {
            InitializeComponent();

            _taskFunction = task;
            _arg = arg;

            _report = new TaskProgressReport(worker);
            _report.Report(0, "");

            worker.RunWorkerAsync();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            worker.Abort();
            Close();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _success = _taskFunction(_arg, _report);
                if (!_success)
                {
                    _ex = new Exception(string.Empty);
                }
            }
            catch (Exception ex)
            {
                _ex = ex;
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            taskLabel.Text = (string)e.UserState;
            progressBar.Value = e.ProgressPercentage;
            pLbl.Text = e.ProgressPercentage.ToString() + "%";
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_ex != null)
            {
                if (string.IsNullOrEmpty(_ex.Message))
                    MessageBox.Show("Task failed because of exception: " + _ex.Message);
                else
                    MessageBox.Show("Task failed!");
            }
            Close();
        }
    }

    public class TaskProgressReport
    {
        private AbortableBackgroundWorker _src;

        public TaskProgressReport(AbortableBackgroundWorker src)
        {
            _src = src;
        }

        public void Report(int percent, string name)
        {
            if (percent < 0)
                percent = 0;
            if (percent > 100)
                percent = 100;
            _src.ReportProgress(percent, name);
        }
    }
}
