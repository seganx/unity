using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class GameObjectEx
{
    public static RectTransform GetRectTransform(this GameObject self)
    {
        return self.transform as RectTransform;
    }

    public static GameObject Clone(this GameObject self)
    {
        var res = GameObject.Instantiate(self);
        res.name = self.name;
        if (self.transform.parent != null)
            res.transform.SetParent(self.transform.parent, false);
        res.transform.CopyValuesFrom(self.transform);
        return res;
    }

    public static T Clone<T>(this GameObject self) where T : Component
    {
        return self.Clone().GetComponent<T>();
    }

    public static T Clone<T>(this Component self) where T : Component
    {
        return self.gameObject.Clone<T>();
    }

    public static T Clone<T>(this Component self, Transform parent, bool worldPositionStays = false) where T : Component
    {
        var res = self.gameObject.Clone();
        res.transform.SetParent(parent, worldPositionStays);
        return res.GetComponent<T>();
    }

    public static T Clone<T>(this GameObject self, Transform parent, bool worldPositionStays = false) where T : Component
    {
        var res = self.Clone();
        res.transform.SetParent(parent, worldPositionStays);
        return res.GetComponent<T>();
    }

    public static T GetComponent<T>(this Component self, bool includeChildren, bool includeInactive) where T : Component
    {
        var res = self.GetComponent<T>();
        if (res == null && includeChildren)
            res = self.GetComponentInChildren<T>(includeInactive);
        return res;
    }

    public static List<T> GetComponents<T>(this Component self, bool includeChildren, bool includeInactive) where T : Component
    {
        var res = new List<T>();
        res.AddRange(self.GetComponents<T>());
        if (includeChildren)
            res.AddRange(self.GetComponentsInChildren<T>(includeInactive));
        return res;
    }

    public static void DestroyNow(this GameObject self)
    {
        self.transform.SetParent(null);
        self.SetActive(false);
        GameObject.Destroy(self);
    }

    public static GameObject RefreshMaterials(this GameObject self)
    {
        var renderers = self.GetComponentsInChildren<Renderer>(true);
        foreach (var render in renderers)
            foreach (var mat in render.materials)
                mat.RefreshMaterial();

        var graphics = self.GetComponentsInChildren<UnityEngine.UI.Graphic>(true);
        foreach (var graphic in graphics)
        {
            graphic.material.RefreshMaterial();
            graphic.materialForRendering.RefreshMaterial();
        }
        return self;
    }

    public static Material RefreshMaterial(this Material self)
    {
        if (self.shader == null || self.hideFlags != HideFlags.None) return self;
        var shader = Shader.Find(self.shader.name);
        if (shader == null) return self;
        self.shader = shader;
        return self;
    }

    public static Material Clone(this Material self)
    {
        var res = Object.Instantiate(self);
        res.name = self.name;
        return res;
    }
}
