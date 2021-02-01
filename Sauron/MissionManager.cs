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
        Dictionary<long, PanelMission> TheContainer;
        public Queue<PanelMission> FinishedMissionQueue;

        public MissionManager(SqlServerConnector theSqlserver, FileManager theFileContainer)
        {
            this.Thefilecontainer = theFileContainer;
            this.Thesqlserver = theSqlserver;
            MissionQueue = new Queue<PanelMission>();
            Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\eye of sauron\log\missionmanager\log-.txt",rollingInterval:RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            long newmissionnumber = new Random().Next();
            for (int i = 0; i < 1000000; i++)
            {
                MissionNumberQueue.Enqueue(newmissionnumber);
                newmissionnumber += 1;
            }

            TheContainer = new Dictionary<long, PanelMission>();
        }

        public void AddMisionByServer()
        {
            // 获取SQL server近一小时的C52000N站点近一小时E级产品添加任务
            List<string> missionDataSet = Thesqlserver.get_oninspect_mission();
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

        public PanelMission GetMission()
        {
            PanelMission returnpanel = MissionQueue.Dequeue();
            TheContainer.Add(returnpanel.MissionNumber, returnpanel);
            return returnpanel;
        }

        public void SendResult(PanelMissionResult newresult)
        {
            // Add result sended form the clint,if finished add to queue waitting for insert to database;
            PanelMission thePanelMission = TheContainer[newresult.MissionNumber];
            thePanelMission.AddResult(newresult);
            if (thePanelMission.finished)
            {
                FinishedMissionQueue.Enqueue(thePanelMission);
                TheContainer.Remove(thePanelMission.MissionNumber);
            }
        }
    }

}
