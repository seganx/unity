using UnityEngine;
using System.Collections;

namespace SeganX
{
    public class Gesture_Tap : Gesture_Base
    {
        [Tooltip("Tap will be ignored if user holds his finger over this time.")]
        public float cancelTime = 20;

        [Tooltip("Tap will be ignored if user moves his finger over this threshold.")]
        public float cancelThreshold = 20;

        private bool firstTouch = false;
        private float touchTime = 3;
        private Vector3 firstPos;

        //  gestures will be sorted by priorities
        public override GestureManager.Type Type
        {
            get { return GestureManager.Type.Tap; }
        }

        //  this is called per frame to let the recognizer understand what's happening and return true on success
        public override bool Recognize()
        {
            if (touchCount != 1) return CancelTap();

            if (TouchDown(0))
            {
                firstTouch = true;
                touchTime = cancelTime;
                firstPos = TouchPosition();
            }
            else if (firstTouch)
            {
                if (TouchHold(0))
                {
                    touchTime -= Time.deltaTime;
                    if (touchTime < 0)
                    {
                        return CancelTap();
                    }
                    else if (Vector3.Distance(firstPos, TouchPosition()) > cancelThreshold * dpiFactor)
                    {
                        return CancelTap();
                    }
                }
                else if (TouchReleased(0))
                {
                    state = GestureManager.State.Activated;
                    return true;
                }
            }

            return false;
        }

        public override void DoUpdate(Gesture_Base hooker)
        {
            if (hooker == this)
            {
                //Debug.Log("Tap on : " + manager.currPosition);
            }
            else CancelTap();
        }

        bool CancelTap()
        {
            firstTouch = false;
            touchTime = 0;
            firstPos.Set(Mathf.Infinity, Mathf.Infinity, 0);
            return false;
        }
    }
}