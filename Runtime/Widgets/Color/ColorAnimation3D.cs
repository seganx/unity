using UnityEngine;

namespace SeganX.Widgets
{
    public abstract class ColorAnimation3D : ColorAnimationBase<Renderer>
    {
        protected Color initialColor;
        [SerializeField] private int targetMaterialElement;

        protected override bool IsReferenceAvailable
        {
            get
            {
#if UNITY_EDITOR
                if (target == null)
                    Debug.LogWarning($"{name}:{GetType().Name}: Target in empty!");
                if (targetMaterialElement >= target.materials.Length)
                    Debug.LogWarning($"{name}:{GetType().Name}: Index out of range!");
#endif
                return target != null && targetMaterialElement < target.materials.Length;
            }
        }

        protected override void SetColor()
        {
            switch (copyMode)
            {
                case CopyMode.All:
                    target.materials[targetMaterialElement].color = color.Evaluate(time / 1);
                    break;

                case CopyMode.RGB:
                {
                    var targetColor = color.Evaluate(time / 1);
                    targetColor.a = target.materials[targetMaterialElement].color.a;
                    target.materials[targetMaterialElement].color = targetColor;
                }
                    break;

                case CopyMode.Alpha:
                {
                    var targetColor = target.materials[targetMaterialElement].color;
                    targetColor.a = color.Evaluate(time / 1).a;
                    target.materials[targetMaterialElement].color = targetColor;
                }
                    break;
            }
        }

        protected override void AfterAnimate()
        {
            if (IsReferenceAvailable)
                target.materials[targetMaterialElement].color = initialColor;
        }

        protected override void BeforeAnimate()
        {
            if (IsReferenceAvailable)
                initialColor = target.materials[targetMaterialElement].color;
        }
    }
}