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
using ExamManager;

namespace EyeOfSauron
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        private void userid_box_KeyDown(object sender, KeyEventArgs e)
        {
            // binding with the keyboard enter.
            //if (e.KeyCode == Keys.Enter)
            //{
            //    Avibutton_Click(sender, e);
            //}
        }
        private void UserCheckIn(InspectSection section)
        {
            // 检查用户名密码正确后 显示检查界面
            Operator newoperater = new Operator(this.userid_box.Text, this.userid_box.Text);
            var user = NewSeverConnecter.CheckPassWord(newoperater);
            if (user != null)
            {
                InspectForm newinspectform = new InspectForm();
                Manager newmanager = new Manager();
                newmanager.SetInspectSection(section);
                newmanager.SetOperater(user);
                newinspectform.ConnectManager(newmanager);
                this.Hide();
                newinspectform.login(user);   // 写入用户
                newinspectform.ReadData();    // 刷新数据
                newinspectform.ShowDialog();
            }
            else
            {
                MessageBox.Show("用户名密码错误");
            }
        }
        private void Inspectbutton_Click(object sender, EventArgs e)
        {
            UserCheckIn(InspectSection.AVI);
        }
        private void Evilbutton_Click(object sender, EventArgs e)
        {
            UserCheckIn(InspectSection.EXAM);
        }
        private void ExamManagerButton_Click(object sender, EventArgs e)
        {
            examManageForm newexamform = new examManageForm();
            this.Hide();
            newexamform.ShowDialog();
        }
    }
}