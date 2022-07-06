using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMonitor
{
    internal class MailMessage
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string[] To { get; set; }
        public string From { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public void Send(string subject, string body)
        {
            var mail = new MimeKit.MimeMessage();
            mail.From.Add(new MimeKit.MailboxAddress("address1", this.From));
            this.To.ToList().ForEach(x => mail.To.Add(new MimeKit.MailboxAddress("address2", x)));
            mail.Subject = subject;

            var bodyText = new MimeKit.TextPart("Plain");
            bodyText.Text = body;
            mail.Body = bodyText;

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(this.Server, this.Port, MailKit.Security.SecureSocketOptions.None);
                client.Authenticate(this.UserName, this.Password);
                client.Send(mail);
                client.Disconnect(true);
            }
        }
    }
}


/*
MailMessage mail = new MailMessage()
{
    Server ="smtp.example.com",
    Port = 587,
    To = new[] {"aaaaaaaa@bbbb.com", "a@b.com"},
    From = "test@test.aaa.bbb.com",
    UserName = "username",
    Password = "password",
};
mail.Send("testtesttest", "これはテストメールです");
*/
