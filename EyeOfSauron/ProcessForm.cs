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
            backgroundWorkerForDownload.WorkerReportsProgress = true;
            backgroundWorkerForDownload.DoWork += DownloadWork;
            backgroundWorkerForDownload.ProgressChanged += AddProcessBarValue;
            backgroundWorkerForDownload.RunWorkerCompleted += Finished;
            backgroundWorkerForDownload.RunWorkerAsync();
        }

        private void DownloadWork(object sender, EventArgs e)
        {
            for (int i = 0; i < TheManager.SystemParameter.PreLoadQuantity; i++)
            {
                TheManager.AddOneMission();
                backgroundWorkerForDownload.ReportProgress(i / TheManager.SystemParameter.PreLoadQuantity);
            }
        }

        private void AddProcessBarValue(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Finished(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
