using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using Newtonsoft.Json;

namespace server
{
    class BaseMessage:NetMQMessage
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

    class PanelMissionMessage:BaseMessage
    {
        public PanelMission ThePanelMission;

        public PanelMissionMessage(BaseMessage theMessage):base(theMessage)
        {
            ThePanelMission = Transferstring2Mission(theMessage[1].ConvertToString());
        }

        public PanelMissionMessage(NetMQMessage theMessage)
        {
            TheMessageType = (MessageType)theMessage[0].ConvertToInt32();
            ThePanelMission = Transferstring2Mission(theMessage[1].ConvertToString());
        }

        public PanelMissionMessage(MessageType messageType,PanelMission panelMission) : base(messageType)
        {
            ThePanelMission = panelMission;
            this.Append(TransferMission2string(ThePanelMission));
        }

        string TransferMission2string(PanelMission panelMission)
        {
            return JsonConvert.SerializeObject(panelMission);
        }

        PanelMission Transferstring2Mission(string missionstring)
        {
            return JsonConvert.DeserializeObject<PanelMission>(missionstring);
        }
    }
}
