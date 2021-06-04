﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;
using EyeOfSauron;
using Container;
using NetMQ.Sockets;
using Container.Message;
using NetMQ;

namespace ExamManager
{
    public delegate void ExamManagerWorkMethod();
    public partial class examManageForm : Form
    {
        DataSet dataset;
        SqlConnection TheDataBase;
        BindingSource bdsource;
        LoginForm TheParentForm;
        Defectcode defect_translator;
        Manager TheManager;
        SqlCommandBuilder Builder;
        SqlDataAdapter adp;
        Queue<PanelImageContainer> waitqueue = new Queue<PanelImageContainer>();
        ImageFormManager imageFormManager;
        List<string> InfoList;
        public examManageForm()
        {
            InitializeComponent();
        }
        public examManageForm(LoginForm parentForm, Manager theManager)
        {
            InitializeComponent();
            dataInitial();
            TheParentForm = parentForm;
            TheManager = theManager;
            imageFormManager = new ImageFormManager(this.pictureBox1, this.pictureBox2, this.pictureBox3);
            defect_translator = new Defectcode(Parameter.CodeNameList);
            AddDefectCode();
            AddInfoFilter();
        }
        private void dataInitial()
        {
            dataset = new DataSet();
            TheDataBase = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
            string querystring = @"SELECT [ID],[PanelID]
      ,[Judge]
      ,[DefectCode]
      ,[DefectName]
      ,[Section]
      ,[Info]
      ,[DelFlag]
  FROM [EDIAS_DB].[dbo].[AET_IMAGE_EXAM]
WHERE [DelFlag] = '0'";
            SqlCommand newcommand = new SqlCommand(querystring, TheDataBase);
            TheDataBase.Open();
            adp = new SqlDataAdapter();
            adp.SelectCommand = newcommand;
            Builder = new SqlCommandBuilder(adp);
            adp.Fill(dataset);
            TheDataBase.Close();
            bdsource = new BindingSource();
            bdsource.DataSource = dataset.Tables[0];
            this.ExamDBGridView.DataSource = bdsource;
            foreach (DataGridViewColumn Col in this.ExamDBGridView.Columns)
            {
                Col.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
                this.ExamDBGridView.Sort(this.ExamDBGridView.Columns[7], ListSortDirection.Ascending);
            }
            this.ExamDBGridView.Columns[7].Visible = false;
        }
        private void AddDefectCode()
        {
            Defect S_Defect = new Defect("S", "", InspectSection.MAIN);
            foreach (var item in Parameter.CodeNameList)
            {
                this.DefectcomboBox.Items.Add(item);
            }
            this.DefectcomboBox.Items.Add(S_Defect);
            this.DefectcomboBox.Text = Parameter.CodeNameList[0].DefectName;
        }
        private void AddInfoFilter()
        {
            InfoList = new List<string>();
            InfoList.Add("");
            for (int i = 1; i < this.ExamDBGridView.Rows.Count; i++)
            {
                string info = this.ExamDBGridView.Rows[i].Cells[6].Value.ToString();
                if (!InfoList.Contains(info))
                {
                    InfoList.Add(info);
                }
            }
            this.comboBox1.DataSource = InfoList;
        }
        private void AddPanelIdbutton_Click(object sender, EventArgs e)
        {
            PanelIdAddForm idform = new PanelIdAddForm(AddPanelId);
            idform.ShowDialog();
        }
        private void AddPanelId(string[] panelidarray, InspectSection section)
        {
            // 将 ID array 中的id添加任务；预加载图片及添加至newidlistbox中；
            var pathdic = NewSeverConnecter.GetPanelPathByID(panelidarray);
            for (int i = 0; i < panelidarray.Length; i++)
            {
                var panelid = panelidarray[i];
                if (pathdic.ContainsKey(panelid))
                {
                    var pathlist = pathdic[panelid];
                    pathlist = pathlist.Where(x => (x.PcSection == section)).ToList();
                    foreach (var item in pathlist)
                    {
                        // 加入设备中多次出现的图片
                        if (pathlist.Count > 1)
                        {
                            this.waitqueue.Enqueue(new PanelImageContainer(panelid, item, true));
                        }
                        else
                        {
                            this.waitqueue.Enqueue(new PanelImageContainer(panelid, item));
                        }
                    }
                }
                else
                {
                    string errorString = string.Format("panel: {0} cannot find the path", panelid);
                    throw new ApplicationException(errorString);
                }
            }
            ProcessForm newprocessform = new ProcessForm(AddOnePanelSafe);
            newprocessform.ShowDialog();
            Task.Run((Action)AddMutiPanel);
        }
        /// <summary>
        /// using for threadsafe method call acrossed different form;
        /// </summary>
        int AddOnePanelSafe()
        {
            if (this.waitqueue.Count == 0)
            {
                return 100;
            }
            else
            {
                this.Invoke(new ExamManagerWorkMethod(AddOnePanel), new object[] { });
                int percent = 100 * this.NewIdListBox.Items.Count / Parameter.PreLoadQuantity;
                return percent;
            }
        }
        void AddOnePanel()
        {
            var panel = this.waitqueue.Dequeue();
            panel.Download();
            this.NewIdListBox.Items.Add(panel);
        }
        void AddMutiPanel()
        {
            foreach (var item in this.waitqueue)
            {
                item.Download();
            }
            while (this.waitqueue.Count != 0)
            {
                this.Invoke(new ExamManagerWorkMethod(AddOnePanel), new object[] { });
            }
        }
        private void del_button_Click(object sender, EventArgs e)
        {
            if (this.ExamDBGridView.SelectedRows.Count != 0)
            {
                this.ExamDBGridView.SelectedRows[0].Cells[7].Value = Convert.ToInt16(this.ExamDBGridView.SelectedRows[0].Cells[7].Value) == 1 ? 0 : 1;
                int RowIndex = this.ExamDBGridView.CurrentRow.Index;
                this.bdsource.EndEdit();
                refreshDataSet();
                if (Convert.ToInt16(this.ExamDBGridView.SelectedRows[0].Cells[7].Value) == 1)
                {
                    this.ExamDBGridView.CurrentCell = this.ExamDBGridView[0, RowIndex];
                }
            }
        }
        private void refreshDataSet()
        {
            TheDataBase.Open();
            Builder.GetUpdateCommand();
            var asd = dataset.GetChanges();
            adp.Update(dataset);
            TheDataBase.Close();
        }
        private void dataGridViewRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            for (int i = 0; i < this.ExamDBGridView.Rows.Count; i++)
            {
                this.ExamDBGridView.Rows[i].DefaultCellStyle.BackColor = Convert.ToInt16(this.ExamDBGridView.Rows[i].Cells[7].Value) == 1 ? Color.LightPink : Color.White;
            }
        }
        private void CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.ExamDBGridView.SelectedRows.Count != 0)
            {
                PanelInfo panel = new PanelInfo(Convert.ToString(this.ExamDBGridView.CurrentRow.Cells[1].Value), (InspectSection)Enum.Parse(typeof(InspectSection), Convert.ToString(this.ExamDBGridView.CurrentRow.Cells[5].Value)));
                List<PanelInfo> panelIdList = new List<PanelInfo>();
                panelIdList.Add(panel);
            }
        }
        private void Cleanbutton_Click(object sender, EventArgs e)
        {
            this.NewIdListBox.Items.Clear();
        }
        private void NewIdListBox_Click(object sender, EventArgs e)
        {
            this.ExamDBGridView.ClearSelection();

            this.button4.Text = "添加";
            this.button4.BackColor = Color.FromArgb(255, 240, 240, 240);
            this.Refresh();
        }
        private void ExamDBGridView_MouseClick(object sender, MouseEventArgs e)
        {
            this.NewIdListBox.ClearSelected();
            if (this.ExamDBGridView.SelectedRows.Count != 0)
            {
                this.button3.Text = Convert.ToInt16(this.ExamDBGridView.CurrentRow.Cells[7].Value) == 0 ? "删除" : "取消删除";
                var color = this.button3.BackColor;
                this.button3.BackColor = Convert.ToInt16(this.ExamDBGridView.CurrentRow.Cells[7].Value) == 0 ? Color.Orange : Color.FromArgb(255, 240, 240, 240);
            }
            this.button4.Text = "修改";
            this.button4.BackColor = Color.GreenYellow;
            this.Refresh();
        }
        private void NewIdListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox theListBox = (ListBox)sender;
            if (theListBox.SelectedItems.Count == 1)
            {
                PanelImageContainer item = (PanelImageContainer)theListBox.SelectedItem;
                if (item.Section == InspectSection.AVI)
                {
                    var filearray = item.GetFile(Parameter.AviImageNameList);
                    imageFormManager.SetImageArray(filearray);
                }
                else if (item.Section == InspectSection.SVI)
                {
                    var filearray = item.GetFile(Parameter.SviImageNameList);
                    imageFormManager.SetImageArray(filearray);
                }
                else if (item.Section == InspectSection.APP)
                {
                    var filearray = item.GetFile(Parameter.AppImageNameList);
                    imageFormManager.SetImageArray(filearray);
                }
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                imageFormManager.RefreshForm();
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        private void AddButton_Click(object sender, EventArgs e)
        {
            var defect = (Defect)this.DefectcomboBox.SelectedItem;
            if (this.ExamDBGridView.SelectedRows.Count != 0)
            {
                this.ExamDBGridView.SelectedRows[0].Cells[2].Value = this.DefectcomboBox.Text == "S" ? "S" : "F";
                this.ExamDBGridView.SelectedRows[0].Cells[3].Value = defect.DefectCode;
                this.ExamDBGridView.SelectedRows[0].Cells[4].Value = this.DefectcomboBox.Text == "S" ? "" : defect.DefectName;
                this.ExamDBGridView.SelectedRows[0].Cells[6].Value = this.comboBox1.Text;
            }
            else
            {
                if (this.comboBox1.Text != "")
                {
                    foreach (PanelImageContainer item in this.NewIdListBox.SelectedItems)
                    {
                        DataRow newRow = dataset.Tables[0].NewRow();
                        newRow[1] = item.PanelId;
                        newRow[2] = this.DefectcomboBox.Text == "S" ? "S" : "F";
                        newRow[3] = defect.DefectCode;
                        newRow[4] = defect.DefectName;
                        newRow[5] = item.Section;
                        newRow[6] = this.comboBox1.Text;
                        newRow[7] = "0";
                        dataset.Tables[0].Rows.Add(newRow);
                        item.Save(@"\\172.16.145.22\NetworkDrive2\D_Drive\Mordor\ExamSimple\"+ item.Section);
                    }
                    refreshDataSet();
                    dataset.Clear();
                    TheDataBase.Open();
                    adp.Fill(dataset);
                    TheDataBase.Close();
                    bdsource.DataSource = dataset.Tables[0];
                }
            }
            refreshDataSet();
            this.bdsource.EndEdit();
            this.ExamDBGridView.ClearSelection();
            this.Refresh();
        }
        private void FilterChanged(object sender, EventArgs e)
        {
            bdsource.Filter = "info = '" + this.comboBox1.Text + "'";
            this.ExamDBGridView.ClearSelection();
        }
        private void InfoFilterAdd(object sender, EventArgs e)
        {
            InfoList.Add(this.comboBox1.Text);
            this.comboBox1.DataSource = InfoList;
            FilterChanged(sender, e);
        }
    }
    public static class NewSeverConnecter
    {
        static private RequestSocket request;
        static NewSeverConnecter()
        {
            request = new RequestSocket();
            request.Connect("tcp://172.16.145.22:5555");
        }
        public static Operator CheckPassWord(Operator theuser)
        {
            UserCheckMessage newMessage = new UserCheckMessage(MessageType.CLINET_CHECK_USER, theuser);
            request.SendMultipartMessage(newMessage);
            UserCheckMessage returnUser = new UserCheckMessage(request.ReceiveMultipartMessage());
            if (returnUser.TheMessageType == MessageType.SERVER_SEND_USER_TRUE)
                return returnUser.TheOperator;
            else
                return null;
        }
        public static Dictionary<string, List<PanelPathContainer>> GetPanelPathByID(string[] panelIdList)
        {
            BaseMessage newmessage = new PanelPathMessage(MessageType.CLINET_GET_PANEL_PATH, panelIdList);
            request.SendMultipartMessage(newmessage);
            var returnmessage = new PanelPathMessage(request.ReceiveMultipartMessage());
            return returnmessage.panelPathDic;
        }
    }
    static class ExamFileManager
    {
        static string examFilePath = @"\\172.16.145.22\NetworkDrive\D_Drive\Mordor\ExamSimple";
        public static void DeleteFile(string panelid, InspectSection section)
        {
            // 当同一张屏存在与不同任务集中时不删除；
            string filePath = examFilePath;
            if (section == InspectSection.AVI)
            {
                filePath += @"\AVI";
            }
            else if (section == InspectSection.SVI)
            {
                filePath += @"\SVI";
            }
            DirectoryInfo dir = new DirectoryInfo(filePath);
            if (dir.Exists)
            {
                dir.Delete();
            }
        }
        public static void AddFile(DirContainer dir, InspectSection section)
        {
            if (section == InspectSection.AVI)
            {
                dir.SaveDirInDisk(examFilePath + @"\AVI");
            }
            else if (section == InspectSection.SVI)
            {
                dir.SaveDirInDisk(examFilePath + @"\SVI");
            }
        }
    }
}

