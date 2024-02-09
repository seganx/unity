using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Widgets
{
    public class TransformerState : MonoBehaviour
    {
        [SerializeField] private Space space = Space.World;
        [SerializeField] private AnimationCurve easingCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });

        public Space SimulationSpace => space;
        public AnimationCurve EasingCurve => easingCurve;

        private void OnEnable()
        {
            all.Add(this);
        }

        private void OnDisable()
        {
            all.Remove(this);
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static readonly List<TransformerState> all = new List<TransformerState>();

        public static TransformerState FindByName(string name)
        {
            return all.Find(x => x.name == name);
        }
    }
}
