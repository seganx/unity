using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SeganX
{
    public static class Console
    {
        private const float ScreenBaseSize = 650;
        private const float FloatButtonSize = 0.03f;
        private const string CommandFieldName = "ConsoleCommandField";

        private class Mono : BaseRaycaster
        {
            private const float HeaderHeight = 20f;
            private const float FooterHeight = 30f;

            private RaycastResult raycastResult;

            public override Camera eventCamera => Camera.main;
            public override int sortOrderPriority => int.MaxValue;
            public override int renderOrderPriority => int.MaxValue;

            private void OnApplicationQuit() => Save();

            private void OnApplicationPause(bool pauseStatus) => Save();

            protected override void Start()
            {
                base.Start();
                raycastResult = new RaycastResult { gameObject = gameObject, module = this, distance = 0, worldPosition = Vector3.zero, };
            }

            private void LateUpdate()
            {
                lock (logEntries)
                {
                    while (logEntries.Count > 0)
                    {
                        var entry = logEntries[0];
                        HandleLog(entry.condition, entry.trace, entry.type);
                        logEntries.RemoveAt(0);
                    }
                }
                FpsCounter.frames++;
                FpsCounter.time += Time.unscaledDeltaTime;
                if (FpsCounter.time < 1) return;
                FpsCounter.fps = FpsCounter.frames / FpsCounter.time;
                FpsCounter.time = 0;
                FpsCounter.frames = 0;
            }

            private void OnGUI()
            {
                if (settings.disabled) return;

                EnsureStyles();
                var uiScale = Mathf.Lerp(Screen.width / ScreenBaseSize, Screen.height / ScreenBaseSize, 0.5f);

                if (!visible)
                {
                    HandleFloatingButtonInput();
                    Styles.floatingButton.fontSize = Mathf.RoundToInt(14 * uiScale);
                    GUI.Button(FloatingButton.rect, "C:", Styles.floatingButton);
                    return;
                }

                var oldColor = GUI.color;
                GUI.color = settings.blockTouch ? settings.backgroundColorBlocker : settings.backgroundColor;
                GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), GUIContent.none, Styles.background);
                GUI.color = oldColor;

                var prevMatrix = GUI.matrix;

                GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(uiScale, uiScale, 1f));

                var areaRect = new Rect(Screen.safeArea.x / uiScale, 0, Screen.safeArea.width / uiScale, Screen.height / uiScale);
                GUILayout.BeginArea(areaRect);
                {
                    DrawHeader(HeaderHeight);
                    DrawBody(areaRect.width, areaRect.height - HeaderHeight - FooterHeight);
                    DrawFooter(FooterHeight);
                }
                GUILayout.EndArea();

                GUI.matrix = prevMatrix;

                HandleKeyboardSubmit();
            }

            public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
            {
                if (!settings.blockTouch) return;
                if (!visible && !FloatingButton.rect.Contains(new Vector2(eventData.position.x, Screen.height - eventData.position.y)))
                    return;
                resultAppendList.Add(raycastResult);
                eventData.pointerClick = gameObject;
                eventData.Use();
            }
        }


        ///////////////////////////////////////////////////////////////////
        //// STATIC MEMBERS
        ///////////////////////////////////////////////////////////////////
        public static event Action<string> OnCommandEntered;

        private static readonly List<LogEntry> logEntries = new(512);
        private static readonly List<Log> logs = new(512);
        private static string commandInput = string.Empty;
        private static Settings settings = new();
        private static bool visible;
        private static string filter;
        private static int infoCount;
        private static int warnCount;
        private static int errorCount;
        private static float scrollPosition;

        public static bool Disabled
        {
            get => settings.disabled;
            set => settings.disabled = value;
        }

#if STAGING_BUILD || UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void RuntimeInitializeOnLoad()
        {
            Application.logMessageReceivedThreaded += (condition, trace, type) =>
            {
                if (settings.disabled) return;

                lock (logEntries)
                {
                    logEntries.Add(new LogEntry() { condition = condition, trace = trace, type = type });
                }
            };

            Load();
            Debug.Log($"<color=green>Started at {DateTime.Now} on device {SystemInfo.deviceUniqueIdentifier}</color>");
            var instance = new GameObject(nameof(Console)).AddComponent<Mono>();
            UnityEngine.Object.DontDestroyOnLoad(instance);
        }
