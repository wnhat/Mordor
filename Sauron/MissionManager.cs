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
        ILogger Logger;
        List<ExamMission> ExamMissionList;
        Dictionary<string, List<ExamMission>> ExamMissionDic = new Dictionary<string, List<ExamMission>>();
        Dictionary<string, Lot> OnInspectLotDic = new Dictionary<string, Lot>();                     // MES下发任务；
        Queue<Lot> LotWaitQueue = new Queue<Lot>();
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
            RefreshExamList();
        }

        private void AddMission(Lot lot)
        {
            LotWaitQueue.Enqueue(lot);
        }
        public void RefreshExamList()
        {
            Dictionary<string, List<ExamMission>> newExamMissionDic = new Dictionary<string, List<ExamMission>>();
            var missionlist = Thesqlserver.GetExamMission();
            string[] aviExamFileList = Directory.GetDirectories("\\\\172.16.145.22\\NetworkDrive\\D_Drive\\Mordor\\ExamSimple\\AVI");
            string[] sviExamFileList = Directory.GetDirectories("\\\\172.16.145.22\\NetworkDrive\\D_Drive\\Mordor\\ExamSimple\\SVI");
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
                            ExamMission newmission = new ExamMission(item.PanelId, filepath, item.PcSection, item.Defect, item.Judge,item.MissionInfo);
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
                            Logger.Error("panel ID: {0} ,do not have result file in {1}", item.PanelId); // TODO:ADD FILE path
                        }
                        break;
                    case InspectSection.SVI:
                        if (sviid.Contains(item.PanelId))
                        {
                            var filepath = sviExamFileList.Where(x => x.Substring(x.Length - 17) == item.PanelId).First();
                            ExamMission newmission = new ExamMission(item.PanelId, filepath, item.PcSection, item.Defect, item.Judge, item.MissionInfo);
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
                            Logger.Error("panel ID: {0} ,do not have result file in {1}", item.PanelId); // TODO:ADD FILE path
                        }
                        break;
                }
            }
            ExamMissionDic = newExamMissionDic;
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
            // TODO: 发送mes，添加sqlserver；
            a.Socket.SignalOK();
        }
        public void GetExamMission(NetMQSocketEventArgs a, NetMQMessage M)
        {
            ExamMissionMessage newexammission = new ExamMissionMessage(M);
            string examinfo = newexammission.ExamRequestInfo;
            var rnd = new Random();
            foreach (var item in ExamMissionDic[examinfo])
            {
                item.sortint = rnd.Next();
            }
            ExamMissionDic[examinfo].Sort();
            a.Socket.SendMultipartMessage(new ExamMissionMessage(MessageType.SERVER_SEND_MISSION, ExamMissionDic[examinfo], examinfo));
        }
        public void AddMissionByServer(NetMQSocketEventArgs a, NetMQMessage M)
        {
            PanelMissionMessage Mission = new PanelMissionMessage(M);
            AddMission(Mission.ThePanelMissionLot);
        }

        public Dictionary<string, List<PanelPathContainer>> GetPanelPathList(string[] SampleInfoList)
        {
            Dictionary<string, List<PanelPathContainer>> newPanelPathDic = new Dictionary<string, List<PanelPathContainer>>();
            foreach (var item in SampleInfoList)
            {
                //TODO:
                var panelPathContainer = Thefilecontainer.GetPanelPathList(item);
                newPanelPathDic.Add(item, panelPathContainer);
            }
            return newPanelPathDic;
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
