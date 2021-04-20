using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container
{
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
    public class ExamMission : IComparable
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
}
