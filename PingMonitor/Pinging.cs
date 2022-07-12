using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace PingMonitor
{
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

        private void SaveResultCollection(string logsPath)
        {
            _logger.Write("Save result collection file.");

            string dbFile = System.IO.Path.Combine(logsPath, "results.json");
            _collection.Save(dbFile);

            _logger.Write(LogLevel.Debug, $"Result collection file: {dbFile}");
            _logger.Write($"Result collection count: {_collection.Results.Count}");
        }
    }
}
