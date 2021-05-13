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
        LoginForm TheParentForm;
        Defectcode defect_translator;
        Manager TheManager;
        int Refreshflag;
        Bitmap[] ImageArray;
        string[] ImageNameArray;
        public InspectForm(LoginForm parentForm, Manager theManager)
        {
            // initial AviInspectForm:
            InitializeComponent();
            TheParentForm = parentForm;
            TheManager = theManager;
            Refreshflag = 0;
            defect_translator = new Defectcode(Parameter.CodeNameList);
            SetDefectButton();
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
        public void SetImageLabel(string[] imagenamelist)
        {
            imagelabel1.Text = imagenamelist[0];
            imagelabel2.Text = imagenamelist[1];
            imagelabel3.Text = imagenamelist[2];
        }
        public void SetImage(Bitmap[] imagearray)
        {
            pictureBox1.Image = imagearray[0];
            pictureBox2.Image = imagearray[1];
            pictureBox3.Image = imagearray[2];
        }
        public void RefreshForm()
        {
            if((Refreshflag)*3 < ImageArray.Count())
            {
                SetImageLabel(ImageNameArray.Skip((Refreshflag)*3).Take(3).ToArray());
                SetImage(ImageArray.Skip((Refreshflag)*3).Take(3).ToArray());
                Refreshflag++;
            }
            else
            {
                Refreshflag = 0;
                RefreshForm();
            }
            // 刷新文本内容
            cell_id_label.Text = TheManager.OnInspectedMission.PanelId;
            remain_label.Text = string.Format("已进行：{0}",TheManager.FinishedMissionCount.ToString());
        }
        private void judge_function(object sender, EventArgs e)
        {
            Button sender_button = (Button)sender;
            string defectname = sender_button.Text;
            string defectcode = defect_translator.name2code(sender_button.Text);
            JudgeGrade newjudge = defect_translator.name2judge(sender_button.Text);
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
                ImageNameArray = TheManager.OnInspectedMission.ImageNameList;
                Bitmap[] newImageArray = new Bitmap[ImageNameArray.Length];
                for (int i = 0; i < ImageNameArray.Length; i++)
                {
                    var imagestream = TheManager.OnInspectedMission.GetFileFromMemory(ImageNameArray[i]);
                    newImageArray[i] = new Bitmap(imagestream);
                }
                ImageArray = newImageArray;
                Refreshflag = 0;
                RefreshForm();
            }
        }
        private void logout(object sender, EventArgs e)
        {
            login_button.Text = "用户登录";
            login_button.BackColor = System.Drawing.Color.SandyBrown;
            this.Close();
            TheParentForm.Show();
        }
        public void login(Operator newop)
        {
            login_button.Text = newop.Name;
            login_button.BackColor = System.Drawing.Color.PaleGreen;
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            TheManager.SendUnfinishedMissionBackToServer();
            TheManager.OperaterCheckOut();
            TheParentForm.Show();
            base.OnFormClosed(e);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                RefreshForm();
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
    }
}
