using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        SqlConnection TheDataBase = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
        SqlDataAdapter adp = new SqlDataAdapter();
        DataTable dataset;
        BindingSource bdsource;
        public Form1()
        {
            TheDataBase.Open();
            dataset = new DataTable();
            data();
            bdsource = new BindingSource();
            bdsource.DataSource = dataset;
            InitializeComponent();
            this.dataGridView1.DataSource = bdsource;
            TheDataBase.Close();
        }
        private void data()
        {
            string querystring = @"SELECT TOP (1000) [ID]
      ,[UserId]
      ,[PassWord]
      ,[UserName]
  FROM [EDIAS_DB].[dbo].[AET_IMAGE_USER]";

            SqlCommand newcommand = new SqlCommand(querystring, TheDataBase);
            adp.SelectCommand = newcommand;
            SqlCommandBuilder Builder = new SqlCommandBuilder(adp);
            adp.Fill(dataset.Rows.Count, Int16.MaxValue, dataset);
        }
        void insertdata()
        {
            string insertstring = @"INSERT INTO [dbo].[AET_IMAGE_USER] VALUES ('111111', '000000',N'哪吒')";
            SqlCommand newcommand = new SqlCommand(insertstring, TheDataBase);
            newcommand.ExecuteNonQuery();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            TheDataBase.Open();
            insertdata();
            data();
            TheDataBase.Close();
        }
    }
}