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
        private PanelMission missionInfo;
        DirectoryInfo ImageFile;                        // Result directory
        string[] ImageNameList;                         // The image name in reuslt file which we need to inspect
        List<FileContainer> FileArray;                  // File in result directory
        string SavePath;
        
        public InspectMission(PanelMission missioninfo, string[] imageNameList, string savePath)
        {
            missionInfo = missioninfo;
            ImageNameList = imageNameList;
            SavePath = savePath;
            ImageFile = new DirectoryInfo(MissionInfo.PanelPath.Result_path);
            DownloadFileInMemory();
        }
        public void DownloadFileInMemory()
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
        public void ChangeSavePath(string newsavepath)
        {
            SavePath = newsavepath;
        }
        public MemoryStream[] GetImageArray()
        {
            MemoryStream[] returnarray = new MemoryStream[ImageNameList.Count()];
            int i = 0;
            foreach (var file in FileArray)
            {
                if (ImageNameList.Contains(file.Name))
                {
                    returnarray[i] = file.GetFileFromMemory();
                    i++;
                }
            }
            return returnarray;
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
        public PanelMission MissionInfo
        {
            get
            {
                return missionInfo;
            }
        }
        public PanelMission Finish(string defectName)
        {
            // add defect in to the panelmission`s defect table;
            SaveFileInDisk();
            return missionInfo;
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
        public MemoryStream GetFileFromMemory()
        {
            return FileMemory;
        }
        public string Name
        {
            get
            {
                return FileInformation.Name;
            }
        }
    }
}
