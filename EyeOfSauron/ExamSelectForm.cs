using Container;
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
    public partial class ExamSelectForm : Form
    {
        Operator user;
        public ExamSelectForm(Operator op)
        {
            InitializeComponent();
            InitalComboBox();
            user = op;
        }
        public void InitalComboBox()
        {

        }
        private void Startbutton_Click(object sender, EventArgs e)
        {
            InspectForm newinspectform = new InspectForm();
            Manager newmanager = new Manager();
            newmanager.SetInspectSection(InspectSection.EXAM);
            newmanager.SetOperater(user);
            newinspectform.ConnectManager(newmanager);
            newmanager.TheMissionBuffer.GetExamMissions(this.ExamIfoncomboBox.Text);
            this.Hide();
            newinspectform.login(user);   // 写入用户
            newinspectform.ReadData();    // 刷新数据
            newinspectform.ShowDialog();
            this.Close();
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            this.Parent.Show();
            base.OnFormClosed(e);
        }
    }
}
