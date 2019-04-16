using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SeganX
{
    public class InputScreenButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public enum Type
        {
            Jump = 0,
            Fire = 1,
            Accelerate = 2,
            Boost = 3,
            Break = 4,
        }

        [Tooltip("Select which component of the input will be changed")]
        public Type type = Type.Jump;

        [Tooltip("Image of the button when it is normal")]
        public Image normalImage = null;

        [Tooltip("Image of the button when it pressed")]
        public Image pressImage = null;

        private InputManager.Button button = null;

        void Awake()
        {
            switch (type)
            {
                case Type.Jump: button = InputManager.Jump; break;
                case Type.Fire: button = InputManager.Fire; break;
                case Type.Accelerate: button = InputManager.Accelerate; break;
                case Type.Boost: button = InputManager.Boost; break;
                case Type.Break: button = InputManager.Break; break;
            }

            pressImage.gameObject.SetActive(false);
        }

        void LateUpdate()
        {
            button.OnLateUpdate();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pressImage.gameObject.SetActive(true);
            button.OnPointerDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pressImage.gameObject.SetActive(false);
            button.OnPointerUp();
        }
    }
}
