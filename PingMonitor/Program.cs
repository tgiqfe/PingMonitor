using PingMonitor;

System.Environment.CurrentDirectory =
    System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

Setting setting = Setting.Load("setting.txt");

Logger logger = new Logger(setting.LogsPath, "monitor");
logger.Write("Start.");

var pinging = new Pinging(setting, logger);
pinging.Check();

logger.Write("End.");
Console.ReadLine();
