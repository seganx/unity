using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    public class UIGradientColor : MonoBehaviour
    {
        public Gradient gradient;

        public Color Evaluate(float t)
        {
            return gradient.Evaluate(t);
        }

        public Color Evaluate(float value, float max)
        {
            return gradient.Evaluate(value / max);
        }
    }

}
