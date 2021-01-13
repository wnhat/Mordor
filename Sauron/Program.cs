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
            old();
            //test();
        }

        static void test()
        {
            SqlServerConnector sqlserver = new SqlServerConnector();
            sqlserver.get_oninspect_mission();
        }

        static void old()
        {
            var timer = new NetMQTimer(TimeSpan.FromSeconds(1800));
            ResponseSocket responseSocket = new ResponseSocket("@tcp://*:5555");
            //using (var req = new RequestSocket(">tcp://127.0.0.1:5555"))
            using (var poller = new NetMQPoller { responseSocket, timer })
            {
                SqlServerConnector sqlserver = new SqlServerConnector();
                IP_TR ip_tr = new IP_TR();
                FileManager file_container = new FileManager(ip_tr);
                MissionManager TheMissionManager = new MissionManager(sqlserver, file_container);

                responseSocket.ReceiveReady += (s, a) =>
                {
                    NetMQMessage messageIn = a.Socket.ReceiveMultipartMessage();
                    BaseMessage switchmessage = new BaseMessage(messageIn);
                    switch (switchmessage.TheMessageType)
                    {
                        case MessageType.CLIENT_SEND_MISSION_RESULT:
                            // TODO:
                            PanelMissionMessage finishedMission = new PanelMissionMessage(messageIn);

                            break;
                        case MessageType.CLINET_GET_MISSION:
                            // 
                            Console.WriteLine("start send mission");
                            PanelMissionMessage newmission = new PanelMissionMessage(MessageType.SERVER_SEND_MISSION, TheMissionManager.GetMission());
                            a.Socket.SendMultipartMessage(newmission);
                            break;
                        case MessageType.CLINET_GET_MISSION_LIST:
                            break;
                        case MessageType.CLINET_CLEAR_MISSION:
                            Console.WriteLine("start clean");
                            TheMissionManager.MissionQueue.Clear();
                            BaseMessage clearMissionMessage = new BaseMessage(MessageType.SERVER_SEND_FINISH);
                            a.Socket.SendMultipartMessage(clearMissionMessage);
                            break;
                        case MessageType.CLINET_ADD_MISSION:
                            Console.WriteLine("start add mission");
                            TheMissionManager.AddMisionByServer();
                            BaseMessage addMissionMessage = new BaseMessage(MessageType.SERVER_SEND_FINISH);
                            a.Socket.SendMultipartMessage(addMissionMessage);
                            break;
                        case MessageType.CLINET_CHECK_USER:
                            UserCheckMessage userInfo = new UserCheckMessage(messageIn);
                            // TODO:check user Password & ID；
                            if (true)
                            {
                                a.Socket.SignalOK();
                            }
                            else
                            {
                                a.Socket.SignalError();
                            }
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine("finish");
                };

                timer.Elapsed += (s, a) =>
                {
                    //
                    Console.WriteLine("start refresh the panel list");
                    file_container.Refresh_file_list();
                };
                poller.Run();
            }
        }
    }
}
