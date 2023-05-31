using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

//Source: Velvary YT
public static class DataHandler
{
    public static void SavetoJSON<T>(List<T> toSave, string fileName)
    {
        Debug.Log(GetPath(fileName));
        string content = JSONHelper.ToJSON<T>(toSave.ToArray());
        WriteFile(GetPath(fileName), content);
    }

    public static List<T> ReadFromJSON<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));
        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }

        List<T> res = JSONHelper.FromJSON<T>(content).ToList();
        return res;
    }

    private static string GetPath(string fileName)
    {
        return Application.dataPath + "/" + fileName;
    }

    public static void WriteFile(string path, string content)
    {
        FileStream filestream = new(path, FileMode.Create);

        using StreamWriter writer = new(filestream);
        writer.Write(content);
    }

    private static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            StreamReader reader = new(path);
            string content = reader.ReadToEnd();
            return content;
        }
        return "";
    }
}