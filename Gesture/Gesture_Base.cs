/********************************************************************
	filename: 	Gesture_Base.cs
	Author:		Sajad Beigjani
	eMail:		seganx@gmail.com
	Site:		www.seganx.com
	Desc:		This file contain a base gesture recognizer for Unity Engine
    
    Concept:    Any gesture recognizer should be extended from this class. 
                The base class will attaches itself to the Gesture Manager 
                on Awake function. 
                So the gesture will be available on manager and a reference 
                to the manager would be exist in gesture automatically.

                The Type parameter will be used as priority. So each gestures
                with higher priority has more chance to recognize the gesture
                first and became as selected one.
                
                There is two important functions which must be overridden. one 
                for process and recognize the gesture and the other function 
                for update the data on the manager.

                for more information just scroll your eyes down.
*********************************************************************/
using UnityEngine;
using System;
using System.Collections;

namespace SeganX
{
    public abstract class Gesture_Base : MonoBehaviour, IComparable<Gesture_Base>
    {
        //  state will show the current state of the touch
        public GestureManager.State state = GestureManager.State.Sleep;

        //  the owner of this gesture
        protected GestureManager manager = null;

        //  Use this before initialization
        void Awake()
        {
            manager = GestureManager.Instance;
            manager.gestureList.Add(this);
        }

        //  will be true if the state is ACTIVATED
        public bool IsActivated
        {
            get { return state == GestureManager.State.Activated; }
        }

        #region virtual functions
        //  gestures will be sorted by priorities
        public virtual GestureManager.Type Type
        {
            get { throw new Exception("Please override Type property for class " + this.GetType() + "."); }
        }

        //  this is called per frame to let the recognizer understand what's happening and return ture on success
        public virtual bool Recognize()
        {
            //  just place the codes to understand if the gesture is recognized or not
            //  if this one recognize the gesture it should change the status to ACTIVATED
            throw new Exception("Function Recognize() in " + this.GetType() + " is not exist! Please override this function.");
        }

        //  will be called if this gesture recognized the touch
        public virtual void DoUpdate(Gesture_Base hooker)
        {
            //  this will be called when the stats is ACTIVATED even if the other
            //  recognizer with higher priorities has been activated. The hooker is
            //  the first recognizer with higher priority which is activated. So this
            //  one is responsible to stop updating or update data in manager simultaneously.
            //  Note that the hooker could be this one ;)
        }
        #endregion

        #region helper functions
        //  return number of touches on the screen
        protected int touchCount
        {
            get { return manager.touchCount; }
        }
        //  return true if the specified touch has been down
        protected bool TouchDown(int index = 0)
        {
            return manager.TouchDown(index);
        }

        //  return true if the specified touch is being hold
        protected bool TouchHold(int index = 0)
        {
            return manager.TouchHold(index);
        }

        //  return true if the specified touch has been released
        protected bool TouchReleased(int index = 0)
        {
            return manager.TouchReleased(index);
        }

        //  return the position of a touch specified by index
        protected Vector3 TouchPosition(int index = 0)
        {
            return manager.TouchPosition(index);
        }

        // dpi factor can be multiplied to some values which is related to the scale of the screen to correct errors on varies screen sizes
        protected float dpiFactor
        {
            get { return GestureManager.DpiFactor; }
        }
        #endregion

        #region interface functions
        public int CompareTo(Gesture_Base other)
        {
            int i = (int)this.Type;
            int o = (int)other.Type;
            return i.CompareTo(o);
        }

        public override string ToString()
        {
            return string.Format("[{0}] state: {1}", this.GetType(), state);
        }
        #endregion

    }
}