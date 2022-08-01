using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingMonitor
{
    public class Setting : SettingBase
    {
        public string LogsPath { get; set; }
        public int? RetentionPeriod { get; set; }
        public PingSetting Ping { get; set; }
        public MailSetting Mail { get; set; }


        /*
        public string ListPath { get; set; }
        
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
        */
    }
}
