﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;

namespace EyeOfSauron
{
    public class Manager
    {
        Serverconnecter TheServerConnecter;
        Operator Operater;
        public Parameter SystemParameter;
        public InspectSection Section { get; set; }
        public InspectMission OnInspectedMission { get; set; }
        private Queue<InspectMission> PreDownloadedMissionQueue;        //已加载的文件队列
        readonly object Predownloadlock = new object();
        private int DownloadQuantity;                                   //预加载图像文件的个数
        public string ImageSavingPath { get; set; }
        public int Count { get { return PreDownloadedMissionQueue.Count; } }
        public int FinishedMissionCount { get { return Operater.MissionFinished; } }
        string[] ImageNameList
        {
            get
            {
                if (Section == InspectSection.AVI)
                {
                    return SystemParameter.AviImageNameList;
                }
                else if (Section == InspectSection.SVI)
                {
                    return SystemParameter.SviImageNameList;
                }
                else
                {
                    return SystemParameter.AppImageNameList;
                }
            }
        }
        public bool PreLoadOneMission()
        {
            var mission = TheServerConnecter.GetMission(Section);
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
            SystemParameter = InitParameter();
            TheServerConnecter = new Serverconnecter(SystemParameter);
            Operater = null;
            OnInspectedMission = null;
            DownloadQuantity = SystemParameter.PreLoadQuantity;
            ImageSavingPath = SystemParameter.SavePath;
            PreDownloadedMissionQueue = new Queue<InspectMission>();
        }
        public void CleanMission()
        {
            PreDownloadedMissionQueue = new Queue<InspectMission>();
            OnInspectedMission = null;
        }
        public void ChangeDownloadQuantity()
        {
            DownloadQuantity += 40;
        }
        public void AddExamMissions()
        {
            TheServerConnecter.GetExamMissions();
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
            return TheServerConnecter.CheckPassWord(newUser);
        }
        public void SetOperater(Operator newUser)
        {
            Operater = newUser;
        }
        public void OperaterCheckOut()
        {
            Operater = null;
        }
        public Parameter InitParameter()
        {
            string sysconfig_path = @"\\172.16.145.22\NetworkDrive\D_Drive\Mordor\sysconfig.json";
            FileInfo sysconfig = new FileInfo(sysconfig_path);
            if (sysconfig.Exists)
            {
                var jsonstring = new StreamReader(sysconfig.OpenRead());
                JsonSerializer file_serial = new JsonSerializer();
                Parameter newparameter = (Parameter)file_serial.Deserialize(new JsonTextReader(jsonstring), typeof(Parameter));
                return newparameter;
            }
            return null;
        }
        public void SaveParameter() { }
        public void SetInspectSection(InspectSection section)
        {
            Section = section;
        }
        public void InspectFinished(Defect defect, JudgeGrade judge)
        {
            PanelMissionResult newresult = new PanelMissionResult(judge, defect, this.Section, this.Operater,OnInspectedMission.PanelId,OnInspectedMission.MissionNumber);
            TheServerConnecter.FinishMission(newresult);
            FillPreDownloadMissionQueue();
            Operater.MissionFinished++;
        }
        public void SendUnfinishedMissionBackToServer()
        {
            TheServerConnecter.SendUnfinishedMissionBack(Section);
            OnInspectedMission = null;
            PreDownloadedMissionQueue = new Queue<InspectMission>();
        }
    }
}
