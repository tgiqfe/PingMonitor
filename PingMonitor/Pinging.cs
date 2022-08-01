﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingMonitor
{
    public class Pinging
    {
        private Setting _setting2 = null;
        private Logger _logger2 = null;

        private List<string> _list2 = null;
        private CheckResultCollection _collection = null;









        private static bool _enabled = false;
        private static Setting _setting = null;
        private static Logger _logger = null;
        private static List<string> _list = null;

        public static void Prepare(Setting setting)
        {
            _setting = setting;
            _logger = new Logger(setting.LogsPath, "monitor");
            _logger.Write("Start.");

            _logger.Write(LogLevel.Debug, $"Setting parameter:\r\n" +
                System.Text.Json.JsonSerializer.Serialize(setting,
                    new System.Text.Json.JsonSerializerOptions()
                    {
                        IgnoreReadOnlyProperties = true,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true,
                    }));
            _logger.Write(LogLevel.Debug, $"CurrentDirectory: {System.Environment.CurrentDirectory}");

            if (setting.Ping == null || string.IsNullOrEmpty(setting.Ping.ListPath))
            {
                _logger.Write(LogLevel.Error, $"Ping setting is undefined.");
                return;
            }
            _logger.Write(LogLevel.Debug, $"Ping option: Interval=>{setting.Ping.Interval}ms Count=>{setting.Ping.Count} MaxFailedCount=>{setting.Ping.MaxFailedCount} MinRestoreCount=>{setting.Ping.MinRestoreCount}");

            if (!System.IO.File.Exists(setting.Ping.ListPath))
            {
                _logger.Write(LogLevel.Error, $"List file is missing: {setting.Ping.ListPath}");
                return;
            }
            _enabled = true;
        }

        public static void LoadListFile()
        {
            if (!_enabled) return;

            _logger.Write("Load list file.");
            _logger.Write(LogLevel.Debug, $"List file: {_setting.Ping.ListPath}");

            _list = new();
            using (var stream = new System.IO.StreamReader(_setting.Ping.ListPath, System.Text.Encoding.UTF8))
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






        /*
        public void Check()
        {
            _logger2.Write(LogLevel.Debug, $"Setting parameter:\r\n" +
                System.Text.Json.JsonSerializer.Serialize(_setting,
                    new System.Text.Json.JsonSerializerOptions()
                    {
                        WriteIndented = true,
                    }));
            _logger2.Write(LogLevel.Debug, $"CurrentDirectory: {System.Environment.CurrentDirectory}");
            _logger2.Write(LogLevel.Debug, $"Ping option: PingInterval=>{_setting.PingInterval}ms PingCount=>{_setting.PingCount} MaxFailedCount=>{_setting.MaxFailedCount}");

            if (!System.IO.File.Exists(_setting.ListPath))
            {
                _logger2.Write(LogLevel.Error, $"List file is missing: {_setting.ListPath}");
                return;
            }

            LoadListFile(_setting.ListPath);

            LoadResultCollection(_setting.LogsPath);

            SendPing(_setting.PingInterval ?? 1000, _setting.PingCount ?? 4);

            SendMail(_setting.MailSmtpServer, _setting.MailSmtpPort ?? 25, _setting.MailTo, _setting.MailFrom);

            SaveResultCollection(_setting.LogsPath);
        }
        */

        /*
        private void LoadListFile(string listPath)
        {
            _logger2.Write("Load list file.");
            _logger2.Write(LogLevel.Debug, $"List file: {listPath}");

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
            _logger2.Write("Check target:\r\n" +
                string.Join("\r\n", _list.Select(x => "  - " + x)));
        }
        */

        private void LoadResultCollection(string logsPath)
        {
            _logger2.Write("Load result collection file.");

            string dbFile = System.IO.Path.Combine(logsPath, "results.json");
            _collection = CheckResultCollection.Load(dbFile);

            _logger2.Write(LogLevel.Debug, $"Result collection file: {dbFile}");
            _logger2.Write($"Result collection count: {_collection.Results.Count}");
        }

        private void SendPing(int interval, int count)
        {
            _logger2.Write("Send ping and reply check.");

            System.Net.NetworkInformation.Ping ping = new();

            foreach (string target in _list2)
            {
                _logger2.Write($"Ping send: {target}");

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
                    _logger2.Write(LogLevel.Info, $"Ping success: {target}");
                    _collection.AddSuccessTarget(target);
                }
                else
                {
                    _logger2.Write(LogLevel.Warn, $"Ping failed: {target}");
                    _collection.AddFailTarget(target);
                }
            }
        }

        private void SendMail(string smtpServer, int port, string[] toAddress, string fromAddress)
        {
            _logger2.Write("Send mail if alert or restore occures.");

            MailMessage mail = new MailMessage()
            {
                Server = smtpServer,
                Port = port,
                To = toAddress,
                From = fromAddress,
            };

            var alertTargets = _collection.GetAlertTarget(_setting2.Ping.MaxFailedCount ?? 5);
            var restoreTargets = _collection.GetRestreTarget();
            if (alertTargets?.Length > 0 || restoreTargets?.Length > 0)
            {
                _logger2.Write(LogLevel.Debug, "Send mail parameter:\r\n" +
                    $"  SMTP : {smtpServer}\r\n" +
                    $"  Port : {port}\r\n" +
                    $"  To   : {string.Join(", ", toAddress)}\r\n" +
                    $"  From : {fromAddress}");
            }

            try
            {
                if (alertTargets?.Length > 0)
                {
                    _logger2.Write("Start send alert mail process.");
                    var template = MailTemplate.CreateAlertMail(alertTargets);
                    //mail.Send(template.Subject, template.Body);
                    mail.Subject = template.Subject;
                    mail.Body = template.Body;
                    mail.Send();
                }
                if (restoreTargets?.Length > 0)
                {
                    _logger2.Write("Start send restore mail process.");
                    var template = MailTemplate.CreateRestoreMail(restoreTargets);
                    //mail.Send(template.Subject, template.Body);
                    mail.Subject = template.Subject;
                    mail.Body = template.Body;
                    mail.Send();
                }

            }
            catch (Exception e)
            {
                _logger2.Write(LogLevel.Error, "Email sending failed.");
                _logger2.Write(LogLevel.Debug, e.ToString());
            }
        }

        private void SaveResultCollection(string logsPath)
        {
            _logger2.Write("Save result collection file.");

            string dbFile = System.IO.Path.Combine(logsPath, "results.json");
            _collection.Save(dbFile);

            _logger2.Write(LogLevel.Debug, $"Result collection file: {dbFile}");
            _logger2.Write($"Result collection count: {_collection.Results.Count}");
        }
    }
}
