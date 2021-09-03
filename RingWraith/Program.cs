using Container;
using Container.MQMessage;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingWraith
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = new NetMQTimer(TimeSpan.FromSeconds(300));
            ResponseSocket responseSocket = new ResponseSocket("@tcp://172.16.150.100:7262");
            FileManager TheManager = new FileManager();
            TheManager.RefreshFileList();
            using (var poller = new NetMQPoller { responseSocket, timer })
            {
                responseSocket.ReceiveReady += (s, a) =>
                {
                    /* 对客户端发送的事件进行分Type响应（按照Message首位）*/
                    NetMQMessage messageIn = a.Socket.ReceiveMultipartMessage();
                    
                    BaseMessage switchmessage;
                    try
                    {
                        // 转换为自定义Message类型;
                        switchmessage = new BaseMessage(messageIn);

                        if (switchmessage.TheMessageType == MessageType.CLINET_GET_PANEL_PATH)
                        {
                            PanelPathMessage panelIdInfo = new PanelPathMessage(messageIn);
                            string[] panelid = panelIdInfo.panelPathDic.Keys.ToArray();
                            var pathDict = TheManager.GetPanelPathList(panelid);
                            PanelPathMessage newpanelinfomassage = new PanelPathMessage(pathDict);
                            a.Socket.SendMultipartMessage(newpanelinfomassage);
                        }
                    }
                    catch (VersionException e)
                    {
                        FilePathLogClass.Logger.Error("收到版本有差异的消息；{0}", e);
                        throw;
                    }
                };
                timer.Elapsed += (s, a) =>
                {
                    TheManager.RefreshFileList();
                };
                poller.Run(); // 启动轮询器
            }
        }
    }
}
