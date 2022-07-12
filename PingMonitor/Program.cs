using PingMonitor;

Setting setting = Setting.Load("setting.txt");

string json = System.Text.Json.JsonSerializer.Serialize(setting, 
    new System.Text.Json.JsonSerializerOptions()
    {
        WriteIndented = true,
    });
Console.WriteLine(json);

Logger logger = new Logger(setting.LogsPath, "monitor");
logger.Write("開始");
logger.Write("テスト");

var pinging = new Pinging(setting, logger);
pinging.Check();

logger.Write("終了");
Console.ReadLine();
