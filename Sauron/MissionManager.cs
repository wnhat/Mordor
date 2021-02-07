using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;
using Serilog;
using System.Threading;

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

        public MissionManager(SqlServerConnector theSqlserver, FileManager theFileContainer)
        {
            this.Thefilecontainer = theFileContainer;
            this.Thesqlserver = theSqlserver;
            Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\eye of sauron\log\missionmanager\log-.txt",rollingInterval:RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            MissionQueue = new Queue<PanelMission>();
            MissionNumberQueue = new Queue<long>();
            AviOnInspectMissionQueue = new Queue<PanelMission>();
            SviOnInspectMissionQueue = new Queue<PanelMission>();
            AppOnInspectMissionQueue = new Queue<PanelMission>();
            FinishedMissionQueue = new Queue<PanelMission>();

            long newmissionnumber = new Random().Next();
            for (int i = 0; i < 1000000; i++)
            {
                MissionNumberQueue.Enqueue(newmissionnumber);
                newmissionnumber += 1;
            }

            OninspectMissionContainer = new Dictionary<long, PanelMission>();
        }

        public void AddMisionByServer()
        {
            // 获取SQL server近一小时的C52000N站点近一小时E级产品添加任务
            List<string> missionDataSet = Thesqlserver.GetInputPanelMission();
            foreach (var missionid in missionDataSet)
            {
                // TODO: 当一张屏多次进入设备时返回的列表将不是单一值；
                // TODO: 调查无法找到的id原因；
                var mission = Thefilecontainer.GetPanel(missionid);
                if (mission != null)
                {
                    MissionQueue.Enqueue(new PanelMission(missionid, MissionType.PRODUCITVE, mission,MissionNumberQueue.Dequeue()));
                }
                else
                {
                    Logger.Information("can not find panel path in the PathContainer, PanelId : {0}", missionid);
                }
            }
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

    }
}
