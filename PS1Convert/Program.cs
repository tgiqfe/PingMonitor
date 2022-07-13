using System.IO;

string outputPath = @"..\..\..\..\PingMonitor\PingMonitor.ps1";
string targetDir = @"..\..\..\..\PingMonitor";


foreach (string file in Directory.GetFiles(targetDir, "*.cs", SearchOption.AllDirectories))
{
    string fileName = Path.GetFileName(file);
    if(fileName == "Program.cs")
    {
        continue;
    }


}



Console.ReadLine();
