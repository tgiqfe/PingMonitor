$source = @"
using System;
using System.Linq;
using System.Collections.Generic;

public class Setting
{
    public string ListPath { get; set; }
    public string LogsPath { get; set; }
    public int? PingInterval { get; set; }
    public int? PingCount { get; set; }
    public int? MaxFailedCount { get; set; }
    public string MailSmtpServer { get; set; }
    public int? MailSmtpPort { get; set; }
    public string[] MailTo { get; set; }
    public string MailFrom { get; set; }

    private static readonly string[] _falseCandidate = new string[]
    {
        "", "0", "-", "false", "fals", "no", "not", "none", "non", "empty", "null", "否", "不", "無", "dis", "disable", "disabled"
    };

    public void Init()
    {
        this.ListPath = @"Store\TargetList.txt";
        this.LogsPath = @"Store\Logs";
        this.PingInterval = 1000;
        this.PingCount = 4;
        this.MaxFailedCount = 5;
        this.MailSmtpServer = "smtp.example.com";
        this.MailSmtpPort = 25;
        this.MailTo = new[] { "alerm@sample.net", "emergency@sample.org" };
        this.MailFrom = "info@example.com";
    }

    public static Setting Load(string settingFile)
    {
        Setting setting = null;
        try
        {
            using (var stream = new System.IO.StreamReader(settingFile, System.Text.Encoding.UTF8))
            using (var reader = new System.IO.StringReader(stream.ReadToEnd()))
            {
                var ret = new Setting();
                var props = typeof(Setting).GetProperties(
                    System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.DeclaredOnly);
                string readLine = "";
                while ((readLine = reader.ReadLine()) != null)
                {
                    if (readLine.Contains(":"))
                    {
                        string key = readLine.Substring(0, readLine.IndexOf(":")).Trim();
                        string val = readLine.Substring(readLine.IndexOf(":") + 1).Trim();
                        var prop = props.FirstOrDefault(x => x.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
                        if (prop != null)
                        {
                            var type = prop.PropertyType;
                            if (type == typeof(string))
                            {
                                prop.SetValue(ret, val);
                            }
                            else if (type == typeof(int?))
                            {
                                prop.SetValue(ret, int.TryParse(val, out int num) ? num : null);
                            }
                            else if (type == typeof(bool?))
                            {
                                bool? bol = null;
                                if (string.IsNullOrEmpty(val)) bol = !_falseCandidate.Any(x => x.Equals(val.ToLower()));
                                prop.SetValue(ret, bol);
                            }
                            else if (type == typeof(string[]))
                            {
                                prop.SetValue(ret, val.Split(',').Select(x => x.Trim()).ToArray());
                            }
                            else if (type == typeof(DateTime?))
                            {
                                prop.SetValue(ret, DateTime.TryParse(val, out DateTime dt) ? dt : null);
                            }
                        }
                    }
                }
                setting = ret;
            }
        }
        catch { }
        if (setting == null)
        {
            setting = new Setting();
            setting.Init();
            setting.Save(settingFile);
        }
        return setting;
    }

    public void Save(string settingFile)
    {
        try
        {
            using (var stream = new System.IO.StreamWriter(settingFile, false, System.Text.Encoding.UTF8))
            {
                var props = this.GetType().GetProperties(
                    System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.DeclaredOnly);
                foreach (var prop in props)
                {
                    var type = prop.PropertyType;
                    object val = prop.GetValue(this);
                    if (val != null)
                    {
                        if (type == typeof(string) ||
                            type == typeof(int?) ||
                            type == typeof(bool?) ||
                            type == typeof(DateTime?))
                        {
                            stream.WriteLine($"{prop.Name}: {val}");
                        }
                        else if (type == typeof(string[]))
                        {
                            stream.WriteLine($"{prop.Name}: {string.Join(", ", val as string[])}");
                        }
                    }
                }
            }
        }
        catch { }
    }
}

public enum LogLevel
{
    Debug = -1,
    Info = 0,
    Attention = 1,
    Warn = 2,
    Error = 3,
}

public class Logger
{
    private string _logPath = null;

    public Logger(string logsDir, string preFileName)
    {
        if (!System.IO.Directory.Exists(logsDir)) System.IO.Directory.CreateDirectory(logsDir);
        string today = DateTime.Now.ToString("yyyyMMdd");
        _logPath = System.IO.Path.Combine(logsDir, $"{preFileName}_{today}.log");
    }

    public void Write(LogLevel level, string message)
    {
        using (var stream = new System.IO.StreamWriter(_logPath, true, System.Text.Encoding.UTF8))
        {
            string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            stream.WriteLine($"[{now}]<{level}> {message}");

            //Console.WriteLine($"[{now}]<{level}> {message}");
        }
    }

    public void Write(string message)
    {
        Write(LogLevel.Info, message);
    }
}

public class CheckResult
{
    public string Target { get; set; }
    public DateTime LastCheckTime { get; set; }
    public int? FailedCount { get; set; }
    public bool? IsNotified { get; set; }
    public bool? IsRestore { get; set; }
}

public class CheckResultCollection
{
    public List<CheckResult> Results { get; set; }

