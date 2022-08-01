
namespace PingMonitor
{
    public class CheckResult
    {
        public string Monitor { get; set; }
        public string Target { get; set; }
        public DateTime LastCheckTime { get; set; }
        public int? FailedCount { get; set; }
        public int? RestoreCount { get; set; }
        public bool? IsNotified { get; set; }
    }
}
