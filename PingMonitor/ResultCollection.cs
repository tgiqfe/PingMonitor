using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMonitor
{
    public class ResultCollection
    {
        public class Result
        {
            public string Target { get; set; }
            public DateTime LastCheckTime { get; set; }
            public int FailedCount { get; set; }
            public bool IsNotified { get; set; }
        }

        public List<Result> List { get; set; }

        public void Add(string target)
        {
            var index = this.List.FindIndex(x => x.Target == target);
            if (index >= 0)
            {
                List[index].LastCheckTime = DateTime.Now;
                List[index].FailedCount++;
            }
            else
            {
                List.Add(new Result()
                {
                    Target = target,
                    LastCheckTime = DateTime.Now,
                    FailedCount = 1,
                });
            }
        }
    }
}
