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
        public examManageForm()
        {
            InitializeComponent();
        }
        public examManageForm(LoginForm parentForm, Manager theManager)
        {
            InitializeComponent();
            TheParentForm = parentForm;
            TheManager = theManager;
            defect_translator = new Defectcode(TheManager.SystemParameter.CodeNameList);
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
        private void Form1_Load(object sender, EventArgs e)
        {

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
        //    if (idNum < this.idTextBox.Lines.Length)
        //    {
        //        this.imageIDTextBox.Text = this.idTextBox.Lines[idNum];
        //        idNum++;
        //        List<PanelInfo> panelIdList = new List<PanelInfo>();
        //        PanelInfo panelInfo;
        //        string panelID = this.imageIDTextBox.Text;
        //        this.Refresh();
        //        Regex regex = new Regex(@"^7[A-Za-z0-9]{16}$");
        //        if (regex.IsMatch(panelID))
        //        {
        //            //Sauron版
        //            panelInfo = new PanelInfo(panelID, InspectSection.AVI);
        //            panelIdList.Add(panelInfo);
        //            panelInfo = TheManager.GetPanelInfo(panelIdList).Dequeue();
        //            string imagePath = panelInfo.Image_path + @"\MNImg\";
                     
        //            //string imagePath = "";
        //            //string connStr = "server = 172.16.150.100; uid = sa; pwd = 1qaz@WSX; database = AET_IMAGE_URL";
        //            //SqlConnection conn = new SqlConnection(connStr);
        //            //String sqlString = "SELECT [ImageURL] FROM dbo.AET_IMAGE_URL_AVI WHERE VcrID = '" + panelID + "'";
        //            try
        //            {
        //                //conn.Open();
        //                //SqlDataReader dr = null;
        //                //SqlCommand sc = new SqlCommand(sqlString, conn);
        //                //dr = sc.ExecuteReader();
        //                //if (dr.Read())
        //                //{
        //                    //imagePath = dr[0].ToString().Replace("Origin", "Result") + @"MNImg\";
        //                    if (File.Exists(imagePath + "04_WHITE_Pre-Input.jpg"))
        //                    {
        //                        this.pictureBox1.Image = Image.FromFile(imagePath + "04_WHITE_Pre-Input.jpg");
        //                    }
        //                    else
        //                    {
        //                        this.pictureBox1.Image = EyeOfSauron.Properties.Resources.emptyImage;
        //                    }
        //                    if (File.Exists(imagePath + "06_G64_Pre-Input.jpg"))
        //                    {
        //                        this.pictureBox2.Image = Image.FromFile(imagePath + "06_G64_Pre-Input.jpg");
        //                    }
        //                    else
        //                    {
        //                        this.pictureBox2.Image = EyeOfSauron.Properties.Resources.emptyImage;
        //                    }
        //                    if (File.Exists(imagePath + "08_G64-2_Pre-Input.jpg"))
        //                    {
        //                        this.pictureBox3.Image = Image.FromFile(imagePath + "08_G64-2_Pre-Input.jpg");
        //                    }
        //                    else
        //                    {
        //                        this.pictureBox3.Image = EyeOfSauron.Properties.Resources.emptyImage;
        //                    }
        //                //}
                            
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                /*
        //                if (conn != null)
        //                {
        //                    conn.Close();
        //                }
        //                */
        //            }
        //        }
        //        else
        //        {
        //            //imageIDTextBox.Text = "ID Illegal";
        //        }
        //    }
        //    else
        //    {
        //        //imageIDTextBox.Text = "ID Empty";
        //    }
        }
        private void upToTopButton_Click(object sender, EventArgs e)
        {
            idNum = 0;
            ///*this*/.imageIDTextBox.Text = this.idTextBox.Lines[idNum];
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

