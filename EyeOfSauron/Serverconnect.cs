using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using System.IO;
using Container;
using Container.Message;

namespace EyeOfSauron
{
    class Serverconnecter
    {
        private RequestSocket request;
        public Serverconnecter()
        {
            request = new RequestSocket();
            request.Connect("tcp://localhost:5555");
        }

        public PanelMission GetMission()
        {
            // get new panel mission from server;
            BaseMessage newMessage = new BaseMessage(MessageType.CLINET_GET_MISSION_AVI);
            request.SendMultipartMessage(newMessage);
            PanelMissionMessage returnMessage = new PanelMissionMessage(request.ReceiveMultipartMessage());
            return returnMessage.ThePanelMission;
        }

        public bool check_user_password(string userId,string passWord)
        {
            UserCheckMessage newMessage = new UserCheckMessage(userId, passWord);
            request.SendMultipartMessage(newMessage);
            bool returnbool = request.ReceiveSignal();
            return returnbool;
        }

        public bool FinishMission(PanelMission finishedMission)
        {
            // TODO: check is themission finished,if not log the mission;
            PanelMissionMessage newMessage = new PanelMissionMessage(MessageType.CLIENT_SEND_MISSION_RESULT, finishedMission);
            request.SendMultipartMessage(newMessage);
            bool returnbool = request.ReceiveSignal();
            return returnbool;
        }

        public void SendUnfinishedMissionBack(InspectSection section,PanelMission mission)
        {
            MessageType missionsection = MessageType.CLINET_SEND_UNFINISHED_MISSION_AVI;
            switch (section)
            {
                case InspectSection.AVI:
                    missionsection = MessageType.CLINET_SEND_UNFINISHED_MISSION_AVI;
                    break;
                case InspectSection.SVI:
                    missionsection = MessageType.CLINET_SEND_UNFINISHED_MISSION_SVI;
                    break;
                case InspectSection.APP:
                    missionsection = MessageType.CLINET_SEND_UNFINISHED_MISSION_APP;
                    break;
            }
            PanelMissionMessage message = new PanelMissionMessage(missionsection, mission);
            request.SendMultipartMessage(message);
        }
    }
}
