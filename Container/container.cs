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
        PRODUCITVE  , // 正常设备量产产生的任务；
        INS_CHECK   , // 作为核对检查准确性发布的任务；
        
    }

    public enum InspectSection
    {
        AVI,
        SVI,
        APP,
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
            get {
                return new List<string>{ "F_Drive", "G_Drive" , "H_Drive" , "I_Drive" , "J_Drive" ,
                    "K_Drive" , "L_Drive" , "M_Drive" , "N_Drive" , "O_Drive" , "P_Drive", "Q_Drive", "R_Drive", "S_Drive" };
                }
        }
    }

    public class PanelPathContainer
    {
        public string Panel_id { get; }
        public string Origin_image_path { get; }
        public string Result_path { get; }
        public string Pc_name { get; }
        public int Eq_id { get; }
        public string Disk_name { get; }

        public PanelPathContainer(string panel_id, string origin_image_path, string result_path,int eq_id, string pc_name,string disk_name)
        {
            this.Panel_id = panel_id;
            this.Origin_image_path = origin_image_path;
            this.Result_path = result_path;
            this.Eq_id = eq_id;
            this.Pc_name = pc_name;
            this.Disk_name = disk_name;
        }

    }

    public class JudgeTable
    {
        public Operator[] OperaterArray;
        public Defect[] DefectArray;
        public JudgeGrade[] JudgeArray;
        public JudgeGrade LastJudge;

        public JudgeTable()
        {
            //int arratlen = getnames
            OperaterArray = new Operator[3];
            DefectArray = new Defect[3];


        }
        
        public void AddDefect(Defect newdefect)
        {
            switch (newdefect.Section)
            {
                case InspectSection.AVI:

                    break;
                case InspectSection.SVI:

                    break;
                case InspectSection.APP:

                    break;
                default:
                    break;
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

    }

    public class PanelMission
    {
        public string PanelId;
        public int Repetition;
        public List<Defect> DefectList;
        public MissionType Type;
        public DateTime AddTime;
        public PanelPathContainer PanelPath;
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

        // TODO: Add Defect rank later;
        public string DefectCode
        {
            get
            {
                if (DefectList.Count != 0)
                {
                    return DefectList[0].DefectCode;
                }
                else
                {
                    return null;
                }
            }  }
        public string DefectName
        {
            get
            {
                if (DefectList.Count != 0)
                {
                    return DefectList[0].DefectName;
                }
                else
                {
                    return null;
                }
            }
        }


        public PanelMission(string panelId, MissionType type, PanelPathContainer panelPath, long missionnumber)
        {
            PanelId = panelId;
            Repetition = 1;                         // TODO:设置任务人员检查次数
            Type = type;
            AddTime = DateTime.Now;
            AviFinished = SviFinished = AppFinished = false;
            AppFinished = true; // 暂时不管；
            AviOp = SviOp = AppOp = null;
            AviDefect = SviDefect = AppDefect = null;
            
            PanelPath = panelPath;
            MissionNumber = missionnumber;
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

        }
    }

    public class Defect
    {
        public string DefectName;
        public string DefectCode;
        public InspectSection Section;     // where the defect generated(like "AVI" OR "SVI");
        public int[] Position;
    }
    

    public class Operator
    {
        public Operator(string name,string id)
        {
            Name = name;
            Id = id;
        }
        public string Name { get;}
        public string Id { get;}
    }

    public class PC
    {
        public int EqId { get; set; }
        public string PcIp { get; set; }
        public string PcName { get; set; }
        public string PcSide { get; set; }
        public bool IsPcInType(List<int> eq_id_list, List<string> pc_name_list, List<string> pc_side_list)
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

    public class PcContainer
    {
        public List<PC> pc_list_all { get; set; }
    }
}
