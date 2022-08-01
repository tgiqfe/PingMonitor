
namespace MailTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && System.IO.File.Exists(args[0]))
            {
                var message = MailMessage.Load(args[0]);

                var msg = new MimeKit.MimeMessage();
                msg.From.Add(new MimeKit.MailboxAddress("", message.From));
                message.To.ToList().
                    ForEach(x => msg.To.Add(new MimeKit.MailboxAddress("", x)));
                msg.Subject = msg.Subject;

                var body = new MimeKit.TextPart("Plain");
                body.Text = message.Body;
                msg.Body = body;

                try
                {
                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.Connect(message.Server, message.Port, MailKit.Security.SecureSocketOptions.None);
                        if (!string.IsNullOrEmpty(message.UserName) && !string.IsNullOrEmpty(message.Password))
                        {
                            client.Authenticate(message.UserName, message.Password);
                        }
                        client.Send(msg);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
