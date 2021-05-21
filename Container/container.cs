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
    public class PanelPathContainer:IComparable
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
        public string OriginImagePath
        {
            get
            {   // \\172.16.180.83\NetworkDrive\F_Drive\Defect Info\Origin
                string returnstring = "\\\\" + PcInfo.PcIp + "\\NetworkDrive\\" + DiskName + "\\Defect Info\\Origin\\" + PanelId;
                return returnstring;
            }
        }
        public string ResultPath
        {
            get
            {   // \\172.16.180.83\NetworkDrive\F_Drive\Defect Info\Origin
                string returnstring = "\\\\" + PcInfo.PcIp + "\\NetworkDrive\\" + DiskName + "\\Defect Info\\Result\\" + PanelId;
                return returnstring;
            }
        }
        public override string ToString()
        {
            return PanelId;
        }

        public int CompareTo(object obj)
        {
            DirectoryInfo self = new DirectoryInfo(ResultPath);
            var other = (PanelPathContainer)obj;
            DirectoryInfo otherdir = new DirectoryInfo(other.ResultPath);
            return self.CreationTime.CompareTo(otherdir.CreationTime);
        }
    }
    public class PanelInfo
    {
        public string PanelId;
        public List<PanelPathContainer> PanelPath;
        public InspectSection PcSection { get; set; }
        public int Length { get { return PanelPath.Count; } }
        public PanelInfo()
        {

        }
        public PanelInfo(string panelId, List<PanelPathContainer> panelPath, InspectSection pcSection, Defect defect)
        {
            PanelId = panelId;
            PanelPath = panelPath;
            PcSection = pcSection;
        }
        public PanelInfo(string panelId, InspectSection pcSection)
        {
            PanelId = panelId;
            PcSection = pcSection;
        }
        public override string ToString()
        {
            return PanelId;
        }
    }
    public class ImageContainer
    {
        public string Name;
        public InspectSection Section;
        private DirContainer Dir = null;
        public bool ReadComplete
        {
            get
            {
                if (Dir == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public void Read(string dirPath)
        {
            Dir = new DirContainer(dirPath);
        }
        public MemoryStream GetFile(string fileName)
        {
            return Dir.GetFileFromMemory(fileName);
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
        public override string ToString()
        {
            return DefectName;
        }
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
}