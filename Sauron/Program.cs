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
using Container.MQMessage;
using Container;

namespace Sauron
{
    class Program
    {
        static void Main(string[] args)
        {
            Server(); //启动服务器；
            //Test();
        }
        static void Test()
        {
            var timer1 = new NetMQTimer(TimeSpan.FromSeconds(10));
            var timer2 = new NetMQTimer(TimeSpan.FromSeconds(10));
            var poller = new NetMQPoller { timer1, timer2 };
            timer1.Elapsed += (s, a) =>
            {
                ConsoleLogClass.Logger.Error("time1 start;");
                Thread.Sleep(2000);
                ConsoleLogClass.Logger.Error("time1 end;");
            };
            timer2.Elapsed += (s, a) =>
            {
                ConsoleLogClass.Logger.Error("time2 start;");
                Thread.Sleep(1000);
                ConsoleLogClass.Logger.Error("time2 end;");
            };
            poller.Run(); // 启动轮询器
        }
        static void Server()
        {
            var timer = new NetMQTimer(TimeSpan.FromSeconds(3600));
            ResponseSocket responseSocket = new ResponseSocket("@tcp://172.16.145.22:5555");
            using (var poller = new NetMQPoller { responseSocket, timer })
            {
                MissionManager TheMissionManager = new MissionManager();
                responseSocket.ReceiveReady += (s, a) =>
                {
                    /* 对客户端发送的事件进行分Type响应（按照Message首位）*/
                    NetMQMessage messageIn = a.Socket.ReceiveMultipartMessage();
                    // 转换为自定义Message类型;
                    BaseMessage switchmessage = new BaseMessage(messageIn);
                    switch (switchmessage.TheMessageType)
                    {
                        case MessageType.CLIENT_SEND_MISSION_RESULT:
                            TheMissionManager.FinishMission(a,messageIn);
                            break;
                        case MessageType.CLIENT_SEND_EXAM_RESULT:
                            TheMissionManager.FinishExam(a,messageIn);
                            break;
                        case MessageType.CLINET_GET_PANEL_MISSION:
                            TheMissionManager.GetMission(a,messageIn);
                            break;
                        case MessageType.CLINET_GET_EXAM_MISSION_LIST:
                            TheMissionManager.GetExamMission(a, messageIn);
                            break;
                        case MessageType.CLINET_GET_EXAMINFO:
                            TheMissionManager.GetExamInfo(a);
                            break;
                        case MessageType.CLINET_GET_PANEL_PATH:
                            // 获取设备中存在的原图路径；
                            PanelPathMessage panelIdInfo = new PanelPathMessage(messageIn);
                            var newPanelPathDic = TheMissionManager.GetPanelPathList(panelIdInfo.panelPathDic.Keys.ToArray());
                            PanelPathMessage newpanelinfomassage = new PanelPathMessage(MessageType.CLINET_GET_PANEL_PATH, ServerVersion.Version, newPanelPathDic);
                            a.Socket.SendMultipartMessage(newpanelinfomassage);
                            break;
                        case MessageType.CLINET_GET_PRODUCTINFO:
                            TheMissionManager.GetProductInfo(a);
                            break;
                        case MessageType.CONTROLER_CLEAR_MISSION:
                            break;
                        case MessageType.CONTROLER_ADD_MISSION:
                            TheMissionManager.AddMissionByControlor(a,messageIn);
                            break;
                        case MessageType.CONTROLER_REFRESH_EXAM:
                            TheMissionManager.RefreshExamList(a);

                            break;
                        case MessageType.CLINET_CHECK_USER:
                            UserCheckMessage userInfo = new UserCheckMessage(messageIn);
                            var op = TheMissionManager.CheckUser(userInfo.TheOperator);
                            if (op != null)
                            {
                                UserCheckMessage newmessage = new UserCheckMessage(MessageType.SERVER_SEND_USER_TRUE, ServerVersion.Version, op);
                                a.Socket.SendMultipartMessage(newmessage);
                            }
                            else
                            {
                                UserCheckMessage newmessage = new UserCheckMessage(MessageType.SERVER_SEND_USER_FLASE, ServerVersion.Version, null);
                                a.Socket.SendMultipartMessage(newmessage);
                            }
                            break;
                        default:
                            break;
                    }
                };
                timer.Elapsed += (s, a) =>
                {
                    ConsoleLogClass.Logger.Error("start refresh the panel list and add mission(test);");
                    TheMissionManager.RefreshFileContainer();
                    //TheMissionManager.AddMissionTest();
                };
                poller.Run(); // 启动轮询器
            }
        }
    }
}
