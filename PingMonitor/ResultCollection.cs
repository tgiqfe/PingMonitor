using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingMonitor
{
    public class ResultCollection
    {
        public List<CheckResult> List { get; set; }

        public void Init()
        {
            this.List = new();
        }

        #region load/save

        public static ResultCollection Load(string dbFile)
        {
            ResultCollection collection = null;
            try
            {
                collection = System.Text.Json.JsonSerializer.Deserialize<ResultCollection>(
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
            var index = this.List.FindIndex(x => x.Target == target);
            if (index >= 0)
            {
                List[index].DeleteReserve = true;
            }
        }

        public void FailTarget(string target)
        {
            var index = this.List.FindIndex(x => x.Target == target);
            if (index >= 0)
            {
                List[index].LastCheckTime = DateTime.Now;
                List[index].FailedCount++;
            }
            else
            {
                List.Add(new CheckResult()
                {
                    Target = target,
                    LastCheckTime = DateTime.Now,
                    FailedCount = 1,
                });
            }
        }
    }
}
