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
        User Operater;
        MissionPackage TheMissionPackage;
        public Parameter SystemParameter;
        bool StopToCellFlag;
        public Manager()
        {
            SystemParameter = GetParameter();
            TheServerConnecter = new Serverconnecter();
            Operater = null;
            StopToCellFlag = false;
            TheMissionPackage = new MissionPackage(SystemParameter);
        }

        public bool CheckUser(User newUser)
        {
            return TheServerConnecter.check_user_password(newUser.UserId, newUser.PassWord);
        }
        public void OperaterCheckIn(User newUser)
        {
            Operater = newUser;
        }
        public void OperaterCheckOut()
        {
            Operater = null;
        }

        public Parameter GetParameter()
        {
            string sysconfig_path = @"D:\1218180\program2\c#\Mordor\EyeOfSauron\sysconfig.json";
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
        public void SaveParameter()
        {

        }

        public void InspectStart()
        {
            TheMissionPackage.NewMission();
        }
        public void InspectFinished(string defectName)
        {
            var finishedMission = TheMissionPackage.OnInspectedMission.Finish(defectName);
            TheServerConnecter.FinishMission(finishedMission);
            TheMissionPackage.NewMission();
            if (!StopToCellFlag)
            {
                AddMission();
            }
        }

        public Bitmap[] GetOnInspectPanelImage()
        {
            Bitmap[] returnarray = new Bitmap[SystemParameter.ImageNameList.Count()];
            for (int i = 0; i < SystemParameter.ImageNameList.Count(); i++)
            {
                returnarray[i] = new Bitmap(TheMissionPackage.OnInspectedMission.GetFileFromMemory(SystemParameter.ImageNameList[i]));
            }
            return returnarray;
        }

        public string GetOnInspectPanelId()
        {
            return TheMissionPackage.OnInspectedMission.MissionInfo.PanelId;
        }
        public int RemainMissionCount { get { return TheMissionPackage.Count; } }
        void CheckRemainMissionCount()
        {
            for (int i = 0; i < 20; i++)
            {
                AddOneMission();
            }
        }
        public void AddMission()
        {
            for (int i = 0; i < 20; i++)
            {
                AddOneMission();
            }
        }
        public void AddOneMission()
        {
            TheMissionPackage.AddMission(TheServerConnecter.GetMission());
        }
        public void PreLoadOneMission()
        {
            TheMissionPackage.PreDownloadFile(TheServerConnecter.GetMission());
        }
        public void SendUnfinishedMissionBackToServer(InspectSection section)
        {
            Queue<PanelMission> returnmissionqueue = TheMissionPackage.GetUnfinishedMission();
            while (returnmissionqueue.Count == 0)
            {
                TheServerConnecter.SendUnfinishedMissionBack(section, returnmissionqueue.Dequeue());
            }
            TheMissionPackage.CleanMission();
        }
    }
}
