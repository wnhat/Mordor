using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;
using Serilog;
using System.Threading;
using System.IO;

namespace Sauron
{
    /* 
    
    */
    class MissionManager
    {
        SqlServerConnector Thesqlserver;
        FileManager Thefilecontainer;  //管理设备文件路径；
        public Queue<PanelMission> MissionQueue;
        ILogger Logger;
        public Queue<long> MissionNumberQueue; //Missionnumber是该任务在 OninspectMissionContainer 中的key，用于不同工位分时完成检查后进行结果的登录；
        Dictionary<long, PanelMission> OninspectMissionContainer; //已被添加至任务队列的Panelmission储存器；
        Queue<PanelMission> AviOnInspectMissionQueue;
        Queue<PanelMission> SviOnInspectMissionQueue;
        Queue<PanelMission> AppOnInspectMissionQueue;
        Queue<PanelMission> FinishedMissionQueue;
        List<ExamMission> ExamMissionList;

        public MissionManager()
        {
            string ip_path = @"D:\1218180\program2\c#\Mordor\Sauron\IP.json";
            IP_TR ip_tr = new IP_TR(ip_path);
            this.Thefilecontainer = new FileManager(ip_tr);
            this.Thesqlserver = new SqlServerConnector();
            Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\eye of sauron\log\missionmanager\log-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            MissionQueue = new Queue<PanelMission>();
            MissionNumberQueue = new Queue<long>();
            AviOnInspectMissionQueue = new Queue<PanelMission>();
            SviOnInspectMissionQueue = new Queue<PanelMission>();
            AppOnInspectMissionQueue = new Queue<PanelMission>();
            FinishedMissionQueue = new Queue<PanelMission>();
            OninspectMissionContainer = new Dictionary<long, PanelMission>();

            long newmissionnumber = 0;
            for (int i = 0; i < 1000000; i++)
            {
                MissionNumberQueue.Enqueue(newmissionnumber);
                newmissionnumber += 1;
            }
            RefreshExamList();
        }
        public void AddMissionByServer()
        {
            // 获取SQL server近一小时的C52000N站点近一小时E级产品添加任务；
            List<string> missionDataSet = Thesqlserver.GetInputPanelMission();
            foreach (var missionid in missionDataSet)
            {
                // TODO: 调查无法找到的id原因；新建异常类替换if语句；
                List<PanelPathContainer> pathList = Thefilecontainer.GetPanelPathList(missionid);
                if (pathList != null)
                {
                    // TODO: 当一张屏多次进入设备时返回的列表将不是单一值； 设备上无该图片时会发生错误；
                    var avipath = pathList.Where(x => x.PcSection == InspectSection.AVI).ToArray()[0];
                    var svipath = pathList.Where(x => x.PcSection == InspectSection.AVI).ToArray()[0];
                    // TODO: ADD Section app;
                    MissionQueue.Enqueue(new PanelMission(missionid, MissionType.PRODUCITVE, MissionNumberQueue.Dequeue(), avipath, svipath));
                }
                else
                {
                    Logger.Information("can not find panel path in the PathContainer, PanelId : {0}", missionid);
                }
            }
            Logger.Information("Inspect mission add finished；");
        }
        public void RefreshExamList()
        {
            List<ExamMission> newExamMissionList = new List<ExamMission>();
            var missionlist = Thesqlserver.GetExamMission();
            string[] aviExamFileList = Directory.GetDirectories("\\\\172.16.145.22\\NetworkDrive\\D_Drive\\Mordor\\ExamSimple\\AVI"); // TODO:
            string[] sviExamFileList = Directory.GetDirectories("\\\\172.16.145.22\\NetworkDrive\\D_Drive\\Mordor\\ExamSimple\\SVI"); // TODO:
            string[] aviid = new string[aviExamFileList.Length];
            for (int i = 0; i < aviExamFileList.Length; i++)
            {
                aviid[i] = aviExamFileList[i].Substring(aviExamFileList[i].Length -17);
            }
            string[] sviid = new string[sviExamFileList.Length];
            for (int i = 0; i < sviExamFileList.Length; i++)
            {
                sviid[i] = sviExamFileList[i].Substring(sviExamFileList[i].Length -17);
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
                            Logger.Error("panel ID: {0} ,do not have result file in {1}",item.PanelId); // TODO:ADD FILE path
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
                            Logger.Error("panel ID: {0} ,do not have result file in {1}",item.PanelId); // TODO:ADD FILE path
                        }
                        break;
                }
            }
            ExamMissionList = newExamMissionList;
        }
        public PanelMission GetAviMission()
        {
            if (AviOnInspectMissionQueue.Count == 0)
            {
                AddMissionInQueue();
            }
            PanelMission newpanelmission = AviOnInspectMissionQueue.Dequeue();
            return newpanelmission;
        }
        public PanelMission GetSviMission()
        {
            if (SviOnInspectMissionQueue.Count == 0)
            {
                AddMissionInQueue();
            }
            PanelMission newpanelmission = SviOnInspectMissionQueue.Dequeue();
            return newpanelmission;
        }
        public PanelMission GetAppMission()
        {
            if (AppOnInspectMissionQueue.Count == 0)
            {
                AddMissionInQueue();
            }
            PanelMission newpanelmission = AppOnInspectMissionQueue.Dequeue();
            return newpanelmission;
        }
        public List<ExamMission> GetExamMission()
        {
            var rnd = new Random();
            foreach (var item in ExamMissionList)
            {
                item.sortint = rnd.Next();
            }
            ExamMissionList.Sort();
            return ExamMissionList;
        }
        private void AddMissionInQueue()
        {
            PanelMission newmission = MissionQueue.Dequeue();
            OninspectMissionContainer.Add(newmission.MissionNumber, newmission);
            AviOnInspectMissionQueue.Enqueue(newmission);
            SviOnInspectMissionQueue.Enqueue(newmission);
            //AppOnInspectMissionQueue.Enqueue(newmission);
        }
        public void SendResult(PanelMissionResult newresult)
        {
            // Add result sended form the clint,if finished add to queue waitting for insert to database;
            PanelMission thePanelMission = OninspectMissionContainer[newresult.MissionNumber];
            thePanelMission.AddResult(newresult);
            if (thePanelMission.finished)
            {
                FinishedMissionQueue.Enqueue(thePanelMission);
                OninspectMissionContainer.Remove(thePanelMission.MissionNumber);
                MissionNumberQueue.Enqueue(thePanelMission.MissionNumber);
                Thesqlserver.InsertFinishedMission(thePanelMission);
            }
        }
        public void RefreshFileContainer()
        {
            Thefilecontainer.RefreshFileList();
        }
        public Operator CheckUser(Operator op)
        {
            var opdict = Thesqlserver.GetOperatorDict();
            if (opdict.ContainsKey(op.Id))
            {
                var newop = opdict[op.Id];
                if (newop.CheckPassWord(op.PassWord))
                {
                    return newop;
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
        public void FinishExam(List<ExamMission> missionlist)
        {
            Thesqlserver.InsertExamResult(missionlist);
        }
    }
}
