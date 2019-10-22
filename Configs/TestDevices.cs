using UnityEngine;

namespace SeganX
{
#if UNITY_EDITOR
    public class TestDevices : StaticConfig<TestDevices>
    {
        [SerializeField] private bool active = false;
        [Space(50)]
        [Header("Selected device index")]
        [SerializeField] private int selectedIndex = 0;
        [Space(50)]
        [Header("List of devices for test")]
        [SerializeField] private string[] deviceList = new string[0];

        public static string DeviceId
        {
            get
            {
                return Instance.active && Instance.deviceList.Length > 0 ? Instance.deviceList[Instance.selectedIndex % Instance.deviceList.Length] : SystemInfo.deviceUniqueIdentifier;
            }
        }

        protected override void OnInitialize() { }
    }
#endif
}
