using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Container;

namespace EyeOfSauron
{
    public partial class InspectForm : Form
    {
        Manager TheManager;
        ImageFormManager imageManager;
        public InspectForm()
        {
            // initial InspectForm:
            InitializeComponent();
            imageManager = new ImageFormManager(this.pictureBox1, this.pictureBox2, this.pictureBox3);
            imageManager.BindLabel(this.imagelabel1,this.imagelabel2,this.imagelabel3);
            SetDefectButton();
        }
        public void ConnectManager(Manager M)
        {
            TheManager = M;
        }
        private void SetDefectButton()
        {
            this.defect_button_1.Text = Parameter.CodeNameList[0].DefectName;
            this.defect_button_2.Text = Parameter.CodeNameList[1].DefectName;
            this.defect_button_3.Text = Parameter.CodeNameList[2].DefectName;
            this.defect_button_4.Text = Parameter.CodeNameList[3].DefectName;
            this.defect_button_5.Text = Parameter.CodeNameList[4].DefectName;
            this.defect_button_6.Text = Parameter.CodeNameList[5].DefectName;
            this.defect_button_7.Text = Parameter.CodeNameList[6].DefectName;
            this.defect_button_8.Text = Parameter.CodeNameList[7].DefectName;
            this.defect_button_9.Text = Parameter.CodeNameList[8].DefectName;
            this.defect_button_10.Text = Parameter.CodeNameList[9].DefectName;
            this.defect_button_11.Text = Parameter.CodeNameList[10].DefectName;
            this.defect_button_12.Text = Parameter.CodeNameList[11].DefectName;
        }
        private void judge_function(object sender, EventArgs e)
        {
            Button sender_button = (Button)sender;
            string defectname = sender_button.Text;
            string defectcode = Defectcode.name2code(sender_button.Text);
            JudgeGrade newjudge = Defectcode.name2judge(sender_button.Text);
            Defect newdefect = new Defect(defectname, defectcode, TheManager.Section);
            TheManager.InspectFinished(newdefect,newjudge);
            ReadData();
        }
        public void ReadData()
        {
            if (!TheManager.NextMission())
            {
                MessageBox.Show("there is no mission left.");
                this.Close();
            }
            else
            {
                this.cell_id_label.Text = TheManager.OnInspectedMission.PanelId;
                this.remain_label.Text = string.Format("已完成：{0}\n 剩余：{1}",TheManager.FinishedMissionCount,TheManager.MissionLeft);
                imageManager.SetArray(TheManager.OnInspectedMission.ImageArray, TheManager.OnInspectedMission.ImageNameList);
            }
        }
        protected override void OnShown(EventArgs e)
        {
            ProcessForm newprocessform = new ProcessForm(this.TheManager.PreLoadMissions);
            newprocessform.ShowDialog();
            base.OnShown(e);
            this.ReadData();
        }
        private void logout(object sender, EventArgs e)
        {
            // TODO: 确认后登出；
            login_button.Text = "用户登录";
            login_button.BackColor = System.Drawing.Color.SandyBrown;
            this.Close();
        }
        public void login(Operator newop)
        {
            login_button.Text = newop.Name;
            login_button.BackColor = System.Drawing.Color.PaleGreen;
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            TheManager.OperaterCheckOut();
            base.OnFormClosed(e);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                imageManager.RefreshForm();
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
    }
}
