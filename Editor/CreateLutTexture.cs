#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SeganX
{
    public static class CreateLutTexture
    {
        [MenuItem("SeganX/Tools/Create LUT Texture")]
        private static void CreateLutTextureFrom2D()
        {
            // get all selected *assets*
            var selectedAssets = Selection.objects.Where(o => !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(o))).ToArray();
            if (selectedAssets.Length < 1) return;

            foreach (var item in selectedAssets)
            {
                var asset = Convert(item as Texture2D);
                if (asset == null) continue;
                AssetDatabase.CreateAsset(asset, AssetDatabase.GetAssetPath(item).ExcludeFileExtention() + ".asset");
                AssetDatabase.SaveAssets();
            }
        }


        public static Texture3D Convert(Texture2D tex2D)
        {
            if (tex2D.IsTypeOf<Texture2D>() == false) return null;

            int dim = tex2D.height;

            Color[] c = tex2D.GetPixels();
            Color[] newC = new Color[c.Length];

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    for (int k = 0; k < dim; k++)
                    {
                        int j_ = dim - j - 1;
                        newC[i + (j * dim) + (k * dim * dim)] = c[k * dim + i + j_ * dim * dim];
                    }
                }
            }

            var res = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
            res.SetPixels(newC);
            res.Apply();
            res.wrapMode = TextureWrapMode.Clamp;
            res.filterMode = FilterMode.Trilinear;
            res.anisoLevel = 1;
            return res;
        }
    }
}
#endif