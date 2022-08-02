using System.IO;
using System.Text;

string outputPath = @"output.ps1";
string targetDir = @"..\..\..\..\PingMonitor";

if (File.Exists(outputPath))
{
    File.Delete(outputPath);
}

using (var sw = new StreamWriter(outputPath, true, System.Text.Encoding.UTF8))
{
    sw.WriteLine("$source = @\"");
}

string[] exDir = new string[]
{
    System.IO.Path.Combine(targetDir, "bin"),
    System.IO.Path.Combine(targetDir, "obj")
};

foreach (string file in Directory.GetFiles(targetDir, "*.cs", SearchOption.AllDirectories))
{
    string fileName = Path.GetFileName(file);
    if (fileName == "Program.cs")
    {
        continue;
    }
    if (exDir.Any(x => file.StartsWith(x)))
    {
        continue;
    }

    string content = File.ReadAllText(file);
    List<string> list = new();
    using (var reader = new StringReader(content))
    {
        bool during1 = false;
        bool during2 = false;
        string readLine = "";
        while ((readLine = reader.ReadLine()) != null)
        {
            if (during2)
            {
                list.Add(readLine);
            }
            else if (during1 && readLine == "{")
            {
                during2 = true;
            }
            else if (readLine.StartsWith("namespace "))
            {
                during1 = true;
            }
        }
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] == "}")
            {
                list.RemoveRange(i, list.Count - i);
                break;
            }
        }
    }
    using (var sw = new StreamWriter(outputPath, true, System.Text.Encoding.UTF8))
    {
        sw.WriteLine(string.Join("\r\n", list));
    }
}

using (var sw = new StreamWriter(outputPath, true, System.Text.Encoding.UTF8))
{
    sw.WriteLine("\"@");
}



Console.ReadLine();
