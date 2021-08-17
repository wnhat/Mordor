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
        J,
        W,
        D,
        E,
        F,
        G,  //LotGrade
        N,  //LotGrade
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
        NORMAL,
    }
    public enum Side
    {
        LEFT,
        RIGHT,
        NULL,
    }
    public enum DiskPart
    {
        E_Drive,
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
        public string EqName { get { return PcInfo.EqName; } }
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
            // 该操作IO延迟较高，可能引起反应迟钝；
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
            Dir = new DirContainer(dirPath,true);
        }
        public MemoryStream GetFile(string fileName)
        {
            return Dir.GetFileFromMemory(fileName);
        }
    }
    public class PanelMissionResult
    {
        public string PanelId;
        public JudgeGrade Judge;
        public Defect defect;
        public User Op;
        public PanelMissionResult(){ }
        public PanelMissionResult(JudgeGrade judge, Defect defect, User op, string panelId)
        {
            Judge = judge;
            this.defect = defect;
            Op = op;
            PanelId = panelId;
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
    public class PC
    {
        public string EqName { get
            {
                if (EqId>9)
                {
                    return "7CTCT" + EqId.ToString();
                }
                else
                {
                    return "7CTCT0" + EqId.ToString();
                }
            } }
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