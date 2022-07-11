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
        private List<string> _list = null;

        public void Check(string listPath, int interval, int count)
        {
            LoadListFile(listPath);
            SendPing(interval, count);
        }

        public void LoadListFile(string listPath)
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

        public void SendPing(int interval, int count)
        {
            System.Net.NetworkInformation.Ping ping = new();

            foreach (string target in _list)
            {
                for (int i = 0; i < count; i++)
                {
                    System.Net.NetworkInformation.PingReply reply = ping.Send(target);
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        //  Ping成功した場合
                    }
                    else
                    {
                        //  Ping失敗した場合
                    }
                }
            }
        }
    }
}
