using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Widgets
{
    public class TransformerAgent : TransformerBase
    {
        public void Play(string targetState)
        {
            var target = TransformerState.FindByName(targetState);
            if (target == null) return;

            switch (target.SimulationSpace)
            {
                case Space.World: StartTransform(target.transform.position, target.transform.rotation, target.transform.GetWorldScale(), Space.World, target.EasingCurve); break;
                case Space.Self: StartTransform(target.transform.localPosition, target.transform.localRotation, target.transform.localScale, Space.Self, target.EasingCurve); break;
            }
        }
    }
}