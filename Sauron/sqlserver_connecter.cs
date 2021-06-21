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
        string[] ModelId = new string[] { "606_AUTO_ET_L5", "606_AUTO_ET_B11" };
        public SqlServerConnector()
        {
            TheDataBase = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
            LastDate = DateTime.Now.AddHours(-2);
        }
        string FormateDateString(DateTime thedate)
        {
            return thedate.ToString("yyyyMMddHH0000");
        }
        /* 从服务器中获取近一小时N站点生产的cell数据 */
        public List<string[]> GetInputPanelMission()
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
            AND OperationID = 'C52000N' 
            AND LastJudge = 'E'", datestart, dateend);
            List<string[]> newPanelList = new List<string[]>();
            SqlCommand newcommand = new SqlCommand(commandstring, TheDataBase);
            TheDataBase.Open();
            SqlDataReader newDataReader = newcommand.ExecuteReader();
            if (newDataReader.HasRows)
            {
                while (newDataReader.Read())
                {
                    if (ModelId.Contains(newDataReader["ModelID"].ToString()))
                    {
                        newPanelList.Add(new string[] { newDataReader["VcrID"].ToString(), newDataReader["EqpID"].ToString() });
                    }
                }
            }
            TheDataBase.Close();
            return newPanelList;
        }
        public void InsertFinishedMission(PanelMission[] panellist)
        {
            TheDataBase.Open();
            foreach (var panel in panellist)
            {
                string commandstring = string.Format(@"INSERT INTO [dbo].[AET_IMAGE_INSPECT_RESULT]
           ([PanelID]
      ,[OperaterID]
      ,[OperaterName]
      ,[Judge]
      ,[DefectCode]
      ,[DefectName]
      ,[MissionAddTime]
      ,[MissionFinishTime]
      ,[AllDefect]
      ,[ImageEqpID])
     VALUES
           ('{0}'
           ,'{1}'
           ,N'{2}'
           ,'{3}'
           ,'{4}'
           ,N'{5}'
           ,'{6}'
           ,'{7}'
           ,N'{8}'
           ,'{9}')"
                , new object[] {
                panel.PanelId,
                panel.Op.Id,
                panel.Op.Name,
                panel.LastJudge,
                panel.DefectCode,
                panel.DefectName,
                FormateDateString(panel.AddTime),
                FormateDateString(panel.FinishTime),
                panel.DefectName,
                panel.AviPanelPath.EqId,
            });
                SqlCommand newcommand = new SqlCommand(commandstring, TheDataBase);
                newcommand.ExecuteNonQuery();
            }
            TheDataBase.Close();
        }
        /* 
        从服务器中获取考试任务
         */
        public List<ExamMission> GetExamMission()
        {
            string commandstring = string.Format(@"SELECT ALL [PanelID]
      ,[Judge]
      ,[DefectCode]
      ,[DefectName]
      ,[Info]
  FROM [EDIAS_DB].[dbo].[AET_IMAGE_EXAM]");
            List<ExamMission> newPanelList = new List<ExamMission>();
            SqlCommand newcommand = new SqlCommand(commandstring, TheDataBase);
            TheDataBase.Open();
            SqlDataReader newDataReader = newcommand.ExecuteReader();
            if (newDataReader.HasRows)
            {
                while (newDataReader.Read())
                {
                    string panelid = newDataReader["PanelId"].ToString();
                    string info = newDataReader["Info"].ToString();
                    Defect newdefect = new Defect(newDataReader["DefectName"].ToString(), newDataReader["DefectCode"].ToString(),InspectSection.EXAM);
                    JudgeGrade newjudge = (JudgeGrade)Enum.Parse(typeof(JudgeGrade), newDataReader["Judge"].ToString(), true);
                    newPanelList.Add(new ExamMission(panelid,InspectSection.EXAM, newdefect, newjudge, info));
                }
            }
            TheDataBase.Close();
            return newPanelList;
        }
        /* 从服务器获取用户工号、姓名； */
        public Dictionary<string, Operator> GetOperatorDict()
        {
            string commandstring = string.Format(@"SELECT ALL [UserId]
                    ,[PassWord]
                    ,[UserName]
                FROM [EDIAS_DB].[dbo].[AET_IMAGE_USER]");
            SqlCommand newcommand = new SqlCommand(commandstring, TheDataBase);
            TheDataBase.Open();
            SqlDataReader newDataReader = newcommand.ExecuteReader();
            Dictionary<string, Operator> newdict = new Dictionary<string, Operator>();
            if (newDataReader.HasRows)
            {
                while (newDataReader.Read())
                {
                    string userid = newDataReader["UserId"].ToString();
                    string password = newDataReader["PassWord"].ToString();
                    string username = newDataReader["UserName"].ToString();
                    newdict.Add(userid, new Operator(password, username, userid));
                }
            }
            TheDataBase.Close();
            return newdict;
        }
        public void InsertExamResult(ExamMission mission)
        {
            string commandstring = string.Format(@"INSERT INTO [dbo].[AET_IMAGE_EXAM_RESULT]
         VALUES
               ('{0}'
           ,'{1}'
           ,'{2}'
           ,N'{3}'
           ,'{4}'
           ,'{5}'
           ,N'{6}'
           ,'{7}'
           ,'{8}'
           ,N'{9}'
           ,'{10}'
)",
               new object[]
               {
                   mission.PanelId,
                   mission.Judge,
                   mission.Defect.DefectCode,
                   mission.Defect.DefectName,
                   mission.PcSection,
                   mission.Op.Id,
                   mission.Op.Name,
                   mission.JudgeU,
                   mission.DefectU.DefectCode,
                   mission.DefectU.DefectName,
                   mission.FinishTime.ToString()
               });
            SqlCommand newcommand = new SqlCommand(commandstring, TheDataBase);
            TheDataBase.Open();
            newcommand.ExecuteNonQuery();
            TheDataBase.Close();
        }
        public void InsertExamResult(List<ExamMission> missionlist)
        {
            foreach (var item in missionlist)
            {
                InsertExamResult(item);
            }
        }
    }
}
