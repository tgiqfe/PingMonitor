using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMonitor
{
    public class MailTemplate
    {
        public string Subject { get; set; }
        public string Body { get; set; }

        public static MailTemplate AlertMail(CheckResult[] results)
        {
            MailTemplate template = new();
            template.Subject = "[PingMonitor]Alert: No ping response server.";


            //  ここにtemplateのbodyを記載


            return null;
        }

    }
}
