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
using Container.SeverConnection;
using System.Threading;

namespace ExamManager
{
    public delegate void ExamManagerWorkMethod();
    public partial class examManageForm : Form
    {
        DataSet dataset;
        SqlConnection TheDataBase;
        BindingSource bdsource;
        SqlCommandBuilder Builder;
        SqlDataAdapter adp;
        Queue<string> waitqueue = new Queue<string>();
        Dictionary<string, List<PanelPathContainer>> NewPathDic;
        ImageFormManager imageFormManager;
        List<string> InfoList;
        enum DelFlag
        {
            NORMAL,
            DELETE,
            ADD
        };
        public examManageForm()
        {
            InitializeComponent();
            dataInitial();
            imageFormManager = new ImageFormManager(this.pictureBox1, this.pictureBox2, this.pictureBox3);
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
      ,[Info]
      ,[DelFlag]
  FROM [EDIAS_DB].[dbo].[AET_IMAGE_EXAM]
WHERE [DelFlag] = '0' OR [DelFlag] = '2'";
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
                this.ExamDBGridView.Sort(this.ExamDBGridView.Columns[6], ListSortDirection.Ascending);
            }
            this.ExamDBGridView.Columns[6].Visible = false;
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
                string info = this.ExamDBGridView.Rows[i].Cells[5].Value.ToString();
                if (!InfoList.Contains(info))
                {
                    InfoList.Add(info);
                }
            }
            this.ExamInfocomboBox.DataSource = InfoList;
        }
        private void AddPanelIdbutton_Click(object sender, EventArgs e)
        {
            PanelIdAddForm idform = new PanelIdAddForm(AddPanelId);
            idform.ShowDialog();
        }
        private void AddPanelId(string[] panelidarray)
        {
            // 将 ID array 中的id添加任务；预加载图片及添加至newidlistbox中；
            foreach (var item in panelidarray)
            {
                this.waitqueue.Enqueue(item);
            }
            NewPathDic = SeverConnecter.GetPanelPathByID(panelidarray);
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
                return percent >= 100 ? 100: percent;
            }
        }
        void AddOnePanel()
        {
            var panelid = this.waitqueue.Dequeue();
            try
            {
                var pathlist = NewPathDic[panelid];
                List<string> eqidlist = new List<string>();
                foreach (var item in pathlist)
                {
                    if (!eqidlist.Contains(item.EqId))
                    {
                        eqidlist.Add(item.EqId);
                    }
                }
                foreach (var item in eqidlist)
                {
                    // 设备中多次出现会重复加入；
                    PanelPathContainer avipath = pathlist.Where(x => (x.EqId == item && x.PcSection == InspectSection.AVI)).ToArray().First();
                    PanelPathContainer svipath = pathlist.Where(x => (x.EqId == item && x.PcSection == InspectSection.SVI)).ToArray().First();
                    var newpanel = new PanelImageContainer(panelid, avipath, svipath, true);
                    newpanel.Download();
                    this.NewIdListBox.Items.Add(newpanel);
                }
            }
            catch (NullReferenceException)
            {
                string errorString = string.Format("panel: {0} cannot find the path", panelid);
            }
            catch (FileContainerException)
            {
                string errorString = string.Format("panel: {0} 文件不存在（可能原路径文件已被删除）", panelid);
            }
            catch (InvalidOperationException)
            {
                string errorString = string.Format("panel: {0} 文件不存在（可能原路径文件已被删除）", panelid);
            }
        }
        void AddMutiPanel()
        {
            while (this.waitqueue.Count != 0)
            {
                //Thread.Sleep(2000);
                this.Invoke(new ExamManagerWorkMethod(AddOnePanel), new object[] { });
            }
        }
        private void del_button_Click(object sender, EventArgs e)
        {
            if (this.ExamDBGridView.SelectedRows.Count != 0)
            {
                this.ExamDBGridView.SelectedRows[0].Cells[6].Value = Convert.ToInt16(this.ExamDBGridView.SelectedRows[0].Cells[6].Value) == 1 ? 0 : 1;
                int RowIndex = this.ExamDBGridView.CurrentRow.Index;
                this.bdsource.EndEdit();
                refreshDataSet();
                if (Convert.ToInt16(this.ExamDBGridView.SelectedRows[0].Cells[6].Value) == 1)
                {
                    this.ExamDBGridView.CurrentCell = this.ExamDBGridView[0, RowIndex];
                }
                ButtonTextChange(sender, e);
                this.ExamDBGridView.Focus();
            }
        }
        private void refreshDataSet()
        {
            TheDataBase.Open();
            Builder.GetUpdateCommand();
            var asd = dataset.GetChanges();
            adp.Update(dataset);
            dataset.AcceptChanges();
            TheDataBase.Close();
        }
        private void dataGridViewRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex >= this.ExamDBGridView.Rows.Count)
                return;
            DataGridViewRow row = this.ExamDBGridView.Rows[e.RowIndex];
            switch (Convert.ToInt16(row.Cells[6].Value))
            {
                case 0:
                    row.DefaultCellStyle.BackColor = Color.White;
                    break;
                case 1:
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200, 221);
                    break;
                case 2:
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 189, 224, 254);
                    break;
                default:
                    break;
            }
        }
        private void Cleanbutton_Click(object sender, EventArgs e)
        {
            this.NewIdListBox.Items.Clear();
        }
        private void NewIdListBox_Click(object sender, EventArgs e)
        {
            this.ExamDBGridView.ClearSelection();
            this.AddButton.Text = "添加";
        }
        private void ExamDBGridView_MouseClick(object sender, MouseEventArgs e)
        {
            this.NewIdListBox.ClearSelected();
            ExamDBGridView_SelectChange(sender,e);
            ButtonTextChange(sender,e);
        }
        private void NewIdListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox theListBox = (ListBox)sender;
            if (theListBox.SelectedItems.Count == 1)
            {
                PanelImageContainer item = (PanelImageContainer)theListBox.SelectedItem;
                if (item.HasMajorFile)
                {
                    imageFormManager.SetImageArray(item.GetImage());
                }
                else
                {
                    string message = string.Format("panel id: {0} 的图像文件不完整或不存在，请检查原设备情况，",item.MutiString);
                    MessageBox.Show(message);
                }
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
                this.ExamDBGridView.SelectedRows[0].Cells[5].Value = this.ExamInfocomboBox.Text;
            }
            else
            {
                if (this.ExamInfocomboBox.Text != "")
                {
                    foreach (PanelImageContainer item in this.NewIdListBox.SelectedItems)
                    {
                        if (item.HasMajorFile)
                        {
                            item.Save(this.ExamInfocomboBox.Text);
                            DataRow newRow = dataset.Tables[0].NewRow();
                            newRow[1] = item.PanelId;
                            newRow[2] = this.DefectcomboBox.Text == "S" ? "S" : "F";
                            newRow[3] = defect.DefectCode;
                            newRow[4] = defect.DefectName;
                            newRow[5] = this.ExamInfocomboBox.Text;
                            newRow[6] = "2";
                            dataset.Tables[0].Rows.Add(newRow);
                        }
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
            ButtonTextChange(sender, e);
            if (this.AddButton.Text == "添加")
            {
                this.NewIdListBox.Focus();
            }
        }
        private void FilterChanged(object sender, EventArgs e)
        {
            bdsource.Filter = "info = '" + this.ExamInfocomboBox.Text + "'";
            this.ExamDBGridView.ClearSelection();
            ButtonTextChange(sender, e);
        }
        private void InfoFilterAdd(object sender, EventArgs e)
        {
            InfoList.Add(this.ExamInfocomboBox.Text);
            this.ExamInfocomboBox.DataSource = InfoList;
            FilterChanged(sender, e);
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
        private void ExplorePath(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
        private void NewIdListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.NewIdListBox.SelectedItems.Count == 1)
            {
                PanelImageContainer panel = (PanelImageContainer)this.NewIdListBox.SelectedItem;
                string path = SeverConnecter.GetPanelPathByID(panel.PanelId)[panel.PanelId].Where(x => x.PcSection == InspectSection.AVI).First().OriginImagePath;
                ExplorePath(path);
            }
        }
        private void NewIdListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                ExploreOrigin(InspectSection.AVI);
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                ExploreOrigin(InspectSection.SVI);
            }
            else if (e.Alt && e.KeyCode == Keys.A)
            {
                ExploreResult(InspectSection.AVI);
            }
            else if (e.Alt && e.KeyCode == Keys.S)
            {
                ExploreResult(InspectSection.SVI);
            }
        }
        private void ExploreOrigin(InspectSection section)
        {
            PanelImageContainer panel = (PanelImageContainer)this.NewIdListBox.SelectedItem;
            var pathlist = SeverConnecter.GetPanelPathByID(panel.PanelId)[panel.PanelId].Where(x => x.PcSection == section && x.EqId == panel.Eqid);
            if (pathlist.Count() > 0)
            {
                string path = pathlist.First().OriginImagePath;
                ExplorePath(path);
            }
            else
            {
                string message = string.Format("panel id：{0} 的 {1} Origin 文件夹不存在；", panel.PanelId, section);
                MessageBox.Show(message);
            }
        }
        private void ExploreResult(InspectSection section)
        {
            PanelImageContainer panel = (PanelImageContainer)this.NewIdListBox.SelectedItem;
            var pathlist = SeverConnecter.GetPanelPathByID(panel.PanelId)[panel.PanelId].Where(x => x.PcSection == section && x.EqId == panel.Eqid);
            if (pathlist.Count() > 0)
            {
                string path = pathlist.First().ResultPath;
                ExplorePath(path);
            }
            else
            {
                string message = string.Format("panel id：{0} 的 {1} Result 文件夹不存在；", panel.PanelId, section);
                MessageBox.Show(message);
            }
        }
        private void ButtonTextChange(object sender, EventArgs e)
        {
            this.AddButton.Text = "添加";
            if (this.ExamDBGridView.SelectedRows.Count != 0)
            {
                switch ((DelFlag)Convert.ToInt32(this.ExamDBGridView.CurrentRow.Cells[6].Value))
                {
                    case DelFlag.NORMAL:
                    case DelFlag.ADD:
                        this.DelButton.Text = "删除";
                        break;
                    case DelFlag.DELETE:
                        this.DelButton.Text = "取消删除";
                        break;
                    default:
                        break;
                }
                this.AddButton.Text = "修改";
            }
        }
        private void CommitButtonClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < this.ExamDBGridView.Rows.Count; i++)
            {
                if (Convert.ToInt16(this.ExamDBGridView.Rows[i].Cells[6].Value) == 2)
                {
                    this.ExamDBGridView.Rows[i].Cells[6].Value = 0;
                }
            }
            refreshDataSet();
        }
        private void ButtonBackColorChange(object sender, EventArgs e)
        {
            switch (this.DelButton.Text)
            {
                case "删除":
                    this.DelButton.BackColor = Color.FromArgb(255, 85, 91, 110);
                    break;
                case "取消删除":
                    this.DelButton.BackColor = Color.FromArgb(255, 250, 249, 249);
                    break;
                default:
                    break;
            }
            switch (this.AddButton.Text)
            {
                case "添加":
                    this.DelButton.BackColor = Color.FromArgb(255, 85, 91, 110);
                    break;
                case "修改":
                    this.DelButton.BackColor = Color.FromArgb(255, 250, 249, 249);
                    break;
                default:
                    break;
            }
        }
        private void ExamDBGridView_SelectChange(object sender, EventArgs e)
        {
            if (this.ExamDBGridView.SelectedRows.Count != 0 && this.ExamDBGridView.Focused)
            {
                ButtonTextChange(sender, e);
                string panelid = this.ExamDBGridView.SelectedRows[0].Cells[1].Value.ToString();
                try
                {
                    PanelImageContainer newpanel = new PanelImageContainer(panelid, this.ExamInfocomboBox.Text);
                    imageFormManager.SetImageArray(newpanel.GetImage());
                }
                catch (FileContainerException)
                {
                    string errorstring = string.Format("考试文件不存在，请检查文件夹中的内容，panel id：{0}", panelid);
                    MessageBox.Show(errorstring);
                }
            }
        }
        private void ServerRefreshbutton_Click(object sender, EventArgs e)
        {
            SeverConnecter.SendBaseMessage(MessageType.CONTROLER_REFRESH_EXAM);
        }
        private void IDList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if(e.Index >= 0)
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                Brush RectBrush = Brushes.White;
                e.Graphics.FillRectangle(RectBrush, e.Bounds);
                e.Graphics.DrawString(this.NewIdListBox.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, null);
            }
        }

        private void CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string text = Convert.ToString(this.ExamDBGridView.CurrentCell.Value);
            Clipboard.SetDataObject(text);
        }
        private void GroupDEL(object sender, EventArgs e)
        {
            // 无法完全删除，待修改
            //if (MessageBox.Show("确认删除 " + this.ExamInfocomboBox.Text + " 中的所有信息", "删除不可恢复", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    int RowCount = this.ExamDBGridView.Rows.Count;
            //    for (int i = 0; i < RowCount; i++)
            //    {
            //        int delflagValue = Convert.ToInt32(this.ExamDBGridView.Rows[i].Cells[6].Value);
            //        this.ExamDBGridView.Rows[i].Cells[6].Value = 1;
            //        delflagValue = Convert.ToInt32(this.ExamDBGridView.Rows[i].Cells[6].Value);
            //    }
            //    refreshDataSet();
            //    //DelFromDB(this.ExamInfocomboBox.Text);
            //}
        }
        private void DelFromDB(string Groupinfo,string PanelID)
        {
            string filterString = "[Info] = '"+Groupinfo + "' AND [PanelID] = '" + PanelID + "'";
            DelFromDB(filterString);
        }
        private void DelFromDB(string filterString)
        {
            int RowCount;
            string querystring = @"DELETE FROM [EDIAS_DB].[dbo].[AET_IMAGE_EXAM]
                WHERE " + filterString;
            SqlCommand newcommand = new SqlCommand(querystring, TheDataBase);
            TheDataBase.Open();
            RowCount = Convert.ToInt32(newcommand.ExecuteNonQuery());
            TheDataBase.Close();
        }
    }
}