using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EyeOfSauron
{
    public delegate int WorkMethod();
    public partial class ProcessForm : Form
    {
        public WorkMethod TheWorkMethod;
        public ProcessForm(WorkMethod work)
        {
            InitializeComponent();
            TheWorkMethod = work;
            // 进度条事件；
            backgroundWorkerForDownload.WorkerReportsProgress = true;
            backgroundWorkerForDownload.DoWork += DownloadWork;
            backgroundWorkerForDownload.ProgressChanged += AddProcessBarValue;
            backgroundWorkerForDownload.RunWorkerCompleted += Finished;
            // 异步运行加载，保证画面不假死；
            backgroundWorkerForDownload.RunWorkerAsync();
        }
        private void DownloadWork(object sender, EventArgs e)
        {
            var percentage = 0;
            while (percentage < 100)
            {
                percentage = TheWorkMethod();
                backgroundWorkerForDownload.ReportProgress(percentage);
            }
        }
        private void AddProcessBarValue(object sender, ProgressChangedEventArgs e)
        {
            // 更新进度条, 100 为满；
            progressBar1.Value = e.ProgressPercentage;
        }
        private void Finished(object sender, EventArgs e)
        {
            this.Close();  // 完成后关闭该窗口；
        }
    }
}
