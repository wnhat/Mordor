using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using Newtonsoft.Json;

namespace Container.Message
{
    public enum MessageType
    {
        CLIENT_GET_PANEL_GREAD,
        CLIENT_SEND_MISSION_RESULT,
        CLIENT_SEND_EXAM_RESULT,
        CLINET_GET_MISSION_AVI,
        CLINET_GET_MISSION_SVI,
        CLINET_GET_MISSION_APP,
        CLINET_GET_EXAM_MISSION_LIST,
        CLINET_CHECK_USER,
        CLINET_SEND_UNFINISHED_MISSION_AVI,
        CLINET_SEND_UNFINISHED_MISSION_SVI,
        CLINET_SEND_UNFINISHED_MISSION_APP,

        CONTROLER_CLEAR_MISSION,
        CONTROLER_ADD_MISSION,

        SERVER_SEND_MISSION,
        SERVER_SEND_FINISH,
        SERVER_SEND_EORRO,
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
    public class PanelMissionMessage : BaseMessage
    {
        public PanelMission ThePanelMission;

        public PanelMissionMessage(BaseMessage theMessage) : base(theMessage)
        {
            ThePanelMission = TransferToMission(theMessage[1].ConvertToString());
        }
        public PanelMissionMessage(NetMQMessage theMessage):base()
        {
            ThePanelMission = TransferToMission(theMessage[1].ConvertToString());
        }
        public PanelMissionMessage(MessageType messageType, PanelMission panelMission) : base(messageType)
        {
            ThePanelMission = panelMission;
            this.Append(TransferToString(ThePanelMission));
        }
        string TransferToString(PanelMission panelMission)
        {
            return JsonConvert.SerializeObject(panelMission, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }

        PanelMission TransferToMission(string missionstring)
        {
            return JsonConvert.DeserializeObject<PanelMission>(missionstring, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
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
        public List<ExamMission> ExamMissionList;
        public ExamMissionMessage(MessageType messageType, List<ExamMission> examMissionList) : base(messageType)
        {
            ExamMissionList = examMissionList;
            this.Append(TransferToString(ExamMissionList));
        }
        public ExamMissionMessage(NetMQMessage message):base()
        {
            ExamMissionList = TransferToResult(message[1].ConvertToString());
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
}