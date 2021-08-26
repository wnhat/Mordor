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
        CLINET_GET_PRODUCTINFO,

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
        SERVER_SEND_PRODUCTINFO,
    }
    public enum MessageFieldName
    {
        MessageType,
        Version,
        Field1,
        Field2,
        Field3,
    }
    public static class JsonSerializerSetting
    {
        public static JsonSerializerSettings Setting;
        static JsonSerializerSetting()
        {
            Setting = new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        }
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
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }
            Version = version;
            this.Append((int)TheMessageType);
            this.Append(TransferToString(version));
        }
        string TransferToString(VersionCheckClass version)
        {
            return JsonConvert.SerializeObject(version, JsonSerializerSetting.Setting);
        }
        VersionCheckClass TransferToVersion(string versionstring)
        {
            return JsonConvert.DeserializeObject<VersionCheckClass>(versionstring, JsonSerializerSetting.Setting);
        }
    }
    public class ProductInfoMessage:BaseMessage
    {
        public List<ProductInfo> InfoList;
        public ProductInfoMessage(List<ProductInfo> list):base(MessageType.SERVER_SEND_PRODUCTINFO,ServerVersion.Version)
        {
            InfoList = list;
            this.Append(TransferToString(InfoList));
        }
        public ProductInfoMessage(NetMQMessage theMessage) : base(theMessage)
        {
            InfoList = TransferToMission(theMessage[(int)MessageFieldName.Field1].ConvertToString());
        }
        string TransferToString(List<ProductInfo> panelMission)
        {
            return JsonConvert.SerializeObject(panelMission, JsonSerializerSetting.Setting);
        }
        List<ProductInfo> TransferToMission(string missionstring)
        {
            return JsonConvert.DeserializeObject<List<ProductInfo>>(missionstring, JsonSerializerSetting.Setting);
        }
    }
    public class PanelMissionRequestMessage : BaseMessage
    {
        // TODO: 将任务申请operator 添加为附件；
        public ProductInfo Info;
        public User Operater;
        public PanelMissionRequestMessage(ProductInfo info,User op):base(MessageType.CLINET_GET_PANEL_MISSION,ClientVersion.Version)
        {
            Info = info;
            Operater = op;
            this.Append(TransferToString(Info));
            this.Append(TransferToString(Operater));
        }
        public PanelMissionRequestMessage(ProductInfo info) : base(MessageType.CONTROLER_ADD_MISSION, ClientVersion.Version)
        {
            Info = info;
            Operater = null;
            this.Append(TransferToString(Info));
            this.Append(TransferToString(Operater));
        }
        string TransferToString(object field)
        {
            return JsonConvert.SerializeObject(field, JsonSerializerSetting.Setting);
        }
        public PanelMissionRequestMessage(NetMQMessage theMessage):base(theMessage)
        {
            Info = JsonConvert.DeserializeObject<ProductInfo>(theMessage[(int)MessageFieldName.Field1].ConvertToString(), JsonSerializerSetting.Setting);
            Operater = JsonConvert.DeserializeObject<User>(theMessage[(int)MessageFieldName.Field2].ConvertToString(), JsonSerializerSetting.Setting);
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
            return JsonConvert.SerializeObject(panelMission, JsonSerializerSetting.Setting);
        }
        MissionLot TransferToMission(string missionstring)
        {
            return JsonConvert.DeserializeObject<MissionLot>(missionstring, JsonSerializerSetting.Setting);
        }
    }
    public class UserCheckMessage : BaseMessage
    {
        public User TheOperator;
        public UserCheckMessage(MessageType type, VersionCheckClass version, User op) : base(type,version)
        {
            TheOperator = op;
            this.Append(TransferToString(TheOperator));
        }
        public UserCheckMessage(NetMQMessage theMessage):base(theMessage)
        {
            TheMessageType = (MessageType)theMessage[(int)MessageFieldName.MessageType].ConvertToInt32();
            TheOperator = TransferToOp(theMessage[(int)MessageFieldName.Field1].ConvertToString());
        }
        string TransferToString(User op)
        {
            return JsonConvert.SerializeObject(op, JsonSerializerSetting.Setting);
        }
        User TransferToOp(string opstring)
        {
            return JsonConvert.DeserializeObject<User>(opstring, JsonSerializerSetting.Setting);
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
            return JsonConvert.SerializeObject(result, JsonSerializerSetting.Setting);
        }
        PanelMissionResult TransferToResult(string resultstring)
        {
            return JsonConvert.DeserializeObject<PanelMissionResult>(resultstring, JsonSerializerSetting.Setting);
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
            return JsonConvert.SerializeObject(result, JsonSerializerSetting.Setting);
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
            return JsonConvert.SerializeObject(result, JsonSerializerSetting.Setting);
        }
        string[] TransferToResult(string resultstring)
        {
            return JsonConvert.DeserializeObject<string[]>(resultstring, JsonSerializerSetting.Setting);
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
            return JsonConvert.SerializeObject(result, JsonSerializerSetting.Setting);
        }
        Dictionary<string, List<PanelPathContainer>> TransferToResult(string resultstring)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, List<PanelPathContainer>>>(resultstring, JsonSerializerSetting.Setting);
        }
    }
}