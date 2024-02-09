using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SeganX
{
    public class UIDragable : Base, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public enum State { StartDrag, Dragging, EndDrag }

        [System.Serializable]
        public class Bound
        {
            public bool activated = false;
            public Vector2 min = Vector2.zero;
            public Vector2 max = Vector2.zero;
        }

        [System.Serializable]
        public class Snap
        {
            public bool activated = false;
            public Vector2 threshold = Vector2.one;
        }

        public Snap snap = new Snap();
        public Bound bound = new Bound();
        public Action<State> OnStateChanged = new Action<State>(s => { });

        private Vector2 offset = Vector2.zero;

        public bool IsFreezed { get; set; }
        public bool IsDragging { get { return current == this; } }
        public Vector2 Position { get; private set; }
        public Vector2 Delta { get; private set; }


        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, eventData.enterEventCamera ?? eventData.pressEventCamera, out offset);
            offset = rectTransform.anchoredPosition - offset;
            OnStateChanged(State.StartDrag);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            cancelDrag = false;
            current = null;
            OnStateChanged(State.EndDrag);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (cancelDrag)
            {
                eventData.pointerDrag = null;
                OnEndDrag(eventData);
                return;
            }

            if (IsFreezed) return;
            current = this;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, eventData.enterEventCamera ?? eventData.pressEventCamera, out localPoint);
            localPoint += offset;

            if (snap.activated)
            {
                localPoint.x = Mathf.RoundToInt(localPoint.x / snap.threshold.x) * snap.threshold.x;
                localPoint.y = Mathf.RoundToInt(localPoint.y / snap.threshold.y) * snap.threshold.y;
            }

            if (bound.activated)
            {
                localPoint.x = Mathf.Clamp(localPoint.x, bound.min.x, bound.max.x);
                localPoint.y = Mathf.Clamp(localPoint.y, bound.min.y, bound.max.y);
            }

            Position = localPoint;
            Delta = eventData.delta;

            OnStateChanged(State.Dragging);
        }


        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        public static UIDragable current = null;
        private static bool cancelDrag = false;
        public static void CancelDrag()
        {
            if (current)
                cancelDrag = true;
        }
    }

}

