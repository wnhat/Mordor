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
        DataSet dataset;
        BindingSource bdsource;
        public Form1()
        {
            dataset = new DataSet();
            data();
            bdsource = new BindingSource();
            bdsource.DataSource = dataset.Tables[0];
            InitializeComponent();
            this.dataGridView1.DataSource = bdsource;
        }
        private void data()
        {
            SqlConnection TheDataBase = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
            string path = @"D:\1218180\program2\c#\123";
            string querystring = @"SELECT TOP (1000) [ID]
      ,[UserId]
      ,[PassWord]
      ,[UserName]
  FROM [EDIAS_DB].[dbo].[AET_IMAGE_USER]";
            SqlCommand newcommand = new SqlCommand(querystring, TheDataBase);
            TheDataBase.Open();
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = newcommand;
            SqlCommandBuilder Builder = new SqlCommandBuilder(adp);
            adp.Fill(dataset);
            
        }
    }
}
