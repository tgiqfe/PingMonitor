using System.Linq;

namespace PingMonitor
{
    public class MailMessage
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string[] To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        //public string UserName { get; set; }
        //public string Password { get; set; }

        /*
        public void Send(string subject, string body)
        {
            var msg = new MimeKit.MimeMessage();
            msg.From.Add(new MimeKit.MailboxAddress(null, this.From));
            this.To.ToList().ForEach(x => msg.To.Add(new MimeKit.MailboxAddress(null, x)));
            msg.Subject = subject;

            var bodyText = new MimeKit.TextPart("Plain");
            bodyText.Text = body;
            msg.Body = bodyText;

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(this.Server, this.Port, MailKit.Security.SecureSocketOptions.None);
                //client.Authenticate(this.UserName, this.Password);
                client.Send(msg);
                client.Disconnect(true);
            }
        }
        */

        public void Send()
        {
            var msg = new MimeKit.MimeMessage();
            msg.From.Add(new MimeKit.MailboxAddress(null, this.From));
            this.To.ToList().ForEach(x => msg.To.Add(new MimeKit.MailboxAddress(null, x)));
            msg.Subject = this.Subject;

            var bodyText = new MimeKit.TextPart("Plain");
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
 * サンプル
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
