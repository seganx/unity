using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SeganX
{
    public static class EventSystemEx
    {
        private static List<RaycastResult> raycastResultCache = new List<RaycastResult>();

        public static PointerEventData GetEventData(this EventSystem self, PointerEventData cached = null)
        {
            return GetTouchPointerEventData(self, cached);
        }

        public static PointerEventData GetTouchPointerEventData(EventSystem eventSystem, PointerEventData cached = null)
        {
            if (cached == null)
                cached = new PointerEventData(eventSystem);
            else
                cached.Reset();

            Vector2 pos = Input.mousePosition;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                // We don't want to do ANY cursor-based interaction when the mouse is locked
                cached.position = new Vector2(-1.0f, -1.0f);
                cached.delta = Vector2.zero;
            }
            else
            {
                cached.delta = pos - cached.position;
                cached.position = pos;
            }

            cached.scrollDelta = Input.mouseScrollDelta;
            cached.button = PointerEventData.InputButton.Left;
            eventSystem.RaycastAll(cached, raycastResultCache);
            var raycast = FindFirstRaycast(raycastResultCache);
            cached.pointerCurrentRaycast = raycast;
            raycastResultCache.Clear();
            return cached;
        }

        public static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            for (var i = 0; i < candidates.Count; ++i)
            {
                if (candidates[i].gameObject == null)
                    continue;

                return candidates[i];
            }
            return new RaycastResult();
        }
    }
}