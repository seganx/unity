using UnityEngine;
using UnityEditor;

namespace SeganX
{
    [CustomEditor(typeof(UiParticleRenderer)), CanEditMultipleObjects]
    public class UiParticleRendererEditor : Editor
    {
        protected SerializedProperty material;
        protected SerializedProperty raycastTarget;
        protected SerializedProperty maskable;
        protected SerializedProperty sprite;
        protected SerializedProperty spriteSize;
        protected SerializedProperty colorOverLife;
        protected SerializedProperty sizeOverLife;
        protected SerializedProperty rotateOverLife;
        protected SerializedProperty velocityOverLife;
        protected SerializedProperty windOverLife;
        protected SerializedProperty gravityOverLife;
        protected SerializedProperty noiseAmplitude;
        protected SerializedProperty noiseFrequency;
        protected SerializedProperty destinationFactor;
        protected SerializedProperty OnParticleDead;
        protected SerializedProperty OnFinished;

        protected virtual void OnEnable()
        {
            material = serializedObject.FindProperty("m_Material");
            raycastTarget = serializedObject.FindProperty("m_RaycastTarget");
            maskable = serializedObject.FindProperty("m_Maskable");
            sprite = serializedObject.FindProperty("sprite");
            spriteSize = serializedObject.FindProperty("spriteSize");
            colorOverLife = serializedObject.FindProperty("colorOverLife");
            sizeOverLife = serializedObject.FindProperty("sizeOverLife");
            rotateOverLife = serializedObject.FindProperty("rotateOverLife");
            velocityOverLife = serializedObject.FindProperty("velocityOverLife");
            gravityOverLife = serializedObject.FindProperty("gravityOverLife");
            windOverLife = serializedObject.FindProperty("windOverLife");
            noiseAmplitude = serializedObject.FindProperty("noiseAmplitude");
            noiseFrequency = serializedObject.FindProperty("noiseFrequency");
            destinationFactor = serializedObject.FindProperty("destinationFactor");
            OnParticleDead = serializedObject.FindProperty("OnParticleDead");
            OnFinished = serializedObject.FindProperty("OnFinished");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(sprite);
            EditorGUILayout.PropertyField(spriteSize);
            EditorGUILayout.PropertyField(material);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(colorOverLife);
            EditorGUILayout.PropertyField(sizeOverLife);
            EditorGUILayout.PropertyField(rotateOverLife);
            EditorGUILayout.PropertyField(velocityOverLife);
            EditorGUILayout.PropertyField(gravityOverLife);
            EditorGUILayout.PropertyField(windOverLife);
            EditorGUILayout.PropertyField(noiseAmplitude);
            EditorGUILayout.PropertyField(noiseFrequency);
            EditorGUILayout.PropertyField(destinationFactor);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(raycastTarget);
            EditorGUILayout.PropertyField(maskable);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(OnParticleDead);
            EditorGUILayout.PropertyField(OnFinished);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
