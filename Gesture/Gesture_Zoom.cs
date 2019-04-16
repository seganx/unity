using UnityEngine;
using System.Collections;

namespace SeganX
{
    public class Gesture_Zoom : Gesture_Base
    {
        //  gestures will be sorted by priorities
        public override GestureManager.Type Type
        {
            get { return GestureManager.Type.Zoom; }
        }


    }
}