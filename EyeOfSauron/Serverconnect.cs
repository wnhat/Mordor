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
        Queue<PanelMission> panelMissionList;
        Queue<ExamMission> examMissionList;
        Parameter SystemParameter;
        public Serverconnecter(Parameter systemParameter)
        {
            request = new RequestSocket();
            request.Connect("tcp://localhost:5555");
            SystemParameter = systemParameter;
        }
        PanelMission GetPanelMission()
        {
            // get new panel mission from server;
            BaseMessage newMessage = new BaseMessage(MessageType.CLINET_GET_MISSION_AVI);
            request.SendMultipartMessage(newMessage);
            PanelMissionMessage returnMessage = new PanelMissionMessage(request.ReceiveMultipartMessage());
            return returnMessage.ThePanelMission;
        }
        public Operator CheckPassWord(Operator theuser)
        {
#if !DEBUG
            UserCheckMessage newMessage = new UserCheckMessage(MessageType.CLINET_CHECK_USER,theuser);
            request.SendMultipartMessage(newMessage);
            UserCheckMessage returnUser = new UserCheckMessage(request.ReceiveMultipartMessage());
            if (returnUser.TheMessageType == MessageType.SERVER_SEND_USER_TRUE)
                return returnUser.TheOperator;
            else
                return null;
#else
            return new Operator("password", "testop", "testid");
#endif
        }
        public bool FinishMission(PanelMissionResult finishedMission)
        {
            PanelResultMessage newMessage = new PanelResultMessage(MessageType.CLIENT_SEND_MISSION_RESULT, finishedMission);
            request.SendMultipartMessage(newMessage);
            bool returnbool = request.ReceiveSignal();
            return returnbool;
        }
        public void SendUnfinishedMissionBack(InspectSection section, PanelMission mission)
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
        public List<ExamMission> GetExamMissions()
        {
            BaseMessage newmessage = new BaseMessage(MessageType.CLINET_GET_EXAM_MISSION_LIST);
            request.SendMultipartMessage(newmessage);
            var returnmessage = new ExamMissionMessage(request.ReceiveMultipartMessage());
            return returnmessage.ExamMissionList;
        }
        public InspectMission GetMission(InspectSection section)
        {
            string[] imagenamelist = SystemParameter.AviImageNameList;
            if (section == InspectSection.AVI)
            {
                PanelMission newmission = panelMissionList.Dequeue();
                return new InspectMission(newmission, section, SystemParameter.AviImageNameList);
            }
            else if (section == InspectSection.SVI)
            {
                PanelMission newmission = panelMissionList.Dequeue();
                return new InspectMission(newmission, section, SystemParameter.SviImageNameList);
            }
            else if (section == InspectSection.APP)
            {
                PanelMission newmission = panelMissionList.Dequeue();
                return new InspectMission(newmission, section, SystemParameter.AppImageNameList);
            }
            else
            {
                ExamMission newExamMission = examMissionList.Dequeue();
                switch (newExamMission.PcSection)
                {
                    case InspectSection.AVI:
                        imagenamelist = SystemParameter.AviImageNameList;
                        break;
                    case InspectSection.SVI:
                        imagenamelist = SystemParameter.SviImageNameList;
                        break;
                    case InspectSection.APP:
                        imagenamelist = SystemParameter.AppImageNameList;
                        break;
                }
                InspectMission newinspectmission = new InspectMission(newExamMission, imagenamelist);
                return newinspectmission;
            }
        }
    }
}