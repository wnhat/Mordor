using Container;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron
{
    /* 
    
     */
    class ExamMissionManager
    {
        SqlConnection TheDataBase;
        public List<ExamMission> ExamMissionList;
        SqlDataAdapter SqlAdapter;
        SqlCommandBuilder CMDBuilder;
        DataSet ExamList;                           // 用于存放数据库中表 AET_IMAGE_EXAM 的副本
        string[] PrimaryFileNameList;
        public ExamMissionManager(SqlConnection theDataBaseConnection)
        {
            TheDataBase = theDataBaseConnection;
            ExamMissionList = new List<ExamMission>();
            TheDataBase.Open();
            SqlAdapter = new SqlDataAdapter();
            ExamList = new DataSet();
            SqlCommand selectcommand = new SqlCommand(SelectString, TheDataBase);
            SqlAdapter.SelectCommand = selectcommand;
            SqlAdapter.Fill(ExamList);
            CMDBuilder = new SqlCommandBuilder(SqlAdapter);
            TheDataBase.Close();
        }
        public void UpdateExamDB(DataSet changedDataSet)
        {
            TheDataBase.Open();
            CMDBuilder.GetUpdateCommand();
            SqlAdapter.Update(ExamList);
            TheDataBase.Close();
        }
        private string SelectString
        {
            get{return @"SELECT ALL [PanelID],[Judge],[DefectCode],[DefectName],[Section] FROM [EDIAS_DB].[dbo].[AET_IMAGE_EXAM]";}
        }
        public DataSet GetExamDB()
        {
            return ExamList;
        }
        public void RefreshExamList()
        {
            TheDataBase.Open();
            SqlAdapter.Fill(ExamList);
            TheDataBase.Close();
        }
        public void RefreshExamMissionList()
        {
            List<ExamMission> newExamMissionList = new List<ExamMission>();
            var missionlist = Thesqlserver.GetExamMission();
            string[] aviExamFileList = Directory.GetDirectories("\\\\172.16.145.22\\NetworkDrive\\D_Drive\\Mordor\\ExamSimple\\AVI");
            string[] sviExamFileList = Directory.GetDirectories("\\\\172.16.145.22\\NetworkDrive\\D_Drive\\Mordor\\ExamSimple\\SVI");
            // TODO: 验证每个文件夹中必要文件完整性；
            string[] aviid = new string[aviExamFileList.Length];
            for (int i = 0; i < aviExamFileList.Length; i++)
            {
                aviid[i] = aviExamFileList[i].Substring(aviExamFileList[i].Length - 17);
            }
            string[] sviid = new string[sviExamFileList.Length];
            for (int i = 0; i < sviExamFileList.Length; i++)
            {
                sviid[i] = sviExamFileList[i].Substring(sviExamFileList[i].Length - 17);
            }
            foreach (var item in missionlist)
            {
                switch (item.PcSection)
                {
                    case InspectSection.AVI:
                        if (aviid.Contains(item.PanelId))
                        {
                            var filepath = aviExamFileList.Where(x => x.Substring(x.Length - 17) == item.PanelId).First();
                            newExamMissionList.Add(new ExamMission(item.PanelId, filepath, item.PcSection, item.Defect, item.Judge));
                        }
                        else
                        {
                            Logger.Error("panel ID: {0} ,do not have result file in {1}", item.PanelId); // TODO:ADD FILE path
                        }
                        break;
                    case InspectSection.SVI:
                        if (sviid.Contains(item.PanelId))
                        {
                            var filepath = sviExamFileList.Where(x => x.Substring(x.Length - 17) == item.PanelId).First();
                            newExamMissionList.Add(new ExamMission(item.PanelId, filepath, item.PcSection, item.Defect, item.Judge));
                        }
                        else
                        {
                            Logger.Error("panel ID: {0} ,do not have result file in {1}", item.PanelId); // TODO:ADD FILE path
                        }
                        break;
                }
            }
            ExamMissionList = newExamMissionList;
        }
        public List<ExamMission> GetMission(string missionInfo)
        {
            
        }
        public bool CheckPrimaryFileExist(string filepath)
        {
            DirContainer file = new DirContainer(filepath);
        }
    }
}
