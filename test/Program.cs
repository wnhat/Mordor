using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
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
using System.Windows.Forms;
using Newtonsoft.Json.Linq;


namespace test
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            testclass a = new testclass("zhangshaobo", 18);
            changename(ref a);
            Console.WriteLine(a);
        }
        static private void changename(ref testclass strin)
        {
            testclass b = new testclass("wangxue", 16);
            strin = b;
        }
        static private void TestDB()
        {
            SqlConnection TheDataBase = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
            string path = @"D:\1218180\program2\c#\123";
            DataSet set = new DataSet();
            string querystring = @"SELECT TOP (1000) [ID],[UserId],[PassWord],[UserName] FROM [EDIAS_DB].[dbo].[AET_IMAGE_USER]";
            SqlCommand newcommand = new SqlCommand(querystring, TheDataBase);
            TheDataBase.Open();
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = newcommand;
            SqlCommandBuilder Builder = new SqlCommandBuilder(adp);
            adp.Fill(set);
            ChangeRows(set);
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
    class testclass
    {
        public string name;
        public int age;
        public testclass(string aname, int aage)
        {
            name = aname;
            age = aage;
        }
    }
    static class Parameter
    {
        public static string SavePath;
        public static string[] AviImageNameList;
        public static string[] SviImageNameList;
        public static string[] AppImageNameList;
        public static int PreLoadQuantity;
        public static Defect[] CodeNameList;

        static Parameter()
        {
            string sysConfigPath = @"\\172.16.145.22\NetworkDrive\D_Drive\Mordor\sysconfig.json";
            FileInfo sysconfig = new FileInfo(sysConfigPath);
            if (sysconfig.Exists)
            {
                var jsonreader = new StreamReader(sysconfig.OpenRead());
                var jsonstring = jsonreader.ReadToEnd();
                JObject jsonobj = JObject.Parse(jsonstring);
                var fieldcollection = typeof(Parameter).GetFields();
                var asd = fieldcollection.ToArray();
                List<string> fieldnamelist = new List<string>();
                List<string> jsonnamelist = new List<string>();
                foreach (var item in fieldcollection)
                {
                    fieldnamelist.Add(item.Name);
                }
                foreach (var item in jsonobj)
                {
                    jsonnamelist.Add(item.Key);
                }
                if (fieldnamelist.Except(jsonnamelist).Count() != 0)
                {
                    throw new ApplicationException("系统参数多于文件记录，请检查版本对应关系");
                }
                else if (jsonnamelist.Except(fieldnamelist).Count() != 0)
                {
                    throw new ApplicationException("文件记录多于系统参数，请检查版本对应关系");
                }
                else
                {
                    foreach (var item in fieldcollection)
                    {
                        var propertyName = item.Name;
                        var value = jsonobj.GetValue(propertyName);
                        Type propertytype = item.FieldType;
                        var convertvalue = value.ToObject(propertytype);
                        item.SetValue(null,convertvalue);
                    }
                }
            }
            else
            {
                throw new ApplicationException("sysconfig.json 文件不存在，请检查与 145.22电脑的链接或相应地址是否存在设置文件；");
            }
        }
    }
}
