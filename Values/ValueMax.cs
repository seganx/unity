using UnityEngine;
using System;

namespace SeganX
{
    [Serializable]
    public class ValueMax : Value
    {
        [SerializeField]
        protected float max = 100;

        public bool IsMax { get { return Mathf.Approximately(current, max); } }

        public override float Current
        {
            set { SetCurrent(Mathf.Clamp(value, 0, max)); }
        }

        public virtual float Percent
        {
            get
            {
                float d = max;
                return (d > Mathf.Epsilon) ? current * 100 / d : 0;
            }
            set { current = value * 0.01f * max; }
        }

        public float Max
        {
            set
            {
                max = value;
                if (max < 0) max = 0;
                if (max < current) Current = max;
            }
            get { return max; }
        }

        public override void CopyFrom(Value src)
        {
            base.CopyFrom(src);
            if (src is ValueMax)
                max = src.As<ValueMax>().max;
        }

        public override string ToString()
        {
            return base.ToString() + " Percent[" + Percent + "] Max[" + Max + "]";
        }

        public static ValueMax operator ++(ValueMax v)
        {
            v.Current++;
            return v;
        }

        public static ValueMax operator --(ValueMax v)
        {
            v.Current--;
            return v;
        }
    }
}
