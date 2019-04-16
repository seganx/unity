using UnityEngine;
using System.Collections;

namespace SeganX
{
    public class Gesture_Hold : Gesture_Base
    {
        [Tooltip("Hold will be recognized if user hold his finger on a point over this time threshold.")]
        public float startThreshold = 2;

        [Tooltip("Hold will be ignored if user moves his finger over this threshold.")]
        public float cancelThreshold = 20;

        public bool continuesHold = true;

        private float holdTime = 0;
        private Vector3 firstPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, 0);

        //  gestures will be sorted by priorities
        public override GestureManager.Type Type
        {
            get { return GestureManager.Type.Hold; }
        }

        //  this is called per frame to let the recognizer understand what's happening and return true on success
        public override bool Recognize()
        {
            if (touchCount != 1) return CancelHold();

            if (TouchDown())
            {
                holdTime = 0;
                firstPoint = TouchPosition();
                return false;
            }
            else if (TouchHold())
            {
                if (Vector3.Distance(firstPoint, TouchPosition()) < cancelThreshold * dpiFactor)
                {
                    holdTime += Time.deltaTime;
                    if (holdTime >= startThreshold)
                    {
                        state = GestureManager.State.Activated;
                        return true;
                    }
                    else return false;
                }
                else
                {
                    if (continuesHold)
                    {
                        holdTime = 0;
                        firstPoint = TouchPosition();
                        return true;
                    }
                    else return CancelHold();
                }
            }
            else return CancelHold();
        }

        public override void DoUpdate(Gesture_Base hooker)
        {
            if (hooker == this)
            {
            }
            else CancelHold();
        }

        bool CancelHold()
        {
            holdTime = 0;
            firstPoint.Set(Mathf.Infinity, Mathf.Infinity, 0);
            return false;
        }
    }
}