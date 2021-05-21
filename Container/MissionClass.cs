using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container
{
    public class Panel
    {
        string panelId;
        public Panel(string panelId)
        {
            this.panelId = panelId;
        }
        public string PanelId
        {
            // TODO: 校验ID是否符合编码规范；
            get
            {
                return panelId;
            }
        }
        public string GlassId
        {
            get
            {
                return panelId;
            }
        }
        public string HalfId
        {
            get
            {
                return panelId;
            }
        }
        public string LotId
        {
            get
            {
                return panelId;
            }
        }
    }
    public class PanelImageContainer:Panel
    {
        bool MutiFlag;  //如果有相同ID在不同设备出现时，设置该项为true，tostring将显示设备及投入时间
        PanelPathContainer Path;
        DirContainer dir = null; // 默认不提前读取;
        public PanelImageContainer(string panelId, PanelPathContainer path, bool mutiFlag = false):base(panelId)
        {
            MutiFlag = mutiFlag;
            Path = path;
        }
        public void Download()
        {
            if (dir == null || !dir.ReadComplete)
            {
                this.dir = new DirContainer(this.Path.ResultPath);
                Dir.Read();
            }
        }
        public void Save(string savepath)
        {
            Dir.SaveDirInDisk(savepath);
        }
        public override string ToString()
        {
            if (this.MutiFlag)
            {
                string name = this.PanelId+ " #" + this.Path.EqId + " " + this.Dir.CreationTime.ToString("MM/dd HH:mm");
                return name;
            }
            else
            {
                return base.ToString();
            }
        }
        InspectSection Section { get { return Path.PcSection; } }
        bool ReadComplete { get { return Dir.ReadComplete && (dir!=null); } }
        public DirContainer Dir { get { return dir; } }
    }
    public class InspectMission : Panel
    {
        public string[] ImageNameList;                         // The image name in reuslt file which we need to inspect
        DirContainer Container;
        public long MissionNumber;
        public Defect[] DefectJudgeList;
        public InspectMission(PanelMission missioninfo, InspectSection section, string[] imageNameList) : base(missioninfo.PanelId)
        {
            switch (section)
            {
                case InspectSection.AVI:
                    Container = new DirContainer(missioninfo.AviPanelPath.ResultPath);
                    break;
                case InspectSection.SVI:
                    Container = new DirContainer(missioninfo.SviPanelPath.ResultPath);
                    break;
                case InspectSection.APP:
                    Container = new DirContainer(missioninfo.AppPanelPath.ResultPath);
                    break;
                default:
                    break;
            }
            ImageNameList = imageNameList;
            MissionNumber = missioninfo.MissionNumber;
        }
        public InspectMission(ExamMission missioninfo, string[] imageNameList):base(missioninfo.PanelId)
        {
            Container = new DirContainer(missioninfo.ResultPath);
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
    /// <summary>
    /// 
    /// </summary>
    public class ExamMission : IComparable
    {
        public string PanelId;
        public string ResultPath;
        public string MissionInfo;
        public InspectSection PcSection { get; set; }
        public bool Exsit { get { return new DirectoryInfo(ResultPath).Exists; } }
        public Defect Defect;
        public JudgeGrade Judge;
        public Defect DefectU;                          // op JUDGE;
        public JudgeGrade JudgeU;                       // op JUDGE;
        public Operator Op;
        public DateTime FinishTime;
        public int sortint;                             // 用于任务随机排序；
        public ExamMission()
        {

        }
        public ExamMission(string panelId, string result_path, InspectSection pcSection, Defect defect, JudgeGrade judge)
        {
            PanelId = panelId;
            ResultPath = result_path;
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
            ResultPath = path;
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
}
