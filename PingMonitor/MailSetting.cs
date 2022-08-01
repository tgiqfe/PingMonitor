
namespace PingMonitor
{
    public class MailSetting : SettingBase
    {
        protected override int IndentLength { get { return 2; } }

        public string SmtpServer { get; set; }
        public int? Port { get; set; }
        public string[] To { get; set; }
        public string From { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
