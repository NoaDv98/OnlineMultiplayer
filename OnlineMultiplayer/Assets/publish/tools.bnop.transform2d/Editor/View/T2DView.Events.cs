//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using tools.bnop.Transform2d.Data;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private void DispatchTransformChange(T2DViewChangeEventType type)
        {
            var e = new T2DViewChangeEvent
            {
                Type = type,
                Data = type switch
                {
                    T2DViewChangeEventType.Position => _data.Transform.Position,
                    T2DViewChangeEventType.Size => _data.Transform.Size,
                    T2DViewChangeEventType.Scale => _data.Transform.Scale,
                    T2DViewChangeEventType.Rotation => _data.Transform.Rotation,
                    _ => (dynamic)null
                }
            };
            Change?.Invoke(e);
        }
        
        private void DispatchSettingsChange()
        {
            var e = new T2DViewChangeEvent
            {
                Type = T2DViewChangeEventType.Settings,
                Data = _data.Settings
            };
            Change?.Invoke(e);
        }

        private void DispatchClickAlign(AlignOptions option)
        {
            var e = new T2DViewClickEvent
            {
                Type = T2DViewClickEventType.Align,
                Data = option
            };
            Click?.Invoke(e);
        }

        private void DispatchClickDistribute(DistributeOptions option, float space = float.NaN)
        {
            var e = new T2DViewClickEvent
            {
                Type = T2DViewClickEventType.Distribute,
                Data = new DistributeData
                {
                    Option = option,
                    Space = space
                }
            };
            Click?.Invoke(e);
        }

        private void DispatchClickOpenSpriteEditor()
        {
            var e = new T2DViewClickEvent
            {
                Type = T2DViewClickEventType.OpenSpriteEditor,
            };
            Click?.Invoke(e);
        }

        private void DispatchClickFindSpriteAsset()
        {
            var e = new T2DViewClickEvent
            {
                Type = T2DViewClickEventType.FindSpriteAsset,
            };
            Click?.Invoke(e);
        }

        private void DispatchClickFixSpritePpu()
        {
            var e = new T2DViewClickEvent
            {
                Type = T2DViewClickEventType.FixSpritePpu,
            };
            Click?.Invoke(e);
        }

        private void DispatchClickFixSpritePivot()
        {
            var e = new T2DViewClickEvent
            {
                Type = T2DViewClickEventType.FixSpritePivot,
            };
            Click?.Invoke(e);
        }
    }
}
