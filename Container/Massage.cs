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
        CLIENT_SEND_MISSION_RESULT,
        CLIENT_SEND_EXAM_RESULT,
        CLINET_GET_PANEL_MISSION,
        CLINET_GET_PANEL_PATH,
        CLINET_GET_EXAM_MISSION_LIST,
        CLINET_GET_EXAMINFO,
        CLINET_CHECK_USER,

        CONTROLER_CLEAR_MISSION,
        CONTROLER_ADD_MISSION,
        CONTROLER_REFRESH_EXAM,

        SERVER_SEND_MISSION,
        SERVER_SEND_FINISH,
        SERVER_SEND_EORRO,
        SERVER_SEND_EXAMINFO,
        SERVER_SEND_PANEL_GREAD,
        SERVER_SEND_USER_FLASE,
        SERVER_SEND_USER_TRUE,
    }
    public class BaseMessage : NetMQMessage
    {
        public MessageType TheMessageType;
        public BaseMessage(MessageType messageType)
        {
            TheMessageType = messageType;
            this.Append((int)TheMessageType);
        }
        public BaseMessage(NetMQMessage message)
        {
            TheMessageType = (MessageType)message[0].ConvertToInt32();
        }
        public BaseMessage()
        {
        }
    }
    public class PanelMissionRequestMessage : BaseMessage
    {
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
        public Lot ThePanelMissionLot;
        public PanelMissionMessage(BaseMessage theMessage) : base(theMessage)
        {
            ThePanelMissionLot = TransferToMission(theMessage[1].ConvertToString());
        }
        public PanelMissionMessage(NetMQMessage theMessage):base()
        {
            ThePanelMissionLot = TransferToMission(theMessage[1].ConvertToString());
        }
        public PanelMissionMessage(MessageType messageType, Lot panelMission) : base(messageType)
        {
            ThePanelMissionLot = panelMission;
            this.Append(TransferToString(ThePanelMissionLot));
        }
        string TransferToString(Lot panelMission)
        {
            return JsonConvert.SerializeObject(panelMission, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
        Lot TransferToMission(string missionstring)
        {
            return JsonConvert.DeserializeObject<Lot>(missionstring, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
    }
    public class UserCheckMessage : BaseMessage
    {
        public Operator TheOperator;
        public UserCheckMessage(MessageType type, Operator op) : base(type)
        {
            TheOperator = op;
            this.Append(TransferToString(TheOperator));
        }
        public UserCheckMessage(NetMQMessage theMessage)
        {
            TheMessageType = (MessageType)theMessage[0].ConvertToInt32();
            TheOperator = TransferToOp(theMessage[1].ConvertToString());
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
        public PanelResultMessage(MessageType type, PanelMissionResult result) : base(type)
        {
            TheResult = result;
            this.Append(TransferToString(result));
        }
        public PanelResultMessage(NetMQMessage massage):base()
        {
            TheResult = TransferToResult(massage[1].ConvertToString());
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
        public ExamMissionMessage(MessageType messageType, List<ExamMission> examMissionList,string examRequestInfo) : base(messageType)
        {
            this.ExamMissionList = examMissionList;
            this.ExamRequestInfo = examRequestInfo;
            this.Append(TransferToString(ExamMissionList));
            this.Append(new NetMQFrame(ExamRequestInfo,Encoding.UTF8));
        }
        public ExamMissionMessage(NetMQMessage message):base()
        {
            ExamMissionList = TransferToResult(message[1].ConvertToString());
            ExamRequestInfo = message[2].ConvertToString(Encoding.UTF8);
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
        public ExamInfoMessage(string[] examInfo) : base(MessageType.SERVER_SEND_EXAMINFO)
        {
            this.ExamInfoArray = examInfo;
            this.Append(TransferToString(ExamInfoArray));
        }
        public ExamInfoMessage(NetMQMessage message) : base()
        {
            ExamInfoArray = TransferToResult(message[1].ConvertToString());
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
        public PanelPathMessage(MessageType messageType, Dictionary<string, List<PanelPathContainer>> panelpathdic) : base(messageType)
        {
            panelPathDic = panelpathdic;
            this.Append(TransferToString(panelPathDic));
        }
        public PanelPathMessage(MessageType messageType, string[] panelidarray) : base(messageType)
        {
            panelPathDic = new Dictionary<string, List<PanelPathContainer>>();
            foreach (var item in panelidarray)
            {
                panelPathDic.Add(item,null);
            }
            this.Append(TransferToString(panelPathDic));
        }
        //对收到的Massage进行反序列化
        public PanelPathMessage(NetMQMessage message) : base()
        {
            panelPathDic = TransferToResult(message[1].ConvertToString());
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