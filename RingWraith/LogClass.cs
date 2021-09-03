using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace RingWraith
{
    public static class FilePathLogClass
    {
        public static ILogger Logger;
        static FilePathLogClass()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\RingWraith\Log\log-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();
        }
    }
}
