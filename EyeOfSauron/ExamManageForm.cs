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
        DataSet dataset;
        BindingSource bdsource;
        LoginForm TheParentForm;
        Defectcode defect_translator;
        Manager TheManager;
        int idNum = 0;
        List<string> idlist;

        public examManageForm()
        {
            dataset = new DataSet();
            data();
            bdsource = new BindingSource();
            bdsource.DataSource = dataset.Tables[0];
            InitializeComponent();
            this.ExamDBGridView.DataSource = bdsource;
        }
        public examManageForm(LoginForm parentForm, Manager theManager)
        {
            dataset = new DataSet();
            data();
            bdsource = new BindingSource();
            bdsource.DataSource = dataset.Tables[0];
            InitializeComponent();
            this.ExamDBGridView.DataSource = bdsource;
            TheParentForm = parentForm;
            TheManager = theManager;
            defect_translator = new Defectcode(TheManager.SystemParameter.CodeNameList);
            idlist = new List<string>();
        }
        private void data()
        {
            SqlConnection TheDataBase = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
            string path = @"D:\1218180\program2\c#\123";
            string querystring = @"SELECT [ID],[PanelID]
      ,[Judge]
      ,[DefectCode]
      ,[DefectName]
      ,[Section]
      ,[Info]
  FROM [EDIAS_DB].[dbo].[AET_IMAGE_EXAM]";
            SqlCommand newcommand = new SqlCommand(querystring, TheDataBase);
            TheDataBase.Open();
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = newcommand;
            SqlCommandBuilder Builder = new SqlCommandBuilder(adp);
            adp.Fill(dataset);
            TheDataBase.Close();

        }
        public static bool chcekIsTextFile(string fileName)
        {
            FileStream fs = new FileStream(fileName,FileMode.Open, FileAccess.Read);
            bool isTextFile = true;
            byte data;
            int i = 0;
            int length = (int)fs.Length;
            try
            {
                while(i<length && isTextFile)
                {
                    data = (byte)fs.ReadByte();
                    isTextFile = (data != 0);
                    i++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null);
                {
                    fs.Close();
                }
            }
            return isTextFile;
        }
        private void commitButton_Click(object sender, EventArgs e)
        {
            //ArrayList list = new ArrayList();
            //for (int i=0; i<this.idTextBox.Lines.Length; i++)
            //{
            //    list.Add(idTextBox.Lines[i]);
            //}
            //TODO:验证ID对应图片是否存在
            //TODO:获取图片并上传到数据库
        }
        private void fileSelectButton_Click(object sender, EventArgs e)
        {
            //this.idTextBox.Text = "ID Input";
            idNum = 0;
        }
        private void imageViewButton_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Image = EyeOfSauron.Properties.Resources.emptyImage;
            this.pictureBox2.Image = EyeOfSauron.Properties.Resources.emptyImage;
            this.pictureBox3.Image = EyeOfSauron.Properties.Resources.emptyImage;
        }
        private void upToTopButton_Click(object sender, EventArgs e)
        {
            idNum = 0;
            //.imageIDTextBox.Text = this.idTextBox.Lines[idNum];
        }

        private void AddPanelIdbutton_Click(object sender, EventArgs e)
        {
            PanelIdAddForm idform = new PanelIdAddForm();
            idform.BindIdArray(idlist);
            idform.ShowDialog();
            AddPanelId();
        }
        private void AddPanelId()
        {
            // 将 ID array 中的id添加任务；预加载图片及添加至newidlistbox中；
            foreach (var item in idlist)
            {
                //TODO:
            }
        }
    }
    class SeverConnecter
    {

        private RequestSocket request;
        Parameter SystemParameter;
        public SeverConnecter(Parameter systemParameter)
        {
            request = new RequestSocket();
            request.Connect("tcp://172.16.145.22:5555");
            SystemParameter = systemParameter;
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

