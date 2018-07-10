using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CheckScriptNamespace
{
    private static readonly string rootPath = "/Scripts";

    private static readonly string namespaceTag = "namespace";
    private static readonly string startString = "\nnamespace Test\n{";
    private static readonly string endString = "\n}";

    private static string[] checkStrings = new string[]{ "class", "enum", "struct", "interface" };

    [MenuItem("Assets/AddNamespace")]
    public static void AddNamespace()
    {
        CheckFiles(Application.dataPath + rootPath);
    }

    private static void CheckFiles(string dataPath)
    {
        var files = Directory.GetFiles(dataPath, "*.cs", SearchOption.AllDirectories);
        if(files != null)
        {
            foreach(var item in files)
            {
                if(!ContainNamespace(item))
                {
                    AddNamespaceToFile(item);
                }
            }
        }
    }

    private static bool ContainNamespace(string fileName)
    {
        bool result = false;

        using(StreamReader streamReader = new StreamReader(fileName))
        {
            var content = streamReader.ReadToEnd();
            result = content.Contains(namespaceTag);
            streamReader.Close();
        }

        return result;
    }

    private static void AddNamespaceToFile(string fileName)
    {
        StreamReader streamReader = new StreamReader(fileName);
        StringBuilder stringBuilder = new StringBuilder();

        string line;
        bool hasAdded = false;

        while((line = streamReader.ReadLine()) != null)
        {
            if(FilterLine(line) && !hasAdded)
            {
                stringBuilder.AppendLine(startString);
                hasAdded = true;
            }

            stringBuilder.AppendLine(line);
        }

        if(hasAdded)
        {
            stringBuilder.Append(endString);
        }

        streamReader.Close();

        using(StreamWriter streamWriter = new StreamWriter(fileName))
        {
            streamWriter.WriteLine(stringBuilder.ToString());
            streamWriter.Close();
        }
    }

    private static bool FilterLine(string line)
    {
        if(string.IsNullOrEmpty(line))
        {
            return false;
        }

        bool result = false;

        for(var i = 0; i < checkStrings.Length; ++i)
        {
            if(line.IndexOf(checkStrings[i]) >= 0)
            {
                result = true;
                break;
            }
        }

        return result;
    }
}
