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
    public partial class PartProgressTaskForm : Form
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

        private PartTaskProgressReport _report;

        public PartProgressTaskForm(Func<object, TaskProgressReport, bool> task, object arg)
        {
            InitializeComponent();

            _taskFunction = task;
            _arg = arg;

            _report = new PartTaskProgressReport(worker);
            _report.SetTitle("");
            _report.SetPartPercent(0);
            _report.SetTotalPercent(0);

            worker_ProgressChanged(null, new ProgressChangedEventArgs(0, _report));

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
            if (e.UserState is PartTaskProgressReport)
            {
                PartTaskProgressReport r = (PartTaskProgressReport)e.UserState;

                taskLabel.Text = r.Title;
                progress1.Value = r.Total;
                progress2.Value = r.Part;

                pLbl1.Text = r.Total.ToString() + "%";
                pLbl2.Text = r.Part.ToString() + "%";
            }
            else
            {
                taskLabel.Text = (string)e.UserState;
                progress1.Value = e.ProgressPercentage;
            }
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

    public class PartTaskProgressReport : TaskProgressReport
    {
        private AbortableBackgroundWorker _src;

        public PartTaskProgressReport(AbortableBackgroundWorker src)
            : base(src)
        {
            _src = src;
        }

        private string _title;
        public string Title
        {
            get { return _title; }
        }

        private int _totalPercent;
        public int Total
        {
            get { return _totalPercent; }
        }

        private int _partPercent;
        public int Part
        {
            get { return _partPercent; }
        }

        public void SetTitle(string title)
        {
            _title = title;
        }

        public void SetTotalPercent(int percent)
        {
            if (percent < 0)
                _totalPercent = 0;
            else if (percent > 100)
                _totalPercent = 100;
            else
                _totalPercent = percent;
        }

        public void SetPartPercent(int percent)
        {
            if (percent < 0)
                _partPercent = 0;
            else if (percent > 100)
                _partPercent = 100;
            else
                _partPercent = percent;
        }

        public void Report()
        {
            _src.ReportProgress(0, this);
        }
    }
}
