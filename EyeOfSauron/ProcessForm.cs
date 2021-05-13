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
    public partial class ProcessForm : Form
    {
        Manager TheManager;
        public ProcessForm(Manager theManager)
        {
            TheManager = theManager;
            InitializeComponent();
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
            for (int i = 0; i < Parameter.PreLoadQuantity; i++)
            {
                if (TheManager.PreLoadOneMission())  // 调用 Manager.PreLoadOneMission()完成图像加载；
                {
                    int percentage = i * 100 / Parameter.PreLoadQuantity;
                    backgroundWorkerForDownload.ReportProgress(percentage);
                }
                else
                {
                    break;
                }
            }
        }
        private void AddProcessBarValue(object sender, ProgressChangedEventArgs e)
        {
            // 更新进度条
            progressBar1.Value = e.ProgressPercentage;
        }
        private void Finished(object sender, EventArgs e)
        {
            this.Close();  // 完成后关闭该窗口；
        }
    }
}
