using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using System.Threading;
using System.IO;
using NetMQ;
using Container;
using Container.MQMessage;

namespace Sauron
{
    class MissionManager
    {
        MesConnector theMesConnector;       //管理与MES的链接；
        FileManager Thefilecontainer;       //管理设备文件路径；
        Dictionary<string, List<ExamMission>> ExamMissionDic = new Dictionary<string, List<ExamMission>>();
        public MissionManager()
        {
            this.Thefilecontainer = new FileManager();

            string service = "BOE.B7.MEM.TST.PEMsvr";
            string network = "172.16.145.22";
            string daemon = null;
            string subject = "a.b.c";
            this.theMesConnector = new MesConnector(service, network,daemon,subject);

            RefreshExamList();
            RefreshFileContainer();
            
            ConsoleLogClass.Logger.Information("服务器启动中------开始添加任务（测试版）");
            // TODO: add mission test mod;
            ConsoleLogClass.Logger.Information("服务器启动中------任务添加完成");
        }
        public void RefreshExamList()
        {
            ConsoleLogClass.Logger.Information("开始考试文件刷新；");
            Dictionary<string, List<ExamMission>> newExamMissionDic = new Dictionary<string, List<ExamMission>>();
            ExamMissionDic = newExamMissionDic;
            ConsoleLogClass.Logger.Information("考试文件刷新结束；");
        }
        public void WaittingMissionAdd(ProductInfo info)
        {
            var newMissionMessage = theMesConnector.RequestMission(info);
            DbConnector.AddNewLotFromMes(newMissionMessage.lot);
        }
        public MissionLot WaitingMissionGet(ProductInfo info)
        {
            TrayLot newlot = DbConnector.GetWaitedMission(info);
            IEnumerable<Panel> panelList = from item in newlot.Panel
                                             select item;
            IEnumerable<string> panelidList = from item in newlot.Panel
                                              select item.PanelId;
            var path = GetPanelPathList(panelidList.ToArray());

            List<PanelMission> missionlist = new List<PanelMission>();
            foreach (var item in panelList)
            {
                var pathlist = path[item.PanelId];
                var avipath = pathlist.Where(x => x.PcSection == InspectSection.AVI).FirstOrDefault();
                var svipath = pathlist.Where(x => x.PcSection == InspectSection.SVI).FirstOrDefault();
                PanelMission newpanel = new PanelMission(item, MissionType.PRODUCITVE, avipath, svipath);
                missionlist.Add(newpanel);
            }
            MissionLot newMissionLot = new MissionLot(newlot.MachineName, missionlist, newlot.TrayGroupName);
            return newMissionLot;
        }
        public void GetMission(NetMQSocketEventArgs a, NetMQMessage M)
        {
            // 获取数据库中正在等待检查的任务返回给客户端
            PanelMissionRequestMessage request = new PanelMissionRequestMessage(M);

            // TODO:Add productinfo here;
            ProductInfo info = new ProductInfo { };
            // TODO:Add productinfo here;
            
            MissionLot newMissionLot = WaitingMissionGet(info);
            PanelMissionMessage responseMessage = new PanelMissionMessage(MessageType.SERVER_SEND_MISSION, ServerVersion.Version,newMissionLot);
            a.Socket.SendMultipartMessage(responseMessage);
        }
        public void FinishMission(NetMQSocketEventArgs a, NetMQMessage M)
        {
            PanelMissionMessage finishedMission = new PanelMissionMessage(M);
            a.Socket.SignalOK();
            // TODO: 发送mes;
            DbConnector.finishInspect(finishedMission.ThePanelMissionLot);
            theMesConnector.FinishMission(finishedMission.ThePanelMissionLot);
        }

        public void GetProductInfo(NetMQSocketEventArgs a)
        {
            ProductInfoMessage info = new ProductInfoMessage(DbConnector.GetProductInfo());
            a.Socket.SendMultipartMessage(info);
        }

        public void GetExamMission(NetMQSocketEventArgs a, NetMQMessage M)
        {
            ExamMissionMessage newexammission = new ExamMissionMessage(M);
            string examinfo = newexammission.ExamRequestInfo;
            var rnd = new Random();
            if (ExamMissionDic.ContainsKey(examinfo))
            {
                foreach (var item in ExamMissionDic[examinfo])
                {
                    item.sortint = rnd.Next();
                }
                ExamMissionDic[examinfo].Sort();
                a.Socket.SendMultipartMessage(new ExamMissionMessage(MessageType.SERVER_SEND_MISSION, ServerVersion.Version, ExamMissionDic[examinfo], examinfo));
            }
            else
            {
                ConsoleLogClass.Logger.Error("没有找到相关的任务名， 任务名： {0}",examinfo);
                // TODO： 返回未找到信息；
            }
        }
        public void GetExamInfo(NetMQSocketEventArgs a)
        {
            string[] examinfoarray = ExamMissionDic.Keys.ToArray();
            a.Socket.SendMultipartMessage(new ExamInfoMessage(examinfoarray, ServerVersion.Version));
        }
        public void AddMissionByControlor(NetMQSocketEventArgs a, NetMQMessage M)
        {
            // TODO:控制任务添加；
        }
        public Dictionary<string, List<PanelPathContainer>> GetPanelPathList(string[] SampleInfoList)
        {
            Dictionary<string, List<PanelPathContainer>> newPanelPathDic = new Dictionary<string, List<PanelPathContainer>>();
            foreach (var item in SampleInfoList)
            {
                var panelPathContainer = Thefilecontainer.GetPanelPathList(item);
                newPanelPathDic.Add(item, panelPathContainer);
            }
            return newPanelPathDic;
        }
        public void RefreshFileContainer()
        {
            ConsoleLogClass.Logger.Information("开始刷新设备文件路径");
            Thefilecontainer.RefreshFileList();
            ConsoleLogClass.Logger.Information("文件路径刷新完成");
            GC.Collect();
            ConsoleLogClass.Logger.Information("二次垃圾收集");
        }
        public User CheckUser(User op)
        {
            return null;
        }
        public void FinishExam(List<ExamMission> missionlist)
        {

        }
    }
}
