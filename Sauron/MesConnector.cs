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
        public LotMissionFromMES RequestMission(string FGcode, ProductType productType)
        {
            RemoteTrayGroupInfoDownloadRequest newrequest = new RemoteTrayGroupInfoDownloadRequest(FGcode,productType);
            Message newmessage = newrequest.GetMessage();
            var reply = transport.SendRequest(newmessage,15);
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

                return returnMessage.Lot;
            }
        }

        public void FinishMission(Lot lot)
        {

        }
    }
}
