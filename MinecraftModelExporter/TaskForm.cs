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
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            worker.Abort();
            Close();
        }

        private void worker_DoWork(object sender, EventArgs e)
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

        private void worker_RunWorkerCompleted(object sender, EventArgs e)
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

        private void TaskForm_Shown(object sender, EventArgs e)
        {
            taskLabel.Text = "Starting..";
            uiUpdate.Start();
            worker.RunWorkerAsync();
        }

        private void uiUpdate_Tick(object sender, EventArgs a)
        {
            ProgressChangedEventArgs e = worker.CurrentProgress;

            taskLabel.Text = (string)e.UserState;
            progressBar.Value = e.ProgressPercentage;
            pLbl.Text = e.ProgressPercentage.ToString() + "%";
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
