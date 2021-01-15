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
    public partial class LoginForm : Form
    {
        private Manager TheManager;
        private AviInspectForm AviForm;

        public LoginForm()
        {
            InitializeComponent();
            TheManager = new Manager();
            AviForm = new AviInspectForm(this,TheManager);
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
            UserCheckIn(AviForm);
        }

        private void UserCheckIn(Form form)
        {
            User newoperater = new User(this.userid_box.Text, "");
            if (TheManager.CheckUser(newoperater))
            {
                TheManager.OperaterCheckIn(newoperater);
                this.Hide();
                //form.Show();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("用户名密码错误");
            }
        }
    }
}
