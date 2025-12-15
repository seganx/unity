using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;


namespace SeganX
{
    public static class ConsoleCommands
    {
        private static readonly List<Method> methods = new();

#if STAGING_BUILD || UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void RuntimeInitializeOnLoad()
        {
            Console.OnCommandEntered += RunCommand;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.FullName.IndexOf("Unity", StringComparison.Ordinal) >= 0) continue;
                if (assembly.FullName.IndexOf("System", StringComparison.Ordinal) == 0) continue;
                if (assembly.FullName.IndexOf("Mono", StringComparison.Ordinal) == 0) continue;
                if (assembly.FullName.IndexOf("mscorlib", StringComparison.Ordinal) >= 0) continue;
                if (assembly.FullName.IndexOf("netstandard", StringComparison.Ordinal) >= 0) continue;

                try
                {
                    var allMethods = assembly.GetTypes().SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)).Where(y => y.GetCustomAttribute<ConsoleAttribute>() != null).ToList();
                    foreach (var info in allMethods)
                    {
                        try
                        {
                            var attrib = info.GetCustomAttribute<ConsoleAttribute>();
                            var method = new Method()
                            {
                                name = $"{attrib.name}",
                                help = $"{GenerateMethodHelp(info)} {attrib.help}",
                                info = info
                            };
                            methods.Add(method);
                        }
                        catch (Exception e)
                        {
                            Debug.Log($"<color=yellow>{e.Message}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"<color=yellow>{e.Message}");
                }
            }
        }
#endif

        private static string GenerateMethodHelp(MethodInfo info)
        {
            var methodParams = info.GetParameters();
            if (methodParams.Length < 1) return string.Empty;
            var stringParams = new string[methodParams.Length];
            for (var i = 0; i < methodParams.Length; i++)
                stringParams[i] = $"[{methodParams[i].Name}:{GetTypeName(methodParams[i].ParameterType)}]";
            return string.Join(" ", stringParams);
        }

        private static void RunCommand(string str)
        {
            Debug.Log($"Executing: {str}");

            var cmd = str.Split(' ', 2);

            var name = cmd.Length > 0 ? cmd[0].ToLower() : str.Trim().ToLower();
            var method = methods.Find(x => x.name == name);
            if (method == null)
            {
                Debug.Log("Command not found!");
                return;
            }

            if (!method.info.IsStatic)
            {
                Debug.Log("Function is not static!");
                return;
            }

            var methodParams = method.info.GetParameters();
            if (methodParams.Length == 0)
            {
                method.info.Invoke(null, null);
                return;
            }

            if (cmd.Length < 2)
            {
                Debug.Log($"Mismatched parameters! The command should be like:\n{method.name} {method.help}");
                return;
            }

            var inputParams = cmd[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (methodParams.Length != inputParams.Length)
            {
                Debug.Log($"Mismatched parameters! The command should be like:\n{method.name} {method.help}");
                return;
            }

            var args = new object[methodParams.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var methodParam = methodParams[i];
                var inputParam = inputParams[i];
                if (methodParam.ParameterType == typeof(bool))
                    args[i] = inputParam.ToBoolean();
                else if (methodParam.ParameterType == typeof(int))
                    args[i] = int.TryParse(inputParam, out var intVal) ? intVal : 0;
                else if (methodParam.ParameterType == typeof(string))
                    args[i] = inputParam;
                else if (methodParam.ParameterType == typeof(float))
                    args[i] = float.TryParse(inputParam, out var floatVal) ? floatVal : 0;
                else
                {
                    Debug.Log("Not a type value!");
                    return;
                }
            }

            method.info.Invoke(null, args);
        }

        private static string GetTypeName(Type type)
        {
            if (type == typeof(bool)) return "bool";
            if (type == typeof(int)) return "int";
            if (type == typeof(string)) return "string";
            return type == typeof(float) ? "float" : type.Name;
        }



        ////////////////////////////////////////////////////////////////////////
        /// DEFAULT COMMANDS
        ////////////////////////////////////////////////////////////////////////
        [Console("help")]
        private static void DisplayHelp()
        {
            var helpStr = methods.Count > 0 ? "List of commands:\n" : "No command founded!";
            helpStr = methods.Aggregate(helpStr, (current, item) => current + $"{item.name} {item.help}\n");
            Debug.Log(helpStr);
        }

        [Console("cache.clear")]
        public static void ClearCache()
        {
            Caching.ClearCache();
            PlayerPrefs.DeleteAll();
            PlayerPrefsEx.ClearData();
            Debug.Log("Cache Cleared");
        }


        [Console("path.show.data")]
        public static void ShowPathData()
        {
            Debug.Log(Application.persistentDataPath);
        }

        [Console("path.show.cache")]
        public static void ShowPathCache()
        {
            Debug.Log(Application.temporaryCachePath);
        }

        [Console("system.show.info")]
        public static void ShowSystemInfo()
        {
            var str = "System info:" +
                "\nGPU Memory: " + SystemInfo.graphicsMemorySize + 
                "\nSystem Memory: " + SystemInfo.systemMemorySize +
                "\nTotalReservedMemory: " + UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1048576 +
                "\nTotalAllocatedMemory: " + UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576 +
                "\nTotalUnusedReservedMemory:" + UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong() / 1048576 +
#if UNITY_EDITOR
                "mb\nDrawCalls: " + UnityEditor.UnityStats.drawCalls +
                "\nUsed Texture Memory: " + UnityEditor.UnityStats.usedTextureMemorySize / 1048576 +
                "\nRenderedTextureCount: " + UnityEditor.UnityStats.usedTextureCount;
#else
                "";
#endif
            Debug.Log(str);
        }


        ///////////////////////////////////////////////////////////////////////
        //// NESTED MEMBERS
        ///////////////////////////////////////////////////////////////////////
        private class Method
        {
            public string name;
            public string help;
            public MethodInfo info;
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleAttribute : PreserveAttribute
    {
        public readonly string name;
        public readonly string help;

        public ConsoleAttribute(string name, string help = "")
        {
            this.name = name.ToLower();
            this.help = help;
        }
    }
}