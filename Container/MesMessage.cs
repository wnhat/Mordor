﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TIBCO.Rendezvous;

namespace Container
{
    [System.Serializable]
    public class MesMessageException : System.Exception
    {
        public MesMessageException() { }
        public MesMessageException(string message) : base(message) { }
        public MesMessageException(string message, System.Exception inner) : base(message, inner) { }
        protected MesMessageException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    public class RemoteTrayGroupInfoDownloadRequest
    {
        MesMessageHeader header;
        RemoteTrayGroupInfoDownloadRequestMessageBody Body;
        MesMessageReturn rt;
        public RemoteTrayGroupInfoDownloadRequest(string FGcode, string productType)
        {
            header   = new MesMessageHeader(MesMessageType.RemoteTrayGroupInfoDownloadRequest);
            Body     = new RemoteTrayGroupInfoDownloadRequestMessageBody(FGcode, productType);
            rt = new MesMessageReturn();
        }
        public XmlDocument GetXmlDocument()
        {
            XmlDocument newDoc = new XmlDocument();
            XmlElement root = newDoc.DocumentElement;
            newDoc.InsertBefore(newDoc.CreateXmlDeclaration("1.0", "utf-16", null), root);
            XmlElement newmessage = newDoc.CreateElement("Message");

            newmessage.AppendChild(header.GetElement(newDoc));
            newmessage.AppendChild(Body.GetElement(newDoc));
            newmessage.AppendChild(rt.GetElement(newDoc));

            newDoc.AppendChild(newmessage);
            return newDoc;
        }
        public Message GetMessage()
        {
            Message newmessage = new Message();
            newmessage.AddField("xmlData", GetXmlDocument().InnerXml);
            return newmessage;
        }
        public void SaveDoc(string path)
        {
            var doc = GetXmlDocument();
            doc.Save(path);
        }
    }
    public class RemoteTrayGroupInfoDownloadRequestMessageBody
    {
        public string PRODUCTSPECNAME; //FG CODE;
        public string PRODUCTIONTYPE;
        public StationType PROCESSOPERATIONNAME;
        public string MACHINENAME;

        public RemoteTrayGroupInfoDownloadRequestMessageBody(string pRODUCTSPECNAME, string pRODUCTIONTYPE)
        {
            PRODUCTSPECNAME = pRODUCTSPECNAME;
            PRODUCTIONTYPE = pRODUCTIONTYPE;
            PROCESSOPERATIONNAME = StationType.C52000E;
            MACHINENAME = "7CTCT33";
        }
        public XmlElement GetElement(XmlDocument doc)
        {
            XmlElement newele = doc.CreateElement("Body");

            var newNodeList = GetXmlNodeList(doc);
            foreach (var item in newNodeList)
            {
                newele.AppendChild(item);
            }
            return newele;
        }
        public List<XmlNode> GetXmlNodeList(XmlDocument doc)
        {
            List<XmlNode> newlist = new List<XmlNode>();
            Type T = typeof(RemoteTrayGroupInfoDownloadRequestMessageBody);
            var filed = T.GetFields();
            foreach (var item in filed)
            {
                XmlNode newNode = doc.CreateNode(XmlNodeType.Element, item.Name, "");
                var a = item.GetValue(this);
                string newText = a.ToString();
                newNode.InnerText = newText;
                newlist.Add(newNode);
            }
            return newlist;
        }
    }
    public class RemoteTrayGroupInfoDownloadSend
    {
        XmlDocument OriginalDoc;
        public TrayLot lot;
        public RemoteTrayGroupInfoDownloadSend(XmlDocument replyDoc)
        {
            OriginalDoc = replyDoc;
            lot = new TrayLot { TrayGroupName = InitialField("TRAYGROUPNAME"), MachineName = InitialField("MACHINENAME"), AddTime = DateTime.Now.ToString("yyyyMMddHHmmssffffff") };
            List<Panel> missionList = InitialMission();
            foreach (var item in missionList)
            {
                lot.Panel.Add(item);
            }
        }
        private List<Panel> InitialMission()
        {
            var nodelist = OriginalDoc.GetElementsByTagName("PANEL");
            if (nodelist.Count == 0)
            {
                throw new MesMessageException("该返回消息中没有Panel的信息，请检查Mes消息的完整性；");
            }
            else
            {
                List<Panel> newlist = new List<Panel>();
                foreach (var item in nodelist)
                {
                    Panel newpanel = new PanelMissionFromMES((XmlElement)item);
                    newlist.Add(newpanel);
                }
                return newlist;
            }
        }
        string InitialField(string tagName)
        {
            var tRAYGROUPNAME = OriginalDoc.GetElementsByTagName(tagName)[0];
            if (tRAYGROUPNAME != null)
            {
                return tRAYGROUPNAME.InnerText;
            }
            else
            {
                string errorString = String.Format("{0}为空，请检查MES消息的完整性", tagName);
                throw new MesMessageException(errorString);
            }
        }
        public void Save(string path)
        {
            OriginalDoc.Save(path);
        }
    }
    public class RemoteTrayGroupProcessEnd
    {       
        MesMessageHeader header;
        RemoteTrayGroupProcessEndMessageBody Body;
        MesMessageReturn rt;
        public RemoteTrayGroupProcessEnd(MissionLot lot)
        {
            header  = new MesMessageHeader(MesMessageType.RemoteTrayGroupProcessEnd);
            Body    = new RemoteTrayGroupProcessEndMessageBody(lot);
            rt = new MesMessageReturn();
        }
        public XmlDocument GetXmlDocument()
        {
            XmlDocument newDoc = new XmlDocument();
            XmlElement root = newDoc.DocumentElement;
            newDoc.InsertBefore(newDoc.CreateXmlDeclaration("1.0", "utf-16", null), root);
            XmlElement newmessage = newDoc.CreateElement("Message");

            newmessage.AppendChild(header.GetElement(newDoc));
            newmessage.AppendChild(Body.GetElement(newDoc));
            newmessage.AppendChild(rt.GetElement(newDoc));

            newDoc.AppendChild(newmessage);
            return newDoc;
        }

