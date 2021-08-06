using Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TIBCO.Rendezvous;

namespace Sauron
{
    public class MesConnector
    {
        string service = "BOE.B7.MEM.TST.PEMsvr";
        string network = "172.16.145.22";
        string daemon = null;
        string subject = "a.b.c";
        Transport transport = null;
        public MesConnector(string service, string network, string daemon, string subject)
        {
            this.service = service;
            this.network = network;
            this.daemon = daemon;
            this.subject = subject;
            
            try
            {
                TIBCO.Rendezvous.Environment.Open();
            }
            catch (RendezvousException exception)
            {
                Console.Error.WriteLine("Failed to open Rendezvous Environment: {0}", exception.Message);
                Console.Error.WriteLine(exception.StackTrace);
                System.Environment.Exit(1);
            }

            // Create Network transport
            try
            {
                transport = new NetTransport(service, network, daemon);
            }
            catch (RendezvousException exception)
            {
                Console.Error.WriteLine("Failed to create NetTransport");
                Console.Error.WriteLine(exception.StackTrace);
                System.Environment.Exit(1);
            }
        }
        public RemoteTrayGroupInfoDownloadSend RequestMission(string FGcode, ProductType productType)
        {
            RemoteTrayGroupInfoDownloadRequest newrequest = new RemoteTrayGroupInfoDownloadRequest(FGcode,productType);
            Message newmessage = newrequest.GetMessage();
            newmessage.SendSubject = subject;
            var reply = transport.SendRequest(newmessage,Parameter.MesConnectTimeOut);
            // 超时未接到返回信息时，返回值为null；
            if (reply == null)
            {
                MesLogClass.Logger.Error("向MES请求任务超时，请检查与MES的连接或网络问题；");
                return null;
            }
            else
            {
                XmlDocument returnxml = reply.GetFieldByIndex(0); //MessageField可隐式转换为xmldocument；
                MesMessageType messagetype = (MesMessageType)Enum.Parse(typeof(MesMessageType),returnxml.GetElementsByTagName("MESSAGENAME")[0].InnerText);
                if (messagetype == MesMessageType.OpCallSend)
                {
                    string errorstring = returnxml.GetElementsByTagName("OPCALLDESCRIPTION")[0].InnerText;
                    // TODO：当异常发生时进行的操作；
                    MesLogClass.Logger.Error("向MES请求任务时发生错误，错误信息为：{0}",errorstring);
                    return null;
                }
                else
                {
                    return new RemoteTrayGroupInfoDownloadSend(returnxml);
                }
            }
        }
        public void FinishMission(Lot lot)
        {
            RemoteTrayGroupProcessEnd newfinished = new RemoteTrayGroupProcessEnd(lot);
            Message newfinishedMessage = new Message();
            newfinishedMessage.SendSubject = subject;
            var reply = transport.SendRequest(newfinishedMessage,15);
            // 超时未接到返回信息时，返回值为null；
            if (reply == null)
            {
                MesLogClass.Logger.Error("向MES发送已完成任务超时，请检查与MES的连接或网络问题；");
            }
            else
            {
                XmlDocument returnxml = reply.GetFieldByIndex(0); //MessageField可隐式转换为xmldocument；
                string errorstring = returnxml.GetElementsByTagName("OPCALLDESCRIPTION")[0].InnerText;
                
                // TODO：当异常发生时进行的操作；
                MesLogClass.Logger.Error("向MES发送已完成任务超时，错误信息为：{0}",errorstring);
            }
        }
    }
}
