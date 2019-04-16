using UnityEngine;

public static class TransformEx
{
    public static Vector2 Scale(this Vector2 self, float x, float y)
    {
        self.x *= x; self.y *= y;
        return self;
    }

    public static Vector3 Scale(this Vector3 self, float x, float y, float z)
    {
        self.x *= x; self.y *= y; self.z *= z;
        return self;
    }

    public static Transform CopyValuesFrom(this Transform self, Transform source)
    {
        self.localScale = source.localScale;
        self.localRotation = source.localRotation;
        self.localPosition = source.localPosition;

        if (self is RectTransform && source is RectTransform)
        {
            RectTransform rself = self as RectTransform;
            RectTransform rsrce = source as RectTransform;
            rself.anchorMax = rsrce.anchorMax;
            rself.anchorMin = rsrce.anchorMin;
            rself.sizeDelta = rsrce.sizeDelta;
            rself.anchoredPosition3D = rsrce.anchoredPosition3D;
        }

        return self;
    }

    public static int GetActiveChildCount(this Transform self)
    {
        int res = 0;
        for (int i = 0; i < self.childCount; i++)
        {
            if (self.GetChild(i).gameObject.activeSelf)
                res++;
        }
        return res;
    }

    public static Transform DestroyNow(this Transform self)
    {
        self.SetParent(null);
        self.gameObject.SetActive(false);
        GameObject.Destroy(self.gameObject);
        return self;
    }

    public static Transform RemoveChildren(this Transform self, int startIndex = 0, int count = -1)
    {
        if (count == 0) return self;

        if (count > 0)
            count = Mathf.Min(count + startIndex, self.childCount);
        else
            count = self.childCount - startIndex;

        while (count-- > 0)
            self.GetChild(startIndex).DestroyNow();

        return self;
    }

    public static Transform RemoveChildrenBut(this Transform self, int except)
    {
        int count = except;
        while (count-- > 0)
            self.GetChild(0).DestroyNow();
        while (self.childCount > 1)
            self.GetChild(1).DestroyNow();
        return self;
    }

    //! this will disable all children but activate a child specified by index
    public static Transform SetActiveChild(this Transform self, int index)
    {
        for (int i = 0; i < self.childCount; i++)
            self.GetChild(i).gameObject.SetActive(i == index);
        return self;
    }

    //! return index of first activated child. return -1 if no active child found
    public static int GetActiveChild(this Transform self)
    {
        for (int i = 0; i < self.childCount; i++)
            if (self.GetChild(i).gameObject.activeSelf)
                return i;
        return -1;
    }

    public static RectTransform AsRectTransform(this Transform self)
    {
        return self as RectTransform;
    }

    public static Vector3 GetAnchordPosition(this Transform self)
    {
        return (self as RectTransform).anchoredPosition3D;
    }

    public static RectTransform SetAnchordPosition(this Transform self, Vector3 pos)
    {
        (self as RectTransform).anchoredPosition3D = pos;
        return self as RectTransform;
    }

    public static RectTransform SetAnchordPosition(this Transform self, float x, float y, float z)
    {
        (self as RectTransform).anchoredPosition3D = new Vector3(x, y, z);
        return self as RectTransform;
    }

    public static RectTransform SetAnchordPositionX(this Transform self, float x)
    {
        Vector3 pos = self.GetAnchordPosition();
        pos.x = x;
        self.SetAnchordPosition(pos);
        return self as RectTransform;
    }

    public static RectTransform SetAnchordPositionY(this Transform self, float y)
    {
        Vector3 pos = self.GetAnchordPosition();
        pos.y = y;
        self.SetAnchordPosition(pos);
        return self as RectTransform;
    }

