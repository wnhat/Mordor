using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using Newtonsoft.Json;

namespace Container.MQMessage
{
    public enum MessageType
    {
        VERSION_CHECK,
        VERSION_ERROR,

        CLIENT_SEND_MISSION_RESULT = 10,
        CLIENT_SEND_EXAM_RESULT,
        CLINET_GET_PANEL_MISSION,
        CLINET_GET_PANEL_PATH,
        CLINET_GET_EXAM_MISSION_LIST,
        CLINET_GET_EXAMINFO,
        CLINET_CHECK_USER,

        CONTROLER_CLEAR_MISSION = 100,
        CONTROLER_ADD_MISSION,
        CONTROLER_REFRESH_EXAM,

        SERVER_SEND_MISSION = 200,
        SERVER_SEND_FINISH,
        SERVER_SEND_EORRO,
        SERVER_SEND_EXAMINFO,
        SERVER_SEND_PANEL_GREAD,
        SERVER_SEND_USER_FLASE,
        SERVER_SEND_USER_TRUE,
    }
    public enum MessageFieldName
    {
        MessageType,
        Version,
        Field1,
        Field2,
        Field3,
    }
    public class BaseMessage : NetMQMessage
    {
        public MessageType TheMessageType;
        public VersionCheckClass Version;

        public BaseMessage(NetMQMessage message)
        {
            TheMessageType = (MessageType)message[(int)MessageFieldName.MessageType].ConvertToInt32();
            Version = TransferToVersion(message[(int)MessageFieldName.Version].ConvertToString());
        }

