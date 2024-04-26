using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileReaderWriter
{
    public static string ReadFromFile(string path)
    {
        if (!File.Exists("Assets/Resources/TextFiles/" + path + ".txt"))
        {
            Debug.LogError($"File at path: Assets/Resources/TextFiles/{path}.txt does not exist.");
        }

        string data;
        StreamReader reader = new StreamReader("Assets/Resources/TextFiles/" + path + ".txt");
        data = reader.ReadToEnd();
        reader.Dispose();
        reader.Close();
        return data;
    }

    public static List<string> ReadLinesFromFile(string path, char sperarator = ';', bool removeSeparator = true)
    {
        List<string> lines = new List<string>(ReadFromFile(path).Split(sperarator));

        if (removeSeparator)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                int lenght = lines[i].Length;
                lines[i].Remove(lenght);
            }
        }
        return lines;
    }

    public static void WriteToFile(string path, string text, bool overrideFile = false)
    {
        if(!File.Exists("Assets/Resources/TextFiles/" + path + ".txt"))
        {
            Debug.Log($"File at: Assets/Resources/TextFiles/{path}.txt does not exist, creating new file.");
            File.Create("Assets/Resources/TextFiles/" + path + ".txt");
        }

        StreamWriter writer = new StreamWriter("Assets/Resources/TextFiles/" + path + ".txt");
        writer.Write(text);
        writer.Flush();
        writer.Dispose();
        writer.Close();
        Debug.Log("Writing to: " + "Assets/Resources/TextFiles/" + path + ".txt");
    }

    public static void WriteLineToFile(string path, string text, bool overrideFile = false)
    {
        if (!File.Exists("Assets/Resources/TextFiles/" + path + ".txt"))
        {
            Debug.Log($"File at: Assets/Resources/TextFiles/{path}.txt does not exist, creating new file.");
            File.Create("Assets/Resources/TextFiles/" + path + ".txt");
        }

        StreamWriter writer = new StreamWriter("Assets/Resources/TextFiles/" + path + ".txt");
        writer.WriteLine(text);
        writer.Dispose();
        writer.Flush();
        writer.Close();
    }

    public static void WriteLinesToFile(string path, List<string> lines, bool overrideFile = false)
    {
        foreach(string line in lines)
        {
            WriteLineToFile(path, line, overrideFile);
        }
    }

    public static void WriteLinesToFile(string path, List<string> lines, char sperarator, bool overrideFile = false)
    {
        for(int i = 0; i < lines.Count; i++)
        {
            lines[i] += sperarator;
        }
        WriteLinesToFile(path, lines, overrideFile);
    }
}
