using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingMonitor
{
    public class MailTemplate
    {
        public string Subject { get; set; }
        public string Body { get; set; }

        public static MailTemplate CreateAlertMail(CheckResult[] results)
        {
            MailTemplate template = new();
            template.Subject = "[PingMonitor][Alert] Detects a server with no ping response.";

            System.Text.StringBuilder sb = new();
            sb.AppendLine($"MonitorServer : {System.Environment.MachineName}");
            sb.AppendLine($"AlertTime     : {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
            sb.AppendLine();

            int maxTargetNameLength = results.Max(x => x.Target.Length);
            int maxLastCheckTimeLength = "yyyy/MM/dd HH:mm:ss".Length;

            sb.AppendLine(
                "Target".PadRight(maxTargetNameLength) + " " +
                "LastCheckTime".PadRight(maxLastCheckTimeLength));
            sb.AppendLine(new string('=', maxTargetNameLength + maxTargetNameLength + 2));
            foreach (var res in results)
            {
                sb.AppendLine(
                    res.Target.PadRight(maxLastCheckTimeLength) + " " +
                    res.LastCheckTime.ToString("yyyy/MM/dd HH:mm:ss").PadRight(maxLastCheckTimeLength));
            }

            template.Body = sb.ToString();

            return template;
        }

        public static MailTemplate CreateRestoreMail(CheckResult[] results)
        {
            MailTemplate template = new();
            template.Subject = "[PingMonitor][Restore] Detects a server with a restored ping response.";

            System.Text.StringBuilder sb = new();
            sb.AppendLine($"MonitorServer : {System.Environment.MachineName}");
            sb.AppendLine($"RestoreTime   : {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
            sb.AppendLine();

            int maxTargetNameLength = results.Max(x => x.Target.Length);
            int maxLastCheckTimeLength = "yyyy/MM/dd HH:mm:ss".Length;

            sb.AppendLine(
                "Target".PadRight(maxTargetNameLength) + " " +
                "LastCheckTime".PadRight(maxLastCheckTimeLength));
            sb.AppendLine(new string('=', maxTargetNameLength + maxTargetNameLength + 2));
            foreach (var res in results)
            {
                sb.AppendLine(
                    res.Target.PadRight(maxLastCheckTimeLength) + " " +
                    res.LastCheckTime.ToString("yyyy/MM/dd HH:mm:ss").PadRight(maxLastCheckTimeLength));
            }

            template.Body = sb.ToString();

            return null;
        }
    }
}