    public static RectTransform SetAnchordHeight(this Transform self, float height)
    {
        (self as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        return self as RectTransform;
    }

    public static RectTransform SetAnchordWidth(this Transform self, float width)
    {
        (self as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        return self as RectTransform;
    }

    public static RectTransform SetAnchordSize(this Transform self, float width, float height)
    {
        (self as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        (self as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        return self as RectTransform;
    }

    public static Transform SetPosition(this Transform self, float x, float y, float z)
    {
        var pos = self.position;
        pos.Set(x, y, z);
        self.position = pos;
        return self;
    }

    public static Transform Scale(this Transform self, Vector3 factors)
    {
        var scl = self.localScale;
        scl.Scale(factors);
        self.localScale = scl;
        return self;
    }

    public static Transform Scale(this Transform self, float x, float y, float z)
    {
        var scl = self.localScale;
        scl.x *= x; scl.y *= y; scl.z *= z;
        self.localScale = scl;
        return self;
    }

    public static Transform ScaleLocalPosition(this Transform self, float x, float y, float z)
    {
        var pos = self.localPosition;
        pos.x *= x; pos.y *= y; pos.z *= z;
        self.localPosition = pos;
        return self;
    }

    public static Transform SetParent(this Transform self, Transform parent, bool holdPosition, bool holdScale, bool holdRotation)
    {
        Vector3 lastPos = self.position;
        Quaternion lastRotation = self.rotation;
        Vector3 lastScale = self.localScale;

        self.SetParent(parent, false);

        if (holdScale) self.localScale = lastScale;
        if (holdRotation) self.rotation = lastRotation;
        if (holdPosition) self.position = lastPos;

        return self;
    }

    public static Transform SetParent(this Transform self, Transform parent, Vector3 localPosition, Vector3 localScale)
    {
        self.SetParent(parent, false);
        self.localScale = localScale;
        self.localPosition = localPosition;
        return self;
    }

    public static RectTransform AttachToCanvas(this Transform self, string canvasName)
    {
        RectTransform rectTransform = self.AsRectTransform();
        rectTransform.SetParent(GameObject.Find(canvasName).transform, false);
        return rectTransform;
    }

    public static T GetComponentInParent<T>(this Transform self, bool inactives) where T : Component
    {
        if (inactives == false)
            return self.GetComponentInParent<T>();

        while (self != null)
        {
            T res = self.GetComponent<T>();
            if (res != null)
                return res;
            self = self.parent;
        }

        return null;
    }

    public static T GetChild<T>(this Transform self, int index) where T : Component
    {
        var res = self.GetChild(index).GetComponent<T>();
        return res == null ? self.GetChild(index).GetComponentInChildren<T>() : res;
    }

    public static Transform FindRecursive(this Transform self, string childName, bool justActivates = false)
    {
        Transform child = self.Find(childName);
        if (child != null)
        {
            if (justActivates)
            {
                if (child.gameObject.activeInHierarchy)
                    return child;
            }
            else return child;
        }

        for (int idx = 0; idx < self.childCount; ++idx)
            if ((child = FindRecursive(self.GetChild(idx), childName, justActivates)) != null)
                return child;

        return null;
    }

    //
    // Summary:
    //     Rotate transform to look at the target. NOTE that transform and target should have same anchors!
    //
    // Parameters:
    //   target:
    //     transform will look at the target in 2D space.
    //
    // Returns:
    //     return self transform
    public static RectTransform LookAt2D(this RectTransform self, Vector2 target)
    {
        Vector2 dis = target - self.anchoredPosition;
        self.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dis.y, dis.x) * Mathf.Rad2Deg);
        return self;
    }

#if OFF
    public static bool IsInViewport2D(this RectTransform self, ScrollListRectInfo viewport)
    {
        Vector2 pos = new Vector2(self.position.x, self.position.y);
        Vector2 max = pos + (Vector2)(self.parent.localToWorldMatrix * self.rect.max);
        Vector2 min = pos + (Vector2)(self.parent.localToWorldMatrix * self.rect.min);
        return min.y <= viewport.TopEdge && max.y >= viewport.BottomEdge && min.x <= viewport.RightEdge && max.x >= viewport.LeftEdge;
    }
#endif
}
