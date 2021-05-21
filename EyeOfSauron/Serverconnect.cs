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
using System.Data;

namespace EyeOfSauron
{
    class Serverconnecter
    {
        private RequestSocket request;
        Queue<PanelMission> panelMissionList;
        Queue<ExamMission> ExamMissionList;
        Queue<ExamMission> ExamBuffer;
        List<ExamMission> ExamResult;
        public Serverconnecter()
        {
            request = new RequestSocket();
            request.Connect("tcp://172.16.145.22:5555");
            panelMissionList = new Queue<PanelMission>();
            ExamMissionList = new Queue<ExamMission>();
            ExamBuffer = new Queue<ExamMission>();
            ExamResult = new List<ExamMission>();
        }
        public Operator CheckPassWord(Operator theuser)
        {
            UserCheckMessage newMessage = new UserCheckMessage(MessageType.CLINET_CHECK_USER,theuser);
            request.SendMultipartMessage(newMessage);
            UserCheckMessage returnUser = new UserCheckMessage(request.ReceiveMultipartMessage());
            if (returnUser.TheMessageType == MessageType.SERVER_SEND_USER_TRUE)
                return returnUser.TheOperator;
            else
                return null;
        }
        public void FinishMission(PanelMissionResult finishedMission)
        {
            switch (finishedMission.Section)
            {
                case InspectSection.AVI:
                case InspectSection.SVI:
                case InspectSection.APP:
                    PanelResultMessage newMessage = new PanelResultMessage(MessageType.CLIENT_SEND_MISSION_RESULT, finishedMission);
                    request.SendMultipartMessage(newMessage);
                    request.ReceiveSignal();
                    break;
                case InspectSection.EXAM:
                    var mission = ExamBuffer.Dequeue();
                    mission.FinishExam(finishedMission);
                    ExamResult.Add(mission);
                    if (ExamBuffer.Count == 0)
                    {
                        ExamMissionMessage newmessage = new ExamMissionMessage(MessageType.CLIENT_SEND_EXAM_RESULT, ExamResult);
                        request.SendMultipartMessage(newmessage);
                        request.ReceiveSignal();
                    }
                    break;
            }
        }
        public void SendUnfinishedMissionBack(InspectSection section)
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
            for (int i = 0; i < panelMissionList.Count; i++)
            {
                PanelMission mission = panelMissionList.Dequeue();
                PanelMissionMessage message = new PanelMissionMessage(missionsection, mission);
                request.SendMultipartMessage(message);
            }
        }
        PanelMission GetPanelMission()
        {
            // get new panel mission from server;
            BaseMessage newMessage = new BaseMessage(MessageType.CLINET_GET_MISSION_AVI);
            request.SendMultipartMessage(newMessage);
            PanelMissionMessage returnMessage = new PanelMissionMessage(request.ReceiveMultipartMessage());
            return returnMessage.ThePanelMission;
        }
        public void GetExamMissions()
        {
            ExamMissionList = new Queue<ExamMission>();
            BaseMessage newmessage = new BaseMessage(MessageType.CLINET_GET_EXAM_MISSION_LIST);
            request.SendMultipartMessage(newmessage);
            var returnmessage = new ExamMissionMessage(request.ReceiveMultipartMessage());
            foreach (var item in returnmessage.ExamMissionList)
            {
                ExamMissionList.Enqueue(item);
            }
        }
        public void SetExamMissionsToDataBase()
        {
            //TODO 
        }
        public InspectMission GetMission(InspectSection section)
        {
            string[] imagenamelist = Parameter.AviImageNameList;
            if (section == InspectSection.AVI)
            {
                var newmission = GetPanelMission();
                if (newmission!=null)
                {
                    panelMissionList.Enqueue(newmission);
                    return new InspectMission(newmission, section, Parameter.AviImageNameList);
                }
                else
                {
                    return null;
                }
            }
            else if (section == InspectSection.SVI)
            {
                var newmission = GetPanelMission();
                if (newmission != null)
                {
                    panelMissionList.Enqueue(newmission);
                    return new InspectMission(newmission, section, Parameter.SviImageNameList);
                }
                else
                {
                    return null;
                }
            }
            else if (section == InspectSection.APP)
            {
                var newmission = GetPanelMission();
                if (newmission != null)
                {
                    panelMissionList.Enqueue(newmission);
                    return new InspectMission(newmission, section, Parameter.AppImageNameList);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (ExamMissionList.Count > 0)
                {
                    ExamMission newExamMission = ExamMissionList.Dequeue();
                    switch (newExamMission.PcSection)
                    {
                        case InspectSection.AVI:
                            imagenamelist = Parameter.AviImageNameList;
                            break;
                        case InspectSection.SVI:
                            imagenamelist = Parameter.SviImageNameList;
                            break;
                        case InspectSection.APP:
                            imagenamelist = Parameter.AppImageNameList;
                            break;
                    }
                    InspectMission newinspectmission = new InspectMission(newExamMission, imagenamelist);
                    ExamBuffer.Enqueue(newExamMission);
                    return newinspectmission;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}