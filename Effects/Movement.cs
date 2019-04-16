using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SeganX.Effects
{
    public class Movement : Base
    {
        [System.Serializable]
        public class CurvePath
        {
            public bool activate = false;
            public float speed = 1;
            public AnimationCurve x = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(10, 10, 0, 0) });
            public AnimationCurve y = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(10, 10, 0, 0) });
            public AnimationCurve z = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(10, 10, 0, 0) });
            public AnimationCurve weight = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(0.5f, 1, 0, 0), new Keyframe(1, 0, 0, 0) });
        }

        public Transform target = null;
        public AnimationCurve speed = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1, 0, 0), new Keyframe(0.5f, 5, 0, 0), new Keyframe(1, 1, 0, 0) });
        public CurvePath curvePath = new CurvePath();
        public UnityEvent OnReachTarget;

        private float moveTime = 0;
        private float moveTimeMax = 0;
        private Vector3 pos = Vector3.zero;
        private Vector3 initPosition = Vector3.zero;

        // Use this for initialization
        void Start()
        {
            initPosition = transform.localPosition;
            moveTime = 0;
            if (target)
                moveTimeMax = Vector3.Distance(transform.position, target.position);
        }

        // Update is called once per frame
        void Update()
        {
            var targetTime = Vector3.Distance(transform.position, target.position) / moveTimeMax;
            if (target)
            {
                initPosition = Vector3.MoveTowards(initPosition, target.position, Time.deltaTime * speed.Evaluate(1 - targetTime));
                if (initPosition == target.position)
                    OnReachTarget.Invoke();
            }

            if (curvePath.activate)
            {
                moveTime += Time.deltaTime;
                var curveTime = moveTime * curvePath.speed;
                pos.x = curvePath.x.Evaluate(curveTime);
                pos.y = curvePath.y.Evaluate(curveTime);
                pos.z = curvePath.z.Evaluate(curveTime);
                pos.Scale(Vector3.one * curvePath.weight.Evaluate(1 - targetTime));
            }

            transform.localPosition = initPosition + pos;
        }
    }
}
