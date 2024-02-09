using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Widgets
{
    [ExecuteAlways, RequireComponent(typeof(MeshRenderer))]
    public class MeshOverdraw : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter = null;
        [SerializeField] private Material material = null;

        private bool isVisible = false;

        private void Reset()
        {
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();

            if (material == null)
            {
                var renderer = GetComponent<Renderer>();
                if (renderer != null)
                    material = renderer.sharedMaterial;
            }
        }

        private void OnEnable()
        {
            all.Add(this);
        }

        private void OnDisable()
        {
            all.Remove(this);
        }

        private void OnWillRenderObject()
        {
            isVisible = true;
        }

        private void Draw()
        {
            if (meshFilter && material)
                Graphics.DrawMesh(meshFilter.sharedMesh, transform.localToWorldMatrix, material, 0, null, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
            isVisible = false;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying == false)
                Draw();
        }
#endif

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static readonly List<MeshOverdraw> all = new List<MeshOverdraw>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnRuntimeInitializeOnLoadMethod()
        {
            var mono = new GameObject("MeshOverdrawer").AddComponent<Mono>();
            DontDestroyOnLoad(mono);
        }

        private class Mono : MonoBehaviour
        {
            private void Update()
            {
                for (int i = 0; i < all.Count; i++)
                    if (all[i].isVisible)
                        all[i].Draw();
            }
        }
    }
}