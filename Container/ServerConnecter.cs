﻿using Container.MQMessage;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container.SeverConnection
{
    public static class SeverConnecter
    {
        static private RequestSocket request;
        static SeverConnecter()
        {
            request = new RequestSocket();
            request.Connect("tcp://172.16.145.22:5555");
        }
        public static bool VersionCheck(VersionCheckClass version)
        {
            // 检查子程序与服务器程序的版本号码是否匹配，当重大更新时推进版本号，防止出现异常；
            BaseMessage newmessage = new BaseMessage(MessageType.VERSION_CHECK,version);
            request.SendMultipartMessage(newmessage);
            var returnVersion = new BaseMessage(request.ReceiveMultipartMessage());
            return returnVersion.Version == version;
        }
        public static bool SendBaseMessage(MessageType m)
        {
            // BaseMessage 包含了message的基础信息，在MessageType中详细描述了该信息传输至服务器时对应的行为含义；
            BaseMessage newMessage = new BaseMessage(m);
            request.SendMultipartMessage(newMessage);
            return request.ReceiveSignal();
        }
        public static Operator CheckPassWord(Operator theuser)
        {
            UserCheckMessage newMessage = new UserCheckMessage(MessageType.CLINET_CHECK_USER, theuser);
            request.SendMultipartMessage(newMessage);
            UserCheckMessage returnUser = new UserCheckMessage(request.ReceiveMultipartMessage());
            if (returnUser.TheMessageType == MessageType.SERVER_SEND_USER_TRUE)
                return returnUser.TheOperator;
            else
                return null;
        }
        public static Dictionary<string, List<PanelPathContainer>> GetPanelPathByID(params string[] panelIdList)
        {
            BaseMessage newmessage = new PanelPathMessage(MessageType.CLINET_GET_PANEL_PATH, panelIdList);
            request.SendMultipartMessage(newmessage);
            var returnmessage = new PanelPathMessage(request.ReceiveMultipartMessage());
            return returnmessage.panelPathDic;
        }
        public static MissionLot GetPanelMission()
        {
            // get new panel mission from server;
            BaseMessage newMessage = new BaseMessage(MessageType.CLINET_GET_PANEL_MISSION);
            request.SendMultipartMessage(newMessage);
            PanelMissionMessage returnMessage = new PanelMissionMessage(request.ReceiveMultipartMessage());
            return returnMessage.ThePanelMissionLot;
        }
        public static void SendPanelMissionResult(MissionLot lot)
        {
            PanelMissionMessage ResultMessage = new PanelMissionMessage(MessageType.CLIENT_SEND_MISSION_RESULT, lot);
            request.SendMultipartMessage(ResultMessage);
            request.ReceiveSignal();
        }
        public static List<ExamMission> GetExamMission(string ExamInfo)
        {
            ExamMissionMessage newMessage = new ExamMissionMessage(MessageType.CLINET_GET_EXAM_MISSION_LIST, null, ExamInfo);
            request.SendMultipartMessage(newMessage);
            ExamMissionMessage returnMessage = new ExamMissionMessage(request.ReceiveMultipartMessage());
            return returnMessage.ExamMissionList;
        }
        public static string[] GetExamInfo()
        {
            BaseMessage newMessage = new BaseMessage(MessageType.CLINET_GET_EXAMINFO);
            request.SendMultipartMessage(newMessage);
            ExamInfoMessage returnMessage = new ExamInfoMessage(request.ReceiveMultipartMessage());
            return returnMessage.ExamInfoArray;
        }
        public static void SendExamMissionResult(List<ExamMission> ExamResult, string ExamInfo)
        {
            ExamMissionMessage newmessage = new ExamMissionMessage(MessageType.CLIENT_SEND_EXAM_RESULT, ExamResult, ExamInfo);
            request.SendMultipartMessage(newmessage);
            request.ReceiveSignal();
        }
        public static void AddPanelMission(MissionLot Newlot)
        {
            PanelMissionMessage ResultMessage = new PanelMissionMessage(MessageType.CONTROLER_ADD_MISSION, Newlot);
            request.SendMultipartMessage(ResultMessage);
            request.ReceiveSignal();
        }
    }
}
