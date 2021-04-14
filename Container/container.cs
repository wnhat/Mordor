using NetMQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container
{
    public enum JudgeGrade
    {
        S,
        A,
        T,
        Q,
        W,
        D,
        E,
        F,
        U,  //unfinish
    }

    public enum MissionType
    {
        PRODUCITVE, // 正常设备量产产生的任务；
        INS_CHECK, // 作为核对检查准确性发布的任务；

    }

    public enum InspectSection
    {
        AVI,
        SVI,
        APP,
        MTP,
        MVI,
        MAIN,
        PRE,
        POST,
        EXAM,
    }
    public enum Side
    {
        LEFT,
        RIGHT,
        NULL,
    }
    public enum DiskPart
    {
        F_Drive,
        G_Drive,
        H_Drive,
        I_Drive,
        J_Drive,
        K_Drive,
        L_Drive,
        M_Drive,
        N_Drive,
        O_Drive,
        P_Drive,
        Q_Drive,
        R_Drive,
        S_Drive,
    }
    public struct Disk_part
    {
        string F_DRIVE { get { return "F_Drive"; } }
        string G_DRIVE { get { return "G_Drive"; } }
        string H_DRIVE { get { return "H_Drive"; } }
        string I_DRIVE { get { return "I_Drive"; } }
        string J_DRIVE { get { return "J_Drive"; } }
        string K_DRIVE { get { return "K_Drive"; } }
        string L_DRIVE { get { return "L_Drive"; } }
        string M_DRIVE { get { return "M_Drive"; } }
        string N_DRIVE { get { return "N_Drive"; } }
        string O_DRIVE { get { return "O_Drive"; } }
        string P_DRIVE { get { return "P_Drive"; } }
        string Q_DRIVE { get { return "Q_Drive"; } }
        string R_DRIVE { get { return "R_Drive"; } }
        string S_DRIVE { get { return "S_Drive"; } }
        public List<string> getall
        {
            get
            {
                return new List<string>{ "F_Drive", "G_Drive" , "H_Drive" , "I_Drive" , "J_Drive" ,
                    "K_Drive" , "L_Drive" , "M_Drive" , "N_Drive" , "O_Drive" , "P_Drive", "Q_Drive", "R_Drive", "S_Drive" };
            }
        }
    }
    public class PanelPathContainer
    {
        public string PanelId { get; set; }
        public PC PcInfo;
        public DiskPart diskName;
        public PanelPathContainer(string panel_id, PC pc, DiskPart diskName)
        {
            PanelId = panel_id;
            PcInfo = pc;
            this.diskName = diskName;
        }
        public string EqId { get { return PcInfo.EqId.ToString(); } }
        public string DiskName { get { return diskName.ToString(); } }
        public InspectSection PcSection { get { return PcInfo.PcName; } }
        public string Origin_image_path
        {
            get
            {   // \\172.16.180.83\NetworkDrive\F_Drive\Defect Info\Origin
                string returnstring = "\\\\" + PcInfo.PcIp + "\\NetworkDrive\\" + DiskName + "\\Defect Info\\Origin\\" + PanelId;
                return returnstring;
            }
        }
        public string Result_path
        {
            get
            {   // \\172.16.180.83\NetworkDrive\F_Drive\Defect Info\Origin
                string returnstring = "\\\\" + PcInfo.PcIp + "\\NetworkDrive\\" + DiskName + "\\Defect Info\\Result\\" + PanelId;
                return returnstring;
            }
        }
    }
    public class PanelMissionResult
    {
        public string PanelId;
        public long MissionNumber;
        public JudgeGrade Judge;
        public Defect defect;
        public InspectSection Section; // AVI OR SVI OR APP;
        public Operator Op;
        public PanelMissionResult(){ }
        public PanelMissionResult(JudgeGrade judge, Defect defect, InspectSection section, Operator op, string panelId, long missionNumber)
        {
            Judge = judge;
            this.defect = defect;
            Section = section;
            Op = op;
            PanelId = panelId;
            MissionNumber = missionNumber;
        }
    }
    public class ExamMission:IComparable
    {
        public string PanelId;
        public string Result_path;
        public InspectSection PcSection { get; set; }
        public Defect Defect;
        public JudgeGrade Judge;
        public Defect DefectU;                          // op JUDGE;
        public JudgeGrade JudgeU;                       // op JUDGE;
        public Operator Op;
        public DateTime FinishTime;
        public int sortint;
        public ExamMission()
        {

        }
        public ExamMission(string panelId, string result_path, InspectSection pcSection, Defect defect, JudgeGrade judge)
        {
            PanelId = panelId;
            Result_path = result_path;
            PcSection = pcSection;
            Defect = defect;
            Judge = judge;
            sortint = new Random().Next();
        }
        public ExamMission(string panelId, InspectSection pcSection, Defect defect, JudgeGrade judge)
        {
            PanelId = panelId;
            PcSection = pcSection;
            Defect = defect;
            Judge = judge;
            sortint = new Random().Next();
        }
        public void SetPath(string path)
        {
            Result_path = path;
        }
        public void FinishExam(PanelMissionResult result)
        {
            this.DefectU = result.defect;
            this.JudgeU = result.Judge;
            this.Op = result.Op;
            this.FinishTime = DateTime.Now;
        }
        public int CompareTo(object obj)
        {
            ExamMission other = (ExamMission)obj;
            return sortint.CompareTo(other.sortint);
        }
    }
    public class PanelMission
    {
        public string PanelId;
        public int Repetition;
        public List<Defect> DefectList;
        public MissionType Type;
        public DateTime AddTime;
        public DateTime FinishTime;
        public PanelPathContainer AviPanelPath;
        public PanelPathContainer SviPanelPath;
        public PanelPathContainer AppPanelPath;
        public long MissionNumber;
        public bool AviFinished;
        public bool SviFinished;
        public bool AppFinished;
        public Operator AviOp;
        public Operator SviOp;
        public Operator AppOp;
        public Defect AviDefect;
        public Defect SviDefect;
        public Defect AppDefect;
        public JudgeGrade AviJudge;
        public JudgeGrade SviJudge;
        public JudgeGrade AppJudge;
        public JudgeGrade LastJudge;
        public bool finished { get { return AviFinished & SviFinished & AppFinished; } }
        public string AllDefect;
        // TODO: Add Defect rank later;
        public string DefectCode
        {
            get
            {
                if (DefectList.Count == 0)
                {
                    return "";
                }
                else
                {
                    string returnstring = "";
                    foreach (var item in DefectList)
                    {
                        returnstring = returnstring + item.DefectCode;
                    }
                    return returnstring;
                }
            }
        }
        public string DefectName
        {
            get
            {
                if (DefectList.Count == 0)
                {
                    return "";
                }
                else
                {
                    string returnstring = "";
                    foreach (var item in DefectList)
                    {
                        returnstring = returnstring + item.DefectName;
                    }
                    return returnstring;
                }
            }
        }
        public PanelMission(string panelId, MissionType type, long missionnumber, PanelPathContainer AvipanelPath, PanelPathContainer SvipanelPath = null, PanelPathContainer ApppanelPath = null)
        {
            PanelId = panelId;
            Repetition = 1;                         // TODO:设置任务人员检查次数;
            Type = type;
            AddTime = DateTime.Now;
            AviFinished = SviFinished = AppFinished = false;
            AppFinished = true;                     // 暂时不管;
            AviOp = SviOp = AppOp = null;
            AviDefect = SviDefect = AppDefect = null;
            AviPanelPath = AvipanelPath;
            SviPanelPath = SvipanelPath;
            AppPanelPath = ApppanelPath;
            MissionNumber = missionnumber;
            DefectList = new List<Defect>();
        }
        public bool Complete
        {
            get
            {
                return (AviFinished & SviFinished & AppFinished);
            }
        }
        public void AddResult(PanelMissionResult newresult)
        {
            switch (newresult.Section)
            {
                case InspectSection.AVI:
                    AviOp = newresult.Op;
                    AviJudge = newresult.Judge;
                    AviFinished = true;
                    AviDefect = newresult.defect;
                    break;
                case InspectSection.SVI:
                    SviOp = newresult.Op;
                    SviJudge = newresult.Judge;
                    SviFinished = true;
                    SviDefect = newresult.defect;
                    break;
                case InspectSection.APP:
                    AppOp = newresult.Op;
                    AppJudge = newresult.Judge;
                    AppFinished = true;
                    AppDefect = newresult.defect;
                    break;
                default:
                    break;
            }
            if (finished)
            {
                FinishTime = DateTime.Now;
            }
        }
    }
    public class Defect
    {
        public string DefectName;
        public string DefectCode;
        public InspectSection Section;     // where the defect generated(like "AVI" OR "SVI");
        public Defect(string defectName, string defectCode, InspectSection section)
        {
            DefectName = defectName;
            DefectCode = defectCode;
            Section = section;
        }
        public Defect() { }
    }
    public class Operator
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string PassWord { get; set; }
        public int MissionFinished { get; set; }
        public Operator() { }
        public Operator(string passWord, string name, string id)
        {
            PassWord = passWord;
            MissionFinished = 0;
            Name = name;
            Id = id;
        }
        public Operator(string passWord, string id)
        {
            PassWord = passWord;
            MissionFinished = 0;
            Id = id;
        }
        public bool CheckPassWord(string pw)
        {
            if (pw == PassWord)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class PC
    {
        public int EqId { get; set; }
        public string PcIp { get; set; }
        public InspectSection PcName { get; set; }
        public Side PcSide { get; set; }
        public bool IsPcInType(List<int> eq_id_list, InspectSection[] pc_name_list, Side[] pc_side_list)
        {
            if (eq_id_list.Contains(EqId) & pc_name_list.Contains(PcName) & pc_side_list.Contains(PcSide))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class InspectMission
    {
        public string PanelId;
        public string[] ImageNameList;                         // The image name in reuslt file which we need to inspect
        DirContainer Container;
        public long MissionNumber;
        public Defect[] DefectJudgeList;
        public InspectMission(PanelMission missioninfo, InspectSection section, string[] imageNameList)
        {
            PanelId = missioninfo.PanelId;
            switch (section)
            {
                case InspectSection.AVI:
                    Container = new DirContainer(missioninfo.AviPanelPath.Result_path);
                    break;
                case InspectSection.SVI:
                    Container = new DirContainer(missioninfo.SviPanelPath.Result_path);
                    break;
                case InspectSection.APP:
                    Container = new DirContainer(missioninfo.AppPanelPath.Result_path);
                    break;
                default:
                    break;
            }
            ImageNameList = imageNameList;
            MissionNumber = missioninfo.MissionNumber;
        }
        public InspectMission(ExamMission missioninfo, string[] imageNameList)
        {
            PanelId = missioninfo.PanelId;
            Container = new DirContainer(missioninfo.Result_path);
            ImageNameList = imageNameList;
        }
        public void SaveFileInDisk(string SavePath)
        {
            Container.SaveDirInDisk(SavePath);
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
            if (filearray.Count()>0)
            {
                FileContainerArray = new FileContainer[filearray.Count()];
                for (int i = 0; i < FileContainerArray.Count(); i++)
                {
                    FileContainerArray[i] = new FileContainer(filearray[i]);
                }
            }
        }
        public void InitialDir()
        {
            DirectoryInfo[] dirarray = DirInfo.GetDirectories();
            if (dirarray.Count() > 0)
            {
                DirContainerArray = new DirContainer[dirarray.Count()];
                for (int i = 0; i < dirarray.Count(); i++)
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
            if (FileContainerArray!= null)
            {
                foreach (var file in FileContainerArray)
                {
                    if (file.Name == fileName)
                    {
                        return file.FileFromMemory;
                    }
                }
            }
            if (DirContainerArray != null)
            {
                foreach (var Dir in DirContainerArray)
                {
                    var returnvalue = Dir.GetFileFromMemory(fileName);
                    if (returnvalue != null)
                    {
                        return returnvalue;
                    }
                }
            }
            return null;
        }
    }
}