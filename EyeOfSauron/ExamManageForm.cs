using System;
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
    public partial class examManageForm : Form
    {
        DataSet             dataset;
        SqlConnection       TheDataBase;
        BindingSource       bdsource;
        LoginForm           TheParentForm;
        Defectcode          defect_translator;
        Manager             TheManager;
        SqlCommandBuilder   Builder;
        SqlDataAdapter      adp;
        List<PanelInfo>     idList;
        ImageFormManager    imageFormManager;
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
            idList = new List<PanelInfo>();
            AddDefectCode();
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
            foreach(DataGridViewColumn Col in this.ExamDBGridView.Columns)
            {
                Col.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
                this.ExamDBGridView.Sort(this.ExamDBGridView.Columns[7], ListSortDirection.Ascending);
            }
            //this.ExamDBGridView.Columns[7].Visible = false;
        }
        public static bool chcekIsTextFile(string fileName)
        {
            //TODO: 1、保存内存中图像图片 ；
            //      2、更新数据库；
            return true;
        }
        private void AddDefectCode()
        {
            foreach (var item in Parameter.CodeNameList)
            {
                this.DefectcomboBox.Items.Add(item.DefectName);
            }
            this.DefectcomboBox.Text = Parameter.CodeNameList[0].DefectName;
        }
        private void AddPanelIdbutton_Click(object sender, EventArgs e)
        {
            PanelIdAddForm idform = new PanelIdAddForm();
            idform.BindIdArray(idList);
            idform.ShowDialog();
            AddPanelId();
        }
        private void AddPanelId()
        {
            // 将 ID array 中的id添加任务；预加载图片及添加至newidlistbox中；
            foreach (var item in idList)
            {
                this.NewIdListBox.Items.Add(item);
            }
        }
        private void del_button_Click(object sender, EventArgs e)
        {
            this.ExamDBGridView.SelectedRows[0].Cells[7].Value = Convert.ToInt16(this.ExamDBGridView.SelectedRows[0].Cells[7].Value) == 1 ? 0: 1;
            int RowIndex = this.ExamDBGridView.CurrentRow.Index;
            this.bdsource.EndEdit();
            refreshDataSet();
            if (Convert.ToInt16(this.ExamDBGridView.SelectedRows[0].Cells[7].Value) == 1)
            {
                this.ExamDBGridView.CurrentCell = this.ExamDBGridView[0, RowIndex];
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
                this.ExamDBGridView.Rows[i].DefaultCellStyle.BackColor = Convert.ToInt16(this.ExamDBGridView.Rows[i].Cells[7].Value) == 1? Color.LightPink: Color.White;
            }
        }
        private void CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.button3.Text = Convert.ToInt16(this.ExamDBGridView.CurrentRow.Cells[7].Value) == 0 ? "删除" : "取消删除";
            this.Refresh();
            PanelInfo panel = new PanelInfo(Convert.ToString(this.ExamDBGridView.CurrentRow.Cells[1].Value), (InspectSection)Enum.Parse(typeof(InspectSection), Convert.ToString(this.ExamDBGridView.CurrentRow.Cells[5].Value)));
            List<PanelInfo> panelIdList = new List<PanelInfo>();
            panelIdList.Add(panel);
        }
        private void Cleanbutton_Click(object sender, EventArgs e)
        {
            this.NewIdListBox.Items.Clear();
            this.idList.Clear();
        }
        private void NewIdListBox_Click(object sender, EventArgs e)
        {
            this.ExamDBGridView.ClearSelection();
        }
        private void ExamDBGridView_MouseClick(object sender, MouseEventArgs e)
        {
            this.NewIdListBox.ClearSelected();
        }
        private void NewIdListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox theListBox = (ListBox)sender;
            if (theListBox.SelectedItems.Count == 1)
            {
                PanelInfo newpanel = (PanelInfo)theListBox.SelectedItem;

                //imageFormManager.SetImageArray();
            }
            // 刷新界面图像
        }
    }
    class SeverConnecter
    {
        private RequestSocket request;
        public SeverConnecter()
        {
            request = new RequestSocket();
            request.Connect("tcp://172.16.145.22:5555");
        }
        public Queue<PanelInfo> GetPanelInfoByID(List<PanelInfo> panelIdList)
        {
            Queue<PanelInfo> SampleInfoList = new Queue<PanelInfo>();
            BaseMessage newmessage = new PanelInfoMessage(MessageType.CLINET_GET_PANEL_INFO, panelIdList);
            request.SendMultipartMessage(newmessage);
            var returnmessage = new PanelInfoMessage(request.ReceiveMultipartMessage());
            foreach (var item in returnmessage.panelInfoList)
            {
                SampleInfoList.Enqueue(item);
            }
            return SampleInfoList;
        }

    }
    class PanelFileManager
    {
        string examFilePath = @"\\172.16.145.22\NetworkDrive\D_Drive\Mordor\ExamSimple";
        Dictionary<int, DirContainer> recycleBin;
        public void DeleteFile(string panelid, InspectSection section)
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
        public void AddFile(DirContainer dir, InspectSection section)
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

