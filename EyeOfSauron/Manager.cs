using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;
using Container.Message;

namespace EyeOfSauron
{
    public class Manager
    {
        MissionBuffer TheMissionBuffer;
        Operator Operater;
        public InspectSection Section { get; set; }
        public InspectMission OnInspectedMission { get; set; }
        private Queue<InspectMission> PreDownloadedMissionQueue;        //已加载的文件队列
        readonly object Predownloadlock = new object();
        private int DownloadQuantity;                                   //预加载图像文件的个数
        public string ImageSavingPath { get; set; }
        public int Count { get { return PreDownloadedMissionQueue.Count; } }
        public int FinishedMissionCount { get { return Operater.MissionFinished; } }
        public bool PreLoadOneMission()
        {
            var mission = TheMissionBuffer.GetMission(Section);
            if (mission != null)
            {
                PreDownloadedMissionQueue.Enqueue(mission);
                return true;
            }
            else
            {
                return false;
            }
        }
        private void FillPreDownloadMissionQueue()
        {
            lock (Predownloadlock)
            {
                Task.Run(() =>
                {
                    while (PreDownloadedMissionQueue.Count <= DownloadQuantity)
                    {
                        if (PreLoadOneMission())
                        {
                        }
                        else
                        {
                            break;
                        }
                    }
                });
            }
        }
        public Manager()
        {
            TheMissionBuffer = new MissionBuffer();
            Operater = null;
            OnInspectedMission = null;
            DownloadQuantity = Parameter.PreLoadQuantity;
            ImageSavingPath = Parameter.SavePath;
            PreDownloadedMissionQueue = new Queue<InspectMission>();
        }
        public void CleanMission()
        {
            PreDownloadedMissionQueue = new Queue<InspectMission>();
            OnInspectedMission = null;
        }
        public void AddExamMissions()
        {
            TheMissionBuffer.GetExamMissions();
        }
        public bool NextMission()
        {
            if (PreDownloadedMissionQueue.Count == 0)
            {
                return false;
            }
            else
            {
                OnInspectedMission = PreDownloadedMissionQueue.Dequeue();
                return true;
            }
        }
        public Operator CheckUser(Operator newUser)
        {
            return NewSeverConnecter.CheckPassWord(newUser);
        }
        public void SetOperater(Operator newUser)
        {
            Operater = newUser;
        }
        public void OperaterCheckOut()
        {
            Operater = null;
        }
        public void SetInspectSection(InspectSection section)
        {
            Section = section;
        }
        public void InspectFinished(Defect defect, JudgeGrade judge)
        {
            PanelMissionResult newresult = new PanelMissionResult(judge, defect, this.Section, this.Operater,OnInspectedMission.PanelId,OnInspectedMission.MissionNumber);
            TheMissionBuffer.FinishMission(newresult);
            FillPreDownloadMissionQueue();
            Operater.MissionFinished++;
        }
        public void SendUnfinishedMissionBackToServer()
        {
            TheMissionBuffer.SendUnfinishedMissionBack(Section);
            OnInspectedMission = null;
            PreDownloadedMissionQueue = new Queue<InspectMission>();
        }
    }
    class MissionBuffer
    {
        Lot OnInspectLot;
        public string ExamInfo;
        Queue<ExamMission> ExamMissionList;
        Queue<ExamMission> ExamBuffer;
        List<ExamMission> ExamResult;
        public MissionBuffer()
        {
            ExamMissionList = new Queue<ExamMission>();
            ExamBuffer = new Queue<ExamMission>();
            ExamResult = new List<ExamMission>();
        }
        public void FinishMission(PanelMissionResult finishedMission)
        {
            switch (finishedMission.Section)
            {
                case InspectSection.NORMAL:
                    PanelResultMessage newMessage = new PanelResultMessage(MessageType.CLIENT_SEND_MISSION_RESULT, finishedMission);
                    request.SendMultipartMessage(newMessage);
                    request.ReceiveSignal();
                    break;
                case InspectSection.EXAM:
                    var mission = ExamBuffer.Dequeue();
                    mission.FinishExam(finishedMission);
                    ExamResult.Add(mission);
                    if (ExamBuffer.Count == 0)
                    {
                        NewSeverConnecter.SendExamMissionResult(ExamResult, ExamInfo);
                        
                    }
                    break;
            }
        }
        public void GetExamMissions(string examinfo)
        {
            NewSeverConnecter.
            BaseMessage newmessage = new BaseMessage(MessageType.CLINET_GET_EXAM_MISSION_LIST);
            request.SendMultipartMessage(newmessage);
            var returnmessage = new ExamMissionMessage(request.ReceiveMultipartMessage());
            foreach (var item in returnmessage.ExamMissionList)
            {
                ExamMissionList.Enqueue(item);
            }
        }
        public InspectMission GetMission(InspectSection section)
        {
            string[] imagenamelist = Parameter.AviImageNameList;
            if (section == InspectSection.AVI)
            {
                var newmission = GetPanelMission();
                if (newmission != null)
                {
                    panelMissionList.Enqueue(newmission);
                    return new InspectMission(newmission, section, Parameter.AviImageNameList);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (ExamMissionList.Count > 0)
                {
                    ExamMission newExamMission = ExamMissionList.Dequeue();
                    switch (newExamMission.PcSection)
                    {
                        case InspectSection.AVI:
                            imagenamelist = Parameter.AviImageNameList;
                            break;
                        case InspectSection.SVI:
                            imagenamelist = Parameter.SviImageNameList;
                            break;
                        case InspectSection.APP:
                            imagenamelist = Parameter.AppImageNameList;
                            break;
                    }
                    InspectMission newinspectmission = new InspectMission(newExamMission, imagenamelist);
                    ExamBuffer.Enqueue(newExamMission);
                    return newinspectmission;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