        public BaseMessage(MessageType theMessageType, VersionCheckClass version)
        {
            TheMessageType = theMessageType;
            Version = version ?? throw new ArgumentNullException(nameof(version));
            this.Append((int)TheMessageType);
            this.Append(TransferToString(version));
        }
        string TransferToString(VersionCheckClass version)
        {
            return JsonConvert.SerializeObject(version, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
        VersionCheckClass TransferToVersion(string versionstring)
        {
            return JsonConvert.DeserializeObject<VersionCheckClass>(versionstring, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
    }
    public class PanelMissionRequestMessage : BaseMessage
    {
        // TODO: 将任务申请operator 添加为附件；
        public string FGcode;
        public ProductType productType;
        public PanelMissionRequestMessage(string fGcode, ProductType productType):base(MessageType.CLINET_GET_PANEL_MISSION)
        {
            FGcode = fGcode;
            this.productType = productType;
            this.Append(FGcode);
            this.Append(productType.ToString());
        }
        public PanelMissionRequestMessage(NetMQMessage theMessage):base()
        {
            FGcode = theMessage[1].ConvertToString();
            productType = (ProductType)Enum.Parse(typeof(ProductType),theMessage[2].ConvertToString());
        }
    }
    public class PanelMissionMessage : BaseMessage
    {
        public MissionLot ThePanelMissionLot;
        public PanelMissionMessage(NetMQMessage theMessage):base(theMessage)
        {
            ThePanelMissionLot = TransferToMission(theMessage[(int)MessageFieldName.Field1].ConvertToString());
        }
        public PanelMissionMessage(MessageType messageType, VersionCheckClass version, MissionLot panelMission) : base(messageType,version)
        {
            ThePanelMissionLot = panelMission;
            this.Append(TransferToString(ThePanelMissionLot));
        }
        string TransferToString(MissionLot panelMission)
        {
            return JsonConvert.SerializeObject(panelMission, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
        MissionLot TransferToMission(string missionstring)
        {
            return JsonConvert.DeserializeObject<MissionLot>(missionstring, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
    }
    public class UserCheckMessage : BaseMessage
    {
        public Operator TheOperator;
        public UserCheckMessage(MessageType type, VersionCheckClass version, Operator op) : base(type,version)
        {
            TheOperator = op;
            this.Append(TransferToString(TheOperator));
        }
        public UserCheckMessage(NetMQMessage theMessage):base(theMessage)
        {
            TheMessageType = (MessageType)theMessage[(int)MessageFieldName.MessageType].ConvertToInt32();
            TheOperator = TransferToOp(theMessage[(int)MessageFieldName.Field1].ConvertToString());
        }
        string TransferToString(Operator op)
        {
            return JsonConvert.SerializeObject(op, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
        Operator TransferToOp(string opstring)
        {
            return JsonConvert.DeserializeObject<Operator>(opstring, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
    }
    public class PanelResultMessage : BaseMessage
    {
        PanelMissionResult TheResult;
        public PanelResultMessage(MessageType type, VersionCheckClass version, PanelMissionResult result) : base(type,version)
        {
            TheResult = result;
            this.Append(TransferToString(result));
        }
        public PanelResultMessage(NetMQMessage massage):base(massage)
        {
            TheResult = TransferToResult(massage[(int)MessageFieldName.Field1].ConvertToString());
        }
        string TransferToString(PanelMissionResult result)
        {
            return JsonConvert.SerializeObject(result, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
        PanelMissionResult TransferToResult(string resultstring)
        {
            return JsonConvert.DeserializeObject<PanelMissionResult>(resultstring, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
    }
    public class ExamMissionMessage : BaseMessage
    {
        public string ExamRequestInfo;
        public List<ExamMission> ExamMissionList;
        public ExamMissionMessage(MessageType messageType, VersionCheckClass version, List<ExamMission> examMissionList,string examRequestInfo) : base(messageType,version)
        {
            this.ExamMissionList = examMissionList;
            this.ExamRequestInfo = examRequestInfo;
            this.Append(TransferToString(ExamMissionList));
            this.Append(new NetMQFrame(ExamRequestInfo,Encoding.UTF8));
        }
        public ExamMissionMessage(NetMQMessage message):base(message)
        {
            ExamMissionList = TransferToResult(message[(int)MessageFieldName.Field1].ConvertToString());
            ExamRequestInfo = message[(int)MessageFieldName.Field2].ConvertToString(Encoding.UTF8);
        }
        string TransferToString(List<ExamMission> result)
        {
            return JsonConvert.SerializeObject(result, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
        List<ExamMission> TransferToResult(string resultstring)
        {
            return JsonConvert.DeserializeObject<List<ExamMission>>(resultstring, new JsonSerializerSettings(){StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
    }
    public class ExamInfoMessage : BaseMessage
    {
        public string[] ExamInfoArray;
        public ExamInfoMessage(string[] examInfo, VersionCheckClass version) : base(MessageType.SERVER_SEND_EXAMINFO,version)
        {
            this.ExamInfoArray = examInfo;
            this.Append(TransferToString(ExamInfoArray));
        }
        public ExamInfoMessage(NetMQMessage message) : base(message)
        {
            ExamInfoArray = TransferToResult(message[(int)MessageFieldName.Field1].ConvertToString());
        }
        string TransferToString(string[] result)
        {
            return JsonConvert.SerializeObject(result, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
        string[] TransferToResult(string resultstring)
        {
            return JsonConvert.DeserializeObject<string[]>(resultstring, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
    }
    public class PanelPathMessage : BaseMessage
    {
        public Dictionary<string, List<PanelPathContainer>> panelPathDic;
        //序列化发送的Massage
        public PanelPathMessage(MessageType messageType, VersionCheckClass version, Dictionary<string, List<PanelPathContainer>> panelpathdic) : base(messageType,version)
        {
            panelPathDic = panelpathdic;
            this.Append(TransferToString(panelPathDic));
        }
        public PanelPathMessage(MessageType messageType, VersionCheckClass version, string[] panelidarray) : base(messageType,version)
        {
            panelPathDic = new Dictionary<string, List<PanelPathContainer>>();
            foreach (var item in panelidarray)
            {
                // 为了客户端请求任务时不多写一个传送ID List 的消息，用该字典装载请求的id信息；
                panelPathDic.Add(item,null);
            }
            this.Append(TransferToString(panelPathDic));
        }
        //对收到的Massage进行反序列化
        public PanelPathMessage(NetMQMessage message) : base(message)
        {
            panelPathDic = TransferToResult(message[(int)MessageFieldName.Field1].ConvertToString());
        }
        //序列化和反序列化实现
        string TransferToString(Dictionary<string, List<PanelPathContainer>> result)
        {
            return JsonConvert.SerializeObject(result, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
        Dictionary<string, List<PanelPathContainer>> TransferToResult(string resultstring)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, List<PanelPathContainer>>>(resultstring, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
    }
}