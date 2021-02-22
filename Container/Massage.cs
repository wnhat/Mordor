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
        CLINET_GET_MISSION_AVI,
        CLINET_GET_MISSION_SVI,
        CLINET_GET_MISSION_APP,

        CLINET_GET_MISSION_LIST,
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
    }

    public class BaseMessage:NetMQMessage
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

        string TransferMission2string(PanelMission panelMission)
        {
            return JsonConvert.SerializeObject(panelMission);
        }
    }

    public class PanelMissionMessage:BaseMessage
    {
        public PanelMission ThePanelMission;

        public PanelMissionMessage(BaseMessage theMessage):base(theMessage)
        {
            ThePanelMission = TransferToMission(theMessage[1].ConvertToString());
        }

        public PanelMissionMessage(NetMQMessage theMessage)
        {
            TheMessageType = (MessageType)theMessage[0].ConvertToInt32();
            ThePanelMission = TransferToMission(theMessage[1].ConvertToString());
        }

        public PanelMissionMessage(MessageType messageType, PanelMission panelMission) : base(messageType)
        {
            ThePanelMission = panelMission;
            this.Append(TransferToString(ThePanelMission));
        }

        string TransferToString(PanelMission panelMission)
        {
            return JsonConvert.SerializeObject(panelMission);
        }

        PanelMission TransferToMission(string missionstring)
        {
            return JsonConvert.DeserializeObject<PanelMission>(missionstring);
        }
    }

    public class UserCheckMessage:BaseMessage
    {
        public string UserId;
        public string PassWord;

        public UserCheckMessage(string userId,string passWord):base(MessageType.CLINET_CHECK_USER)
        {
            UserId = userId;
            this.Append(userId);
            PassWord = passWord;
            this.Append(passWord);
        }
        public UserCheckMessage(NetMQMessage theMessage)
        {
            TheMessageType = (MessageType)theMessage[0].ConvertToInt32();
            UserId = theMessage[1].ConvertToString();
            PassWord = theMessage[2].ConvertToString();
        }

    }

    public class PanelResultMessage:BaseMessage
    {
        PanelMissionResult TheResult;
        public PanelResultMessage(MessageType type,PanelMissionResult result):base(type)
        {
            TheResult = result;
        }

        string TransferToString(PanelMissionResult result)
        {
            return JsonConvert.SerializeObject(result);
        }
        PanelMissionResult TransferToResult(string resultstring)
        {
            return JsonConvert.DeserializeObject<PanelMissionResult>(resultstring);
        }
    }
}
