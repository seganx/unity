using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeganX
{
    [CustomEditor(typeof(UiParticleEmitter)), CanEditMultipleObjects]
    public class UiParticleEmitterEditor : Editor
    {
        private UiParticleEmitter emitter = null;

        private SerializedProperty particleRenderer;
        private SerializedProperty playOnAwake;
        private SerializedProperty duration;
        private SerializedProperty loop;
        private SerializedProperty startLifetime;
        private SerializedProperty startColor;
        private SerializedProperty startSize;
        private SerializedProperty startRotation;
        private SerializedProperty startSpeed;

        private SerializedProperty rateOverTime;
        private SerializedProperty bursts;

        private SerializedProperty emitShape;
        private SerializedProperty emitShapeRect;
        private SerializedProperty emitShapeCircle;
        private SerializedProperty emitFrom;
        private SerializedProperty randomizeDirectionActive;
        private SerializedProperty randomizeDirectionX;
        private SerializedProperty randomizeDirectionY;

        protected virtual void OnEnable()
        {
            emitter = target as UiParticleEmitter;

            particleRenderer = serializedObject.FindProperty("particleRenderer");
            playOnAwake = serializedObject.FindProperty("playOnAwake");
            duration = serializedObject.FindProperty("duration");
            loop = serializedObject.FindProperty("loop");
            startLifetime = serializedObject.FindProperty("startLifetime");
            startColor = serializedObject.FindProperty("startColor");
            startSize = serializedObject.FindProperty("startSize");
            startRotation = serializedObject.FindProperty("startRotation");
            startSpeed = serializedObject.FindProperty("startSpeed");
            rateOverTime = serializedObject.FindProperty("rateOverTime");
            bursts = serializedObject.FindProperty("bursts");
            emitShape = serializedObject.FindProperty("emitShape");
            emitShapeRect = serializedObject.FindProperty("emitShapeRect");
            emitShapeCircle = serializedObject.FindProperty("emitShapeCircle");
            emitFrom = serializedObject.FindProperty("emitFrom");
            var overrideDirection = serializedObject.FindProperty("overrideDirection");
            randomizeDirectionActive = overrideDirection.FindPropertyRelative("active");
            randomizeDirectionX = overrideDirection.FindPropertyRelative("x");
            randomizeDirectionY = overrideDirection.FindPropertyRelative("y");

            EditorApplication.update += Update;
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= Update;

            if (Application.isPlaying == false && emitter != null)
                emitter.Stop(true);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(particleRenderer);
            EditorGUILayout.PropertyField(playOnAwake);
            EditorGUILayout.PropertyField(duration);
            EditorGUILayout.PropertyField(loop);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(startLifetime);
            EditorGUILayout.PropertyField(startColor);
            EditorGUILayout.PropertyField(startSize);
            EditorGUILayout.PropertyField(startRotation);
            EditorGUILayout.PropertyField(startSpeed);

            EditorGUILayout.Space();
            EditorUtils.BeginBoxWithHeader("Emission");
            EditorGUILayout.PropertyField(rateOverTime);
            EditorGUILayout.PropertyField(bursts);
            EditorUtils.EndBoxWithHeader();

            EditorGUILayout.Space();
            EditorUtils.BeginBoxWithHeader("Shape");
            EditorGUILayout.PropertyField(emitShape, new GUIContent("Shape"));
            if (emitter.emitShape == UiParticleEmitter.EmitShape.Rectangle)
                EditorGUILayout.PropertyField(emitShapeRect, new GUIContent(" "));
            else
                EditorGUILayout.PropertyField(emitShapeCircle, new GUIContent(" "));
            EditorGUILayout.PropertyField(emitFrom);
            EditorGUILayout.PropertyField(randomizeDirectionActive, new GUIContent("Override Direction"));
            if (emitter.overrideDirection.active)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(randomizeDirectionX);
                EditorGUILayout.PropertyField(randomizeDirectionY);
            }
            EditorUtils.EndBoxWithHeader();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void Update()
        {
            if (Application.isPlaying || target == null) return;
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();
        }

        protected virtual void OnSceneGUI()
        {
            var rect = new Rect(10, 10, 200, 60);
            var sceneRect = SceneView.currentDrawingSceneView.camera.pixelRect;
            rect.x = sceneRect.width - rect.width - rect.x;
            rect.y = sceneRect.height - rect.height - rect.y;

            Handles.BeginGUI();
            GUILayout.BeginArea(rect, "Particle", GUI.skin.window);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Play"))
                emitter.Play();
            if (GUILayout.Button("Stop"))
                emitter.Stop(false);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            Handles.EndGUI();
        }
    }
}