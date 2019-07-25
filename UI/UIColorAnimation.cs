using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    public class UIColorAnimation : MonoBehaviour
    {
        public enum WrapMode { PingPong, Loop, Once }
        public enum CopyMode { All, RGB, A }

        public Graphic graphic = null;
        public Gradient gradient;
        public CopyMode copyMode = CopyMode.All;
        public WrapMode wrapMode = WrapMode.PingPong;
        public float speed = 1;
        public float length = 1;
        public bool randomTime = true;

        private float time = 0;
        private float timeAdvance = 1;

        void Reset()
        {
            if (graphic == null)
                graphic = GetComponent<Graphic>();
        }

        // Use this for initialization
        void Awake()
        {
            if (graphic == null)
                graphic = GetComponent<Graphic>();
            if (randomTime)
                time = Random.Range(0, length);
        }

        private void Start()
        {
            Update();
        }

        // Update is called once per frame
        void Update()
        {
            switch (wrapMode)
            {
                case WrapMode.PingPong:
                    time = Mathf.Clamp(time + timeAdvance * speed * Time.deltaTime, 0, length);
                    if (time >= length || time <= 0) timeAdvance *= -1;
                    break;

                case WrapMode.Loop:
                    time += speed * Time.deltaTime;
                    if (time > length) time = 0;
                    break;

                case WrapMode.Once:
                    if (time < length)
                        time = Mathf.Clamp(time + speed * Time.deltaTime, 0, length);
                    break;
            }

            switch (copyMode)
            {
                case CopyMode.All:
                    graphic.color = gradient.Evaluate(time / length);
                    break;

                case CopyMode.RGB:
                    {
                        var dc = gradient.Evaluate(time / length);
                        var c = graphic.color;
                        c.r = dc.r;
                        c.g = dc.g;
                        c.b = dc.b;
                        graphic.color = c;
                    }
                    break;

                case CopyMode.A:
                    {
                        var dc = gradient.Evaluate(time / length);
                        var c = graphic.color;
                        c.a = dc.a;
                        graphic.color = c;
                    }
                    break;
            }
        }
    }
}