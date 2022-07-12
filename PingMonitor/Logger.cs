using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingMonitor
{
    public class Logger
    {
        private string _logPath = null;

        public Logger(string logsDir, string preFileName)
        {
            if (!System.IO.Directory.Exists(logsDir)) System.IO.Directory.CreateDirectory(logsDir);
            string today = DateTime.Now.ToString("yyyyMMdd");
            _logPath = System.IO.Path.Combine(logsDir, $"{preFileName}_{today}.log");
        }

        public void Write(LogLevel level, string message)
        {
            using (var stream = new System.IO.StreamWriter(_logPath, true, System.Text.Encoding.UTF8))
            {
                string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                stream.WriteLine($"[{now}]<{level}> {message}");

                //Console.WriteLine($"[{now}]<{level}> {message}");
            }
        }

        public void Write(string message)
        {
            Write(LogLevel.Info, message);
        }
    }
}
