using UnityEngine;
using System.Collections;

namespace SeganX
{
    public class Gesture_Swipe : Gesture_Base
    {
        [Tooltip("Swipe will be started if user moves his finger over this threshold.")]
        public float startThreshold = 20;

        [Tooltip("Swipe vector will has relative values instead of absolute values.")]
        public bool relative = false;

        private bool started = false;
        private bool firstTouch = false;
        private Vector3 firstPos;

        //  gestures will be sorted by priorities
        public override GestureManager.Type Type
        {
            get { return GestureManager.Type.Swipe; }
        }

        //  this is called per frame to let the recognizer understand what's happening and return true on success
        public override bool Recognize()
        {
            if (touchCount != 1) return CancelSwipe();

            if (TouchDown())
            {
                firstTouch = true;
                firstPos = TouchPosition();
                return false;
            }
            else if (firstTouch)
            {
                if (TouchHold())
                {
                    if (started)
                    {
                        state = GestureManager.State.Activated;
                        return true;
                    }
                    else if (Vector3.Distance(firstPos, TouchPosition()) > startThreshold * dpiFactor)
                    {
                        started = true;
                        return true;
                    }
                    else return false;
                }
                else if (TouchReleased())
                {
                    return CancelSwipe();
                }
                else return false;
            }
            else return CancelSwipe();
        }

        public override void DoUpdate(Gesture_Base hooker)
        {
            if (started)
            {
                if (hooker == this)
                {
                    manager.swipeVector = (TouchPosition() - firstPos) * dpiFactor;

                    if (relative)
                        firstPos = TouchPosition();
                }
                else CancelSwipe();
            }
        }

        bool CancelSwipe()
        {
            started = false;
            firstTouch = false;
            firstPos.Set(Mathf.Infinity, Mathf.Infinity, 0);
            manager.swipeVector = Vector3.zero;
            return false;
        }
    }
}