        public Message GetMessage()
        {
            Message newmessage = new Message();
            newmessage.AddField("xmlData", GetXmlDocument().InnerXml);
            return newmessage;
        }
        public void Save(string path)
        {
            var document = GetXmlDocument();
            document.Save(path);
        }
    }
    public class RemoteTrayGroupProcessEndMessageBody
    {
        MissionLot finishedlot;

        public RemoteTrayGroupProcessEndMessageBody(MissionLot lot)
        {
            finishedlot = lot;
        }
        public XmlElement GetElement(XmlDocument doc)
        {
            XmlElement newele = doc.CreateElement("Body");

            var lotid = doc.CreateNode(XmlNodeType.Element, "TRAYGROUPNAME", "");
            lotid.InnerText = finishedlot.TRAYGROUPNAME;
            newele.AppendChild(lotid);

            var eqid = doc.CreateNode(XmlNodeType.Element, "MACHINENAME", "");
            eqid.InnerText = finishedlot.MACHINENAME;
            newele.AppendChild(eqid);

            var newPanelList = GetPanelList(doc);
            newele.AppendChild(newPanelList);

            return newele;
        }
        public XmlElement GetPanelList(XmlDocument doc)
        {
            XmlElement newele = doc.CreateElement("PANELLIST");
            foreach (var item in finishedlot.panelcontainer)
            {
                var newpanel = doc.CreateNode(XmlNodeType.Element, "PANEL", "");
                var PANELNAME = doc.CreateNode(XmlNodeType.Element, "PANELNAME", "");
                var LOTGRADE = doc.CreateNode(XmlNodeType.Element, "LOTGRADE", "");
                var LOTDETAILGRADE = doc.CreateNode(XmlNodeType.Element, "LOTDETAILGRADE", "");
                var USERID = doc.CreateNode(XmlNodeType.Element, "USERID", "");

                PANELNAME.InnerText = item.PanelId;
                newpanel.AppendChild(PANELNAME);
                LOTGRADE.InnerText = item.LotGrade.ToString();
                newpanel.AppendChild(LOTGRADE);
                LOTDETAILGRADE.InnerText = item.PanelJudge.ToString();
                newpanel.AppendChild(LOTDETAILGRADE);
                USERID.InnerText = item.Op.UserId;
                newpanel.AppendChild(USERID);

                var DEFECTLIST = doc.CreateNode(XmlNodeType.Element, "DEFECTLIST", "");
                var DEFECT = doc.CreateNode(XmlNodeType.Element, "DEFECT", "");
                var DEFECTCODE = doc.CreateNode(XmlNodeType.Element, "DEFECTCODE", "");
                DEFECTCODE.InnerText = item.DefectByOp.DefectCode;
                DEFECT.AppendChild(DEFECTCODE);
                DEFECTLIST.AppendChild(DEFECT);
                newpanel.AppendChild(DEFECTLIST);

                newele.AppendChild(newpanel);
            }

            return newele;
        }
    }
    public class RemoteTrayGroupProcessEndReply
    {
        XmlDocument OriginalDoc;
        public string TRAYGROUPNAME;
        public string RESULT;
        public string DESCRIPTION;
        public bool Result
        {
            get
            {
                if (RESULT == "OK")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public RemoteTrayGroupProcessEndReply(XmlDocument doc)
        {
            OriginalDoc = doc;
            TRAYGROUPNAME = InitialField("TRAYGROUPNAME");
            RESULT = InitialField("RESULT");
            DESCRIPTION = InitialField("DESCRIPTION");
        }
        string InitialField(string tagName)
        {
            var tRAYGROUPNAME = OriginalDoc.GetElementsByTagName(tagName)[0];
            if (tRAYGROUPNAME != null)
            {
                return tRAYGROUPNAME.InnerText;
            }
            else
            {
                string errorString = String.Format("{0}为空，请检查MES消息的完整性", tagName);
                throw new MesMessageException(errorString);
            }
        }
    }
    public class OpCallSend
    {
        XmlDocument OriginalDoc;
        public string MACHINENAME;
        public string OPCALLDESCRIPTION;
        public OpCallSend(XmlDocument doc)
        {
            OriginalDoc = doc;
            MACHINENAME = InitialField("MACHINENAME");
            OPCALLDESCRIPTION = InitialField("OPCALLDESCRIPTION");
        }
        string InitialField(string tagName)
        {
            var tRAYGROUPNAME = OriginalDoc.GetElementsByTagName(tagName)[0];
            if (tRAYGROUPNAME != null)
            {
                return tRAYGROUPNAME.InnerText;
            }
            else
            {
                string errorString = String.Format("{0}为空，请检查MES消息的完整性", tagName);
                throw new MesMessageException(errorString);
            }
        }
    }
    public class MesMessageHeader
    {
        public MesMessageType MESSAGENAME;  // 声明消息的用途（请求任务或完成任务）PanelProcessEnd等；
        public string TRANSACTIONID;  //要求唯一，作为MES分辨消息的查询依据（可能是数据库主键）2021072017235803711；
        public string ORIGINALSOURCESUBJECTNAME = "";
        public string SOURCESUBJECTNAME = "BOE.B7.MEM.PRD.7CTCT33";
        public string TARGETSUBJECTNAME = "BOE.B7.MEM.PRD.PEMsvr";
        public string SHOPNAME = "EAC2";
        public string MACHINENAME = "7CTCT33";

        public MesMessageHeader(MesMessageType mESSAGENAME)
        {
            MESSAGENAME = mESSAGENAME;
            // 每次发送时应更新
            TRANSACTIONID = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        }
        public XmlElement GetElement(XmlDocument doc)
        {
            XmlElement newele = doc.CreateElement("Header");

            var newNodeList = GetXmlNodeList(doc);
            foreach (var item in newNodeList)
            {
                newele.AppendChild(item);
            }
            return newele;
        }
        public List<XmlNode> GetXmlNodeList(XmlDocument doc)
        {
            List<XmlNode> newlist = new List<XmlNode>();
            Type T = typeof(MesMessageHeader);
            var filed = T.GetFields();
            foreach (var item in filed)
            {
                XmlNode newNode = doc.CreateNode(XmlNodeType.Element, item.Name, "");
                string newText = item.GetValue(this).ToString();
                newNode.InnerText = newText;
                newlist.Add(newNode);
            }
            return newlist;
        }
    }
    public class MesMessageReturn
    {
        string RETURNCODE;
        string RETURNMESSAGE;

        public MesMessageReturn()
        {
            RETURNCODE = "0";
            RETURNMESSAGE = "";
        }
        public XmlElement GetElement(XmlDocument doc)
        {
            XmlElement newele = doc.CreateElement("Return");
            XmlElement rcode = doc.CreateElement("RETURNCODE");
            rcode.InnerText = RETURNCODE;
            XmlElement rmessage = doc.CreateElement("RETURNMESSAGE");
            rmessage.InnerText = RETURNMESSAGE;
            newele.AppendChild(rcode);
            newele.AppendChild(rmessage);
            return newele;
        }
    }
    public enum MesMessageType
    {
        RemoteTrayGroupInfoDownloadRequest,     //向MES请求任务并hold lot；
        OpCallSend,                             //如果发送的请求失败（如E站点没有待检LOT），返回的报错信息；
        RemoteTrayGroupInfoDownloadSend,        //从MES发回的任务，包含整个lot 的信息；
        RemoteTrayGroupProcessEnd,              //LOT检查完成，向MES发送检查结果；
        RemoteTrayGroupProcessEndReply,         //MES回复检查收到检查结果是否处理成功；
    }
    public enum ProductType
    {
        // 大小写与MES保持严格一致；
        Production,
        PV,
        TPCN,
        Develop,
        Engineer,
    }
    public enum StationType
    {
        C52000N,
        C52000E,
        C52000R,
    }
}
