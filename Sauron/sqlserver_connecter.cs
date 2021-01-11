using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class SqlServerConnector
    {
        SqlConnection data_base;
        DateTime last_date;

        public SqlServerConnector()
        {
            data_base = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
            //last_date = new DateTime();
            last_date = DateTime.Now.AddHours(-1);
        }

        public void last_date_add()
        {
            last_date = last_date.AddHours(1);
        }

        string get_date_span_sqlstring_last()
        {
            return FormateDateString(last_date);
        }

        string get_date_span_sqlstring_now()
        {
            return FormateDateString(last_date.AddHours(1));
        }

        string FormateDateString(DateTime thedate)
        {
            return thedate.ToString("yyyyMMddHH0000");
        }

        DataSet get_input_panel(string commandstring)
        {
            SqlCommand newcommand = new SqlCommand(commandstring, data_base);
            SqlDataAdapter selectadapter = new SqlDataAdapter();
            selectadapter.SelectCommand = newcommand;
            DataSet newdata = new DataSet();
            data_base.Open();
            selectadapter.SelectCommand.ExecuteNonQuery();
            selectadapter.Fill(newdata);
            data_base.Close();
            return newdata;
        }

        List<string> GetInputPanelMession(string commandstring)
        {
            List<string> newPanelList = new List<string>();
            SqlCommand newcommand = new SqlCommand(commandstring, data_base);
            data_base.Open();
            SqlDataReader newDataReader = newcommand.ExecuteReader();
            if (newDataReader.HasRows)
            {
                while (newDataReader.Read())
                {
                    newPanelList.Add(newDataReader["VcrID"].ToString());
                }
            }
            data_base.Close();
            return newPanelList;

        }

        public List<string> get_oninspect_mission()
        {
            string commandstring = string.Format(@"SELECT[EqpID],
                                    [InspDate]
                                    ,[ModelID]
                                    ,[InnerID]
                                    ,[VcrID]
                                    ,[MviUser]
                                    ,[LastResult]
                                    ,[LastJudge]
                                    ,[DbInTime]
                                    ,[OperationID]
                                    ,[StageID]
                                    ,[LastResultName]
                                    ,[ProductType]
                                    ,[MergeToolJudge]
                                    ,[DefectName]
                                    FROM[EDIAS_DB].[dbo].[TAX_PRODUCT_TEST]
                                    WHERE InspDate BETWEEN '{0}' AND '{1}'
                                    AND OperationID = 'C52000N' AND LastJudge = 'E'", 
                                    get_date_span_sqlstring_last(), get_date_span_sqlstring_now());
            List<string> newDataString = GetInputPanelMession(commandstring);
            return newDataString;
        }
    }
}
