using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;

namespace Sauron
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

        public List<string> GetInputPanelMission()
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
            commandstring = @"SELECT[EqpID],
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
        WHERE InspDate BETWEEN '20210120000000' AND '20210125230000' AND EqpID = '7CTCT27' AND OperationID = 'C52000N' AND LastJudge = 'E'";

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

        public void InsertFinishedMission(PanelMission panel)
        {
            string commandstring = string.Format(@"USE [EDIAS_DB] GO
INSERT INTO [dbo].[AET_IMAGE_INSPECT_RESULT]
           ([PanelID]
           ,[AVIOperaterID]
           ,[AVIOperaterName]
           ,[SVIOperaterID]
           ,[SVIOperaterName]
           ,[APPOperaterID]
           ,[APPOperaterName]
           ,[AviJudge]
           ,[SviJudge]
           ,[AppJudge]
           ,[LastJudge]
           ,[DefectCode]
           ,[DefectName]
           ,[MissionAddTime]
           ,[MissionFinishTime]
           ,[AllDefect]
           ,[ImageEqpID])
     VALUES
           ({0}
           ,{1}
           ,{2}
           ,{3}
           ,{4}
           ,{5}
           ,{6}
           ,{7}
           ,{8}
           ,{9}
           ,{10}
           ,{11}
           ,{12}
           ,{13}
           ,{14}
           ,{15}
           ,{16}
GO", new object[] {
                panel.PanelId,
                panel.AviOp.Id,
                panel.AviOp.Name,
                panel.SviOp.Id,
                panel.SviOp.Name,
                panel.AppOp.Id,
                panel.AppOp.Name,
                panel.AviJudge,
                panel.SviJudge,
                panel.AppJudge,
                panel.LastJudge,
                panel.DefectCode,
                panel.DefectName,
                FormateDateString(panel.AddTime),
                FormateDateString(panel.FinishTime),
                panel.AllDefect,
                panel.PanelPath.EqId,
            });
            SqlCommand newcommand = new SqlCommand(commandstring, data_base);
            data_base.Open();
            newcommand.ExecuteNonQuery();
        }
    }
}
