using NetMQ.Sockets;
using NetMQ;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Container.MQMessage;
using Container.SeverConnection;
using Container;

namespace Controler
{
    public partial class Form1 : Form
    {
        private RequestSocket request;
        public Form1()
        {
            InitializeComponent();
            request = new RequestSocket();
            request.Connect("tcp://localhost:5555");
        }
        private void buttonAddmission_Click(object sender, EventArgs e)
        {
            
        }
    }
}
