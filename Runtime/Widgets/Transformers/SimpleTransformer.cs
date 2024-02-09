using UnityEngine;

namespace SeganX.Widgets
{
    public class SimpleTransformer : TransformerBase
    {
        [SerializeField] private Vector3 targetLocalPosition;
        [SerializeField] private Vector3 targetLocalRotation;
        [SerializeField] private Vector3 targetLocalScale = Vector3.one;
        [SerializeField] private AnimationCurve playCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
        [SerializeField] private AnimationCurve returnCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });

        private Vector3 initialLocalPosition;
        private Quaternion initialLocalRotation;
        private Vector3 initialLocalScale;

        protected override void Start()
        {
            base.Start();

            initialLocalPosition = transform.localPosition;
            initialLocalRotation = transform.localRotation;
            initialLocalScale = transform.localScale;
        }

        public void Play()
        {
            StartTransform(targetLocalPosition, Quaternion.Euler(targetLocalRotation), targetLocalScale, Space.Self, playCurve);
        }

        public void Return()
        {
            StartTransform(initialLocalPosition, initialLocalRotation, initialLocalScale, Space.Self, returnCurve);
        }


    }
}