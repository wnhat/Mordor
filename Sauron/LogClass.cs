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
        static ILogger Logger;
        static FilePathLogClass()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\eye of sauron\log\missionmanager\log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
