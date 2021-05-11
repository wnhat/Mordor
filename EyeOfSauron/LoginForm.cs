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
        private Manager TheManager;
        private InspectForm TheInspectForm;
        private examManageForm TheExamManageForm;
        public LoginForm()
        {
            InitializeComponent();
            TheManager = new Manager();
            TheInspectForm = new InspectForm(this,TheManager);
            TheExamManageForm = new examManageForm(this, TheManager);
        }
        private void userid_box_KeyDown(object sender, KeyEventArgs e)
        {
            // binding with the keyboard enter.
            if (e.KeyCode == Keys.Enter)
            {
                Avibutton_Click(sender, e);
            }
        }
        private void UserCheckIn(InspectForm form)
        {
            // 检查用户名密码正确后 显示检查界面
            Operator newoperater = new Operator(this.userid_box.Text, this.userid_box.Text);
            var user = TheManager.CheckUser(newoperater);
            if (user != null)
            {
                TheManager.SetOperater(user);
                this.Hide();
                ProcessForm newDownloadProcess = new ProcessForm(TheManager); // 初始化预加载界面；
                newDownloadProcess.ShowDialog();
                TheManager.ChangeDownloadQuantity();
                form.login(user);   // 写入用户
                form.ReadData();    // 刷新数据
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("用户名密码错误");
            }
        }
        private void Avibutton_Click(object sender, EventArgs e)
        {
            TheManager.SetInspectSection(InspectSection.AVI);
            UserCheckIn(TheInspectForm);
        }
        private void Svibutton_Click(object sender, EventArgs e)
        {
            TheManager.SetInspectSection(InspectSection.SVI);
            UserCheckIn(TheInspectForm);
        }
        private void Evilbutton_Click(object sender, EventArgs e)
        {
            // 考试
            //TheManager.SetInspectSection(InspectSection.EXAM);
            //TheManager.AddExamMissions();
            //UserCheckIn(TheInspectForm);
            this.Hide();
            TheExamManageForm.ShowDialog();
        }
    }
}