using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingMonitor
{
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
                File.WriteAllText(dbFile, json);
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
}
