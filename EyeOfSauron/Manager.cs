using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var imageInMemory = TheMissionPackage.OnInspectedMission.GetImageArray();
            int imageCount = imageInMemory.Count();
            Bitmap[] returnArray = new Bitmap[imageCount];
            for (int i = 0; i < imageCount; i++)
            {
                returnArray[i] = new Bitmap(imageInMemory[i]);
            }
            return returnArray;
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
    }
}
