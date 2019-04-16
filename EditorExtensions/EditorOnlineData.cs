using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EditorOnlineData
{
    [System.Serializable]
    public class Data
    {
        public int version = 0;
        public int bundle_asset_id = 0;
    }

    public static int GenerateAssetId()
    {
        var ws = new WWW("http://unity.seganx.com/bundle_asset_id.php");
        while (ws.isDone == false);
        var sid = JsonUtility.FromJson<Data>(ws.text);
        return sid.bundle_asset_id;
    }
}
