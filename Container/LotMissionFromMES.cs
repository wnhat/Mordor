using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Container
{
    public class LotMissionFromMES
    {
        public string MACHINENAME;
        public string TRAYGROUPNAME;
        public List<PanelMissionFromMES> TRAYLIST;
        public LotMissionFromMES(string mACHINENAME, string tRAYGROUPNAME, List<PanelMissionFromMES> tRAYLIST)
        {
            MACHINENAME = mACHINENAME;
            TRAYGROUPNAME = tRAYGROUPNAME;
            TRAYLIST = tRAYLIST;
        }
    }
    public class PanelMissionFromMES
    {
        public string PANELID;
        public string PANELPOSITION;
        public LotGrade LOTGRADE;
        public JudgeGrade LOTDETAILGRADE;
        public string PIAOI1PANELJUDGE;
        public string PIAOI2PANELJUDGE;
        public string TFEAOIPANELJUDGE;
        public string ACTAOIPANELJUDGE;

        public PanelMissionFromMES(string panelId)
        {
            this.PANELID = panelId;
        }
        public PanelMissionFromMES(XmlElement ele)
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
                throw new MesMessageException("panelid judge2 为空，请检查来自MES信息的完整性");
            }
            if (tfe == null)
            {
                throw new MesMessageException("panelid judge3 为空，请检查来自MES信息的完整性");
            }
            if (act == null)
            {
                throw new MesMessageException("panelid judge4 为空，请检查来自MES信息的完整性");
            }
            PANELID = id.InnerText;
            PANELPOSITION = pos.InnerText;
            LOTGRADE = (LotGrade)Enum.Parse(typeof(LotGrade), grade1.InnerText);
            LOTDETAILGRADE = (JudgeGrade)Enum.Parse(typeof(JudgeGrade), grade2.InnerText);
            PIAOI1PANELJUDGE = aoi1.InnerText;
            PIAOI2PANELJUDGE = aoi2.InnerText;
            TFEAOIPANELJUDGE = tfe.InnerText;
            ACTAOIPANELJUDGE = act.InnerText;
        }
        public string PanelId
        {
            // TODO: 校验ID是否符合编码规范；
            get
            {
                return PANELID;
            }
        }
        public string GlassId
        {
            get
            {
                return PANELID.Substring(0,12);
            }
        }
        public string HalfId
        {
            get
            {
                return PANELID.Substring(0,13);
            }
        }
        public string BPLotId
        {
            get
            {
                return PANELID.Substring(0,9);
            }
        }
        public override string ToString()
        {
            return PanelId;
        }
    }
}
