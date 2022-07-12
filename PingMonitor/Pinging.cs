using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            LoadListFile(_setting.ListPath);

            LoadResultCollection(_setting.LogsPath);

            SendPing(_setting.PingInterval ?? 1000, _setting.PingCount ?? 4);

            SendMail(_setting.MailSmtpServer, _setting.MailSmtpPort ?? 25, _setting.MailTo, _setting.MailFrom);
        }

        private void LoadListFile(string listPath)
        {
            this._list = new();
            using (var stream = new StreamReader(listPath, Encoding.UTF8))
            using (var reader = new StringReader(stream.ReadToEnd()))
            {
                string readLine = "";
                while ((readLine = reader.ReadLine()) != null)
                {
                    string target = System.Text.RegularExpressions.Regex.Replace(readLine, "#.*$", "");
                    if (target.Contains(" ")) target = target.Substring(0, target.IndexOf(" "));
                    if (string.IsNullOrEmpty(target)) continue;
                    _list.Add(target);
                }
            }
        }

        private void LoadResultCollection(string logsPath)
        {
            string dbFile = System.IO.Path.Combine(logsPath, "results.json");
            _collection = CheckResultCollection.Load(dbFile);
        }

        private void SendPing(int interval, int count)
        {
            System.Net.NetworkInformation.Ping ping = new();

            foreach (string target in _list)
            {
                for (int i = 0; i < count; i++)
                {
                    System.Net.NetworkInformation.PingReply reply = ping.Send(target);
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        _collection.SuccessTarget(target);
                    }
                    else
                    {
                        _collection.FailTarget(target);
                    }
                }
            }
        }

        private void SendMail(string smtpServer, int port, string[] toAddress, string fromAddress)
        {
            MailMessage mail = new MailMessage()
            {
                Server = smtpServer,
                Port = port,
                To = toAddress,
                From = fromAddress,
            };

            var alertTargets = _collection.GetAlertTarget(_setting.MaxFailedCount ?? 5);
            if (alertTargets?.Length > 0)
            {
                var template = MailTemplate.CreateAlertMail(alertTargets);
                mail.Send(template.Subject, template.Body);
            }

            var restoreTargets = _collection.GetRestreTarget();
            if (restoreTargets?.Length > 0)
            {
                var template = MailTemplate.CreateRestoreMail(restoreTargets);
                mail.Send(template.Subject, template.Body);
            }
        }
    }
}
