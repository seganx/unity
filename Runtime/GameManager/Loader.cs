using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SeganX
{
    [DefaultExecutionOrder(1000)]
    public static class Loader
    {
        public static event System.Action OnLoadCompleted = null;
        private static readonly List<OrderedAction> actions = new List<OrderedAction>();

        // Add action to preload state
        public static void AddAction(int order, System.Func<Task> action)
        {
            actions.Add(new OrderedAction() { order = order, action = action });
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async void OnRuntimeInitialize()
        {
            actions.Sort((x, y) => x.order - y.order);
            foreach (var item in actions)
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false) return;
#endif
                try
                {
                    await item.action?.Invoke();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            actions.Clear();

            if (OnLoadCompleted == null) return;
            var deligates = OnLoadCompleted.GetInvocationList();
            foreach (var item in deligates)
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false) return;
#endif
                try
                {
                    item.Method.Invoke(item.Target, null);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        //////////////////////////////////////////////////////
        /// HELPER MEMBERS
        //////////////////////////////////////////////////////
        private class OrderedAction
        {
            public int order = 0;
            public System.Func<Task> action = null;
        }
    }
}