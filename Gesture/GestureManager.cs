/********************************************************************
	filename: 	GestureManager.cs
	Author:		Sajad Beigjani
	eMail:		seganx@gmail.com
	Site:		www.seganx.com
	Desc:		This file contain a basic gesture manager for Unity Engine
    
    Concept:    The manager class is a signlton one which has the all the 
                usable parametres:

                    time,
                    panning values,
                    zoom factor,
                    etc..

                The other classes in the game simply could read the values and
                act according to them without concern about who updates them.
                for example an spherical camera class would have:

                    mySpericalCamera.radius = GestureManager.instance.zoomFacor * k;


                The manager never updates parameters, so they will be updated
                by the other gesture recognizers who attached to the manager.
                for instance a zoom recognizer could update zoom factor in 
                the manager:

                    manager.zoomFactor += (newValue - lastValue) * zoomSpeed * Time.deltaTime;
                

                So we could append new recognizers to the manager and update
                any data depend on the game genre.
                Each recognizer should be extended from Gesture_Base class 
                and must override two virtual functions at least. So for
                more information about appending new gestures just take a 
                look into Gesture_Base class

*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    public class GestureManager : MonoBehaviour
    {
        //  Here are the gestures type as priorities of the gestures in process
        public enum Type
        {
            Null = 0,

            Pan,        //  two fingers pan
            Zoom,       //  two fingers zoom
            Rotate,     //  two fingers rotate

            Swipe,      //  one finger swipe
            Hold,       //  one finger hold
            Tap         //  one finger tap  
        }

        //  Here are the states of each gesture
        public enum State
        {
            Sleep = 0,  //  the gesture is still in sleep
            Activated   //  the gesture has been recognized
        }

        public static float TouchTime { get { return Instance.touchTime; } }
        public static float ZoomFactor { get { return Instance.zoomFactor; } }
        public static float RotateDegree { get { return Instance.rotateDegree; } }
        public static Vector3 PanVector { get { return Instance.panVector; } }
        public static Vector3 SwipeVector { get { return Instance.swipeVector; } }
        public static Vector3 CurrentPosition { get { return Instance.currPosition; } }
        public static Vector3 LastPosition { get { return Instance.lastPosition; } }
        public static Type ActiveType { get { return Instance.activatedType; } }

        public float touchTime = 0;
        public Vector3 panVector = new Vector3(0, 0, 0);
        public float zoomFactor = 0;
        public float rotateDegree = 0;
        public Vector3 swipeVector = new Vector3(0, 0, 0);
        public Vector3 currPosition = new Vector3(0, 0, 0);
        public Vector3 lastPosition = new Vector3(0, 0, 0);
        public Type activatedType = Type.Null;

        public Text debugText = null;

        //  list of all gestures has been added
        [HideInInspector]
        public List<Gesture_Base> gestureList = new List<Gesture_Base>();

        //  Use this for initialization
        void Start()
        {
            //  sort them all before start
            gestureList.Sort();
        }

        //  Update is called once per frame
        void Update()
        {
            UpdateTouchs();
            Gesture_Base selected = null;

            //  let the all recognizers think about any things
            foreach (var item in gestureList)
            {
                item.state = State.Sleep;
                if (item.Recognize() && selected == null)
                {
                    selected = item;
                }
            }

            //  now update every of them who is activated
            foreach (var item in gestureList)
            {
                if (item.IsActivated)
                {
                    //  let the gesture decide what to do even if selected is itself ;)
                    item.DoUpdate(selected);
                }
            }

            //  if a recognizer has been selected update some data
            if (selected)
            {
                activatedType = selected.Type;
                touchTime += Time.deltaTime;
            }
            else
            {
                activatedType = Type.Null;
                touchTime = 0;
            }

            // if debug text as been assign just show what's happening
            if (debugText)
            {
                debugText.text = "dpi: " + DpiFactor + "\ntype: " + activatedType.ToString() + " ";
                switch (activatedType)
                {
                    case Type.Null:
                        break;
                    case Type.Pan:
                        debugText.text += panVector.ToString();
                        break;
                    case Type.Zoom:
                        debugText.text += zoomFactor.ToString();
                        break;
                    case Type.Rotate:
                        debugText.text += rotateDegree.ToString();
                        break;
                    case Type.Swipe:
                        debugText.text += swipeVector.ToString();
                        break;
                    case Type.Hold:
                        break;
                    case Type.Tap:
                        debugText.text += currPosition.ToString();
                        break;
                    default:
                        break;
                }
            }
        }

        //  reset all values
        void ResetValues()
        {
            touchTime = zoomFactor = rotateDegree = 0;
            panVector = swipeVector = currPosition = lastPosition = Vector3.zero;
        }

        #region helper functions

        private bool releaseAllTouchs = false;
        private bool[] touchDown = { false, false };
        private bool[] touchHold = { false, false };
        private bool[] touchFree = { false, false };

        public void ReleaseAllTouchs()
        {
            releaseAllTouchs = true;
        }

        void UpdateTouchs()
        {
            lastPosition = currPosition;
            currPosition = TouchPosition();

            for (int index = 0; index < touchDown.Length; index++)
            {
#if UNITY_EDITOR
                bool isDown = Input.GetMouseButtonDown(index);
                bool isUp = Input.GetMouseButtonUp(index);
                bool isHold = Input.GetMouseButton(index);
#elif OFF
            bool isDown = (Input.touches.Length > index && Input.touches[index].phase == TouchPhase.Began);
            bool isUp = (Input.touches.Length > index && (Input.touches[index].phase == TouchPhase.Ended || Input.touches[index].phase == TouchPhase.Canceled));
            bool isHold = (Input.touches.Length > index && (Input.touches[index].phase == TouchPhase.Stationary || Input.touches[index].phase == TouchPhase.Moved));
#else
            bool isDown = Input.GetMouseButtonDown(index);
            bool isUp = Input.GetMouseButtonUp(index);
            bool isHold = Input.GetMouseButton(index);
#endif
                if (releaseAllTouchs)
                {
                    releaseAllTouchs = false;
                    touchFree[index] = touchDown[index] || touchHold[index];
                    touchDown[index] = false;
                    touchHold[index] = false;
                }
                else if (touchFree[index])
                {
                    touchDown[index] = false;
                    touchHold[index] = false;
                    touchFree[index] = false;
                }
                else if (touchDown[index])
                {
                    touchDown[index] = false;
                    touchHold[index] = isDown || isHold;
                    touchFree[index] = !touchHold[index];
                }
                else if (isDown)
                {
                    touchDown[index] = true;
                    touchHold[index] = false;
                    touchFree[index] = false;
                }
                else if (isUp)
                {
                    touchDown[index] = false;
                    touchHold[index] = false;
                    touchFree[index] = true;
                }
                else if (isHold)
                {

                }
                else
                {
                    touchDown[index] = false;
                    touchHold[index] = false;
                    touchFree[index] = false;
                }

            }

        }


        //  return number of touches on the screen
        public int touchCount
        {
            get
            {
#if UNITY_EDITOR
                int res = (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) ? 1 : 0;
                if (res > 0)
                    res += (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1) || Input.GetMouseButtonUp(1)) ? 1 : 0;
                return res;
#elif OFF
            return Input.touchCount;
#else
            int res = (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) ? 1 : 0;
            if (res > 0)
                res += (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1) || Input.GetMouseButtonUp(1)) ? 1 : 0;
            return res;
#endif
            }
        }
        //  return true if the specified touch has been down
        public bool TouchDown(int index = 0, int lockNum = 0)
        {
            return touchDown[index];
        }

        //  return true if the specified touch is being hold
        public bool TouchHold(int index = 0, int lockNum = 0)
        {
            return touchHold[index];
        }

        //  return true if the specified touch has been released
        public bool TouchReleased(int index = 0, int lockNum = 0)
        {
            return touchFree[index];
        }

        //  return the position of a touch specified by index
        public Vector3 TouchPosition(int index = 0)
        {
#if UNITY_EDITOR
            return Input.mousePosition;
#elif OFF
        if (Input.touches.Length > index)
            return Input.touches[index].position;
        else
            return Input.mousePosition;
#else 
        return Input.mousePosition;
#endif
        }

        // dpi factor can be multiplied to some values which is related to the scale of the screen to correct errors on varies screen sizes
        public static float DpiFactor
        {
            get
            {
                if (Screen.dpi > 0)
                {
                    return 0.5f + (Screen.dpi / 192.0f);
                }
                else return 1.0f;
            }
        }
        #endregion



        //////////////////////////////////////////////////
        /// make singleton class
        private static GestureManager instance;
        public static GestureManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<GestureManager>();
                return instance;
            }
        }
        ////////////////////////////////////////////////
    }
}