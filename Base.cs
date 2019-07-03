using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public static Game gameManager { get { return Game.Instance; } }

    public RectTransform rectTransform { get { return transform as RectTransform; } }
    public RectTransform parentRectTransform { get { return transform.parent as RectTransform; } }
    public virtual bool Visible { set { gameObject.SetActive(value); } get { return gameObject.activeSelf; } }

    public void DestroyGameObject(float delay = 0)
    {
        Destroy(gameObject, delay);
    }

    public void DestroyScript(float delay)
    {
        Destroy(this, delay);
    }

    public void DelayCall(float seconds, System.Action callback)
    {
        if (seconds > 0.001f)
            StartCoroutine(DoDelayCall(seconds, callback));
        else
            callback();
    }

    private IEnumerator DoDelayCall(float seconds, System.Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback();
    }
}
