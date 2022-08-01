using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingMonitor
{
    public class CheckResultCollection
    {
        public int MaxFailedCount { get; set; }
        public int MinRestoreCount { get; set; }
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

        public void AddSuccess(string monitor, string target)
        {
            var index = this.Results.FindIndex(
                x => x.Monitor == monitor &&
                target == null ? true : x.Target == target);
            if (index >= 0)
            {
                Results[index].LastCheckTime = DateTime.Now;
                Results[index].FailedCount = 0;
                Results[index].RestoreCount ??= 0;
                Results[index].RestoreCount++;
            }
        }

        public void AddFailed(string monitor, string target = null)
        {
            var index = this.Results.FindIndex(
                x => x.Monitor == monitor &&
                target == null ? true : x.Target == target);
            if (index >= 0)
            {
                Results[index].LastCheckTime = DateTime.Now;
                Results[index].RestoreCount = 0;
                Results[index].FailedCount++;
            }
            else
            {
                Results.Add(new CheckResult()
                {
                    Monitor = monitor,
                    Target = target,
                    LastCheckTime = DateTime.Now,
                    FailedCount = 1,
                });
            }
        }

        public CheckResult[] GetAlert(string monitor)
        {
            var tempList = new List<CheckResult>();
            for (int i = 0; i < Results.Count; i++)
            {
                if (Results[i].Monitor == monitor &&
                    (Results[i].FailedCount ?? 0) > this.MaxFailedCount &&
                    Results[i].IsNotified != true)
                {
                    tempList.Add(Results[i]);
                    Results[i].IsNotified = true;
                }
            }

            return tempList.ToArray();
        }

        public CheckResult[] GetRestre(string monitor)
        {
            var tempList = new List<CheckResult>();
            for (int i = Results.Count - 1; i >= 0; i--)
            {
                if (Results[i].Monitor == monitor &&
                    (Results[i].RestoreCount ?? 0) > this.MinRestoreCount &&
                    Results[i].IsNotified == true)
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
