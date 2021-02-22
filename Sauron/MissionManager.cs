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
    class MissionManager
    {
        SqlServerConnector Thesqlserver;
        FileManager Thefilecontainer;
        public Queue<PanelMission> MissionQueue;
        ILogger Logger;
        public Queue<long> MissionNumberQueue;
        Dictionary<long, PanelMission> OninspectMissionContainer;
        Queue<PanelMission> AviOnInspectMissionQueue;
        Queue<PanelMission> SviOnInspectMissionQueue;
        Queue<PanelMission> AppOnInspectMissionQueue;
        Queue<PanelMission> FinishedMissionQueue;
        Queue<ExamMission> ExamMissionQueue;

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

            long newmissionnumber = new Random().Next();
            for (int i = 0; i < 1000000; i++)
            {
                MissionNumberQueue.Enqueue(newmissionnumber);
                newmissionnumber += 1;
            }
            ExamMissionQueue = new Queue<ExamMission>();
            RefreshExamQueue();
        }

        public void AddMissionByServer()
        {
            // 获取SQL server近一小时的C52000N站点近一小时E级产品添加任务；
            List<string> missionDataSet = Thesqlserver.GetInputPanelMission();
            foreach (var missionid in missionDataSet)
            {
                // TODO: 调查无法找到的id原因；
                List<PanelPathContainer> pathList = Thefilecontainer.GetPanelPathList(missionid);
                if (pathList != null)
                {
                    // TODO: 当一张屏多次进入设备时返回的列表将不是单一值；
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
        }
        public void RefreshExamQueue()
        {
            Queue<ExamMission> newexammissionqueue = new Queue<ExamMission>();
            var missionlist = Thesqlserver.GetExamMission();
            string[] aviexamfilelist = GetExamFileList("aviexamplefile", InspectSection.AVI); // TODO:
            string[] sviexamfilelist = GetExamFileList("sviexamplefile", InspectSection.SVI); // TODO:
            foreach (var item in missionlist)
            {
                switch (item.PcSection)
                {
                    case InspectSection.AVI:
                        if (aviexamfilelist.Contains(item.PanelId))
                        {
                            newexammissionqueue.Enqueue(new ExamMission(item.PanelId,aviexamfilelist.First(),item.PcSection));
                        }
                        break;
                    case InspectSection.SVI:
                        if (sviexamfilelist.Contains(item.PanelId))
                        {
                            newexammissionqueue.Enqueue();
                        }
                        break;
                }
            }
        }
        string[] GetExamFileList(string path, InspectSection section)
        {
            string[] image_directory_list = Directory.GetDirectories(path);
            return image_directory_list;
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
    }
}
