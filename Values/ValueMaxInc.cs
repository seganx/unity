using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SeganX
{
    [Serializable]
    public class ValueMaxInc : ValueMax
    {
        [SerializeField]
        protected float perSecond = 0;

        public bool IsIncremental { get { return Mathf.Approximately(perSecond, 0); } }

        public float PerSecond { get { return perSecond; } set { perSecond = value; } }

        public void Update()
        {
            Current += perSecond * Time.deltaTime;
        }

        public override void CopyFrom(Value src)
        {
            base.CopyFrom(src);
            if (src is ValueMaxInc)
                perSecond = src.As<ValueMaxInc>().perSecond;
        }

        public override string ToString()
        {
            return base.ToString() + " PerSecond[" + PerSecond + "]";
        }
    }

}
