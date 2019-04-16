using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

namespace SeganX.Console
{
    public class Info : MonoBehaviour
    {
        public delegate string OnDisplayInfoEvent(string info);
        public Text label = null;
        public Text systemInfo = null;

        void OnEnable()
        {
            if (gameObject.activeInHierarchy == false) return;
            Invoke("OnEnable", 0.5f);

            string str = "Ver: " + Application.version + "\nId: " + DisplayDeviceID;

            if (OnDisplayInfo != null)
                str = OnDisplayInfo(str);

            label.text = str;
            systemInfo.text = GetSystemInfo();
        }


        //////////////////////////////////////////////////////////////
        //  STATIC MEMBERS
        //////////////////////////////////////////////////////////////
        public static event OnDisplayInfoEvent OnDisplayInfo = null;

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

        public static void SetOnDisplayInfo(OnDisplayInfoEvent onDisplayInfo)
        {
            OnDisplayInfo = onDisplayInfo;
        }
    }
}