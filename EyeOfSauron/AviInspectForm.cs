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
        }
        private void logout(object sender, EventArgs e)
        {
            login_button.Text = "用户登录";
            login_button.BackColor = System.Drawing.Color.SandyBrown;
            // do someting else;
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
            origin_image_Box.Image = TheManager.GetOnInspectPanelImage()[0];
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
