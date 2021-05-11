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
using Container;
namespace ExamManager
{
    public partial class examManageForm : Form
    {
        int idNum = 0;
        public examManageForm()
        {
            InitializeComponent();
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
            ArrayList list = new ArrayList();
            for (int i=0; i<this.idTextBox.Lines.Length; i++)
            {
                list.Add(idTextBox.Lines[i]);
            }
            //TODO:验证ID对应图片是否存在
            //TODO:获取图片并上传到数据库
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void fileSelectButton_Click(object sender, EventArgs e)
        {
            this.idTextBox.Text = "ID Input";
        }
        private void imageViewButton_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Image = ExamManager.Properties.Resources.emptyImage;
            this.pictureBox2.Image = ExamManager.Properties.Resources.emptyImage;
            this.pictureBox3.Image = ExamManager.Properties.Resources.emptyImage;
            if (idNum < this.idTextBox.Lines.Length)
            {
                this.imageIDTextBox.Text = this.idTextBox.Lines[idNum];
                idNum++;
                List<PanelInfo> panelIdList = new List<PanelInfo>();
                PanelInfo panelInfo;
                string panelID = this.imageIDTextBox.Text;
                Regex regex = new Regex(@"^7[A-Za-z0-9]{16}$");
                if (regex.IsMatch(panelID))
                {
                    panelInfo = new PanelInfo(panelID, InspectSection.AVI);
                    panelIdList.Add(panelInfo);
                    Serverconnecter.GetPanelInfoByID(panelIdList);
                    string imagePath = "";



                    /*  
                    if (idNum < this.idTextBox.Lines.Length)
                    {
                        this.imageIDTextBox.Text = this.idTextBox.Lines[idNum];
                        idNum++;
                        string panelID = this.imageIDTextBox.Text;
                        Regex regex = new Regex(@"^7[A-Za-z0-9]{16}$");
                        if (regex.IsMatch(panelID))
                        {
                            string imagePath = "";
                            string connStr = "server = 172.16.150.100; uid = sa; pwd = 1qaz@WSX; database = AET_IMAGE_URL";
                            SqlConnection conn = new SqlConnection(connStr);
                            String sqlString = "SELECT [ImageURL] FROM dbo.AET_IMAGE_URL_AVI WHERE VcrID = '" + panelID + "'";
                            try
                            {
                                conn.Open();
                                SqlDataReader dr = null;
                                SqlCommand sc = new SqlCommand(sqlString, conn);
                                dr = sc.ExecuteReader();
                                if (dr.Read())
                                {
                                    imagePath = dr[0].ToString().Replace("Origin", "Result") + @"MNImg\";
                                    if (File.Exists(imagePath + "04_WHITE_Pre-Input.jpg"))
                                    {
                                        this.pictureBox1.Image = Image.FromFile(imagePath + "04_WHITE_Pre-Input.jpg");
                                    }
                                    else
                                    {
                                        this.pictureBox1.Image = ExamManager.Properties.Resources.emptyImage;
                                    }
                                    if (File.Exists(imagePath + "06_G64_Pre-Input.jpg"))
                                    {
                                        this.pictureBox2.Image = Image.FromFile(imagePath + "06_G64_Pre-Input.jpg");
                                    }
                                    else
                                    {
                                        this.pictureBox2.Image = ExamManager.Properties.Resources.emptyImage;
                                    }
                                    if (File.Exists(imagePath + "08_G64-2_Pre-Input.jpg"))
                                    {
                                        this.pictureBox3.Image = Image.FromFile(imagePath + "08_G64-2_Pre-Input.jpg");
                                    }
                                    else
                                    {
                                        this.pictureBox3.Image = ExamManager.Properties.Resources.emptyImage;
                                    }
                              }
                                else
                                {
                                    this.pictureBox1.Image = ExamManager.Properties.Resources.emptyImage;
                                    this.pictureBox2.Image = ExamManager.Properties.Resources.emptyImage;
                                    this.pictureBox3.Image = ExamManager.Properties.Resources.emptyImage;
                                }


                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                if (conn != null)
                                {
                                    conn.Close();
                                }
                            }
                        }
                        else
                        {
                            imageIDTextBox.Text = "ID Illegal";
                            this.pictureBox1.Image = ExamManager.Properties.Resources.emptyImage;
                            this.pictureBox2.Image = ExamManager.Properties.Resources.emptyImage;
                            this.pictureBox3.Image = ExamManager.Properties.Resources.emptyImage;
                        }
                    }
                    else
                    {
                        imageIDTextBox.Text = "ID Empty";
                        this.pictureBox1.Image = ExamManager.Properties.Resources.emptyImage;
                        this.pictureBox2.Image = ExamManager.Properties.Resources.emptyImage;
                        this.pictureBox3.Image = ExamManager.Properties.Resources.emptyImage;
                    }
                    */
                }

        private void upToTopButton_Click(object sender, EventArgs e)
        {
            idNum = 0;
            this.imageIDTextBox.Text = this.idTextBox.Lines[idNum];
        }
    }
}
