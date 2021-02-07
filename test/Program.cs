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
            Dictionary<double, Tester> asd = new Dictionary<double, Tester>();
            Random newrandomint = new Random(351145);
            for (int i = 0; i < 100000000; i++)
            {
                Tester ddd = new Tester(newrandomint.NextDouble());
                if (asd.ContainsKey(ddd.randomint))
                {
                    Console.WriteLine("contain object : key = {0} ,",ddd.hashcode);
                }
                else
                {
                    asd.Add(ddd.randomint, ddd);
                }
                
            }
        }
    }

    class Tester
    {
        public int hashcode;
        public long Timeclass;
        public double randomint;
        //public string MD5;

        public Tester(double random)
        {
            Timeclass = DateTime.Now.Ticks;
            randomint = random;
            hashcode = GetHashCode();
        }
    }
}
