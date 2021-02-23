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

    class InspectMission
    {
        private PanelMission missionInfo;
        DirContainer Container;                         
        string[] ImageNameList;                         // The image name in reuslt file which we need to inspect
        string SavePath;
        
        public InspectMission(PanelMission missioninfo, string[] imageNameList, string savePath)
        {
            missionInfo = missioninfo;
            ImageNameList = imageNameList;
            SavePath = savePath;
            Container = new DirContainer(MissionInfo.AviPanelPath.Result_path);
        }
        public void ChangeSavePath(string newsavepath)
        {
            SavePath = newsavepath;
        }
        public void SaveFileInDisk()
        {
            Container.SaveDirInDisk(SavePath);
        }
        public PanelMission MissionInfo
        {
            get
            {
                return missionInfo;
            }
        }
        public MemoryStream GetFileFromMemory(string filename)
        {
            return Container.GetFileFromMemory(filename);
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
            // TODO: ADD TRY, if read file error,log it;
            FileInformation.OpenRead().CopyTo(FileMemory);
        }
        public void SaveFileInDisk(string savePath)
        {
            // TODO：async process;
            FileInfo newsavefile = new FileInfo(Path.Combine(savePath, FileInformation.Name));
            var writestream = newsavefile.OpenWrite();
            FileMemory.CopyTo(writestream);
        }
        public MemoryStream FileFromMemory
        {
            get
            {
                return FileMemory;
            }
        }
        public string Name
        {
            get
            {
                return FileInformation.Name;
            }
        }
    }

    /// <summary>
    /// copy the giving path dir(and it`s subdir) to local memory;
    /// </summary>
    class DirContainer
    {
        DirectoryInfo DirInfo;
        FileContainer[] FileContainerArray;
        DirContainer[] DirContainerArray;

        public DirContainer(string dirPath)
        {
            DirInfo = new DirectoryInfo(dirPath);
            Initial();
        }

        public void Initial()
        {
            InitialFile();
            InitialDir();
        }
        public void InitialFile()
        {
            FileInfo[] filearray = DirInfo.GetFiles();
            FileContainerArray = new FileContainer[filearray.Count()];
            for (int i = 0; i < FileContainerArray.Count(); i++)
            {
                FileContainerArray[i] = new FileContainer(filearray[i]);
            }
        }
        public void InitialDir()
        {
            DirectoryInfo[] dirarray = DirInfo.GetDirectories();
            if (dirarray.Count() > 0)
            {
                DirContainerArray = new DirContainer[dirarray.Count()];
                for (int i = 0; i < FileContainerArray.Count(); i++)
                {
                    DirContainerArray[i] = new DirContainer(dirarray[i].FullName);
                }
            }
            else
            {
                DirContainerArray = null;
            }
        }
        public void SaveDirInDisk(string savePath)
        {
            DirectoryInfo savetarget = new DirectoryInfo(savePath);
            DirectoryInfo subDir = savetarget.CreateSubdirectory(DirInfo.Name);
            foreach (var file in FileContainerArray)
            {
                file.SaveFileInDisk(subDir.FullName);
            }

            foreach (var Dir in DirContainerArray)
            {
                Dir.SaveDirInDisk(subDir.FullName);
            }
        }

        public MemoryStream GetFileFromMemory(string fileName)
        {
            foreach (var file in FileContainerArray)
            {
                if (file.Name == fileName)
                {
                    return file.FileFromMemory;
                }
            }
            foreach (var Dir in DirContainerArray)
            {
                var returnvalue = Dir.GetFileFromMemory(fileName);
                if (returnvalue != null)
                {
                    return returnvalue;
                }
            }

            return null;
        }
    }
}
