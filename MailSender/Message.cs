using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MimeKit;

namespace MailSender
{
    internal class Message
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string[] To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public void Send()
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(null, this.From));
            this.To.ToList().ForEach(x => msg.To.Add(new MailboxAddress(null, x)));
            msg.Subject = this.Subject;

            var bodyText = new TextPart("Plain");
            bodyText.Text = this.Body;
            msg.Body = bodyText;

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(this.Server, this.Port, MailKit.Security.SecureSocketOptions.None);
                client.Send(msg);
                client.Disconnect(true);
            }
        }
    }
}


/*

    internal class Example01
    {
        public static void Test01()
        {
            Assembly asm = Assembly.LoadFrom("MailSender.dll");
            Module module = asm.GetModule("MailSender.dll");
            Type type = module.GetType("MailSender.Message");
            var msg = Activator.CreateInstance(type);

            type.GetProperty("Server").SetValue(msg, "smtp.server.org");
            type.GetProperty("Port").SetValue(msg, 25);
            type.GetProperty("To").SetValue(msg, new string[] { "aaa@b.com" });
            type.GetProperty("From").SetValue(msg, "info@example.com");
            type.GetProperty("Subject").SetValue(msg, "TestSubject");
            type.GetProperty("Body").SetValue(msg, "BodyBody");

            MethodInfo send = type.GetMethod("Send");
            send.Invoke(send, new object[0]);
        }
    }

*/