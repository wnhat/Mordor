using Newtonsoft.Json;
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
        private int DownloadQuantity;                                   //预加载图像文件的个数
        public string ImageSavingPath { get; set; }
        public int Count { get { return PreDownloadedMissionQueue.Count; } }
        public int RemainMissionCount { get { return PreDownloadedMissionQueue.Count; } }
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
        public Manager()
        {
            SystemParameter = InitParameter();
            TheServerConnecter = new Serverconnecter(SystemParameter);
            Operater = null;
            DownloadQuantity = SystemParameter.PreLoadQuantity;
            ImageSavingPath = SystemParameter.SavePath;
        }
        public void CleanMission()
        {
            PreDownloadedMissionQueue = new Queue<InspectMission>();
            OnInspectedMission = null;
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
        public void SaveParameter() { }
        public void SetInspectSection(InspectSection section)
        {
            Section = section;
        }
        public String InspectFinished(Defect defect, JudgeGrade judge)
        {
            PanelMissionResult newresult = new PanelMissionResult(judge, defect, this.Section, this.Operater);
            TheServerConnecter.FinishMission(newresult);
            AddMission();
        }
        public void SendUnfinishedMissionBackToServer()
        {
            //TODO
        }
    }
}