#endif

        private static void ClearAll()
        {
            logs.Clear();
            infoCount = errorCount = warnCount = 0;
        }

        private static void HandleLog(string logString, string stackTrace, LogType type)
        {
            Log entry;
            if (logs.Count > 0)
            {
                var log = logs[^1];
                if (log.type == type && log.message == logString)
                {
                    entry = log;
                    entry.count++;
                }
                else logs.Add(entry = new Log() { type = type, count = 1, message = logString });
            }
            else logs.Add(entry = new Log() { type = type, count = 1, message = logString });

            var now = TimeSpan.FromSeconds(Time.unscaledTime);
            entry.display = entry.count > 1
                ? new GUIContent(type != LogType.Log ? $"\n[{now.Minutes:00}:{now.Seconds:00}] [{type}] [{entry.count}] {logString}\n{stackTrace}" : $"[{now.Minutes:00}:{now.Seconds:00}] [{entry.count}] {logString}")
                : new GUIContent(type != LogType.Log ? $"\n[{now.Minutes:00}:{now.Seconds:00}] [{type}] {logString}\n{stackTrace}" : $"[{now.Minutes:00}:{now.Seconds:00}] {logString}");

            UpdateVisibility(entry);

            IncrementCounters(type);
        }

        private static void IncrementCounters(LogType type)
        {
            switch (type)
            {
                case LogType.Warning: warnCount++; break;
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception: errorCount++; break;
                case LogType.Log:
                default: infoCount++; break;
            }
        }

        private static void EnsureStyles()
        {
            if (Styles.floatingButton != null) return;
            Styles.floatingButton = new GUIStyle(GUI.skin.button);

            Styles.background = new GUIStyle { normal = { background = Texture2D.whiteTexture } };
            Styles.scrollbar = new GUIStyle(GUI.skin.verticalScrollbar) { normal = { background = null } };
            Styles.header = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 12 };
            Styles.log = new GUIStyle(GUI.skin.label) { fontSize = 11, wordWrap = true, richText = true, margin = new RectOffset(), padding = new RectOffset() };
            Styles.logWarning = new GUIStyle(Styles.log) { normal = { textColor = Color.yellow } };
            Styles.logError = new GUIStyle(Styles.log) { normal = { textColor = Color.red } };

            Styles.background.hover = Styles.background.active = Styles.background.normal;
            Styles.scrollbar.hover = Styles.scrollbar.active = Styles.scrollbar.normal;
            Styles.header.hover = Styles.header.active = Styles.header.normal;
            Styles.log.hover = Styles.log.active = Styles.log.normal;
            Styles.logWarning.hover = Styles.logWarning.active = Styles.logWarning.normal;
            Styles.logError.hover = Styles.logError.active = Styles.logError.normal;
        }

        private static void DrawHeader(float height)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(height));
            {
                if (GUILayout.Button("X", GUILayout.Width(height)))
                    visible = false;

                GUILayout.Label($"FPS: {FpsCounter.fps:0.}", Styles.header);

                if (GUILayout.Button("Copy"))
                    GUIUtility.systemCopyBuffer = string.Join("\n", logs.Select(x => x.message));

                GUILayout.Space(8f);

                var value = GUILayout.Toggle(settings.showLogs, $"Logs: {infoCount}");
                if (value != settings.showLogs)
                {
                    settings.showLogs = value;
                    logs.ForEach(UpdateVisibility);
                }

                value = GUILayout.Toggle(settings.showWarnings, $"Warnings: {warnCount}");
                if (value != settings.showWarnings)
                {
                    settings.showWarnings = value;
                    logs.ForEach(UpdateVisibility);
                }

                value = GUILayout.Toggle(settings.showErrors, $"Errors: {errorCount}");
                if (value != settings.showErrors)
                {
                    settings.showErrors = value;
                    logs.ForEach(UpdateVisibility);
                }

                GUILayout.FlexibleSpace();

                settings.blockTouch = GUILayout.Toggle(settings.blockTouch, "Block-touch", GUILayout.Width(100f));
                settings.autoScroll = GUILayout.Toggle(settings.autoScroll, "Auto-scroll", GUILayout.Width(100f));
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawBody(float width, float height)
        {
            const float scrollWidth = 25;
            var viewRect = GUILayoutUtility.GetRect(width, width, height, height);
            var rect = new Rect(2, -scrollPosition, width - scrollWidth - 4, 0);

            // validate logs and compute the content height
            var contentHeight = 0f;
            for (var i = 0; i < logs.Count; i++)
            {
                var log = logs[i];
                if (!log.visible) continue;
                if (Event.current.type == EventType.Repaint && log.style == null)
                {
                    log.style = GetStyleForLogType(log.type);
                    log.height = log.style.CalcHeight(log.display, rect.width);
                }

                contentHeight += log.height;
            }

            if (contentHeight < 1) return;

            // draw items
            if (Event.current.type == EventType.Repaint)
            {
                GUI.BeginClip(viewRect);
                {
                    for (var i = 0; i < logs.Count; i++)
                    {
                        var log = logs[i];
                        if (!log.visible) continue;
                        rect.height = log.height;
                        if (rect.yMax > 0)
                            GUI.Label(rect, log.display, log.style);
                        rect.y += rect.height;
                        if (rect.y > viewRect.yMax) break;
                    }
                }
                GUI.EndClip();
            }

            var scrollRect = new Rect(rect.width + 2, viewRect.y, scrollWidth, viewRect.height);
            var lastWidth = GUI.skin.verticalScrollbarThumb.fixedWidth;
            GUI.skin.verticalScrollbarThumb.fixedWidth = scrollWidth;
            scrollPosition = GUI.VerticalScrollbar(scrollRect, scrollPosition, Mathf.Min(scrollRect.height, contentHeight), 0.0f, contentHeight, Styles.scrollbar);
            GUI.skin.verticalScrollbarThumb.fixedWidth = lastWidth;

            if (Event.current.type == EventType.Repaint && settings.autoScroll)
                scrollPosition = Mathf.Infinity;
        }

        private static void DrawFooter(float height)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(height));
            {
                GUI.SetNextControlName(CommandFieldName);
                commandInput = GUILayout.TextField(commandInput);

                var value = IsCommandFilter(commandInput) ? commandInput[2..] : null;
                if (value != filter)
                {
                    filter = value;
                    logs.ForEach(UpdateVisibility);
                }

                if (GUILayout.Button("Run", GUILayout.Width(70f)))
                    SubmitCommand();
            }
            GUILayout.EndHorizontal();
        }

        private static void HandleKeyboardSubmit()
        {
            var e = Event.current;
            if (!e.isKey) return;
            if (e.type != EventType.KeyUp) return;
            if (e.keyCode != KeyCode.Return && e.keyCode != KeyCode.KeypadEnter) return;
            if (GUI.GetNameOfFocusedControl() != CommandFieldName) return;
            SubmitCommand();
            e.Use();
        }

        private static void UpdateVisibility(Log log)
        {
            if (filter != null && !log.message.Contains(filter))
            {
                log.visible = false;
                return;
            }

            log.visible = log.type switch
            {
                LogType.Warning => settings.showWarnings,
                LogType.Error or LogType.Assert or LogType.Exception => settings.showErrors,
                _ => settings.showLogs
            };
        }

        private static GUIStyle GetStyleForLogType(LogType type)
        {
            return type switch
            {
                LogType.Warning => Styles.logWarning,
                LogType.Error or LogType.Assert or LogType.Exception => Styles.logError,
                _ => Styles.log
            };
        }

        private static void SubmitCommand()
        {
            var cmd = commandInput.Trim();
            if (string.IsNullOrEmpty(cmd)) return;
            if (IsCommandFilter(cmd)) return;
            if (cmd.ToLower() is "clear" or "clr")
            {
                ClearAll();
                return;
            }

            OnCommandEntered?.Invoke(cmd);
        }

        private static bool IsCommandFilter(string cmd)
        {
            if (cmd.Length < 3) return false;
            if (cmd[0] != 'f' && cmd[0] != 'F') return false;
            return cmd[1] == ':';
        }

        private static void HandleFloatingButtonInput()
        {
            FloatingButton.rect.height = FloatingButton.rect.width = Screen.width * FloatButtonSize;
            FloatingButton.rect.x = Mathf.Clamp(FloatingButton.rect.x, Screen.safeArea.x, Screen.safeArea.width - FloatingButton.rect.width);
            FloatingButton.rect.y = Mathf.Clamp(FloatingButton.rect.y, Screen.safeArea.y, Screen.safeArea.height - FloatingButton.rect.height);

            var e = Event.current;
            if (e == null) return;
            if (e.type == EventType.MouseDown && FloatingButton.rect.Contains(e.mousePosition))
            {
                FloatingButton.pointerDown = true;
                FloatingButton.isDragging = false;
                FloatingButton.pointerDownPosition = e.mousePosition;
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && FloatingButton.pointerDown)
            {
                if (!FloatingButton.isDragging)
                {
                    var dist = Vector2.Distance(e.mousePosition, FloatingButton.pointerDownPosition);
                    if (dist > FloatingButton.rect.width)
                        FloatingButton.isDragging = true;
                }

                if (FloatingButton.isDragging)
                {
                    FloatingButton.rect.x = e.mousePosition.x - FloatingButton.rect.width * 0.5f;
                    FloatingButton.rect.y = e.mousePosition.y - FloatingButton.rect.height * 0.5f;
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseUp && FloatingButton.pointerDown)
            {
                if (!FloatingButton.isDragging && FloatingButton.rect.Contains(e.mousePosition))
                    visible = true;
                FloatingButton.pointerDown = false;
                FloatingButton.isDragging = false;
                e.Use();
            }
        }

        private static void Save()
        {
            settings.floatingButtonPosition = FloatingButton.rect.position;
            var json = JsonUtility.ToJson(settings);
            PlayerPrefs.SetString("Console.Settings", json);
            PlayerPrefs.Save();
        }

        private static void Load()
        {
            var json = PlayerPrefs.GetString("Console.Settings", string.Empty);
            if (json == string.Empty) return;
            try
            {
                settings = JsonUtility.FromJson<Settings>(json);
                FloatingButton.rect.position = settings.floatingButtonPosition;
            }
            catch
            {
                // ignored
            }
        }

        ///////////////////////////////////////////////////////////
        //// NESTED MEMBERS
        ///////////////////////////////////////////////////////////
        [Serializable]
        private class Settings
        {
            public bool disabled = true;
            public Vector2 floatingButtonPosition;
            public Color backgroundColorBlocker = new Color(0f, 0f, 0f, 0.8f);
            public Color backgroundColor = new Color(0f, 0f, 0f, 0.5f);
            public bool showLogs = true;
            public bool showWarnings = true;
            public bool showErrors = true;
            public bool blockTouch = true;
            public bool autoScroll = true;
        }

        private struct LogEntry
        {
            public string condition;
            public string trace;
            public LogType type;
        }

        private class Log
        {
            public bool visible;
            public LogType type;
            public int count;
            public string message;
            public GUIContent display;
            public GUIStyle style;
            public float height;
        }

        private static class Styles
        {
            public static GUIStyle floatingButton;
            public static GUIStyle background;
            public static GUIStyle scrollbar;
            public static GUIStyle header;
            public static GUIStyle log;
            public static GUIStyle logWarning;
            public static GUIStyle logError;
        }

        private static class FpsCounter
        {
            public static float fps = 60;
            public static float time;
            public static int frames;
        }

        private static class FloatingButton
        {
            public static Rect rect = new(Screen.width * 0.3f, Screen.safeArea.y, Screen.width * FloatButtonSize, Screen.width * FloatButtonSize);
            public static bool isDragging;
            public static bool pointerDown;
            public static Vector2 pointerDownPosition;
        }
    }
}