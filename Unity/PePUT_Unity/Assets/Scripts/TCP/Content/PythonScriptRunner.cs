using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;


public class PythonScriptRunner
{
    Process process;

    public void RunScript(string path, string python_exe_path)
    {
        UnityEngine.Debug.Log("exectuting Python script");
        
        //Check if paths are valid
        if (!File.Exists(python_exe_path))
        {
            UnityEngine.Debug.Log("Python interpreter not found at: " + python_exe_path);
            return;
        }
        if (!File.Exists(path.Split(' ')[0]))
        {
            UnityEngine.Debug.Log("Python script not found at: " + path);
            return;
        }

        //Setup Process
        ProcessStartInfo start = new ProcessStartInfo();
        UnityEngine.Debug.Log("processstart info: " + start);
        start.FileName = python_exe_path;
        start.Arguments = path;
        start.UseShellExecute = true;
        start.RedirectStandardOutput = false;
        start.CreateNoWindow = false;

        //Start Process
        process = Process.Start(start);

        UnityEngine.Debug.Log("script executed");
    }

    public void RunServer(bool blocking = true)
    {
        var current = Application.dataPath + "/../../../Python/Test.py";
        string python_file_fullpath = Path.GetFullPath(current);
        UnityEngine.Debug.Log(python_file_fullpath);

        var current_exe = Application.dataPath + "/../../../Python/venv/Scripts/python.exe";
        string python_exe_fullpath = Path.GetFullPath(current_exe);
        UnityEngine.Debug.Log(python_exe_fullpath);

        string args = string.Format("{0}", python_file_fullpath);
        UnityEngine.Debug.Log(args);
        RunScript(args, python_exe_fullpath);

        if (blocking)
        {
            string processOutput = "";
            while(processOutput != "blocking")
            {
                processOutput = process.StandardOutput.ReadLine();
            }
        }
    }
}
