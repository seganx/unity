using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class UiShowHideConfig : StaticConfig<UiShowHideConfig>
    {
        [System.Serializable]
        public class State
        {
            public string name = string.Empty;
            public float delay = 0;
            public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
            public Vector3 direction = Vector3.zero;
            public Vector3 scale = Vector3.one;
            public float alpha = 0;
            public int id = 0;
        }


#if UNITY_EDITOR
        public bool activated = true;
#endif
        public List<State> configs = new List<State>();


        private void OnValidate()
        {
            int maxId = 0;
            foreach (var item in configs)
            {
                if (item.id > maxId)
                    maxId = item.id;
            }

            foreach (var item in configs)
            {
                if (item.id < 1)
                    item.id = ++maxId;
            }
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
#if UNITY_EDITOR
        public static bool IsActive => Instance.activated;
#endif

        public static State GetStateById(int id, State defaultState = null)
        {
            var res = id > 0 ? Instance.configs.Find(x => x.id == id) : defaultState;
            return res ?? defaultState;
        }
    }
}
