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
            Left = 5,
            Right = 6,
            Horn = 7
        }

        public bool intractable = true;

        [Tooltip("Select which component of the input will be changed")]
        public Type type = Type.Jump;

        [Tooltip("Image of the button when it is disabled")]
        public Image disabledImage = null;

        [Tooltip("Image of the button when it is normal")]
        public Image normalImage = null;

        [Tooltip("Image of the button when it pressed")]
        public Image pressImage = null;

        private InputManager.Button button = null;

        private void Awake()
        {
            switch (type)
            {
                case Type.Jump: button = InputManager.Jump; break;
                case Type.Fire: button = InputManager.Fire; break;
                case Type.Accelerate: button = InputManager.Accelerate; break;
                case Type.Boost: button = InputManager.Boost; break;
                case Type.Break: button = InputManager.Break; break;
                case Type.Left: button = InputManager.Left; break;
                case Type.Right: button = InputManager.Right; break;
                case Type.Horn: button = InputManager.Horn; break;
            }

            if (disabledImage) disabledImage.gameObject.SetActive(!intractable);
            normalImage.gameObject.SetActive(intractable);
            pressImage.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (disabledImage) disabledImage.gameObject.SetActive(!intractable);
            normalImage.gameObject.SetActive(intractable);
            button.OnLateUpdate();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (intractable)
            {
                pressImage.gameObject.SetActive(true);
                button.OnPointerDown();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pressImage.gameObject.SetActive(false);
            button.OnPointerUp();
        }
    }
}
