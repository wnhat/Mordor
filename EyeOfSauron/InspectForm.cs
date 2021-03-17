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
        Defectcode defect_translator = new Defectcode();
        Manager TheManager;
        int Refreshflag;
        Bitmap[] ImageArray;
        string[] ImageNameArray;
        public InspectForm(LoginForm parentForm, Manager theManager)
        {
            // initial AviInspectForm；
            InitializeComponent();
            TheParentForm = parentForm;
            TheManager = theManager;
            Refreshflag = 0;
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
        public void SetPanelId(string panelid)
        {
            cell_id_label.Text = panelid;
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
        }
        private void judge_function(object sender, EventArgs e)
        {
            Button sender_button = (Button)sender;
            string defectname = sender_button.Text;
            string defectcode = defect_translator.name2code(sender_button.Text);
            JudgeGrade newjudge = defect_translator.name2judge(sender_button.Text);
            Defect newdefect = new Defect(defectname, defectcode, TheManager.Section);
            var getNextPanelMessage = TheManager.InspectFinished(newdefect,newjudge);
            if (getNextPanelMessage != null)
            {
                MessageBox.Show(getNextPanelMessage);
            }
            else
            {
                ReadData();
            }
        }
        private void ReadData()
        {
            ImageNameArray = TheManager.OnInspectedMission.ImageNameList;
            Bitmap[] ImageArray = new Bitmap[ImageNameArray.Length];
            for (int i = 0; i < ImageNameArray.Length; i++)
            {
                ImageArray[i] = new Bitmap(TheManager.OnInspectedMission.GetFileFromMemory(ImageNameArray[i]));
            }
        }
        private void logout(object sender, EventArgs e)
        {
            login_button.Text = "用户登录";
            login_button.BackColor = System.Drawing.Color.SandyBrown;
            this.Close();
            TheParentForm.Show();
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            TheManager.SendUnfinishedMissionBackToServer();
            TheParentForm.Show();
            base.OnFormClosed(e);
        }
    }
}