    public void Init()
    {
        this.Results = new();
    }

    #region load/save

    public static CheckResultCollection Load(string dbFile)
    {
        CheckResultCollection collection = null;
        try
        {
            collection = System.Text.Json.JsonSerializer.Deserialize<CheckResultCollection>(
                System.IO.File.ReadAllText(dbFile),
                new System.Text.Json.JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true,
                });

        }
        catch { }
        if (collection == null)
        {
            collection = new();
            collection.Init();
        }
        return collection;
    }

    public void Save(string dbFile)
    {
        try
        {
            string parent = System.IO.Path.GetDirectoryName(dbFile);
            if (System.IO.Directory.Exists(parent)) System.IO.Directory.CreateDirectory(parent);

            string json = System.Text.Json.JsonSerializer.Serialize(this, new System.Text.Json.JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
            });
            System.IO.File.WriteAllText(dbFile, json);
        }
        catch { }
    }

    #endregion

    public void AddSuccessTarget(string target)
    {
        var index = this.Results.FindIndex(x => x.Target == target);
        if (index >= 0)
        {
            Results[index].IsRestore = true;
        }
    }

    public void AddFailTarget(string target)
    {
        var index = this.Results.FindIndex(x => x.Target == target);
        if (index >= 0)
        {
            Results[index].LastCheckTime = DateTime.Now;
            Results[index].FailedCount++;
        }
        else
        {
            Results.Add(new CheckResult()
            {
                Target = target,
                LastCheckTime = DateTime.Now,
                FailedCount = 1,
            });
        }
    }

    public CheckResult[] GetAlertTarget(int maxFailedCount)
    {
        var tempList = new List<CheckResult>();
        for (int i = 0; i < Results.Count; i++)
        {
            if ((Results[i].FailedCount ?? 0) > maxFailedCount &&
                Results[i].IsNotified != true &&
                Results[i].IsRestore != true)
            {
                tempList.Add(Results[i]);
                Results[i].IsNotified = true;
            }
        }
        return tempList.ToArray();
    }

    public CheckResult[] GetRestreTarget()
    {
        var tempList = new List<CheckResult>();
        for (int i = Results.Count - 1; i >= 0; i--)
        {
            if (Results[i].IsRestore ?? false)
            {
                tempList.Add(Results[i]);
                Results.RemoveAt(i);
            }
        }
        tempList.Reverse();
        return tempList.ToArray();
    }
}

public class MailMessage
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string[] To { get; set; }
    public string From { get; set; }
    //public string UserName { get; set; }
    //public string Password { get; set; }

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
}

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
            "Target".PadRight(maxTargetNameLength) + "  " +
            "LastCheckTime".PadRight(maxLastCheckTimeLength));
        sb.AppendLine(new string('=', maxTargetNameLength + maxTargetNameLength + 3));
        foreach (var res in results)
        {
            sb.AppendLine(
                res.Target.PadRight(maxTargetNameLength) + "  " +
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
            "Target".PadRight(maxTargetNameLength) + "  " +
            "LastCheckTime".PadRight(maxLastCheckTimeLength));
        sb.AppendLine(new string('=', maxTargetNameLength + maxTargetNameLength + 3));
        foreach (var res in results)
        {
            sb.AppendLine(
                res.Target.PadRight(maxTargetNameLength) + "  " +
                res.LastCheckTime.ToString("yyyy/MM/dd HH:mm:ss").PadRight(maxLastCheckTimeLength));
        }

        template.Body = sb.ToString();

        return template;
    }
}

public class Pinging
{
    private Setting _setting = null;
    private Logger _logger = null;

    private List<string> _list = null;
    private CheckResultCollection _collection = null;

    public Pinging(Setting setting, Logger logger)
    {
        this._setting = setting;
        this._logger = logger;
    }

    public void Check()
    {
        _logger.Write(LogLevel.Debug, $"Setting parameter:\r\n" +
            System.Text.Json.JsonSerializer.Serialize(_setting,
                new System.Text.Json.JsonSerializerOptions()
                {
                    WriteIndented = true,
                }));
        _logger.Write(LogLevel.Debug, $"CurrentDirectory: {System.Environment.CurrentDirectory}");
        _logger.Write(LogLevel.Debug, $"Ping option: PingInterval=>{_setting.PingInterval}ms PingCount=>{_setting.PingCount} MaxFailedCount=>{_setting.MaxFailedCount}");

        if (!System.IO.File.Exists(_setting.ListPath))
        {
            _logger.Write(LogLevel.Error, $"List file is missing: {_setting.ListPath}");
            return;
        }

        LoadListFile(_setting.ListPath);

        LoadResultCollection(_setting.LogsPath);

        SendPing(_setting.PingInterval ?? 1000, _setting.PingCount ?? 4);

        SendMail(_setting.MailSmtpServer, _setting.MailSmtpPort ?? 25, _setting.MailTo, _setting.MailFrom);

        SaveResultCollection(_setting.LogsPath);
    }

