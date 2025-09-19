//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System.IO;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.Service
{
    public static class T2DAssets
    {
        private static string _pluginAssetPath;

        private static string PluginAssetPath
        {
            get
            {
                if (_pluginAssetPath == null)
                {
                    var guids = AssetDatabase.FindAssets("Transform2D t:Script");
                    if (guids.Length > 0)
                    {
                        var scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                        var pluginDir = Path.GetDirectoryName(Path.GetDirectoryName(scriptPath));

                        _pluginAssetPath = scriptPath.StartsWith("Packages/")
                            ? "Assets/tools.bnop.transform2d"
                            : pluginDir;
                    }
                    else
                    {
                        _pluginAssetPath = "Assets/tools.bnop.transform2d"; 
                    }
                }

                return _pluginAssetPath;
            }
        }

        public static string ResolveRelativeAssetPath(string relativeAssetPath)
        {
            if (string.IsNullOrEmpty(relativeAssetPath))
                return null;

            var fullPath = Path.Combine(PluginAssetPath, relativeAssetPath).Replace('\\', '/');

            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                if (directory != null) Directory.CreateDirectory(directory);
            }

            return fullPath;
        }
    }
}
