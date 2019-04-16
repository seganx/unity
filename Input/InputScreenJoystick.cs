using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SeganX
{
    public class InputScreenJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        public enum JoystickHand
        {
            Left = 0,
            Right = 1
        }

        [Flags]
        public enum ControlMovementDirection
        {
            Horizontal = 0x1,
            Vertical = 0x2,
            Both = Horizontal | Vertical
        }

        public enum Shape
        {
            Circle = 0,
            Square = 1
        }

        public Camera CurrentEventCamera { get; set; }

        // ------- Inspector visible variables ---------------------------------------
        [Tooltip("This is a left joystick or right one?")]
        public JoystickHand hand = JoystickHand.Left;

        [Tooltip("The range in non-scaled pixels for which we can drag the joystick around.")]
        public float MovementRange = 50f;

        [Tooltip("The shape of the joystick")]
        public Shape shape = Shape.Circle;

        [Space(15f)]
        [Tooltip("Should the joystick be hidden on release?")]
        public bool HideOnRelease;

        [Tooltip("Should the Base image move along with the finger without any constraints?")]
        public bool MoveBase = true;

        [Tooltip("Should the joystick snap to finger? If it's FALSE, the MoveBase checkbox logic will be ommited")]
        public bool SnapsToFinger = true;

        [Tooltip("Should the joystick hold last data or just reset data on release? If it's TRUE, the values will not set to zero on release")]
        public bool persistent = false;

        [Tooltip("Constraints on the joystick movement axis")]
        public ControlMovementDirection JoystickMoveAxis = ControlMovementDirection.Both;

        [Tooltip("Image of the joystick base")]
        public Image JoystickBase;

        [Tooltip("Image of the stick itself")]
        public Image Stick;
        // ---------------------------------------------------------------------------

        private InputManager.Joystick joystick = null;
        private Vector2 initialStickPosition;
        private Vector2 intermediateStickPosition;
        private Vector2 initialBasePosition;
        private RectTransform baseTransform;
        private RectTransform stickTransform;
        private float oneOverMovementRange;

        private void Awake()
        {
            switch (hand)
            {
                case JoystickHand.Left: joystick = InputManager.JoystickLeft; break;
                case JoystickHand.Right: joystick = InputManager.JoystickRight; break;
            }

            stickTransform = Stick.GetComponent<RectTransform>();
            baseTransform = JoystickBase.GetComponent<RectTransform>();

            initialStickPosition = stickTransform.anchoredPosition;
            intermediateStickPosition = initialStickPosition;
            initialBasePosition = baseTransform.anchoredPosition;
            oneOverMovementRange = 1f / MovementRange;

            if (HideOnRelease)
            {
                Hide(true);
            }
        }

        void LateUpdate()
        {
            joystick.OnLateUpdate();
            if (!persistent && !joystick.isPointerDown && !joystick.isPointerHold && !joystick.isPointerUp)
                joystick.verticalValue = joystick.horizontalValue = 0;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            // Unity remote multitouch related thing
            // When we feed fake PointerEventData we can't really provide a camera, 
            // it has a lot of private setters via not created objects, so even the Reflection magic won't help a lot here
            // Instead, we just provide an actual event camera as a public property so we can easily set it in the Input Helper class
            CurrentEventCamera = eventData.pressEventCamera ?? CurrentEventCamera;

            // We get the local position of the joystick
            Vector3 worldJoystickPosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(stickTransform, eventData.position, CurrentEventCamera, out worldJoystickPosition);

            // Then we change it's actual position so it snaps to the user's finger
            stickTransform.position = worldJoystickPosition;

            // We then query it's anchored position. It's calculated internally and quite tricky to do from scratch here in C#
            var stickAnchoredPosition = stickTransform.anchoredPosition;

            // Some bitwise logic for constraining the joystick along one of the axis
            // If the "Both" option was selected, non of these two checks will yield "true"
            if ((JoystickMoveAxis & ControlMovementDirection.Horizontal) == 0)
                stickAnchoredPosition.x = intermediateStickPosition.x;
            if ((JoystickMoveAxis & ControlMovementDirection.Vertical) == 0)
                stickAnchoredPosition.y = intermediateStickPosition.y;

            // Find current difference between the previous central point of the joystick and it's current position
            Vector2 difference = stickAnchoredPosition - intermediateStickPosition;

            // Normalization stuff
            var diffMagnitude = difference.magnitude;
            var normalizedDifference = difference / diffMagnitude;

            var isMaxRange = shape == Shape.Circle ? diffMagnitude > MovementRange : (Math.Abs(difference.x) > MovementRange || Math.Abs(difference.y) > MovementRange);

            // If the joystick is being dragged outside of it's range
            if (isMaxRange)
            {
                if (MoveBase && SnapsToFinger)
                {
                    // We move the base so it maps the new joystick center position
                    var baseMovementDifference = difference.magnitude - MovementRange;
                    var addition = normalizedDifference * baseMovementDifference;
                    baseTransform.anchoredPosition += addition;
                    intermediateStickPosition += addition;
                    stickTransform.anchoredPosition = stickAnchoredPosition;
                }
                else
                {
                    if (shape == Shape.Circle)
                    {
                        stickTransform.anchoredPosition = intermediateStickPosition + normalizedDifference * MovementRange;
                    }
                    else
                    {
                        if (Math.Abs(difference.x) > MovementRange) stickAnchoredPosition.x = intermediateStickPosition.x + Math.Sign(difference.x) * MovementRange;
                        if (Math.Abs(difference.y) > MovementRange) stickAnchoredPosition.y = intermediateStickPosition.y + Math.Sign(difference.y) * MovementRange;
                        stickTransform.anchoredPosition = stickAnchoredPosition;
                    }
                }
            }
            else stickTransform.anchoredPosition = stickAnchoredPosition;            

            // We should now calculate axis values based on final position and not on "virtual" one
            Vector2 finalDifference = stickTransform.anchoredPosition - intermediateStickPosition;

            // We don't need any values that are greater than 1 or less than -1
            joystick.horizontalValue = Mathf.Clamp(finalDifference.x * oneOverMovementRange, -1f, 1f);
            joystick.verticalValue = Mathf.Clamp(finalDifference.y * oneOverMovementRange, -1f, 1f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // When we lift our finger, we reset everything to the initial state
            baseTransform.anchoredPosition = initialBasePosition;
            stickTransform.anchoredPosition = initialStickPosition;
            intermediateStickPosition = initialStickPosition;

            joystick.OnPointerUp();

            // We also hide it if we specified that behavior
            if (HideOnRelease)
                Hide(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // When we press, we first want to snap the joystick to the user's finger
            if (SnapsToFinger)
            {
                CurrentEventCamera = eventData.pressEventCamera ?? CurrentEventCamera;

                Vector3 localStickPosition;
                Vector3 localBasePosition;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(stickTransform, eventData.position, CurrentEventCamera, out localStickPosition);
                RectTransformUtility.ScreenPointToWorldPointInRectangle(baseTransform, eventData.position, CurrentEventCamera, out localBasePosition);

                baseTransform.position = localBasePosition;
                stickTransform.position = localStickPosition;
                intermediateStickPosition = stickTransform.anchoredPosition;
            }
            else OnDrag(eventData);

            joystick.OnPointerDown();

            // We also want to show it if we specified that behavior
            if (HideOnRelease)
                Hide(false);
        }

        private void Hide(bool isHidden)
        {
            JoystickBase.gameObject.SetActive(!isHidden);
            Stick.gameObject.SetActive(!isHidden);
        }
    }
}
