using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container.SqlConnector
{
    public static class SqlServerConnector
    {
        static SqlConnection TheDataBase;
        static SqlServerConnector()
        {
            TheDataBase = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
        }
        /* 从服务器中获取近一小时N站点生产的cell数据 */
        public static Dictionary<string,string> GetInputPanelMission(string[] IdList)
        {
            string commandstring = string.Format(@"SELECT [VcrID]
                                    ,[MergeToolJudge]
            FROM[EDIAS_DB].[dbo].[TAX_PRODUCT_TEST]
            WHERE VcrID in ({0}})",IdList2String(IdList));
            SqlCommand newcommand = new SqlCommand(commandstring, TheDataBase);
            TheDataBase.Open();
            SqlDataReader newDataReader = newcommand.ExecuteReader();
            Dictionary<string,string> dict = new Dictionary<string, string>();
            if (newDataReader.HasRows)
            {
                while (newDataReader.Read())
                {
                    dict.Add(newDataReader["VcrID"].ToString(), newDataReader["MergeToolJudge"].ToString());
                }
            }
            TheDataBase.Close();
            return dict;
        }
        static string IdList2String(string[] IdList)
        {
            bool Fisrt = true;
            string returnstring = "";
            foreach (var item in IdList)
            {
                if (Fisrt)
                {
                    returnstring = returnstring + item;
                }
                else
                {
                    returnstring = returnstring + "," + item;
                }
            }
            return returnstring;
        }

    }
}
