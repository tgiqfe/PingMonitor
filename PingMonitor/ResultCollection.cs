using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMonitor
{
    internal class ResultCollection
    {
        public class Result
        {
            public string Target { get; set; }
            public DateTime LastCheckTime { get; set; }
            public int FailedCount { get; set; }
            public bool IsNotified { get; set; }
        }

        public List<Result> List { get; set; }


    }
}
