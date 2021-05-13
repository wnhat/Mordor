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
        LoginForm TheParentForm;
        Defectcode defect_translator;
        Manager TheManager;
        int idNum = 0;
        List<PanelInfo> idlist;
        int Refreshflag;
        Bitmap[] ImageArray;

        public examManageForm()
        {
            InitializeComponent();
        }
        public examManageForm(LoginForm parentForm, Manager theManager)
        {
            InitializeComponent();
            TheParentForm = parentForm;
            TheManager = theManager;
            defect_translator = new Defectcode(Parameter.CodeNameList);
            idlist = new List<PanelInfo>();
            AddDefectCode();
        }
        private void commitButton_Click(object sender, EventArgs e)
        {
            //TODO: 1、保存内存中图像图片 ；
            //      2、更新数据库；
        }
        public void SetImage(Bitmap[] imagearray)
        {
            pictureBox1.Image = imagearray[0];
            pictureBox2.Image = imagearray[1];
            pictureBox3.Image = imagearray[2];
        }
        public void RefreshForm()
        {
            if ((Refreshflag) * 3 < ImageArray.Count())
            {
                SetImage(ImageArray.Skip((Refreshflag) * 3).Take(3).ToArray());
                Refreshflag++;
            }
            else
            {
                Refreshflag = 0;
                RefreshForm();
            }
        }
        private void AddDefectCode()
        {
            foreach (var item in Parameter.CodeNameList)
            {
                this.DefectcomboBox.Items.Add(item.DefectName);
            }
        }
        private void imageViewButton_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Image = EyeOfSauron.Properties.Resources.emptyImage;
            this.pictureBox2.Image = EyeOfSauron.Properties.Resources.emptyImage;
            this.pictureBox3.Image = EyeOfSauron.Properties.Resources.emptyImage;
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

