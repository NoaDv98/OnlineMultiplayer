//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.Service
{
    public class T2DSerializedObject
    {
        public static T FindOrCreateInRelativePath<T>(string relativePath) where T : ScriptableObject
        {
            var path = T2DAssets.ResolveRelativeAssetPath(relativePath);
            var so = AssetDatabase.LoadAssetAtPath<T>(path);
            return so ? so : Create<T>(path);
        }

        private static T Create<T>(string path) where T : ScriptableObject
        {
            var directory = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(directory))
            {
                if (directory != null) System.IO.Directory.CreateDirectory(directory);
            }
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }
    }
}
