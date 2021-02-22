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
        SqlConnection TheDataBase;
        DateTime LastDate;
        public SqlServerConnector()
        {
            TheDataBase = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
            //last_date = new DateTime();
            LastDate = DateTime.Now.AddHours(-2);
        }
        string FormateDateString(DateTime thedate)
        {
            return thedate.ToString("yyyyMMddHH0000");
        }
        public List<string> GetInputPanelMission()
        {
            string datestart = FormateDateString(LastDate);
            LastDate = LastDate.AddHours(1);
            string dateend = FormateDateString(LastDate);
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
                                    datestart, dateend);
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
            SqlCommand newcommand = new SqlCommand(commandstring, TheDataBase);
            TheDataBase.Open();
            SqlDataReader newDataReader = newcommand.ExecuteReader();
            if (newDataReader.HasRows)
            {
                while (newDataReader.Read())
                {
                    newPanelList.Add(newDataReader["VcrID"].ToString());
                }
            }
            TheDataBase.Close();
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
                panel.AviPanelPath.EqId,
            });
            SqlCommand newcommand = new SqlCommand(commandstring, TheDataBase);
            TheDataBase.Open();
            newcommand.ExecuteNonQuery();
        }
        public List<ExamMission> GetExamMission()
        {
            string commandstring = string.Format(@"SELECT[EqpID],");
            List<ExamMission> newPanelList = new List<ExamMission>();
            SqlCommand newcommand = new SqlCommand(commandstring, TheDataBase);
            TheDataBase.Open();
            SqlDataReader newDataReader = newcommand.ExecuteReader();
            if (newDataReader.HasRows)
            {
                while (newDataReader.Read())
                {
                    newPanelList.Add(new ExamMission());
                }
            }
            TheDataBase.Close();
            return newPanelList;
        }
    }
}
