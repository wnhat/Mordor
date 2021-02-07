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
        InspectSection Section;     // AVI\SVI\APP 

        public InspectForm(LoginForm parentForm,Manager theManager)
        {
            // initial AviInspectForm；
            InitializeComponent();
            TheParentForm = parentForm;
            TheManager = theManager;
        }

        public void SetInspectSection(InspectSection section)
        {
            switch (section)
            {
                case InspectSection.AVI:
                    
                    imagelabel1.Text = TheManager.SystemParameter.AviImageNameList[0];
                    imagelabel2.Text = TheManager.SystemParameter.AviImageNameList[1];
                    imagelabel3.Text = TheManager.SystemParameter.AviImageNameList[2];
                    break;
                case InspectSection.SVI:

                    imagelabel1.Text = TheManager.SystemParameter.SviImageNameList[0];
                    imagelabel2.Text = TheManager.SystemParameter.SviImageNameList[1];
                    imagelabel3.Text = TheManager.SystemParameter.SviImageNameList[2];
                    break;
                case InspectSection.APP:

                    break;
                default:
                    Section = section;
                    break;
            }
        }

        private void logout(object sender, EventArgs e)
        {
            login_button.Text = "用户登录";
            login_button.BackColor = System.Drawing.Color.SandyBrown;
            // do someting else;
            // send unfinished mission bak to the server;
            this.Close();
            TheParentForm.Show();
        }
        private void judge_function(object sender, EventArgs e)
        {
            Button sender_button = (Button)sender;
            string defectname = sender_button.Text;
            string defectcode = defect_translator.name2code(sender_button.Text);

            get_next_panel();
        }
        private void get_next_panel()
        {
            TheManager.InspectStart();
            Bitmap[] imagearray = TheManager.GetOnInspectPanelImage();
            pictureBox1.Image = imagearray[0];
            pictureBox2.Image = imagearray[1];
            pictureBox3.Image = imagearray[2];
            cell_id_label.Text = TheManager.GetOnInspectPanelId();
            remain_label.Text = TheManager.RemainMissionCount.ToString();
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            TheManager.SendUnfinishedMissionBackToServer(Section);
            TheParentForm.Show();
            base.OnFormClosed(e);
        }
    }
}
