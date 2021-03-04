using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Container.Message;

namespace Sauron
{
    class Program
    {
        static void Main(string[] args)
        {
            Server();
        }

        static void Server()
        {
            var timer = new NetMQTimer(TimeSpan.FromSeconds(1800));
            ResponseSocket responseSocket = new ResponseSocket("@tcp://*:5555");
            //using (var req = new RequestSocket(">tcp://127.0.0.1:5555"))
            using (var poller = new NetMQPoller { responseSocket, timer })
            {
                MissionManager TheMissionManager = new MissionManager();
                // wait the async process finish;
                Thread.Sleep(TimeSpan.FromSeconds(250));
                TheMissionManager.AddMissionByServer();
                Console.WriteLine("add mission finished.");

                responseSocket.ReceiveReady += (s, a) =>
                {
                    NetMQMessage messageIn = a.Socket.ReceiveMultipartMessage();
                    BaseMessage switchmessage = new BaseMessage(messageIn);
                    switch (switchmessage.TheMessageType)
                    {
                        case MessageType.CLIENT_GET_PANEL_GREAD:
                            break;
                        case MessageType.CLIENT_SEND_MISSION_RESULT:
                            // TODO:
                            PanelResultMessage finishedMission = new PanelResultMessage(messageIn);
                            break;
                        case MessageType.CLINET_GET_MISSION_AVI:
                            PanelMissionMessage newavimission = new PanelMissionMessage(MessageType.SERVER_SEND_MISSION, TheMissionManager.GetAviMission());
                            a.Socket.SendMultipartMessage(newavimission);
                            break;
                        case MessageType.CLINET_GET_MISSION_SVI:
                            PanelMissionMessage newsvimission = new PanelMissionMessage(MessageType.SERVER_SEND_MISSION, TheMissionManager.GetSviMission());
                            a.Socket.SendMultipartMessage(newsvimission);
                            break;
                        case MessageType.CLINET_GET_MISSION_APP:
                            PanelMissionMessage newappmission = new PanelMissionMessage(MessageType.SERVER_SEND_MISSION, TheMissionManager.GetAppMission());
                            a.Socket.SendMultipartMessage(newappmission);
                            break;
                        case MessageType.CLINET_GET_EXAM_MISSION_LIST:
                            ExamMissionMessage newexammission = new ExamMissionMessage(MessageType.SERVER_SEND_MISSION, TheMissionManager.GetExamMission());
                            a.Socket.SendMultipartMessage(newexammission);
                            break;
                        case MessageType.CONTROLER_CLEAR_MISSION:
                            Console.WriteLine("start clean mission queue;");
                            TheMissionManager.MissionQueue.Clear();
                            BaseMessage clearMissionMessage = new BaseMessage(MessageType.SERVER_SEND_FINISH);
                            a.Socket.SendMultipartMessage(clearMissionMessage);
                            break;
                        case MessageType.CONTROLER_ADD_MISSION:
                            Console.WriteLine("start add mission");
                            TheMissionManager.AddMissionByServer();
                            BaseMessage addMissionMessage = new BaseMessage(MessageType.SERVER_SEND_FINISH);
                            a.Socket.SendMultipartMessage(addMissionMessage);
                            break;
                        case MessageType.CLINET_CHECK_USER:
                            UserCheckMessage userInfo = new UserCheckMessage(messageIn);
                            // TODO:check user Password & ID；
                            var op = TheMissionManager.CheckUser(userInfo.TheOperator);
                            if (op != null)
                            {
                                UserCheckMessage newmessage = new UserCheckMessage(MessageType.SERVER_SEND_USER_TRUE, op);
                                a.Socket.SendMultipartMessage(newmessage);
                            }
                            else
                            {
                                UserCheckMessage newmessage = new UserCheckMessage(MessageType.SERVER_SEND_USER_FLASE, null);
                                a.Socket.SendMultipartMessage(newmessage);
                            }
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine("finish");
                };
                timer.Elapsed += (s, a) =>
                {
                    Console.WriteLine("start refresh the panel list");
                    TheMissionManager.RefreshFileContainer();
                };
                poller.Run();
            }
        }
    }
}
