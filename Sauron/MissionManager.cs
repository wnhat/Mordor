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
using Container.Message;

namespace Sauron
{
    class MissionManager
    {
        SqlServerConnector Thesqlserver;
        FileManager Thefilecontainer;                               //管理设备文件路径；
        Dictionary<string, List<ExamMission>> ExamMissionDic = new Dictionary<string, List<ExamMission>>();
        Dictionary<string, Lot> OnInspectLotDic = new Dictionary<string, Lot>();                     // MES下发任务；
        Queue<Lot> LotWaitQueue = new Queue<Lot>();
        public MissionManager()
        {
            this.Thefilecontainer = new FileManager();
            this.Thesqlserver = new SqlServerConnector();
            RefreshExamList();
            Task refreshtask = RefreshFileContainer();
            while (!refreshtask.IsCompleted)
            {
                Thread.Sleep(10000);
            }
            ConsoleLogClass.Logger.Information("服务器启动中------开始添加任务（测试版）");
            AddMissionTest();
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
        public void GetMission(NetMQSocketEventArgs a)
        {
            if (LotWaitQueue.Count == 0)
            {
                a.Socket.SendMultipartMessage(new PanelMissionMessage(MessageType.SERVER_SEND_MISSION, null));
            }
            else
            {
                Lot newlot = LotWaitQueue.Dequeue();
                OnInspectLotDic.Add(newlot.LotId, newlot);
                a.Socket.SendMultipartMessage(new PanelMissionMessage(MessageType.SERVER_SEND_MISSION, newlot));
            }
        }
        public void FinishMission(NetMQSocketEventArgs a, NetMQMessage M)
        {
            PanelMissionMessage finishedMission = new PanelMissionMessage(M);
            OnInspectLotDic.Remove(finishedMission.ThePanelMissionLot.LotId);
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
        public async Task RefreshFileContainer()
        {
            ConsoleLogClass.Logger.Information("开始刷新设备文件路径");
            await Thefilecontainer.RefreshFileList();
            ConsoleLogClass.Logger.Information("文件路径刷新完成");
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
        public void AddMissionTest()
        {
            var newlist = Thesqlserver.GetInputPanelMission();
            string[] idlist = new string[newlist.Count];
            string[] eqplist = new string[newlist.Count];
            for (int i = 0; i < newlist.Count; i++)
            {
                idlist[i] = newlist[i][0];
                eqplist[i] = newlist[i][1];
            }
            var pathdic = GetPanelPathList(idlist);
            Queue<PanelMission> newmissionqueue = new Queue<PanelMission>();
            for (int i = 0; i < newlist.Count; i++)
            {
                string panelid = idlist[i];
                string eqpid = eqplist[i];
                var pathlist = pathdic[panelid];
                if (pathlist!=null)
                {
                    try
                    {
                        var avipath = pathlist.Where(x => x.PcSection == InspectSection.AVI && x.EqName == eqpid).First();
                        var svipath = pathlist.Where(x => x.PcSection == InspectSection.SVI && x.EqName == eqpid).First();
                        PanelMission newpanel = new PanelMission(panelid, MissionType.PRODUCITVE, avipath, svipath);
                        newmissionqueue.Enqueue(newpanel);
                    }
                    catch (Exception e)
                    {
                        ConsoleLogClass.Logger.Error("文件存在问题{0}； panelid：{1}",e.Message, panelid);
                    }
                }
                else
                {
                    ConsoleLogClass.Logger.Error("未查找到文件路径； panelid：{0}", panelid);
                }
            }
            var randomstring = new Random();
            while (newmissionqueue.Count != 0)
            {
                Lot newlot = new Lot(randomstring.Next().ToString() + " " + DateTime.Now.ToString());
                while (true)
                {
                    var newpanel = newmissionqueue.Dequeue();
                    newlot.AddPanel(newpanel);
                    if (newmissionqueue.Count == 0 || newlot.Count >= 240)
                    {
                        AddMission(newlot);
                        break;
                    }
                }
            }
        }
    }
}
