using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PingMonitor
{
    public class Logger
    {
        private string _logPath = null;

        public Logger(string logsDir, string preFileName)
        {
            if (!Directory.Exists(logsDir)) Directory.CreateDirectory(logsDir);
            string today = DateTime.Now.ToString("yyyyMMdd");
            _logPath = Path.Combine(logsDir, $"{preFileName}_{today}.log");
        }

        public void Write(LogLevel level, string message)
        {
            using (var stream = new StreamWriter(_logPath, true, Encoding.UTF8))
            {
                string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                stream.WriteLine($"[{now}]<{level}>{message}");
            }
        }

        public void Write(string message)
        {
            Write(LogLevel.Info, message);
        }
    }
}
