using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class InputManager : MonoBehaviour
    {
        public class Button
        {
            public float pressTime = 0;
            public bool isPointerDown = false;
            public bool isPointerHold = false;
            public bool isPointerUp = false;
            public bool isPointerClick { get { return isPointerDown && !isPointerHold; } }

            public void OnPointerDown()
            {
                isPointerUp = false;
                isPointerDown = true;
            }

            public void OnPointerUp()
            {
                pressTime = 0;
                isPointerUp = true;
                isPointerDown = isPointerHold = false;
            }

            public void OnLateUpdate()
            {
                if (isPointerDown)
                {
                    isPointerHold = true;
                    pressTime += Time.deltaTime;
                }
                isPointerUp = false;
            }
        }

        public class Joystick : Button
        {
            public float verticalValue = 0;
            public float horizontalValue = 0;

            public Vector3 Direction3D
            {
                get
                {
                    tmp3D.Set(horizontalValue, 0, verticalValue);
                    return tmp3D.normalized;
                }
            }

            public Vector3 Direction3DRoundedClamped
            {
                get
                {
                    tmp3D.Set(Mathf.RoundToInt(Mathf.Clamp(horizontalValue, -1.4f, 1.4f)), 0, Mathf.RoundToInt(Mathf.Clamp(verticalValue, -1.4f, 1.4f)));
                    return tmp3D;
                }
            }
        }

        public class SteeringWheel : Button
        {
            public float relativeAngle = 0;
            public float totalAngle = 0;
        }

        public static SteeringWheel Steering = new SteeringWheel();
        public static Joystick JoystickLeft = new Joystick();
        public static Joystick JoystickRight = new Joystick();
        public static Button Jump = new Button();
        public static Button Fire = new Button();
        public static Button Accelerate = new Button();
        public static Button Boost = new Button();
        public static Button Break = new Button();
        public static Button Left = new Button();
        public static Button Right = new Button();
        public static Button Horn = new Button();

        private static Vector3 tmp3D = Vector3.zero;

    }
}