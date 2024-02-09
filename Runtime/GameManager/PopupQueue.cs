using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static class PopupQueue
    {
        private struct Command
        {
            public float delay;
            public System.Action func;
        }

        private class MonoPopupQueue : MonoBehaviour
        {
            private float delayCounter = 0;

            // Update is called once per frame
            void Update()
            {
                if (GameManager.CurrentPopup != null || commands.Count < 1) return;
                var curr = commands[0];
                if (curr.delay > 0.001f)
                {
                    delayCounter += Time.deltaTime;
                    if (delayCounter < curr.delay)
                        return;
                    else
                        delayCounter = 0;
                }
                commands.RemoveAt(0);
                curr.func();
            }
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static readonly List<Command> commands = new List<Command>(20);

        public static void Add(float delay, System.Action callback)
        {
            var cmd = default(Command);
            cmd.delay = delay;
            cmd.func = callback;
            commands.Add(cmd);
        }

        static PopupQueue()
        {
            var go = new GameObject("PopupQueue").AddComponent<MonoPopupQueue>();
            //go.gameObject.hideFlags = HideFlags.HideInHierarchy;
            Object.DontDestroyOnLoad(go);
        }
    }
}
