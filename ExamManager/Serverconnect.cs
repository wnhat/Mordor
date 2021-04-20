using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using System.IO;

namespace WindowsFormsApp1
{
    class Serverconnect
    {
        
        private RequestSocket client;
        public Serverconnect()
        {
            client = new RequestSocket();
            client.Connect("tcp://localhost:5555");
        }
        // delegate & event;

        public delegate void server_login_check_handler();
        public event server_login_check_handler logevent;


        public void Connectortest()
        {
            // for test
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Sending Hello");
                client.SendFrame("Hello");
                var message = client.ReceiveFrameString();
                Console.WriteLine("Received {0}", message);
            }
        }

        public List<List<string>> Get_mission()
        { 
            //return mission_list;
            //// only for offline test;

            // only for offline test(home);
            DirectoryInfo origin_directory = new DirectoryInfo(@"D:\program\c#\testdata\Origin");
            DirectoryInfo[] directory_list = origin_directory.GetDirectories();
            List<List<string>> mission_list = new List<List<string>>();

            foreach (DirectoryInfo panel_directory in directory_list)
            {
                List<string> thispanel = new List<string>();
                thispanel.Add(panel_directory.Name);
                thispanel.Add(panel_directory.FullName);
                thispanel.Add(panel_directory.Name);
                mission_list.Add(thispanel);
            }

            return mission_list;
            // only for offline test(home);

        }

        public void check_user_password(string user_id)
        {
            if (true)// TODO:check the server ,then return true or flase
            {
                logevent();
            }
        }

        public bool uploadjudgement(string panel_id,string op_id,string defectcode,string defectname)
        {
            // TODO: server paneljudge funtion,return true if succese.
            return true;
        }
    }
}
