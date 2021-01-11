using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        // register login event；
        public delegate void login_check_handler(string a);
        public event login_check_handler logevent;


        private void login_function(object sender, EventArgs e)
        {
            string a = userid_box.Text;
            logevent(a);
        }

    }
}
