using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Container;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;


namespace test
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Form1 a = new Form1();
            a.ShowDialog();
        }
    }
}
