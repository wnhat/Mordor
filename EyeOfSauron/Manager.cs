using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;
using Container.MQMessage;
using Container.SeverConnection;

namespace EyeOfSauron
{
    public class Manager
    {
        public MissionBuffer TheMissionBuffer;
        public User Operater;
        public InspectSection Section { get; set; }
        public InspectMission OnInspectedMission { get; set; }
        private Queue<InspectMission> PreDownloadedMissionQueue;        //已加载的文件队列
        readonly object Predownloadlock = new object();
        private int DownloadQuantity;                                   //预加载图像文件的个数
        public string ImageSavingPath { get; set; }
        public int MissionLeft { get { return PreDownloadedMissionQueue.Count + TheMissionBuffer.MissionLeft; } }
        public int FinishedMissionCount { get { return 0; } }           // TODO:添加已完成任务数；
        public int PreLoadMissions()
        {
            var mission = TheMissionBuffer.GetMission();
            if (mission != null)
            {
                PreDownloadedMissionQueue.Enqueue(mission);
                return 100 * PreDownloadedMissionQueue.Count/Parameter.PreLoadQuantity;
            }
            else
            {
                return 100;
            }
        }
        public bool PreLoadOneMission()
        {
            var mission = TheMissionBuffer.GetMission();
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
        public User CheckUser(User newUser)
        {
            return SeverConnecter.CheckPassWord(newUser);
        }
        public void SetOperater(User newUser)
        {
            Operater = newUser;
        }
        public void OperaterCheckOut()
        {
            Operater = null;
        }
        public void SetInspectSection(InspectSection section)
        {
            TheMissionBuffer.section = section;
        }
        public void InspectFinished(Defect defect, JudgeGrade judge)
        {
            PanelMissionResult newresult = new PanelMissionResult(judge, defect, this.Operater,OnInspectedMission.PanelId);
            TheMissionBuffer.FinishMission(newresult);
            FillPreDownloadMissionQueue();
        }
    }
    public class MissionBuffer
    {
        public string ExamInfo;
        public InspectSection section;

        MissionLot OnInspectLot;
        
        private Queue<PanelMission> waitMissionQueue = new Queue<PanelMission>();
        Queue<ExamMission> ExamMissionList = new Queue<ExamMission>();
        Queue<ExamMission> ExamBuffer = new Queue<ExamMission>();
        List<ExamMission> ExamResult = new List<ExamMission>();
        public int MissionLeft { get { return waitMissionQueue.Count + ExamMissionList.Count; } }
        public MissionBuffer()
        {
        }
        public void FinishMission(PanelMissionResult finishedMission)
        {
            switch (section)
            {
                case InspectSection.NORMAL:
                    OnInspectLot.PanelFinish(finishedMission);
                    if (OnInspectLot.Complete)
                    {
                        SeverConnecter.SendPanelMissionResult(OnInspectLot);
                        OnInspectLot = null;
                    }
                    break;
                case InspectSection.EXAM:
                    var mission = ExamBuffer.Dequeue();
                    mission.FinishExam(finishedMission);
                    ExamResult.Add(mission);
                    if (ExamBuffer.Count == 0)
                    {
                        SeverConnecter.SendExamMissionResult(ExamResult, ExamInfo);
                    }
                    break;
            }
        }
        public void GetPanelMission(ProductInfo info,User op)
        {
            var newlot = SeverConnecter.GetPanelMission(info,op);
            if (newlot == null)
            {
                throw new ApplicationException("没有剩余任务以供检查，请退出");
            }
            else
            {
                OnInspectLot = newlot;
            }
            foreach (var item in OnInspectLot.panelcontainer)
            {
                waitMissionQueue.Enqueue(item);
            }
        }
        public void GetExamMissions(string examinfo)
        {
            var NewMissionList = SeverConnecter.GetExamMission(examinfo);
            foreach (var item in NewMissionList)
            {
                ExamMissionList.Enqueue(item);
            }
            ExamInfo = examinfo;
        }
        public InspectMission GetMission()
        {
            if (section == InspectSection.NORMAL)
            {
                if (waitMissionQueue.Count != 0)
                {
                    var newmission = waitMissionQueue.Dequeue();
                    try
                    {
                        return new InspectMission(newmission);
                    }
                    catch (FileNotFoundException)
                    {
                        User autouser = new User { IndexId = 0 };
                        Defect EjudgeDefect = new Defect("FileError","DE00000",InspectSection.NORMAL);
                        PanelMissionResult missionresult = new PanelMissionResult(JudgeGrade.E, EjudgeDefect, autouser, newmission.PanelId);
                        FinishMission(missionresult);
                        return GetMission();
                    }
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
                    InspectMission newinspectmission = new InspectMission(newExamMission);
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
