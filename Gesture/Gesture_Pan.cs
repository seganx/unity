using UnityEngine;
using System.Collections;

namespace SeganX
{
    public class Gesture_Pan : Gesture_Base
    {
#if OFF
    [Tooltip("Pan will be started when fingers move more that start threshold.")]
    public float startThreshold = 50;
#endif

        [Tooltip("Pan will be started if distance between fingers doesn't changed.")]
        public float fingersDistance = 50;

        [Tooltip("Pan vector will has relative values instead of absolute values.")]
        public bool relative = false;

        private Vector3[] firstPos = new Vector3[2];

        //  gestures will be sorted by priorities
        public override GestureManager.Type Type
        {
            get { return GestureManager.Type.Pan; }
        }

        //  this is called per frame to let the recognizer understand what's happening and return ture on success
        public override bool Recognize()
        {
            if (touchCount != 2) return false;
            return CancelPan();
        }

        public override void DoUpdate(Gesture_Base hooker)
        {
            if (hooker == this)
            {
                manager.panVector = GetPanVector();
                if (relative)
                    UpdateFitstPos();
            }
        }

        void UpdateFitstPos()
        {
            firstPos[0] = TouchPosition(0);
            firstPos[1] = TouchPosition(1);
        }

        Vector3 GetPanVector()
        {
            Vector3 d0 = TouchPosition(0) - firstPos[0];
            Vector3 d1 = TouchPosition(1) - firstPos[1];
            return (d0 + d1) * 0.5f;
        }

        bool CancelPan()
        {
            firstPos[0].Set(Mathf.Infinity, Mathf.Infinity, 0);
            firstPos[1].Set(Mathf.Infinity, Mathf.Infinity, 0);
            return false;
        }
    }
}