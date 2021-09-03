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
            //tibrvlisten -service 8410 -network ;225.8.8.41  BOE.B7.MEM.DEV10047792.CNMsvr

            this.theMesConnector = new MesConnector();

            RefreshExamList();
            RefreshFileContainer();
            
            ConsoleLogClass.Logger.Information("服务器启动中------开始添加任务（测试版）");
            // TODO: add mission test mod;
            ConsoleLogClass.Logger.Information("服务器启动中------任务添加完成");
        }
        public void RefreshExamList(NetMQSocketEventArgs a)
        {
            RefreshExamList();
            a.Socket.SignalOK();
        }
        public void GetMission(NetMQSocketEventArgs a, NetMQMessage M)
        {
            // 获取数据库中正在等待检查的任务返回给客户端;请求的任务不存在时返回为null；
            PanelMissionRequestMessage request = new PanelMissionRequestMessage(M);
            ProductInfo info = request.Info;
            User op = request.Operater;
            ConsoleLogClass.Logger.Information("收到量产的任务请求；请求型号为：{0} {1} {2} 检查员为：{3}", info.Name,info.ProductType,info.FGcode,op.UserName);
            MissionLot newMissionLot = WaitingMissionGet(info,op);
            PanelMissionMessage responseMessage = new PanelMissionMessage(MessageType.SERVER_SEND_MISSION, newMissionLot);
            a.Socket.SendMultipartMessage(responseMessage);
        }
        public void FinishMission(NetMQSocketEventArgs a, NetMQMessage M)
        {
            PanelMissionMessage finishedMission = new PanelMissionMessage(M);
            a.Socket.SignalOK();
            ConsoleLogClass.Logger.Information("收到完成的任务 TrayGroupName：{0}", finishedMission.ThePanelMissionLot.TRAYGROUPNAME);
            try
            {
                DbConnector.finishInspect(finishedMission.ThePanelMissionLot);
            }
            catch (ArgumentNullException e)
            {

                ConsoleLogClass.Logger.Error("向数据库储存完成的任务时发生错误,数据库中找不到相对应的OninspectLot,TrayGroupName：{0} ", finishedMission.ThePanelMissionLot.TRAYGROUPNAME);
                throw;
            }

            try
            {
                theMesConnector.FinishInspect(finishedMission.ThePanelMissionLot);
            }
            catch (MesMessageException e)
            {
                MesLogClass.Logger.Error(e.Message);
            }
        }
        public void FinishExam(NetMQSocketEventArgs a, NetMQMessage M)
        {
            ExamMissionMessage finishedExam = new ExamMissionMessage(M);
            ConsoleLogClass.Logger.Information("收到完成的考试 试题名称：{0}", finishedExam.ExamRequestInfo);
            FinishExam(finishedExam.ExamMissionList);
            a.Socket.SignalOK();
        }
        public void GetProductInfo(NetMQSocketEventArgs a)
        {
            // 用于selectform 获取供检查的 productinfo；
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
                a.Socket.SendMultipartMessage(new ExamMissionMessage(MessageType.SERVER_SEND_MISSION, ExamMissionDic[examinfo], examinfo));
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
            a.Socket.SendMultipartMessage(new ExamInfoMessage(examinfoarray));
        }
        public void AddMissionByControlor(NetMQSocketEventArgs a, NetMQMessage M)
        {
            // TODO:控制任务添加；
            PanelMissionRequestMessage message = new PanelMissionRequestMessage(M);
            MesLogClass.Logger.Information("收到来自 Controler的添加任务请求，请求型号为：{0} {1} {2}",message.Info.Name, message.Info.ProductType, message.Info.FGcode);
            try
            {
                RemoteTrayGroupInfoDownloadSend returnmessage = theMesConnector.RequestMission(message.Info);
                DbConnector.AddNewLotFromMes(returnmessage.lot);
                a.Socket.SignalOK();
            }
            catch (MesMessageException e)
            {
                MesLogClass.Logger.Error(e.Message);
                a.Socket.SignalError();
            }
           
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
            return DbConnector.GetOp(op);
        }
        void FinishExam(List<ExamMission> missionlist)
        {

        }
        MissionLot WaitingMissionGet(ProductInfo info,User op)
        {
            TrayLot newlot = DbConnector.GetWaitedMission(info,op);
            if (newlot == null)
            {
                return null;
            }
            else
            {
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
                MissionLot newMissionLot = new MissionLot(newlot, missionlist);
                return newMissionLot;
            }
        }
        public void RefreshExamList()
        {
            ConsoleLogClass.Logger.Information("开始考试文件刷新；");
            // TODO:
            Dictionary<string, List<ExamMission>> newExamMissionDic = new Dictionary<string, List<ExamMission>>();
            ExamMissionDic = newExamMissionDic;
            ConsoleLogClass.Logger.Information("考试文件刷新结束；");
        }
    }
}
