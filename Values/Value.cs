using UnityEngine;
using System;

namespace SeganX
{
    [Serializable]
    public class Value
    {
        public delegate void ChangedCallback(float lastValue, float currentValue);
        public event ChangedCallback OnValueChanged;

        [SerializeField]
        protected float current = 100;

        public bool IsZero { get { return Mathf.Approximately(current, 0); } }

        public virtual float Current
        {
            set { SetCurrent(value); }
            get { return current; }
        }

        public void SetOnValueChanged(ChangedCallback callback)
        {
            OnValueChanged += callback;
        }

        protected void SetCurrent(float value)
        {
            var lastValue = current;
            current = value;
            if (OnValueChanged != null && lastValue != current)
                OnValueChanged(lastValue, current);
        }

        public virtual void CopyFrom(Value src)
        {
            current = src.current;
        }

        public override string ToString()
        {
            return base.ToString() + " Current[" + Current + "]";
        }

        public static implicit operator float(Value v)
        {
            return v.Current;
        }

        public static implicit operator int(Value v)
        {
            return Mathf.RoundToInt(v.Current);
        }

        public static float operator +(Value v, float f)
        {
            return v.Current + f;
        }

        public static Value operator ++(Value v)
        {
            v.Current++;
            return v;
        }
    }
}
