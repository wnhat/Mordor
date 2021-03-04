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
    class MissionPackage
    {
        public InspectMission OnInspectedMission { get; set; }
        private Queue<PanelMission> UnDownloadedMissionQueue;       //待加载文件队列
        private Queue<InspectMission> PreDownloadedMissionQueue;    //已加载的文件队列
        private int DownloadQuantity;                               //预加载图像文件的个数
        public string ImageSavingPath { get; set; }

        /// <summary>
        /// 剩余未检查任务数
        /// </summary>
        public int Count { get { return UnDownloadedMissionQueue.Count + PreDownloadedMissionQueue.Count; } }

        private string[] ImageNameList;

        public MissionPackage(int downloadQuantity, string imageSavingPath, string[] imageNameList)
        {
            UnDownloadedMissionQueue = new Queue<PanelMission>();
            PreDownloadedMissionQueue = new Queue<InspectMission>();
            DownloadQuantity = downloadQuantity;
            ImageSavingPath = imageSavingPath;
            ImageNameList = imageNameList;
        }
        public MissionPackage(Parameter sysParameter):this(sysParameter.PreLoadQuantity, sysParameter.SavePath, sysParameter.ImageNameList){}

        public void CleanMission()
        {
            UnDownloadedMissionQueue = new Queue<PanelMission>();
            PreDownloadedMissionQueue = new Queue<InspectMission>();
            OnInspectedMission = null;
        }
        public void AddMission(PanelMission newmission)
        {
            UnDownloadedMissionQueue.Enqueue(newmission);
        }
        public void AddMissionList(List<PanelMission> newmissionlist)
        {
            foreach (var mission in newmissionlist)
            {
                AddMission(mission);
            }
        }
        public void PreDownloadFile()
        {
            while (PreDownloadedMissionQueue.Count >= DownloadQuantity | UnDownloadedMissionQueue.Count == 0)
            {
                InspectMission newmission = new InspectMission(UnDownloadedMissionQueue.Dequeue(), ImageNameList, ImageSavingPath);
                PreDownloadedMissionQueue.Enqueue(newmission);
            }
        }
        public void PreDownloadFile(PanelMission newpanel)
        {
            InspectMission newmission = new InspectMission(newpanel, ImageNameList, ImageSavingPath);
            PreDownloadedMissionQueue.Enqueue(newmission);
        }
        public void NewMission()
        {
            OnInspectedMission = PreDownloadedMissionQueue.Dequeue();
        }
        public void SetImageNameList(List<string> newnamelist)
        {
            ImageNameList = newnamelist.ToArray();
        }
        public Queue<PanelMission> GetUnfinishedMission()
        {
            UnDownloadedMissionQueue.Enqueue(OnInspectedMission.MissionInfo);
            while (PreDownloadedMissionQueue.Count == 0)
            {
                UnDownloadedMissionQueue.Enqueue(PreDownloadedMissionQueue.Dequeue().MissionInfo);
            }
            return UnDownloadedMissionQueue;
        }
    }
}
