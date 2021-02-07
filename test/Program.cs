using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Container;


namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime time1 = DateTime.Now;
            for (int i = 0; i < 100000000; i++)
            {
                int[] a = { 2, 5, 86, 95 };
                int[] b = { 5, 6, 31, 86 };
                var c = a.Intersect(b);
            }
            DateTime time2 = DateTime.Now;

        }
    }

}
