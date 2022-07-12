using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingMonitor
{
    public class CheckResultCollection
    {
        private List<CheckResult> _list { get; set; }

        public void Init()
        {
            this._list = new();
        }

        #region load/save

        public static CheckResultCollection Load(string dbFile)
        {
            CheckResultCollection collection = null;
            try
            {
                collection = System.Text.Json.JsonSerializer.Deserialize<CheckResultCollection>(
                    File.ReadAllText(dbFile),
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
            }
            return collection;
        }

        public void Save(string dbFile)
        {
            try
            {
                string parent = System.IO.Path.GetDirectoryName(dbFile);
                if (System.IO.Directory.Exists(parent)) System.IO.Directory.CreateDirectory(parent);

                System.Text.Json.JsonSerializer.Serialize(this, new System.Text.Json.JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true,
                });
            }
            catch { }
        }

        #endregion

        public void SuccessTarget(string target)
        {
            var index = this._list.FindIndex(x => x.Target == target);
            if (index >= 0)
            {
                _list[index].IsRestore = true;
            }
        }

        public void FailTarget(string target)
        {
            var index = this._list.FindIndex(x => x.Target == target);
            if (index >= 0)
            {
                _list[index].LastCheckTime = DateTime.Now;
                _list[index].FailedCount++;
            }
            else
            {
                _list.Add(new CheckResult()
                {
                    Target = target,
                    LastCheckTime = DateTime.Now,
                    FailedCount = 1,
                });
            }
        }

        public CheckResult[] GetAlertTarget(int maxFailedCount)
        {
            return _list.Where(x =>
                (x.FailedCount ?? 0) > maxFailedCount &&
                (x.IsNotified != true) &&
                (x.IsRestore != true)).
                ToArray();
        }

        public CheckResult[] GetRestreTarget()
        {
            var tempList = new List<CheckResult>();
            for (int i = _list.Count - 1; i <= 0; i--)
            {
                if (_list[i].IsRestore ?? false)
                {
                    tempList.Add(_list[i]);
                    _list.RemoveAt(i);
                }
            }
            tempList.Reverse();
            return tempList.ToArray();
        }
    }
}
