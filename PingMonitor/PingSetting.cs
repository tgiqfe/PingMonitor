
namespace PingMonitor
{
    public class PingSetting : SettingBase
    {
        protected override int IndentLength { get { return 2; } }

        public string ListPath { get; set; }
        public int? Interval { get; set; }
        public int? Count { get; set; }
        public int? MaxFailedCount { get; set; }
        public int? MinRestoreCount { get; set; }
    }
}
