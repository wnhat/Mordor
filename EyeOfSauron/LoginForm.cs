﻿using System;
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
using Container.SeverConnection;

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
            if (e.KeyCode == Keys.Enter)
            {
                Inspectbutton_Click(sender, e);
            }
        }
        private void Inspectbutton_Click(object sender, EventArgs e)
        {
            Operator newoperater = new Operator(this.userid_box.Text, this.userid_box.Text);
            var user = SeverConnecter.CheckPassWord(newoperater);
            if (user != null)
            {
                InspectForm newinspectform = new InspectForm();
                Manager newmanager = new Manager();
                newmanager.SetInspectSection(InspectSection.NORMAL);
                newmanager.SetOperater(user);
                newinspectform.ConnectManager(newmanager);
                try
                {
                    newmanager.TheMissionBuffer.GetPanelMission();
                    this.Hide();
                    newinspectform.login(user);   // 写入用户
                    newinspectform.ShowDialog();
                    this.Show();
                }
                catch (Exception a)
                {
                    MessageBox.Show(a.Message);
                }
            }
            else
            {
                MessageBox.Show("用户名密码错误");
            }
        }
        private void Evilbutton_Click(object sender, EventArgs e)
        {
            // 考试；
            Operator newoperater = new Operator(this.userid_box.Text, this.userid_box.Text);
            var user = SeverConnecter.CheckPassWord(newoperater);
            if (user != null)
            {
                ExamSelectForm selectform = new ExamSelectForm(user);
                this.Hide();
                selectform.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("用户名密码错误");
            }
        }
        private void ExamManagerButton_Click(object sender, EventArgs e)
        {
            Operator newoperater = new Operator(this.userid_box.Text, this.userid_box.Text);
            var user = SeverConnecter.CheckPassWord(newoperater);
            if (user != null)
            {
                examManageForm newexamform = new examManageForm();
                this.Hide();
                newexamform.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("用户名密码错误");
            }
        }
    }
}