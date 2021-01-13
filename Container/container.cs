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
    }

    public enum MissionType
    {
        PRODUCITVE  , // 正常设备量产产生的任务；
        INS_CHECK   , // 作为核对检查准确性发布的任务；
        
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

    public class container
    {

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

    public class PanelMission
    {
        public string PanelId;
        public int Repetition;
        public PanelJudgeTable PanelJudge;
        public MissionType Type;
        public DateTime AddTime;
        public bool Complete;
        public PanelPathContainer PanelPath;
        public long MissionNumber;

        public PanelMission(string panelId, MissionType type, PanelPathContainer panelPath)
        {
            PanelId = panelId;
            Repetition = 1; // TODO:设置任务人员检查
            PanelJudge = new PanelJudgeTable();
            Type = type;
            AddTime = DateTime.Now;
            Complete = false;
            PanelPath = panelPath;
            MissionNumber = DateTime.Now.ToBinary();
        }

        public PanelJudgeTable GetResult()
        {
            return PanelJudge;
        }

        public bool IsComplete()
        {
            return Complete;
        }

    }

    public class PanelJudgeTable
    {

    }

    public class PanelJudgement
    {
        public Operator Op;
        public JudgeGrade Judge;

    }

    public class Operator
    {
        public string Name { get; set; }
        public string Id { get; set; }
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

    public class PcManager : PC
    {
        public List<string> log_list { get; set; }
        // TODO: use log lib;
    }
}