    private void LoadListFile(string listPath)
    {
        _logger.Write("Load list file.");
        _logger.Write(LogLevel.Debug, $"List file: {listPath}");

        this._list = new();
        using (var stream = new System.IO.StreamReader(listPath, System.Text.Encoding.UTF8))
        using (var reader = new System.IO.StringReader(stream.ReadToEnd()))
        {
            string readLine = "";
            while ((readLine = reader.ReadLine()) != null)
            {
                string target = System.Text.RegularExpressions.Regex.Replace(readLine, "#.*$", "").Trim();
                if (target.Contains(" ")) target = target.Substring(0, target.IndexOf(" "));
                if (string.IsNullOrEmpty(target)) continue;
                _list.Add(target);
            }
        }
        _logger.Write("Check target:\r\n" +
            string.Join("\r\n", _list.Select(x => "  - " + x)));
    }

    private void LoadResultCollection(string logsPath)
    {
        _logger.Write("Load result collection file.");

        string dbFile = System.IO.Path.Combine(logsPath, "results.json");
        _collection = CheckResultCollection.Load(dbFile);

        _logger.Write(LogLevel.Debug, $"Result collection file: {dbFile}");
        _logger.Write($"Result collection count: {_collection.Results.Count}");
    }

    private void SendPing(int interval, int count)
    {
        _logger.Write("Send ping and reply check.");

        System.Net.NetworkInformation.Ping ping = new();

        foreach (string target in _list)
        {
            _logger.Write($"Ping send: {target}");

            bool ret = false;
            for (int i = 0; i < count; i++)
            {
                System.Net.NetworkInformation.PingReply reply = ping.Send(target);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    ret = true;
                    break;
                }
                System.Threading.Thread.Sleep(interval);
            }
            if (ret)
            {
                _logger.Write(LogLevel.Info, $"Ping success: {target}");
                _collection.AddSuccessTarget(target);
            }
            else
            {
                _logger.Write(LogLevel.Warn, $"Ping failed: {target}");
                _collection.AddFailTarget(target);
            }
        }
    }

    private void SendMail(string smtpServer, int port, string[] toAddress, string fromAddress)
    {
        _logger.Write("Send mail if alert or restore occures.");

        MailMessage mail = new MailMessage()
        {
            Server = smtpServer,
            Port = port,
            To = toAddress,
            From = fromAddress,
        };

        var alertTargets = _collection.GetAlertTarget(_setting.MaxFailedCount ?? 5);
        var restoreTargets = _collection.GetRestreTarget();
        if (alertTargets?.Length > 0 || restoreTargets?.Length > 0)
        {
            _logger.Write(LogLevel.Debug, "Send mail parameter:\r\n" +
                $"  SMTP : {smtpServer}\r\n" +
                $"  Port : {port}\r\n" +
                $"  To   : {string.Join(", ", toAddress)}\r\n" +
                $"  From : {fromAddress}");
        }

        try
        {
            if (alertTargets?.Length > 0)
            {
                _logger.Write("Start send alert mail process.");
                var template = MailTemplate.CreateAlertMail(alertTargets);
                mail.Send(template.Subject, template.Body);
            }
            if (restoreTargets?.Length > 0)
            {
                _logger.Write("Start send restore mail process.");
                var template = MailTemplate.CreateRestoreMail(restoreTargets);
                mail.Send(template.Subject, template.Body);
            }

        }
        catch (Exception e)
        {
            _logger.Write(LogLevel.Error, "Email sending failed.");
            _logger.Write(LogLevel.Debug, e.ToString());
        }
    }

    private void SaveResultCollection(string logsPath)
    {
        _logger.Write("Save result collection file.");

        string dbFile = System.IO.Path.Combine(logsPath, "results.json");
        _collection.Save(dbFile);

        _logger.Write(LogLevel.Debug, $"Result collection file: {dbFile}");
        _logger.Write($"Result collection count: {_collection.Results.Count}");
    }
}
"@

[System.Environment]::CurrentDirectory = $PSScriptRoot

Add-Type -TypeDefinition $source -ReferencedAssemblies `
    @(".\MailKit.dll",
      ".\MimeKit.dll",
      "System.Linq",
      "System.Runtime",
      "System.Text.Json",
      "System.Text.RegularExpressions",
      "System.Collections",
      "System.Net.Sockets",
      "System.Net.Ping",
      "System.Net.Primitives",
      "System.ComponentModel.Primitives",
      "System.Threading.Thread")

$setting = [Setting]::Load("setting.txt")
$logger = New-Object Logger -ArgumentList @($setting.LogsPath, "monitor")

$logger.Write("Start.")
$pinging = New-Object Pinging -ArgumentList @($setting, $logger)
$pinging.Check()
$logger.Write("End.")
