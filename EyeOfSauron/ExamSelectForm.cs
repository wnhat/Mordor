using Container;
using Container.SeverConnection;
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
        string[] ExamInfoArray;
        public ExamSelectForm(Operator op)
        {
            InitializeComponent();
            InitalComboBox();
            user = op;
        }
        public void InitalComboBox()
        {
            ExamInfoArray = SeverConnecter.GetExamInfo();
            this.ExamIfoncomboBox.DataSource = ExamInfoArray;
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
            newinspectform.login(user);         // 写入用户
            newinspectform.ShowDialog();
            this.Close();
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }
    }
}
