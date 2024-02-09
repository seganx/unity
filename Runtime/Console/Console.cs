using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Linq;
using System.Reflection;
using UnityEngine.Profiling;
using UnityEngine.Scripting;

namespace SeganX
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleAttribute : PreserveAttribute
    {
        public string cmdClass;
        public string cmdMethod;
        public string cmdInfo;

        public ConsoleAttribute(string className, string methodName, string info = "")
        {
            cmdClass = className.ToLower();
            cmdMethod = methodName.ToLower();
            cmdInfo = info;
        }
    }

    [DefaultExecutionOrder(-99999)]
    public class Console : MonoBehaviour
    {
        public class MethodObject
        {
            public string space;
            public string name;
            public string help;
            public MethodInfo info;
        }

        public class LogText
        {
            public int repeated = 1;
            public Color color = Color.white;
            public string text = string.Empty;
            public float top = 0;
            public float height = 0;
            public Text visual = null;
            public string VisualText { get { return repeated < 2 ? text : "(" + repeated + ") " + text; } }
        }

        [Header("Header")]
        [SerializeField] private Button clearButton = null;
        [SerializeField] private Button savelogButton = null;
        [SerializeField] private Text infoLabel = null;
        [SerializeField] private Text systemInfo = null;
        [SerializeField] private Text fpsLabel = null;
        [Header("Body")]
        [SerializeField] private ScrollRect scrollbox = null;
        [Header("Footer")]
        [SerializeField] private InputField userInput = null;
        [SerializeField] private Button runButton = null;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (SystemInfo.systemMemorySize < 1000)
                maxTextLength = 2000;

            if (EventSystem.current == null)
                new GameObject("EventSystem", new Type[] { typeof(EventSystem), typeof(StandaloneInputModule), typeof(BaseInput) });

            threadedList.Add(new KeyValuePair<string, Color>("Start Version " + Application.version + " on " + Core.DeviceId + " based on " + Core.BaseDeviceId + " at " + DateTime.Now, Color.green));
            Application.logMessageReceivedThreaded += HandleLog;

            textVisual = scrollbox.content.GetChild<Text>(0);
            textVisual.text = string.Empty;

            savelogButton.onClick.AddListener(SaveLog);
            clearButton.onClick.AddListener(() =>
            {
                foreach (var item in textList)
                    if (item.visual != null)
                        item.visual.gameObject.DestroyNow();
                textList.Clear();
                scrollbox.content.transform.SetAnchordHeight(10);
                scrollbox.content.SetAnchordPositionY(0);
            });

            Debug.Log("Searching in all assemblies for console commands:");
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                //Debug.Log($"Searching in assembly {assembly}");

                if (assembly.FullName.IndexOf("Unity") >= 0) continue;
                if (assembly.FullName.IndexOf("System") == 0) continue;
                if (assembly.FullName.IndexOf("Mono") == 0) continue;
                if (assembly.FullName.IndexOf("mscorlib") >= 0) continue;
                if (assembly.FullName.IndexOf("netstandard") >= 0) continue;

                var allMethods = assembly.GetTypes().SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)).Where(y => y.GetCustomAttribute<ConsoleAttribute>() != null).ToList();
                foreach (var method in allMethods)
                {
                    var attrib = method.GetCustomAttribute<ConsoleAttribute>();
                    if (attrib.cmdMethod.IsNullOrEmpty()) attrib.cmdMethod = method.Name.ToLower();
                    if (attrib.cmdInfo.IsNullOrEmpty()) attrib.cmdInfo = GenerateMethodHelp(method);
                    methods.Add(new MethodObject() { space = attrib.cmdClass, name = attrib.cmdMethod, help = attrib.cmdInfo, info = method });
                    //Debug.Log($"Found command {attrib.cmdSpace} {attrib.cmdName}");
                }
            }

            userInput.onEndEdit.AddListener(str => RunCommand(str));
            runButton.onClick.AddListener(() => RunCommand(userInput.text));
        }

        void OnEnable()
        {
            if (gameObject.activeInHierarchy == false) return;
            Invoke("OnEnable", 0.5f);

            string str = "Ver: " + Application.version + "\nId: " + DisplayDeviceID;

            if (OnDisplayInfo != null)
                str = OnDisplayInfo(str);

            infoLabel.text = str;
            systemInfo.text = GetSystemInfo();
        }

        // Update is called once per frame
        private void Update()
        {
            ++fpsFrameCounter;
            fpsTimeCounter += Time.unscaledDeltaTime;
            if (fpsTimeCounter > 0.5f)
            {
                var current = fpsFrameCounter * fpsTimeCounter * 4;
                fpsLabel.text = "FPS:\n" + current.ToString("f1");
                fpsLabel.color = (current >= 30) ? Color.green : ((current < 10) ? Color.red : Color.yellow);
                fpsTimeCounter = 0;
                fpsFrameCounter = 0;
            }

            if (++threadCounter % 3 != 0) return;

            lock (threadedList)
            {
                if (threadedList.Count > 0)
                {
                    var item = threadedList[0];
                    threadedList.RemoveAt(0);
                    AddToLog(item.Key, item.Value);

                    var delta = scrollbox.content.rect.height - scrollbox.content.anchoredPosition.y - 10;
                    scrollbox.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ComputeHeight(textList));
                    if (scrollbox.viewport.rect.height >= delta)
                        scrollbox.verticalNormalizedPosition = 0;
                }
            }

            foreach (var item in textList)
            {
                float topEdge = item.top + scrollbox.content.anchoredPosition.y;
                float botEdge = topEdge - item.height;
                if (topEdge >= -scrollbox.viewport.rect.height && botEdge <= 0)
                {
                    if (item.visual == null)
                    {
                        item.visual = textVisual.gameObject.Clone<Text>();
                        item.visual.rectTransform.SetAnchordPositionY(item.top);
                        item.visual.rectTransform.SetAnchordHeight(item.height);
                        item.visual.color = item.color;
                        item.visual.text = item.VisualText;
                    }
                }
                else if (item.visual != null)
                {
                    item.visual.gameObject.DestroyNow();
                    item.visual = null;
                }
            }
        }


        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        public static Func<string, string> OnDisplayInfo = null;
        private static Console instance = null;
        private static float fpsTimeCounter = 0;
        private static int fpsFrameCounter = 0;
        private static int maxTextLength = 15000;
        private static int threadCounter = 0;
        private static Text textVisual = null;
        private static volatile List<KeyValuePair<string, Color>> threadedList = new List<KeyValuePair<string, Color>>();
        private static readonly List<LogText> textList = new List<LogText>();
        private static readonly List<MethodObject> methods = new List<MethodObject>();

        public static bool Enabled
        {
            get { return instance != null; }
            set
            {
                if (instance != null && value == false)
                    DestroyImmediate(instance.gameObject);
                else if (instance == null && value == true)
                    instance = Resources.Load<Console>("SeganX/Console").Clone<Console>();
            }
        }


        public static string GetSystemInfo()
        {
            return "GPU Memory: " + SystemInfo.graphicsMemorySize + " - System Memory: " + SystemInfo.systemMemorySize +
                "\nTotalAllocatedMemory: " + Profiler.GetTotalAllocatedMemoryLong() / 1048576 +
                "\nTotalReservedMemory: " + Profiler.GetTotalReservedMemoryLong() / 1048576 +
                "\nTotalUnusedReservedMemory:" + Profiler.GetTotalUnusedReservedMemoryLong() / 1048576 +
#if UNITY_EDITOR
                "mb\nDrawCalls: " + UnityEditor.UnityStats.drawCalls +
                "\nUsed Texture Memory: " + UnityEditor.UnityStats.usedTextureMemorySize / 1048576 +
                "\nRenderedTextureCount: " + UnityEditor.UnityStats.usedTextureCount;
#else
                "";
#endif
        }

        public static string DisplayDeviceID
        {
            get
            {
                string res = string.Empty;
                string str = Core.DeviceId;
                for (int i = 0; i < str.Length; i++)
                {
                    if (i > 0 && i % 4 == 0)
                        res += " ";
                    res += str[i];
                }
                return res;
            }
        }

        //////////////////////////////////////////////////////
        /// STATIC HELPER FUNTIONS
        //////////////////////////////////////////////////////
        public static void AddLog(string str, Color color)
        {
            threadedList.Add(new KeyValuePair<string, Color>(str, color));
        }

        #region Helper functions
        private static void HandleLog(string condition, string stacktrace, LogType type)
        {
            if (!Enabled || condition.IsNullOrEmpty()) return;
            lock (threadedList)
            {
                switch (type)
                {
                    case LogType.Assert:
                    case LogType.Warning: threadedList.Add(new KeyValuePair<string, Color>(condition + "\n" + stacktrace, Color.yellow)); break;
                    case LogType.Error:
                    case LogType.Exception: threadedList.Add(new KeyValuePair<string, Color>(condition + "\n" + stacktrace, Color.red)); break;
                    default: threadedList.Add(new KeyValuePair<string, Color>(condition, Color.white)); break;
                }
            }
        }

        private static void AddToLog(string str, Color color)
        {
            if (str.Length > maxTextLength)
            {
                int index = str.Length - maxTextLength;
                AddToLog(str.Remove(index), color);
                str = str.Substring(index);
            }

            var lastText = textList.Count > 0 ? textList[textList.Count - 1] : null;
            if (lastText != null && lastText.text == str)
            {
                lastText.repeated++;
                if (lastText.visual)
                {
                    lastText.visual.text = lastText.VisualText;
                    lastText.height = lastText.visual.preferredHeight;
                }
            }
            else
            {
                textVisual.text = str;
                var logtext = new LogText { text = str, color = color, height = textVisual.preferredHeight };
                textVisual.text = string.Empty;
                textList.Add(logtext);
            }
        }

        private static float ComputeHeight(List<LogText> list)
        {
            float height = 0;
            foreach (var item in list)
            {
                item.top = -height;
                height += item.height;
            }
            return height;
        }

        private static void RunCommand(string str)
        {
            //  handle help command
            if (str.ToLower() == "help")
            {
                string helpStr = methods.Count > 0 ? "List of commands:\n" : "No command founded!";
                foreach (var item in methods)
                    helpStr += item.space + " " + item.name + " " + item.help + "\n";
                Debug.Log(helpStr);
                return;
            }
            else Debug.Log("Execute: " + str);

            string[] cmd = str.Split(' ');
            if (cmd.Length < 2)
            {
                Debug.Log("Nothing to execute!");
                return;
            }

            var space = cmd[0].ToLower();
            var name = cmd[1].ToLower();
            var method = methods.Find(x => x.space == space && x.name == name);
            if (method == null)
            {
                Debug.Log("Command not found!");
                return;
            }
            else if (!method.info.IsStatic)
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
            else if (methodParams.Length != cmd.Length - 2)
            {
                Debug.Log("Mismatched parameters!\n" + method.space + " " + method.name + " " + method.help);
                return;
            }

            var arglist = new object[methodParams.Length];
            for (int i = 0; i < arglist.Length; i++)
            {
                var methodParam = methodParams[i];
                if (methodParam.ParameterType == typeof(bool))
                    arglist[i] = cmd[i + 2].ToBoolean();
                else if (methodParam.ParameterType == typeof(int))
                    arglist[i] = cmd[i + 2].ToInt();
                else if (methodParam.ParameterType == typeof(string))
                    arglist[i] = cmd[i + 2];
                else if (methodParam.ParameterType == typeof(float))
                    arglist[i] = cmd[i + 2].ToFloat();
                else
                {
                    Debug.Log("Not a type value!");
                    return;
                }
            }
            method.info.Invoke(null, arglist);
        }

        private static string GenerateMethodHelp(MethodInfo info)
        {
            var methodParams = info.GetParameters();
            if (methodParams.Length < 1) return string.Empty;
            var stringParams = new string[methodParams.Length];
            for (int i = 0; i < methodParams.Length; i++)
                stringParams[i] = methodParams[i].Name + ":" + GetTypeName(methodParams[i].ParameterType);
            return string.Join(" ", stringParams);
        }

        private static string GetTypeName(System.Type type)
        {
            if (type == typeof(bool)) return "bool";
            else if (type == typeof(int)) return "int";
            else if (type == typeof(string)) return "string";
            else if (type == typeof(float)) return "float";
            return type.Name;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static void ShareText(string title, string message)
        {
            try
            {
                AndroidJavaClass iClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject iObject = new AndroidJavaObject("android.content.Intent");
                iObject.Call<AndroidJavaObject>("setAction", iClass.GetStatic<string>("ACTION_SEND"));
                if (string.IsNullOrEmpty(title) == false)
                    iObject.Call<AndroidJavaObject>("putExtra", iClass.GetStatic<string>("EXTRA_TITLE"), title);
                iObject.Call<AndroidJavaObject>("putExtra", iClass.GetStatic<string>("EXTRA_TEXT"), message);
                iObject.Call<AndroidJavaObject>("setType", "text/plain");
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", iObject);
            }
            catch { }
        }
#endif
        #endregion

        //////////////////////////////////////////////////////
        /// DEFAULT FUNCTIONS
        //////////////////////////////////////////////////////
        #region Default functions in console
        [Console("Save", "Log")]
        public static void SaveLog()
        {
            string str = string.Empty;
            foreach (var item in textList)
                str += item.VisualText + "\n";
            str = str.Replace("\n", "\r\n");

#if UNITY_EDITOR
            var filename = System.IO.Path.GetFullPath("Assets/../" + "log" + System.DateTime.Now.Ticks + ".txt");
#else
            var filename = Application.temporaryCachePath + "/log" + System.DateTime.Now.Ticks + ".txt";
#endif
            System.IO.File.WriteAllText(filename, str);
            Debug.Log("Saved to " + filename);

#if UNITY_ANDROID && !UNITY_EDITOR
            ShareText(Application.productName + " " + Application.version, str);
#else
            Application.OpenURL(filename);
#endif
        }

        [Console("Clear", "Cache")]
        public static void ClearCache()
        {
            PlayerPrefs.DeleteAll();
            Caching.ClearCache();
            Debug.Log("Cache Cleared");
        }

        [Console("Clear", "Data")]
        public static void ClearData()
        {
            ClearCache();
            PlayerPrefsEx.ClearData();
            Debug.Log("Data Cleared");
        }


        [Console("Path", "Data")]
        public static void PathData()
        {
            Debug.Log(Application.persistentDataPath);
        }

        [Console("Path", "Cache")]
        public static void PathCache()
        {
            Debug.Log(Application.temporaryCachePath);
        }
        #endregion
    }

}