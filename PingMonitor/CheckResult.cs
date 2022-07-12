using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMonitor
{
    public class CheckResult
    {
        public string Target { get; set; }
        public DateTime LastCheckTime { get; set; }
        public int? FailedCount { get; set; }
        public bool? IsNotified { get; set; }
        public bool? IsRestore { get; set; }
    }
}
