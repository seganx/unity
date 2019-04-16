using System.Xml;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    [CreateAssetMenu(menuName = "Game/AssetData")]
    public class AssetData : ScriptableObject
    {
        public int id = 0;
        public string type = string.Empty;
        [PersianPreview]
        public string tags = string.Empty;
        public List<AssetItem> prefabs = new List<AssetItem>();


        public bool HasTag(string[] taglist)
        {
            foreach (var item in taglist)
                if (tags.Contains(item))
                    return true;
            return false;
        }

        public override string ToString()
        {
            return id + " : " + type;
        }

        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        public static List<AssetData> all = new List<AssetData>();

        public static List<AssetData> FindByTags(string tags)
        {
            var res = new List<AssetData>();
            if (tags.HasContent())
            {
                var taglist = tags.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (AssetData data in all)
                    if (data.HasTag(taglist))
                        res.Add(data);
            }
            return res;
        }

        public static AssetData LoadEncrypted(int id, byte[] src)
        {
            //  search to see if the asset bundle is already loaded
            {
                var loadedOne = all.Find(x => x.id == id);
                if (loadedOne != null) return loadedOne;
            }

            var data = CryptoService.DecryptWithMac(src, Core.CryptoKey, Core.Salt);
            if (data == null) return null;

            try
            {
                var bundle = AssetBundle.LoadFromMemory(data);
                return GetAssetData(bundle, id);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Can't load asset bundle " + id + " du to error: " + e.Message);
                return null;
            }
        }

        public static AssetData LoadEncrypted(int id, string path)
        {
            //  search to see if the asset bundle is already loaded
            {
                var loadedOne = all.Find(x => x.id == id);
                if (loadedOne != null) return loadedOne;
            }

            //  search for cahched file
            var filename = path + ".cache";
            if (System.IO.File.Exists(filename) == false)
            {
                var bytes = System.IO.File.ReadAllBytes(path);
                var data = CryptoService.DecryptWithMac(bytes, Core.CryptoKey, Core.Salt);
                System.IO.File.WriteAllBytes(filename, data);
            }

            try
            {
                var bundle = AssetBundle.LoadFromFile(filename);
                return GetAssetData(bundle, id);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Can't load asset bundle " + id + " du to error: " + e.Message);
                return null;
            }
        }

        private static AssetData GetAssetData(AssetBundle bundle, int id)
        {
            var paths = bundle.GetAllAssetNames();
            foreach (var item in paths)
            {
                var asset = bundle.LoadAsset<AssetData>(item);
                if (asset != null)
                {
                    all.Remove(asset);
                    all.Add(asset);

                    if (id > 0)
                        asset.id = id;

                    foreach (var assetItem in asset.prefabs)
                        assetItem.tags = asset.tags;

                    return asset;
                }
            }
            return null;
        }
    }
}
