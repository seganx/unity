using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SeganX
{
    public class InputScreenSteering : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [Tooltip("Image of the steering itself")]
        public RectTransform steeringTransform;
        public float steeringSpeed = 10;
        public float maxSteeringAngle = 70;

        private Vector2 lastPoint;
        private float currentAngle = 0;
        private float steeringAngle = 0;


        void Update()
        {
            steeringAngle = Mathf.MoveTowards(steeringAngle, currentAngle, Time.deltaTime * steeringSpeed);

            var rot = steeringTransform.localEulerAngles;
            rot.z = steeringAngle;
            steeringTransform.localEulerAngles = rot;
        }

        void LateUpdate()
        {
            InputManager.Steering.OnLateUpdate();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.AsRectTransform(), eventData.position, eventData.pressEventCamera, out lastPoint);

            InputManager.Steering.OnPointerDown();
            InputManager.Steering.relativeAngle = 0;
            InputManager.Steering.totalAngle = 0;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            currentAngle = 0;

            InputManager.Steering.OnPointerUp();
            InputManager.Steering.relativeAngle = 0;
            InputManager.Steering.totalAngle = 0;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            Vector2 localHandPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.AsRectTransform(), eventData.position, eventData.pressEventCamera, out localHandPoint);

            Debug.Log(localHandPoint);

            var relativeAngle = Vector3.SignedAngle(lastPoint.normalized, localHandPoint.normalized, Vector3.back);
            currentAngle = Mathf.Clamp(currentAngle - relativeAngle, -maxSteeringAngle, maxSteeringAngle);
            lastPoint = localHandPoint;

            InputManager.Steering.relativeAngle = relativeAngle;
            InputManager.Steering.totalAngle += relativeAngle;
        }
    }
}

