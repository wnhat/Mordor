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
        Manager TheManager;

        public LoginForm(Manager theManager)
        {
            InitializeComponent();
            TheManager = theManager;
        }

        // register login event；
        public delegate void login_check_handler(string a);
        public event login_check_handler logevent;
        
        private void login_function(object sender, EventArgs e)
        {
            // binding with the login button.
            string a = userid_box.Text;
            logevent(a);
        }

        private void userid_box_KeyDown(object sender, KeyEventArgs e)
        {
            // binding with the keyboard enter .
            if (e.KeyCode == Keys.Enter)
            {
                login_function(sender, e);
            }
        }

        private void Avibutton_Click(object sender, EventArgs e)
        {
            User newoperater = new User(this.userid_box.Text, "");
            if (TheManager.CheckUser(newoperater))
            {
                TheManager.OperaterCheckIn(newoperater);
                this.Hide();
            }
            else
            {
                MessageBox.Show("用户名密码错误");
            }
        }
    }
}
