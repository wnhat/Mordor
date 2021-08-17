using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Container
{
    public class MissionLot
    {
        public string MACHINENAME;
        public List<PanelMission> panelcontainer = new List<PanelMission>();
        public string TRAYGROUPNAME;
        public MissionLot(string lotId, PanelMission[] panelid)
        {
            TRAYGROUPNAME = lotId;
            panelcontainer.AddRange(panelid);
        }
        public MissionLot(string lotId)
        {
            TRAYGROUPNAME = lotId;
        }
        public MissionLot()
        {
        }
        public MissionLot(string mACHINENAME, List<PanelMission> panelcontainer, string tRAYGROUPNAME) : this(mACHINENAME)
        {
            this.panelcontainer = panelcontainer;
            TRAYGROUPNAME = tRAYGROUPNAME;
        }

        public int Count
        {
            get
            {
                return panelcontainer.Count();
            }
        }
        public void AddPanel(PanelMission panel)
        {
            panelcontainer.Add(panel);
        }
        public void PanelFinish(PanelMissionResult finishedpanel)
        {
            var panel = panelcontainer.Where(x => x.PanelId == finishedpanel.PanelId);
            if (panel.Count()==0)
            {
                string errorstring = String.Format("id:{0},在该LOT中没有相同ID 的产品存在；",finishedpanel.PanelId);
                throw new Exception(errorstring);
            }
            else
            {
                panel.First().AddResult(finishedpanel);
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
    public class PanelImageContainer
    {
        public string PanelId;
        public string MutiString;
        public string Eqid;
        bool MutiFlag = false;
        //如果有相同ID在不同设备出现时，设置该项为true，tostring将显示设备及投入时间
        DirContainer avidir;
        DirContainer svidir;
        public bool HasMajorFile
        {
            get
            {
                if (avidir.Contains(Parameter.AviImageNameList) && svidir.Contains(Parameter.SviImageNameList))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public PanelImageContainer(string panelId, PanelPathContainer avipath, PanelPathContainer svipath, bool mutiFlag = false)
        {
            PanelId = panelId;
            MutiFlag = mutiFlag;
            this.avidir = new DirContainer(avipath.ResultPath);
            this.svidir = new DirContainer(svipath.ResultPath);
            this.Eqid = avipath.EqId;
            MutiString = this.PanelId + " #" + avipath.EqId + " " + this.avidir.CreationTime.ToString("MM/dd HH:mm");
        }
        public PanelImageContainer(string panelId, string examinfo)
        {
            PanelId = panelId;
            MutiFlag = false;
            this.avidir = new DirContainer(Path.Combine(Parameter.AviExamFilePath, examinfo, panelId));
            this.svidir = new DirContainer(Path.Combine(Parameter.SviExamFilePath, examinfo, panelId));
        }
        public void Download()
        {
            if (!avidir.ReadComplete)
            {
                avidir.Read();
            }
            if (!svidir.ReadComplete)
            {
                svidir.Read();
            }
        }
        public void Save(string examinfo)
        {
            // 保存Exam 文件到指定任务中；
            avidir.SaveDirInDisk(Path.Combine(Parameter.AviExamFilePath, examinfo));
            svidir.SaveDirInDisk(Path.Combine(Parameter.SviExamFilePath, examinfo));
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
        public bool ReadComplete { get { return avidir.ReadComplete && svidir.ReadComplete; } }
        public Bitmap[] GetImage()
        {
            List<Bitmap> newlist = new List<Bitmap>();
            foreach (var imagename in Parameter.AviImageNameList)
            {
                newlist.Add(new Bitmap(this.avidir.GetFileFromMemory(imagename)));
            }
            foreach (var imagename in Parameter.SviImageNameList)
            {
                newlist.Add(new Bitmap(this.svidir.GetFileFromMemory(imagename)));
            }
            return newlist.ToArray();
        }
    }
    public class InspectMission
    {
        public string PanelId;
        public string[] ImageNameList;  // The image name in reuslt file which we need to inspect
        public Bitmap[] ImageArray;
        public InspectMission(PanelMission missioninfo)
        {
            PanelId = missioninfo.PanelId;
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
        public InspectMission(ExamMission missioninfo)
        {
            PanelId = missioninfo.PanelId;
            var newimagenamelist = Parameter.AviImageNameList.ToList();
            newimagenamelist.AddRange(Parameter.SviImageNameList);
            newimagenamelist.AddRange(Parameter.AppImageNameList);
            ImageNameList = newimagenamelist.ToArray();
            List<Bitmap> newimagearray = new List<Bitmap>();
            newimagearray.AddRange(InitialImage(missioninfo.AviResultPath, Parameter.AviImageNameList));
            newimagearray.AddRange(InitialImage(missioninfo.SviResultPath, Parameter.SviImageNameList));
            ImageArray = newimagearray.ToArray();
        }
        public Bitmap[] InitialImage(string filepath, string[] NameList)
        {
            DirContainer Container = new DirContainer(filepath, true);
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
        public Panel mesPanel;
        public MissionType Type;

        public DateTime AddTime = DateTime.Now;
        public DateTime FinishTime;

        public PanelPathContainer AviPanelPath;
        public PanelPathContainer SviPanelPath;
        public PanelPathContainer AppPanelPath;

        public User Op;
        public Defect DefectByOp;
        public JudgeGrade LastJudge = JudgeGrade.U;

        public string PanelId
        {
            get
            {
                return mesPanel.PanelId;
            }
        }
        public JudgeGrade LotGrade
        {
            get
            {
                if (PanelJudge == JudgeGrade.S)
                {
                    return JudgeGrade.G;
                }
                else
                {
                    return JudgeGrade.N;
                }
            }
        }
        public JudgeGrade PanelJudge
        {
            // 根据N站点的等级及判定结果确定等级
            get
            {
                if (LastJudge == JudgeGrade.E)
                {
                    return JudgeGrade.E;
                }

                if (mesPanel.LastDetailGrade == JudgeGrade.E.ToString())
                {
                    if (LastJudge == JudgeGrade.S)
                    {
                        return JudgeGrade.E;
                    }
                    else
                    {
                        return JudgeGrade.F;
                    }
                }
                else if (mesPanel.LastDetailGrade == JudgeGrade.T.ToString())
                {
                    if (LastJudge == JudgeGrade.S)
                    {
                        return JudgeGrade.S;
                    }
                    else
                    {
                        return JudgeGrade.F;
                    }
                }
                else if(mesPanel.LastDetailGrade == JudgeGrade.J.ToString())
                {
                    if (LastJudge == JudgeGrade.S)
                    {
                        return JudgeGrade.A;
                    }
                    else
                    {
                        return JudgeGrade.F;
                    }
                }
                else if(mesPanel.LastDetailGrade == JudgeGrade.D.ToString())
                {
                    if (LastJudge == JudgeGrade.S)
                    {
                        return JudgeGrade.W;
                    }
                    else
                    {
                        return JudgeGrade.F;
                    }
                }
                else
                {
                    return LastJudge;
                }
            }
        }
        public PanelMission(Panel panel, MissionType type, PanelPathContainer AvipanelPath, PanelPathContainer SvipanelPath = null, PanelPathContainer ApppanelPath = null)
        {
            mesPanel = panel;
            Type = type;

            AviPanelPath = AvipanelPath;
            SviPanelPath = SvipanelPath;
            AppPanelPath = ApppanelPath;
        }
        public void AddResult(PanelMissionResult newresult)
        {
            FinishTime = DateTime.Now;
            Op = newresult.Op;
            LastJudge = newresult.Judge;
            DefectByOp = newresult.defect;
        }
        public bool Complete
        {
            get
            {
                return LastJudge != JudgeGrade.U ? true : false;
            }
        }
    }
    public class ExamMission : AET_IMAGE_EXAM,IComparable
    {
        public string AviResultPath
        {
            get
            {
                return Path.Combine(Parameter.AviExamFilePath, Info, PanelId);
            }
        }
        public string SviResultPath
        {
            get
            {
                return Path.Combine(Parameter.SviExamFilePath, Info, PanelId);
            }
        }
        public AET_IMAGE_EXAM_RESULT Result;
        public DateTime FinishTime;
        public int sortint = 0;                         // 用于任务随机排序；
        public ExamMission():base() { }
        public void FinishExam(PanelMissionResult result)
        {
            this.Result = new AET_IMAGE_EXAM_RESULT {
                DefectCodeU = result.defect.DefectCode,
                DefectNameU = result.defect.DefectName,
                User = result.Op,
                InspectDate = DateTime.Now.ToString("yyyyMMddHHmmssffffff"),
                AET_IMAGE_EXAM = this,
                JudgeU = result.Judge.ToString(),
            };
        }
        public int CompareTo(object obj)
        {
            ExamMission other = (ExamMission)obj;
            return sortint.CompareTo(other.sortint);
        }
    }
    public class PanelMissionFromMES: Panel
    {
        public PanelMissionFromMES(XmlElement ele):base()
        {
            var id = ele.GetElementsByTagName("PANELID")[0];
            var pos = ele.GetElementsByTagName("PANELPOSITION")[0];
            var grade1 = ele.GetElementsByTagName("LOTGRADE")[0];
            var grade2 = ele.GetElementsByTagName("LOTDETAILGRADE")[0];
            var aoi1 = ele.GetElementsByTagName("PIAOI1PANELJUDGE")[0];
            var aoi2 = ele.GetElementsByTagName("PIAOI2PANELJUDGE")[0];
            var tfe = ele.GetElementsByTagName("TFEAOIPANELJUDGE")[0];
            var act = ele.GetElementsByTagName("ACTAOIPANELJUDGE")[0];

            if (id == null)
            {
                throw new MesMessageException("panelid 为空，请检查来自MES信息的完整性");
            }
            if (pos == null)
            {
                throw new MesMessageException("panel 的tray位置信息为空，请检查来自MES信息的完整性");
            }
            if (grade1 == null)
            {
                throw new MesMessageException("panel Lot Grade为空，请检查来自MES信息的完整性");
            }
            if (grade2 == null)
            {
                throw new MesMessageException("panel 在N站点的等级判定信息为空，请检查来自MES信息的完整性");
            }
            if (aoi1 == null)
            {
                throw new MesMessageException("panel judge1 为空，请检查来自MES信息的完整性");
            }
            if (aoi2 == null)
            {
                throw new MesMessageException("panel judge2 为空，请检查来自MES信息的完整性");
            }
            if (tfe == null)
            {
                throw new MesMessageException("panel judge3 为空，请检查来自MES信息的完整性");
            }
            if (act == null)
            {
                throw new MesMessageException("panel judge4 为空，请检查来自MES信息的完整性");
            }

            this.PanelId = id.InnerText;
            this.LastGrade = grade1.InnerText;
            this.LastDetailGrade = grade2.InnerText;
            this.PIAOI1PANELJUDGE = aoi1.InnerText == "F" ? 1 : 0;
            this.PIAOI2PANELJUDGE = aoi2.InnerText == "F" ? 1 : 0;
            this.TFEAOIPANELJUDGE = tfe.InnerText == "F" ? 1 : 0;
            this.ACTAOIPANELJUDGE = act.InnerText == "F" ? 1 : 0;
        }
        public JudgeGrade LOTDETAILGRADE
        {
            get
            {
                return (JudgeGrade)Enum.Parse(typeof(JudgeGrade), this.LastDetailGrade);
            }
        }
        public JudgeGrade LOTGRADE
        {
            get
            {
                return (JudgeGrade)Enum.Parse(typeof(JudgeGrade), this.LastGrade);
            }
        }
        public string GlassId
        {
            get
            {
                return PanelId.Substring(0,12);
            }
        }
        public string HalfId
        {
            get
            {
                return PanelId.Substring(0,13);
            }
        }
        public string BPLotId
        {
            get
            {
                return PanelId.Substring(0,9);
            }
        }
        public override string ToString()
        {
            return PanelId;
        }
    }
}
