//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEditor;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private void RenderWarnings()
        {
            RenderUnsupportedWarning();
            RenderCameraBoundsWarning();
            RenderTextBoundsWarning();
            RenderPpuWarning();
            RenderPixelPerfectWarning();
        }
        
        private void RenderUnsupportedWarning()
        {
            if (_data.HasPrefab)
            {
                Space(6);
                Message("Operations on Prefab assets in the project panel are currently not supported",
                    MessageType.Info
                );
            }
            else if (_data.HasNesting)
            {
                Space(6);
                Message("Operations on nested selection (parent and child) are currently not supported",
                    MessageType.Info
                );
            }
        }

        private void RenderCameraBoundsWarning()
        {
            if (!_data.HasCamera) return;
            
            if (_data.IsSingleSelection)
            {
                Space(6);
                Message("Using Camera Bounds", MessageType.Info);
            }
            else
            {
                Space(6);
                var count = _data.CameraCount;
                var pluralSuffix = count > 1 ? "s" : "";
                Message(
                    $"Using Camera Bounds for {count} Game Object{pluralSuffix}",
                    MessageType.Info);
            }
        }

        private void RenderTextBoundsWarning()
        {
            if (!_data.HasText) return;
            
            if (_data.IsSingleSelection)
            {
                Space(6);
                Message("Using Text Bounds", MessageType.Info);
            }
            else
            {
                Space(6);
                var count = _data.TextCount;
                var pluralSuffix = count > 1 ? "s" : "";
                Message(
                    $"Using Text Bounds for {count} Game Object{pluralSuffix}",
                    MessageType.Info);
            }
        }

        private void RenderPpuWarning()
        {
            if (!_data.HasRenderer || _data.IsMatchingPPU) return;

            Space(6);

            if (_data.IsSingleSelection)
                Message("Sprite PPU is different than the project PPU set in Preferences.",
                    MessageType.Warning,
                    "Fix",
                    DispatchClickFixSpritePpu
                );
            else
                Message("One or more sprites have different PPU than the project PPU set in Preferences.",
                    MessageType.Warning);
        }

        private void RenderPixelPerfectWarning()
        {
            if (_data.PivotIsPixelPerfect || _data.SpriteIsDownscaled) return;

            Space(6);
            Message("Non-rounded sprite pivot detected. Snap to Pixel will not function properly.",
                MessageType.Warning,
                "Fix",
                DispatchClickFixSpritePivot
            );
        }
    }
}
