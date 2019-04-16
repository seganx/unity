using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SeganX;

[CanEditMultipleObjects]
[CustomEditor(typeof(AssetData))]
public class AssetDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var rect = EditorGUILayout.GetControlRect();
        var maxWidth = rect.width;
        rect.width = 100;
        if (GUI.Button(rect, "Generate Id"))
        {
            foreach (var target in targets)
            {
                var asset = target as AssetData;
                var path = AssetDatabase.GetAssetPath(target);
                var importer = AssetImporter.GetAtPath(path);
                asset.id = EditorOnlineData.GenerateAssetId();
                importer.assetBundleName = "asset" + asset.id;
                EditorUtility.SetDirty(asset);
            }
        }


        foreach (var target in targets)
        {
            var asset = target as AssetData;
            var path = AssetDatabase.GetAssetPath(target);
            var importer = AssetImporter.GetAtPath(path);
    
            if (string.IsNullOrEmpty(importer.assetBundleName))
            {
                importer.assetBundleName = "asset" + asset.id;
            }
        }

        base.OnInspectorGUI();
    }
}
