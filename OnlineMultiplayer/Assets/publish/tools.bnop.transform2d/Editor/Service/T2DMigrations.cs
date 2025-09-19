//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.IO;

namespace tools.bnop.Transform2d.Service
{
    public static class T2DMigrations
    {
        public static void DeleteObsoleteFiles()
        {
            string[] obsoleteRelativePaths = {
                "Guides.asset", 
                "GuidesList.asset", 
                "Transform 2D Manual.pdf", 
                "Resources" 
            };

            foreach (string relativePath in obsoleteRelativePaths)
            {
                string assetPath = T2DAssets.ResolveRelativeAssetPath(relativePath);
                
                if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) != null || Directory.Exists(assetPath))
                {
                    if (AssetDatabase.DeleteAsset(assetPath))
                    {
                        Debug.Log($"Deleted obsolete asset: {assetPath}");
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to delete obsolete asset: {assetPath}");
                    }
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
