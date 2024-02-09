using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public enum TimeScaleMode
    {
        ScaledTime,
        UnScaledTime
    }

    public abstract class BroadcastableBase<T> : MonoBehaviour where T : BroadcastableBase<T>
    {
        [SerializeField] protected TimeScaleMode timeMode;
        protected float deltaTime;

        public abstract void Play(bool reset = false);
        public abstract void Stop();

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        protected static readonly List<BroadcastableBase<T>> all = new List<BroadcastableBase<T>>();

        public static void PlayAll(bool reset = false)
        {
            foreach (T item in all)
                item.Play(reset);
        }

        public static void StopAll()
        {
            foreach (T item in all)
                item.Stop();
        }

        public static void PlayAllByTag(string tag, bool reset = false)
        {
            foreach (T item in all)
                if (item.CompareTag(tag))
                    item.Play(reset);
        }

        public static void StopAllByTag(string tag)
        {
            foreach (T item in all)
                if (item.CompareTag(tag))
                    item.Stop();
        }
    }
}