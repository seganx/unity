using UnityEditor;
using UnityEngine;

namespace SeganX
{
    [CustomEditor(typeof(FitToWidthProjection))]
    public class FitToWidthProjectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var fitters = GameObject.FindObjectsOfType<FitToWidthProjection>();
            foreach (var fitter in fitters)
                fitter.UpdateProjectionMatrix();
        }
    }
}