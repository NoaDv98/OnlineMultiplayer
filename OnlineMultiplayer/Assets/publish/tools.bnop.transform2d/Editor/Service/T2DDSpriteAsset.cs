//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using tools.bnop.Transform2d.Data;
using UnityEditor;
using UnityEditor.U2D;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace tools.bnop.Transform2d.Service
{
    public static class T2DDSpriteAsset
    {
        public static Sprite GetSprite(GameObject target)
        {
            var renderer = target?.GetComponent<SpriteRenderer>();
            var sprite = renderer?.sprite;
            if (!target || !renderer || !sprite)
                throw new Exception("Missing GameObject, or GameObject missing SpriteRenderer or Sprite");
            return sprite;
        }

        public static string GetSpritePath(GameObject target)
        {
            var sprite = GetSprite(target);
            return AssetDatabase.GetAssetPath(sprite);
        }

        public static bool IsPivotPixelPerfect(GameObject target, bool unscaled = true)
        {
            var sprite = GetSprite(target);
            var localScale = target.transform.localScale;
            var spritePivot = unscaled ? sprite.pivot : Vector2.Scale(sprite.pivot, localScale);
            var isRound = Mathf.Approximately(spritePivot.x, Mathf.Round(spritePivot.x)) &&
                          Mathf.Approximately(spritePivot.y, Mathf.Round(spritePivot.y));
            return isRound;
        }

        /// <seealso cref="http://docs.unity3d.com/Packages/com.unity.2d.sprite@1.0/manual/DataProvider.html"/>
        public static void SnapPivotToPixel(GameObject target, bool unscaled = true, bool roundAllSpriteSheet = false)
        {
            EditorApplication.delayCall += () =>
            {
#if UNITY_2020_2_OR_NEWER

                if (!EditorUtility.DisplayDialog("Update Sprite Asset Pivot",
                        "Round current pivot to whole pixels? " +
                        "Note: No undo action for this change",
                        "Yes", "No")) return;

                var sprite = GetSprite(target);
                var factory = new SpriteDataProviderFactories();
                factory.Init();

                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(sprite.texture);
                dataProvider.InitSpriteEditorDataProvider();

                var rects = dataProvider.GetSpriteRects();
                foreach (var rect in rects)
                {
                    if (!roundAllSpriteSheet && rect.spriteID != sprite.GetSpriteID()) continue;

                    var width = rect.rect.width;
                    var height = rect.rect.height;

                    if (!unscaled)
                    {
                        var localScale = target.transform.localScale;
                        width *= localScale.x;
                        height *= localScale.y;
                    }

                    var pivot = new Vector2(Mathf.Round(rect.pivot.x * width), Mathf.Round(rect.pivot.y * height));
                    pivot.x /= width;
                    pivot.y /= height;

                    rect.pivot = pivot;
                    rect.alignment = SpriteAlignment.Custom;
                }

                dataProvider.SetSpriteRects(rects);
                dataProvider.Apply();

                var assetImporter = dataProvider.targetObject as AssetImporter;
                if (assetImporter != null) assetImporter.SaveAndReimport();
                else throw new Exception("Failed to update sprite asset importer");
#else
            EditorUtility.DisplayDialog("Update Sprite Asset Pivot", 
                "This feature is only supported for Unity version 2020.2 and above."
                + "Please updated manually using the Sprite Editor.", "Ok");

#endif
            };
        }

        public static void SetPixelsPerUnit(T2DGameObject target, float targetPpu, Action onPpuChanged = null)
        {
            EditorApplication.delayCall += () =>
            {
                var spritePath = GetSpritePath(target.Target);
                var importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
                
                if (importer == null)
                    throw new Exception("Missing TextureImporter");
                
                if (!IsSpriteAssetEditable(importer)) 
                {
                    EditorUtility.DisplayDialog("Cannot Update Sprite Asset PPU", 
                        "Sprite asset is not editable. " +
                        "Please try to update manually by using the sprite asset Inspector.", "Ok");
                    return;
                }

                if (!EditorUtility.DisplayDialog("Update Sprite Asset PPU",
                        $"Change sprite asset PPU from {importer.spritePixelsPerUnit} to {targetPpu}?\n" +
                        "Note: No undo action for this change",
                        "Yes", "No")) return;

                var prevSize = target.Size;
                var prevPosition = target.Position;
                importer.spritePixelsPerUnit = targetPpu;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
                target.Size = prevSize;
                target.Position = prevPosition;
                
                onPpuChanged?.Invoke();
            };
        }

        public static float GetSpritePPUSetting(GameObject gameObject)
        {
            var renderer = gameObject?.GetComponent<SpriteRenderer>();
            var sprite = renderer ? renderer.sprite : null;
            if (!renderer || !sprite || !sprite.texture) return float.NaN;

            var assetPath = AssetDatabase.GetAssetPath(sprite.texture);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
    
            return importer == null ? float.NaN : importer.spritePixelsPerUnit;
        }
        
        public static bool IsSpriteAssetEditable(TextureImporter importer)
        {
            var assetPath = AssetDatabase.GetAssetPath(importer);
            return assetPath.StartsWith("Assets/");
            
        }
        
        public static void FindSpriteAsset(GameObject target)
        {
            var spritePath = GetSpritePath(target);
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(spritePath);
            EditorGUIUtility.PingObject(asset);
        }
    }
}
