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
            while (PreDownloadedMissionQueue.Count >= DownloadQuantity)
            {
                if (UnDownloadedMissionQueue.Count > 0)
                {
                    InspectMission newmission = new InspectMission(UnDownloadedMissionQueue.Dequeue(), ImageNameList, ImageSavingPath);
                    PreDownloadedMissionQueue.Enqueue(newmission);
                }
                else
                {
                    break;
                }
            }
        }
        public void NewMission()
        {
            OnInspectedMission = PreDownloadedMissionQueue.Dequeue();

        }
        public void SetImageNameList(List<string> newnamelist)
        {
            ImageNameList = newnamelist.ToArray();
        }

    }

    class InspectMission
    {
        private PanelMission MissionInfo;
        private string[] ImageNameList;
        private string SavePath;
        private InspectFileContainer FileContainer;

        public InspectMission(PanelMission missionInfo, string[] imageNameList, string savePath)
        {
            MissionInfo = missionInfo;
            ImageNameList = imageNameList;
            SavePath = savePath;
            FileContainer = null;
            DownloadFile();
        }

        public void DownloadFile()
        {
            FileContainer = new InspectFileContainer(MissionInfo.PanelPath.Result_path, ImageNameList);
        }

        public void ChangeSavePath(string newsavepath)
        {
            SavePath = newsavepath;
        }

    }

    class InspectFileContainer
    {
        DirectoryInfo ImageFile;        // Result directory
        string[] ImageNameList;         // The image name in reuslt file which we need to inspect
        List<FileContainer> FileArray;  // File in result directory
        string SavePath;
        public InspectFileContainer(string SrcfilePath, string[] imageNameList)
        {
            ImageFile = new DirectoryInfo(SrcfilePath);
            ImageNameList = imageNameList;
            GetFileInMemory();
        }

        public void GetFileInMemory()
        {
            FileInfo[] filearray = ImageFile.GetFiles();
            foreach (var file in filearray)
            {
                if (ImageNameList.Contains(file.Name))
                {
                    FileContainer newfile = new FileContainer(file);
                    // TODO: ADD TRY, if read file error,log it;
                    FileArray.Add(newfile);
                }
            }
        }

        public void SaveFileInDisk()
        {
            DirectoryInfo savedirectory = new DirectoryInfo(SavePath);
            savedirectory.Create();
            foreach (var file in FileArray)
            {
                file.SaveFileInDisk(savedirectory.FullName);
            }
        }

    }

    class FileContainer
    {
        FileInfo FileInformation;
        MemoryStream FileMemory;
        public FileContainer(FileInfo fileInformation)
        {
            FileInformation = fileInformation;
            FileMemory = new MemoryStream();
            ReadFileInMemory();
        }

        public void ReadFileInMemory()
        {
            FileInformation.OpenRead().CopyTo(FileMemory);
        }

        public void SaveFileInDisk(string savePath)
        {
            FileInfo newsavefile = new FileInfo(Path.Combine(savePath, FileInformation.Name));
            var writestream = newsavefile.OpenWrite();
            FileMemory.CopyTo(writestream);
        }
    }
}
