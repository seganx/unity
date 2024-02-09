using System;
using UnityEngine;

public static class WindowsProcess
{
    public static void RunCmd(string args, string workingDirectory = null)
    {
        RunCommand("cmd.exe", "/c " + args, workingDirectory);
    }

    public static void RunCommand(string command, string args, string workingDirectory = null)
    {
        Debug.Log(command + " " + args);

        var process = new System.Diagnostics.Process();

        // redirect the output stream of the child process.
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = args;

        if (!string.IsNullOrEmpty(workingDirectory))
        {
            process.StartInfo.WorkingDirectory = workingDirectory;
        }
        else
        {
            process.StartInfo.WorkingDirectory = Application.temporaryCachePath;
        }

        string output = null;
        try
        {
            process.Start();
            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        catch (Exception e)
        {
            Debug.LogError("Run error" + e.ToString()); // or throw new Exception
        }
        finally
        {
            Debug.Log(output + "\nExit code: " + process.ExitCode);
            process.Dispose();
        }
    }
}
