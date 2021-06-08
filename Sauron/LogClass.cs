using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Sauron
{
    public static class FilePathLogClass
    {
        public static ILogger Logger;
        static FilePathLogClass()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\Mordor\LOG\File search\log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
    public static class ConsoleLogClass
    {
        public static ILogger Logger;
        static ConsoleLogClass()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\Mordor\LOG\Console\log-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
