using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMover : Base
{
    public float totalTime = 1;
    public AnimationCurve movement = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 0, 0) });

    private Vector3 initPosition;
    private Vector3 destPosition;
    private float timer = 0;

    public Vector3 Destination
    {
        get { return destPosition; }
        set
        {
            destPosition = value;
            initPosition = rectTransform.anchoredPosition3D;
            timer = 0;
        }
    }

    public UIMover Setup(Vector3 position)
    {
        rectTransform.anchoredPosition3D = initPosition = destPosition = position;
        timer = 1;
        return this;
    }

    void OnValidate()
    {
        var keys = movement.keys;
        for (int i = 0; i < keys.Length; i++)
            keys[i].time = Mathf.Clamp01(keys[i].time);
        movement.keys = keys;
    }

    void Awake()
    {
        Setup(rectTransform.anchoredPosition3D);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= totalTime) return;
        timer += Time.deltaTime;
        var t = movement.Evaluate(timer / totalTime);
        rectTransform.anchoredPosition3D = Vector3.LerpUnclamped(initPosition, destPosition, t);
    }
}
