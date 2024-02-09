#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SeganX.Builder
{
    public static class BuilderFileHandler
    {
        private const string meta = ".meta";

        public static string AssetsFolder => Path.GetFullPath(Application.dataPath + Path.DirectorySeparatorChar);
        public static string ProjectFolder => Path.GetFullPath(Path.Combine(AssetsFolder, "../"));

        public static string PreparePath(string path, bool makeAsDirectoryPath = true)
        {
            if (makeAsDirectoryPath)
            {
                var res = path.Replace("\\", "/");
                return res.EndsWith("/") ? res : (res + "/");
            }
            else return path.Replace("\\", "/");
        }

        public static string MakePath(string destinationFolder, string sourceFolder, string sourcePath)
        {
            destinationFolder = PreparePath(destinationFolder);
            sourceFolder = PreparePath(sourceFolder);
            sourcePath = PreparePath(sourcePath, false);
            var filename = sourcePath.Replace(sourceFolder, "");
            return Path.Combine(destinationFolder, filename);
        }

        public static string MakePath(string str1, string str2)
        {
            return Path.GetFullPath(Path.Combine(str1, str2));
        }

        public static void ValidateDirectory(string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);
        }

        public static bool SaveFile(string filename, byte[] data)
        {
            if (data == null) return false;
            try
            {
                filename = MakePath(Application.dataPath, filename);
                ValidateDirectory(filename);
                File.WriteAllBytes(filename, data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public static async Task ExecuteFile(string filename)
        {
            if (filename.IsNullOrEmpty()) return;
            filename = MakePath(Application.dataPath, filename);
            var process = System.Diagnostics.Process.Start(filename);
            while (process.HasExited == false)
                await Task.Delay(1000);
        }

        public static bool MoveFile(string srcFilename, string destFilename, bool deleteSourceFile)
        {
            ValidateDirectory(destFilename);
            try
            {
                if (File.Exists(srcFilename))
                    File.Copy(srcFilename, destFilename, true);
                if (File.Exists(srcFilename + meta))
                    File.Copy(srcFilename + meta, destFilename + meta, true);
                if (deleteSourceFile) DeleteFile(srcFilename);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            return true;
        }

        public static bool DeleteFile(string filename)
        {
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                if (File.Exists(filename + meta))
                    File.Delete(filename + meta);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            return true;
        }

        public static bool ReplaceFileText(string filename, string what, string with)
        {
            if (File.Exists(filename) == false) return false;
            try
            {
                var text = File.ReadAllText(filename);
                text = text.Replace(what, with);
                File.WriteAllText(filename, text);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            return true;
        }

        public static bool ReplaceWholeFile(string srcFilename, string destFilename)
        {
            if (File.Exists(srcFilename) == false) return false;
            try
            {
                ValidateDirectory(destFilename);
                File.WriteAllBytes(destFilename, File.ReadAllBytes(srcFilename));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            return true;
        }

        public static void RemoveEmptyDirectories(string path)
        {
            if (Directory.Exists(path) == false) return;

            var dires = Directory.GetDirectories(path);
            foreach (var dir in dires)
                RemoveEmptyDirectories(dir);

            var files = Directory.EnumerateFileSystemEntries(path);
            var filecount = files.Count(x => x.EndsWith(meta) == false);
            if (filecount == 0)
            {
                try
                {
                    if (File.Exists(path + meta))
                        File.Delete(path + meta);
                    Directory.Delete(path);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private static bool IsDirectoryEmpty(string path)
        {
            var files = Directory.GetFiles(path);
            if (files.Length > 0) return false;
            var dires = Directory.GetDirectories(path);
            foreach (var dir in dires)
                if (IsDirectoryEmpty(dir) == false)
                    return false;
            return true;
        }

        public static class Externals
        {
            private static string externalPath = string.Empty;
            private static readonly List<string> externalFiles = new List<string>();

            public static void Cleanup()
            {
                foreach (var item in externalFiles)
                {
                    DeleteFile(item);
                }
            }

            public static bool Bring(string asset)
            {
                externalPath = Path.GetFullPath(asset);
                return BringToUnity(asset);
            }

            private static bool BringToUnity(string path)
            {
                path = Path.GetFullPath(path);
                if (File.Exists(path))
                    return BringFile(path);

                var files = Directory.GetFiles(path);
                var dires = Directory.GetDirectories(path);
                foreach (var item in files)
                {
                    var extension = Path.GetExtension(item);
                    if (extension == meta) continue;

                    if (BringFile(item) == false)
                        return false;
                }

                foreach (var item in dires)
                    if (BringToUnity(item) == false)
                        return false;

                return true;
            }

            private static bool BringFile(string filename)
            {
                if (externalFiles.Exists(x => x.Equals(filename, StringComparison.OrdinalIgnoreCase))) return true;

                var destFilename = MakePath(AssetsFolder, externalPath, filename);

                if (MoveFile(filename, destFilename, false))
                {
                    externalFiles.Add(destFilename);
                    return true;
                }
                return false;
            }
        }

        public static class Bakcup
        {
            private class BackupFiles
            {
                public string filename = string.Empty;
                public string backpath = string.Empty;
            }

            private static readonly List<BackupFiles> backupFiles = new List<BackupFiles>();

            public static string BackFolder => Path.GetFullPath(Path.Combine(AssetsFolder, "../BuilderBackup") + Path.DirectorySeparatorChar);

            private static string PathToAssets(string filename)
            {
                return MakePath(AssetsFolder, BackFolder, filename);
            }

            private static string PathToBackup(string filename)
            {
                return MakePath(BackFolder, AssetsFolder, filename);
            }

            public static bool BackupFile(string filename)
            {
                filename = Path.GetFullPath(filename);
                if (File.Exists(filename) == false) return false;
                if (backupFiles.Exists(x => x.filename.Equals(filename, StringComparison.OrdinalIgnoreCase))) return true;

                var backup = new BackupFiles
                {
                    filename = PathToAssets(filename),
                    backpath = PathToBackup(filename)
                };

                if (MoveFile(backup.filename, backup.backpath, false))
                {
                    backupFiles.Add(backup);
                    return true;
                }
                return false;
            }

            public static bool DisableFileOrDir(string filename)
            {
                filename = Path.GetFullPath(filename);
                if (File.Exists(filename))
                    return DisableFile(filename);

                var files = Directory.GetFiles(filename);
                var dires = Directory.GetDirectories(filename);
                foreach (var item in files)
                {
                    var extension = Path.GetExtension(item);
                    if (extension == meta) continue;

                    if (DisableFile(item) == false)
                        return false;
                }

                foreach (var item in dires)
                    if (DisableFileOrDir(item) == false)
                        return false;

                return true;
            }

            private static bool DisableFile(string filename)
            {
                if (File.Exists(filename) == false) return true;
                if (backupFiles.Exists(x => x.filename.Equals(filename, StringComparison.OrdinalIgnoreCase))) return true;

                var backup = new BackupFiles
                {
                    filename = PathToAssets(filename),
                    backpath = PathToBackup(filename)
                };

                if (MoveFile(backup.filename, backup.backpath, true))
                {
                    backupFiles.Add(backup);
                    return true;
                }
                return false;
            }

            public static bool RestoreFile(string filename)
            {
                filename = Path.GetFullPath(filename);
                var backup = backupFiles.Find(x => x.filename == filename);
                if (backup == null) return true;
                if (MoveFile(backup.filename, backup.backpath, false))
                {
                    backupFiles.Remove(backup);
                    return true;
                }
                return false;
            }

            public static bool RestoreAllFiles()
            {
                bool noError = true;

                // restore all files
                foreach (var item in backupFiles)
                {
                    if (MoveFile(item.backpath, item.filename, false) == false)
                        noError = false;
                }

                // remove all files from backup place
                if (noError)
                {
                    foreach (var item in backupFiles)
                    {
                        DeleteFile(item.backpath);
                    }

                    backupFiles.Clear();
                    try
                    {
                        if (Directory.Exists(BackFolder))
                            Directory.Delete(BackFolder, true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                return noError;
            }
        }



        [MenuItem("SeganX/Tools/Cleanup Folders")]
        public static void RemoveEmptyDirectoriesFromAssets()
        {
            RemoveEmptyDirectories(Application.dataPath);
        }
    }
}
#endif