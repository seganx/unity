using UnityEngine;
using System;

namespace SeganX
{
    [Serializable]
    public class ValueMinMax : ValueMax
    {
        [SerializeField]
        protected float min = 0;

        public bool IsMin { get { return Mathf.Approximately(current, min); } }

        public override float Current
        {
            set { SetCurrent(Mathf.Clamp(value, min, max)); }
        }

        public override float Percent
        {
            get
            {
                float d = (max - min);
                return (d > Mathf.Epsilon) ? current * 100 / d : 0;
            }
            set { current = value * 0.01f * (max - min); }
        }

        public float Min
        {
            set
            {
                min = value;
                if (min > max) max = min;
                if (min > current) Current = min;
            }
            get { return min; }
        }

        public override void CopyFrom(Value src)
        {
            base.CopyFrom(src);
            if (src is ValueMinMax)
                min = src.As<ValueMinMax>().min;
        }

        public override string ToString()
        {
            return base.ToString() + " Min[" + Min + "]";
        }
    }
}
