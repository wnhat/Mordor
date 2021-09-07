using Container.MQMessage;
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
        static private RequestSocket Request;
        static private RequestSocket PathRequest;
        static SeverConnecter()
        {
            Request = new RequestSocket();
            Request.Connect("tcp://172.16.145.22:5555");

            PathRequest = new RequestSocket();
            PathRequest.Connect("tcp://172.16.150.100:7262");
        }
        public static bool VersionCheck(VersionCheckClass version)
        {
            // 检查子程序与服务器程序的版本号码是否匹配，当重大更新时推进版本号，防止出现异常；
            BaseMessage newmessage = new BaseMessage(MessageType.VERSION_CHECK,version);
            Request.SendMultipartMessage(newmessage);
            var returnVersion = new BaseMessage(Request.ReceiveMultipartMessage());
            return returnVersion.Version == version;
        }
        public static bool SendBaseMessage(MessageType m)
        {
            // BaseMessage 包含了message的基础信息，在MessageType中详细描述了该信息传输至服务器时对应的行为含义；
            BaseMessage newMessage = new BaseMessage(m);
            Request.SendMultipartMessage(newMessage);
            return Request.ReceiveSignal();
        }
        public static User CheckPassWord(User theuser)
        {
            UserCheckMessage newMessage = new UserCheckMessage(MessageType.CLINET_CHECK_USER, theuser);
            Request.SendMultipartMessage(newMessage);
            UserCheckMessage returnUser = new UserCheckMessage(Request.ReceiveMultipartMessage());
            if (returnUser.TheMessageType == MessageType.SERVER_SEND_USER_TRUE)
                return returnUser.TheOperator;
            else
                return null;
        }
        public static Dictionary<string, List<PanelPathContainer>> GetPanelPathByID(params string[] panelIdList)
        {
            BaseMessage newmessage = new PanelPathMessage(panelIdList);
            PathRequest.SendMultipartMessage(newmessage);
            var returnmessage = new PanelPathMessage(PathRequest.ReceiveMultipartMessage());
            return returnmessage.panelPathDic;
        }
        public static MissionLot GetPanelMission(ProductInfo info,User op)
        {
            // get new panel mission from server;
            PanelMissionRequestMessage newMessage = new PanelMissionRequestMessage(info,op);
            Request.SendMultipartMessage(newMessage);
            PanelMissionMessage returnMessage = new PanelMissionMessage(Request.ReceiveMultipartMessage());
            return returnMessage.ThePanelMissionLot;
        }
        public static void SendPanelMissionResult(MissionLot lot)
        {
            PanelMissionMessage ResultMessage = new PanelMissionMessage(MessageType.CLIENT_SEND_MISSION_RESULT, lot);
            Request.SendMultipartMessage(ResultMessage);
            Request.ReceiveSignal();
        }
        public static List<ExamMission> GetExamMission(string ExamInfo)
        {
            ExamMissionMessage newMessage = new ExamMissionMessage(MessageType.CLINET_GET_EXAM_MISSION_LIST, null, ExamInfo);
            Request.SendMultipartMessage(newMessage);
            ExamMissionMessage returnMessage = new ExamMissionMessage(Request.ReceiveMultipartMessage());
            return returnMessage.ExamMissionList;
        }
        public static string[] GetExamInfo()
        {
            BaseMessage newMessage = new BaseMessage(MessageType.CLINET_GET_EXAMINFO);
            Request.SendMultipartMessage(newMessage);
            ExamInfoMessage returnMessage = new ExamInfoMessage(Request.ReceiveMultipartMessage());
            return returnMessage.ExamInfoArray;
        }
        public static void SendExamMissionResult(List<ExamMission> ExamResult, string ExamInfo)
        {
            ExamMissionMessage newmessage = new ExamMissionMessage(MessageType.CLIENT_SEND_EXAM_RESULT, ExamResult, ExamInfo);
            Request.SendMultipartMessage(newmessage);
            Request.ReceiveSignal();
        }
        public static bool AddPanelMission(ProductInfo newInfo)
        {
            PanelMissionRequestMessage RequestMessage = new PanelMissionRequestMessage(newInfo);
            Request.SendMultipartMessage(RequestMessage);
            return Request.ReceiveSignal();
        }
        public static List<ProductInfo> GetProductInfo()
        {
            BaseMessage newmessage = new BaseMessage(MessageType.CLINET_GET_PRODUCTINFO);
            Request.SendMultipartMessage(newmessage);
            ProductInfoMessage returnMessage = new ProductInfoMessage(Request.ReceiveMultipartMessage());
            return returnMessage.InfoList;
        }
    }
}
