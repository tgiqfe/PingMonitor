using System.IO;
using System.Text;

string outputPath = @"output.txt";
string targetDir = @"..\..\..\..\PingMonitor";


foreach (string file in Directory.GetFiles(targetDir, "*.cs", SearchOption.AllDirectories))
{
    string fileName = Path.GetFileName(file);
    if (fileName == "Program.cs")
    {
        continue;
    }

    string content = File.ReadAllText(file);
    using (var reader = new StringReader(content))
    {
        StringBuilder sb = new StringBuilder();
        bool during = false;
        string readLine = "";
        while ((readLine = reader.ReadLine()) != null)
        {
            if (during)
            {
                sb.AppendLine(readLine);
            }
        }
    }
}



Console.ReadLine();
