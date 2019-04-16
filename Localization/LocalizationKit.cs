using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    [CreateAssetMenu(menuName = "Game/LocalizationKit")]
    public class LocalizationKit : ScriptableObject
    {
        [System.Serializable]
        public class LocalKitData
        {
            [System.Serializable]
            public class LocalStrings
            {
                public int i = 0;
                public string s = string.Empty;
                public override string ToString() { return i + ":" + s; }
            }

            public int baseId = 111000;
            public string language = "fa persian farsi";
            public List<LocalStrings> strings = new List<LocalStrings>();
        }

        public LocalKitData kit = new LocalKitData();

        public string Get(int id)
        {
            var obj = kit.strings.Find(x => x.i == id);
            return (obj == null) ? id.ToString() : obj.s;
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            LocalizationService.CheckService();
        }

        public int AddString(string text)
        {
            var res = new LocalKitData.LocalStrings() { i = kit.baseId++, s = text };
            if (text == null) res.s = res.i.ToString();
            kit.strings.Add(res);
            UnityEditor.EditorUtility.SetDirty(this);
            return res.i;
        }

        public int UpdateString(int index, string text)
        {
            var res = kit.strings.Find(x => x.i == index);
            if (res == null)
            {
                res = new LocalKitData.LocalStrings() { i = kit.baseId++, s = text };
                kit.strings.Add(res);
            }
            else res.s = text;
            UnityEditor.EditorUtility.SetDirty(this);
            return res.i;
        }
#endif
    }
}
