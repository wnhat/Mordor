﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Container
{
    public class Lot
    {
        public string MACHINENAME;
        public List<PanelMission> panelcontainer = new List<PanelMission>();
        public string TRAYGROUPNAME;
        public Lot(string lotId, PanelMission[] panelid)
        {
            TRAYGROUPNAME = lotId;
            panelcontainer.AddRange(panelid);
        }
        public Lot(string lotId)
        {
            TRAYGROUPNAME = lotId;
        }
        public Lot()
        {
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
        string PanelId;
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
        string PanelId;
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
        public PanelMissionFromMES mesPanel;
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
        public string AviResultPath;
        public string SviResultPath;
        public string MissionInfo;
        public InspectSection PcSection { get; set; }
        public Defect Defect;
        public JudgeGrade Judge;
        public Defect DefectU;                          // user JUDGE;
        public JudgeGrade JudgeU;                       // user JUDGE;
        public Operator Op;
        public DateTime FinishTime;
        public int sortint = 0;                         // 用于任务随机排序；
        public ExamMission() { }
        public ExamMission(string panelId, string avipath, string svipath, InspectSection pcSection, Defect defect, JudgeGrade judge, string info)
        {
            PanelId = panelId;
            AviResultPath = avipath;
            SviResultPath = svipath;
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
