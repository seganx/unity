using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SymbolicLinker
{
    private const string excludes = "Excludes";

    public static bool ExistExcludes => Directory.Exists(Path.GetFullPath(Application.dataPath + "/" + excludes));

    [MenuItem("SeganX/Symbolic Links/Add " + excludes)]
    public static void AddExcludes()
    {
        AddSymbolicDirectory(excludes, "../" + excludes);        
    }

    [MenuItem("SeganX/Symbolic Links/Remove " + excludes)]
    public static void RemoveExcludes()
    {
        RemoveSymbolicDirectory(excludes);
    }

    public static void AddSymbolicDirectory(string destDirectory, string srcDirectory)
    {
        var srcsPath = Path.GetFullPath(Application.dataPath + "/" + srcDirectory);
        if (Directory.Exists(srcsPath) == false)
        {
            try
            {
                Directory.CreateDirectory(srcsPath);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                return;
            }
        }

        var destPath = Path.GetFullPath(Application.dataPath + "/" + destDirectory);
        if (Directory.Exists(destPath)) return;

        var command = "mklink /J \"" + destPath + "\" \"" + srcsPath + "\"";
        WindowsProcess.RunCmd(command);
        AssetDatabase.Refresh();
    }

    public static void RemoveSymbolicDirectory(string directory)
    {
        var path = Path.GetFullPath(Application.dataPath + "/" + directory);
        if (Directory.Exists(path) == false) return;
        WindowsProcess.RunCmd("rmdir " + path);
        path += ".meta";
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        AssetDatabase.Refresh();
    }
}
