using PingMonitor;
using System.Text.Json;

System.Environment.CurrentDirectory =
    System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

/*
Setting setting = Setting.Load("setting.txt");

Logger logger = new Logger(setting.LogsPath, "monitor");
logger.Write("Start.");

var pinging = new Pinging(setting, logger);
pinging.Check();

logger.Write("End.");
*/

string settingFile = "Setting.txt";
var seeker = new TextSeeker(System.IO.File.ReadAllText(settingFile));
Setting setting = new Setting();
setting.Load(seeker);

string json = JsonSerializer.Serialize(setting,
    new JsonSerializerOptions()
    {
        WriteIndented = true,
    });

Console.WriteLine(json);



Console.ReadLine();
