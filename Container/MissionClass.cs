using System;
using System.Collections.Generic;
using System.Drawing;
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
        public override string ToString()
        {
            return PanelId;
        }
    }
    public class Lot
    {
        public List<PanelMission> panelcontainer = new List<PanelMission>();
        public int Count
        {
            get
            {
                return panelcontainer.Count();
            }
        }
        public string LotId;

        public Lot(string lotId, PanelMission[] panelid)
        {
            LotId = lotId;
            panelcontainer.AddRange(panelid);
        }
        public Lot(string lotId)
        {
            LotId = lotId;
        }
        public Lot()
        {
        }
        public void AddPanel(PanelMission panel)
        {
            panelcontainer.Add(panel);
        }
        public void PanelFinish(PanelMissionResult finishedpanel)
        {
            foreach (var item in panelcontainer)
            {
                if (item.PanelId == finishedpanel.PanelId)
                {
                    item.AddResult(finishedpanel);
                }
            }
        }
        public void PanelFinish(PanelMissionResult[] finishedpanel)
        {
            foreach (var item in finishedpanel)
            {
                this.PanelFinish(item);
            }
        }
        public bool Complete
        {
            get
            {
                foreach (var item in panelcontainer)
                {
                    if (!item.Complete)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
    public class PanelImageContainer : Panel
    {
        public string MutiString;
        bool MutiFlag = false;
        //如果有相同ID在不同设备出现时，设置该项为true，tostring将显示设备及投入时间
        DirContainer dir; // 默认不提前读取;
        string ResultPath;
        public InspectSection Section;
        public PanelPathContainer path;
        public bool HasMajorFile
        {
            get
            {
                if (Section == InspectSection.AVI)
                {
                    return Dir.Contains(Parameter.AviImageNameList);
                }
                else if(Section == InspectSection.SVI)
                {
                    return Dir.Contains(Parameter.SviImageNameList);
                }
                else
                {
                    return dir.Contains(Parameter.AppImageNameList);
                }
                
            }
        }
        public PanelImageContainer(string panelId, PanelPathContainer path, bool mutiFlag = false) : base(panelId)
        {
            this.path = path;
            MutiFlag = mutiFlag;
            ResultPath = path.ResultPath;
            this.dir = new DirContainer(ResultPath);
            Section = path.PcSection;
            MutiString = this.PanelId + " #" + path.EqId + " " + this.Dir.CreationTime.ToString("MM/dd HH:mm");
        }
        public PanelImageContainer(string panelId, string path,InspectSection section) : base(panelId)
        {
            ResultPath = path;
            this.dir = new DirContainer(ResultPath);
            Section = section;
        }
        public void Download()
        {
            if (!dir.ReadComplete)
            {
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
                return MutiString;
            }
            else
            {
                return base.ToString();
            }
        }
        public bool ReadComplete { get { return Dir.ReadComplete; } }
        public DirContainer Dir { get { return dir; } }
        public MemoryStream[] GetFile(string[] namelist)
        {
            if (!ReadComplete)
            {
                Download();
            }
            MemoryStream[] returnarray = new MemoryStream[namelist.Length];
            for (int i = 0; i < namelist.Length; i++)
            {
                returnarray[i] = dir.GetFileFromMemory(namelist[i]);
            }
            return returnarray;
        }
    }
    public class InspectMission : Panel
    {
        public string[] ImageNameList;// The image name in reuslt file which we need to inspect
        public Bitmap[] ImageArray;
        public InspectMission(PanelMission missioninfo) : base(missioninfo.PanelId)
        {
            var newimagenamelist = Parameter.AviImageNameList.ToList();
            newimagenamelist.AddRange(Parameter.SviImageNameList);
            newimagenamelist.AddRange(Parameter.AppImageNameList);
            ImageNameList = newimagenamelist.ToArray();
            List<Bitmap> newimagearray = new List<Bitmap>();
            newimagearray.AddRange(InitialImage(missioninfo.AviPanelPath.ResultPath, Parameter.AviImageNameList));
            newimagearray.AddRange(InitialImage(missioninfo.SviPanelPath.ResultPath, Parameter.SviImageNameList));
            //newimagearray.AddRange(InitialImage(missioninfo.AppPanelPath.ResultPath, Parameter.AppImageNameList));
            ImageArray = newimagearray.ToArray();
        }
        public InspectMission(ExamMission missioninfo) : base(missioninfo.PanelId)
        {
            if (missioninfo.PcSection == InspectSection.AVI)
            {
                ImageNameList = Parameter.AviImageNameList;
            }
            else if (missioninfo.PcSection == InspectSection.SVI)
            {
                ImageNameList = Parameter.SviImageNameList;
            }
            else if (missioninfo.PcSection == InspectSection.APP)
            {
                ImageNameList = Parameter.AppImageNameList;
            }
            ImageArray = InitialImage(missioninfo.ResultPath, ImageNameList);
        }
        public Bitmap[] InitialImage(string filepath, string[] NameList)
        {
            DirContainer Container = new DirContainer(filepath,true);
            Bitmap[] NewImageArray = new Bitmap[NameList.Length];
            for (int i = 0; i < NameList.Length; i++)
            {
                var file = Container.GetFileFromMemory(NameList[i]);
                if (file != null)
                {
                    NewImageArray[i] = new Bitmap(file);
                }
                else
                {
                    string errorstring = string.Format("文件夹中缺少必要文件，panel id：{0},file name {1}", PanelId, NameList[i]);
                    throw new FileNotFoundException(errorstring);
                }
            }
            return NewImageArray;
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
        public Operator Op;
        public JudgeGrade LastJudge = JudgeGrade.U;
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
        public PanelMission(string panelId, MissionType type, PanelPathContainer AvipanelPath, PanelPathContainer SvipanelPath = null, PanelPathContainer ApppanelPath = null)
        {
            PanelId = panelId;
            Repetition = 1;                         // TODO:设置任务人员检查次数;
            Type = type;
            AddTime = DateTime.Now;
            AviPanelPath = AvipanelPath;
            SviPanelPath = SvipanelPath;
            AppPanelPath = ApppanelPath;
            DefectList = new List<Defect>();
        }
        public void AddResult(PanelMissionResult newresult)
        {
            FinishTime = DateTime.Now;
            Op = newresult.Op;
            LastJudge = newresult.Judge;
            DefectList.Add(newresult.defect);
        }
        public bool Complete
        {
            get
            {
                return LastJudge != JudgeGrade.U ? true : false;
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
        public Defect DefectU;                          // user JUDGE;
        public JudgeGrade JudgeU;                       // user JUDGE;
        public Operator Op;
        public DateTime FinishTime;
        public int sortint = 0;                             // 用于任务随机排序；
        public ExamMission()
        {

        }
        public ExamMission(string panelId, string result_path, InspectSection pcSection, Defect defect, JudgeGrade judge, string info)
        {
            PanelId = panelId;
            ResultPath = result_path;
            PcSection = pcSection;
            Defect = defect;
            Judge = judge;
            MissionInfo = info;
        }
        public ExamMission(string panelId, InspectSection pcSection, Defect defect, JudgeGrade judge, string info)
        {
            PanelId = panelId;
            PcSection = pcSection;
            Defect = defect;
            Judge = judge;
            MissionInfo = info;
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
