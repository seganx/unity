using UnityEngine;

namespace SeganX
{
    public class BrushPrefab : MonoBehaviour
    {
#if UNITY_EDITOR
        public enum EditMode
        {
            Select,
            Create
        }

        public enum Options : int
        {
            RandomRotationX = 0x01,
            RandomRotationY = 0x02,
            RandomRotationZ = 0x04
        }

        public static GameObject lastCreated = null;

        public GameObject prefab = null;

        [EnumToggle]
        public EditMode editMode;

        [EnumFlag]
        public Options options;
#endif
    }
}