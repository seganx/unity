using UnityEngine;
using UnityEngine.EventSystems;

namespace SeganX
{
    public class UiPlayerControllerBase : MonoBehaviour
    {
        public enum State { Up, Down, Hold }

        public System.Action<State> onStateChanged = null;

        protected RectTransform rectTransform = null;
        protected State state = State.Up;
        protected Vector2 position = Vector2.zero;
        protected Vector2 delta = Vector2.zero;

        private PointerEventData eventData = null;


        protected virtual void Awake()
        {
            rectTransform = transform as RectTransform;
        }

        protected virtual void Update()
        {
            eventData = EventSystem.current.GetEventData(eventData);

            bool accessable = eventData.pointerCurrentRaycast.gameObject == gameObject;
            if (accessable && Input.GetMouseButton(0))
            {
                switch (state)
                {
                    case State.Up:
                        {
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out position);
                            state = State.Down;
                            onStateChanged?.Invoke(state);
                        }
                        break;
                    case State.Down:
                        {
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out position);
                            state = State.Hold;
                            onStateChanged?.Invoke(state);
                        }
                        break;
                    case State.Hold:
                        {
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out Vector2 nextPosition);
                            delta = nextPosition - position;
                            position = nextPosition;
                        }
                        break;
                }
            }
            else if (state != State.Up)
            {
                state = State.Up;
                onStateChanged?.Invoke(state);
            }
        }
    }

}