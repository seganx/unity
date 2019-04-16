using UnityEngine;
using System;

namespace SeganX
{
    [Serializable]
    public class ValueInt
    {
        public delegate void ChangedCallback(int lastValue, int currentValue);
        public event ChangedCallback OnValueChanged;

        [SerializeField]
        protected int current = int.MinValue;

        public bool IsZero { get { return Mathf.Approximately(current, 0); } }

        public virtual int Current
        {
            set { SetCurrent(value); }
            get { return current; }
        }

        public void SetOnValueChanged(ChangedCallback callback)
        {
            OnValueChanged += callback;
        }

        protected void SetCurrent(int value)
        {
            var lastValue = current;
            current = value;
            if (OnValueChanged != null && lastValue != current)
                OnValueChanged(lastValue, current);
        }

        public virtual void CopyFrom(ValueInt src)
        {
            current = src.current;
        }

        public override string ToString()
        {
            return base.ToString() + " Current[" + Current + "]";
        }

        public static implicit operator float(ValueInt v)
        {
            return v.Current;
        }

        public static float operator +(ValueInt v, int f)
        {
            return v.Current + f;
        }

        public static ValueInt operator ++(ValueInt v)
        {
            v.Current++;
            return v;
        }
    }
}
