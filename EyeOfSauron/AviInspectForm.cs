﻿using System;
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

namespace EyeOfSauron
{
    public partial class AviInspectForm : Form
    {
        LoginForm TheParentForm;
        Defectcode defect_translator = new Defectcode();
        Manager TheManager;

        public AviInspectForm(LoginForm parentForm,Manager theManager)
        {
            // initial AviInspectForm；
            InitializeComponent();
            TheParentForm = parentForm;
            TheManager = theManager;

            // set image name label；
            imagelabel1.Text = TheManager.SystemParameter.ImageNameList[0];
            imagelabel2.Text = TheManager.SystemParameter.ImageNameList[1];
            imagelabel3.Text = TheManager.SystemParameter.ImageNameList[2];
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
            TheParentForm.Show();
            base.OnFormClosed(e);
        }
    }
}
