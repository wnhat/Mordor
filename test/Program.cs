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
            int[] a = new int[2] { 0, 21 };
            int[] b = new int[2] { 3, 2 };
            a.CopyTo(b,b.Length);
        }
    }
    class NewFileContainer
    {

    }
}
