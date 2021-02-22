using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Container;

namespace EyeOfSauron
{
    public partial class LoginForm : Form
    {
        private Manager TheManager;
        private InspectForm TheInspectForm;

        public LoginForm()
        {
            InitializeComponent();
            TheManager = new Manager();
            TheInspectForm = new InspectForm(this,TheManager);
        }

        private void userid_box_KeyDown(object sender, KeyEventArgs e)
        {
            // binding with the keyboard enter .
            if (e.KeyCode == Keys.Enter)
            {
                Avibutton_Click(sender, e);
            }
        }

        private void Avibutton_Click(object sender, EventArgs e)
        {
            TheInspectForm.SetInspectSection(InspectSection.AVI);
            UserCheckIn(TheInspectForm);
        }

        private void UserCheckIn(Form form)
        {
            Operator newoperater = new Operator(this.userid_box.Text, "");
            if (TheManager.CheckUser(newoperater))
            {
                TheManager.OperaterCheckIn(newoperater);
                this.Hide();
                ProcessForm newDownloadProcess = new ProcessForm(TheManager);
                newDownloadProcess.ShowDialog();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("用户名密码错误");
            }
        }

        private void Svibutton_Click(object sender, EventArgs e)
        {
            TheInspectForm.SetInspectSection(InspectSection.SVI);
            UserCheckIn(TheInspectForm);
        }

        private void Evilbutton_Click(object sender, EventArgs e)
        {
            TheInspectForm.SetInspectSection(InspectSection.SVI);
            UserCheckIn(TheInspectForm);
        }
    }
}
