using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;


namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration().WriteTo.Console();
        }
    }
}
