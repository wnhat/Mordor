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
using NetMQ;
using Container.MQMessage;

namespace Sauron
{
    class MissionManager
    {
        MesConnector theMesConnector;       //管理与MES的链接；
        SqlServerConnector Thesqlserver;    //管理与sqlsqerver 的链接；
        FileManager Thefilecontainer;       //管理设备文件路径；
        Dictionary<string, List<ExamMission>> ExamMissionDic = new Dictionary<string, List<ExamMission>>();
        Dictionary<string, Lot> OnInspectLotDic = new Dictionary<string, Lot>();// MES下发任务；
        Queue<Lot> LotWaitQueue = new Queue<Lot>();
        Dictionary<LotInfo, Queue<Lot>> LotWait = new Dictionary<LotInfo, Queue<Lot>>();
        public MissionManager()
        {
            this.Thefilecontainer = new FileManager();
            this.Thesqlserver = new SqlServerConnector();
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
        private void AddMission(Lot lot)
        {
            LotWaitQueue.Enqueue(lot);
        }
        public void RefreshExamList()
        {
            ConsoleLogClass.Logger.Information("开始考试文件刷新；");
            Dictionary<string, List<ExamMission>> newExamMissionDic = new Dictionary<string, List<ExamMission>>();
            var missionlist = Thesqlserver.GetExamMission();
            foreach (var item in missionlist)
            {
                var avipath = new DirectoryInfo(Path.Combine(Parameter.AviExamFilePath, item.MissionInfo, item.PanelId));
                var svipath = new DirectoryInfo(Path.Combine(Parameter.SviExamFilePath, item.MissionInfo, item.PanelId));
                if (avipath.Exists && svipath.Exists)
                {
                    ExamMission newmission = new ExamMission(item.PanelId, avipath.FullName,svipath.FullName, item.PcSection, item.Defect, item.Judge, item.MissionInfo);
                    if (newExamMissionDic.ContainsKey(newmission.MissionInfo))
                    {
                        newExamMissionDic[newmission.MissionInfo].Add(newmission);
                    }
                    else
                    {
                        newExamMissionDic.Add(newmission.MissionInfo, new List<ExamMission>() { newmission });
                    }
                }
                else
                {
                    ConsoleLogClass.Logger.Error("Refresh Exam; panel ID: {0} ,do not have result file in {1}", item.PanelId); // TODO:ADD FILE path
                }
            }
            ExamMissionDic = newExamMissionDic;
            ConsoleLogClass.Logger.Information("考试文件刷新结束；");
        }
        public void WaittingMissionAdd(LotInfo info,Lot newlot)
        {
            if (LotWait.ContainsKey(info))
            {
                LotWait[info].Enqueue(newlot);
            }
            else
            {
                LotWait.Add(info, new Queue<Lot>());
                WaittingMissionAdd(info,newlot);
            }
        }
        public Lot WaitingMissionGet(LotInfo info)
        {
            if (LotWait.ContainsKey(info))
            {
                var newlotqueue = LotWait[info];
                if (newlotqueue.Count > 0)
                {
                    return newlotqueue.Dequeue();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public void GetMission(NetMQSocketEventArgs a, NetMQMessage M)
        {
            PanelMissionRequestMessage request = new PanelMissionRequestMessage(M);
            var newmission = theMesConnector.RequestMission(request.FGcode,request.productType);
            if (newmission != null)
            {

            }
            else
            {

            }

            // old //
            if (LotWaitQueue.Count == 0)
            {
                a.Socket.SendMultipartMessage(new PanelMissionMessage(MessageType.SERVER_SEND_MISSION, null));
            }
            else
            {
                Lot newlot = LotWaitQueue.Dequeue();
                OnInspectLotDic.Add(newlot.TRAYGROUPNAME, newlot);
                a.Socket.SendMultipartMessage(new PanelMissionMessage(MessageType.SERVER_SEND_MISSION, newlot));
            }
        }
        //public Lot GetMission(string FGcode, ProductType type)
        //{

        //}
        public void FinishMission(NetMQSocketEventArgs a, NetMQMessage M)
        {
            PanelMissionMessage finishedMission = new PanelMissionMessage(M);
            OnInspectLotDic.Remove(finishedMission.ThePanelMissionLot.TRAYGROUPNAME);
            a.Socket.SignalOK();
            // TODO: 发送mes；
            Thesqlserver.InsertFinishedMission(finishedMission.ThePanelMissionLot.panelcontainer.ToArray());
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
        public void AddMissionByServer(NetMQSocketEventArgs a, NetMQMessage M)
        {
            PanelMissionMessage Mission = new PanelMissionMessage(M);
            AddMission(Mission.ThePanelMissionLot);
            a.Socket.SignalOK();
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
    class LotControler
    {

    }
    struct LotInfo
    {
        string FGcode;
        ProductType Type;

        public LotInfo(string fGcode, ProductType type)
        {
            FGcode = fGcode;
            Type = type;
        }

        //public override bool Equals(object obj)
        //{
        //    return obj is LotInfo info && FGcode == info.FGcode && Type == info.Type;
        //}

        public override int GetHashCode()
        {
            int hashCode = -2086568036;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FGcode);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            return hashCode;
        }
    }
}
