using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Container;
using System.Data;
using System.Data.SqlClient;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection TheDataBase = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
            string path = @"D:\1218180\program2\c#\123";
            DataSet set = new DataSet();
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
            adp.Fill(set);
            ChangeRows(set);
            //var change = set.GetChanges();
            //AddRows(set);
            //var add = set.GetChanges();
            DeleteRows(set);
            var del = set.GetChanges();
            var cmd = Builder.GetUpdateCommand();
            adp.Update(del);
        }

        static private void ChangeRows(DataSet dataSet)
        {
            // For each table in the DataSet, print the row values.
            foreach (DataTable table in dataSet.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    var asd = row["UserName"].ToString();
                    Console.WriteLine(asd);
                    if (asd == "张少博")
                    {
                        row["Password"] = "1218180";
                    }
                }
                
            }
        }
        static private void AddRows(DataSet dataSet)
        {
            DataRow newrow = dataSet.Tables[0].NewRow();
            newrow["UserName"] = "王老师";
            newrow["Password"] = "1995";
            newrow["UserId"] = "74585";
            dataSet.Tables[0].Rows.Add(newrow);
        }
        static private void DeleteRows(DataSet dataSet)
        {
            // For each table in the DataSet, print the row values.
            foreach (DataTable table in dataSet.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    var asd = row["UserName"].ToString();
                    Console.WriteLine(asd);
                    if (asd == "王老师")
                    {
                        row.Delete();
                    }
                }

            }
        }
    }

}
