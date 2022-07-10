$source = @"
using System.Linq;

public class MailMessage
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
"@

Add-Type -TypeDefinition $source -Language CSharp -ReferencedAssemblies `
    @("..\MailKit.dll",
      "..\MimeKit.dll",
      "System.Linq",
      "System.Collections",
      "System.Net.Sockets",
      "System.Net.Primitives")

$mss = New-Object MailMessage

