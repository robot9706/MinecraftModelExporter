using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace MinecraftModelExporter
{
    public class AbortableBackgroundWorker : Component
    {
        private Thread workerThread;

        public event EventHandler DoWork;
        public event EventHandler RunWorkerCompleted;

        public ProgressChangedEventArgs CurrentProgress;

        public AbortableBackgroundWorker()
        {
            workerThread = new Thread(new ThreadStart(DoWork_Thread));
            workerThread.IsBackground = true;
        }

        private void DoWork_Thread()
        {
            if (DoWork != null)
                DoWork(this, null);

            if (RunWorkerCompleted != null)
                SafeEventCall.CallSafe(RunWorkerCompleted, this, null);
        }

        public void RunWorkerAsync()
        {
            workerThread.Start();
        }

        public void Abort()
        {
            if (workerThread != null)
            {
                workerThread.Abort();
                workerThread = null;
            }
        }

        public void ReportProgress(int percent, object userData)
        {
            CurrentProgress = new ProgressChangedEventArgs(percent, userData);
        }
    }
}
