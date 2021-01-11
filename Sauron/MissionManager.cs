using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class MissionManager
    {
        SqlServerConnector Thesqlserver;
        FileManager Thefilecontainer;
        public Queue<PanelMission> MissionQueue;
        List<string> logger;

        public MissionManager(SqlServerConnector theSqlserver, FileManager theFileContainer)
        {
            this.Thefilecontainer = theFileContainer;
            this.Thesqlserver = theSqlserver;
            MissionQueue = new Queue<PanelMission>();
            logger = new List<string>();
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
                    MissionQueue.Enqueue(new PanelMission(missionid, MissionType.PRODUCITVE, mission));
                }
                else
                {
                    logger.Add(String.Format("can not find panel path in the PathContainer, PanelId : {0}", missionid));
                }
            }
        }

        public PanelMission GetMission()
        {
            PanelMission returnpanel = MissionQueue.Dequeue();
            return returnpanel;
        }
    }

    class MissionDict
    {
        Dictionary<long, PanelMission> TheContainer = new Dictionary<long, PanelMission>();

        public void Add(PanelMission newMission)
        {
            TheContainer.Add(newMission.MissionNumber, newMission);
        }

        public void Remove(long missionNumber)
        {
            TheContainer.Remove(missionNumber);
        }

    }
}
