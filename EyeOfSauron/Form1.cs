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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // TODO: initial serverconnect and panel_inf
        Serverconnect server = new Serverconnect();
        Defectcode defect_translator = new Defectcode();
        Panel on_inspect_panel;
        Panel_inf panel_manager;
        string user = "";

        public Form1()
        {
            // initial form1；

            InitializeComponent();
            // register event；
        }

        // 

        private void login(object sender, EventArgs e)
        {
            if (user.Length == 0)
            {
                Form2 the_login_form = new Form2();
                the_login_form.logevent += new Form2.login_check_handler(server.check_user_password);
                the_login_form.logevent += new Form2.login_check_handler(setlogintxt);
                server.logevent += new Serverconnect.server_login_check_handler(the_login_form.Close);
                the_login_form.ShowDialog();
            }
            else
            {
                logout();
            }
        }

        private void setlogintxt(string a)
        {
            login_button.Text = a;
            login_button.BackColor = System.Drawing.Color.YellowGreen;
            user = a;
            panel_manager = new Panel_inf(server.Get_mission());
            get_next_panel();
        }

        private void logout()
        {
            user = "";
            login_button.Text = "用户登录";
            login_button.BackColor = System.Drawing.Color.SandyBrown;
            // do someting else;
        }

        private void judge_function(object sender, EventArgs e)
        {
            Button sender_button = (Button)sender;
            string defectname = sender_button.Text;
            string defectcode = defect_translator.name2code(sender_button.Text);
            string judge_op = user;
            string panel_id = "";
            if (!server.uploadjudgement(panel_id, judge_op, defectcode, defectname))
            {
                // TODO;if connect to server failed, do something;
                MessageBox.Show("panel judge upload failed");
            }
            get_next_panel();
        }
        private void get_next_panel()
        {
            
            if (panel_manager.Get_remain_job() != 0)
            {
                panel_manager.Destroy_fist_panel();
                on_inspect_panel = panel_manager.get_first_panel();
                origin_image_Box.Image = new Bitmap(on_inspect_panel.get_image());
                cell_id_label.Text = on_inspect_panel.panel_id;
                remain_label.Text = panel_manager.Get_remain_job().ToString();
                Thread refreash = new Thread(panel_manager.Refreshpanellist);
                refreash.Start();
            }
            else
            {
                MessageBox.Show("任务已完成");
            }
        }
    }
}
