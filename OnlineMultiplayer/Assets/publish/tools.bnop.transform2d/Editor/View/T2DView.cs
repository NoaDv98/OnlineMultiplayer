//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using tools.bnop.Transform2d.Data;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        internal Action<T2DViewChangeEvent> Change;
        internal Action<T2DViewClickEvent> Click;
        internal Action RepaintRequest;

        private T2DViewData _data;

        public T2DView()
        {
            LoadResources();
        }
        

        internal T2DViewData Data
        {
            set => _data = value;
        }

        public void Render()
        {
            if (!_data.Settings.IsEnabled)
                RenderDisabledMode();
            
            else if (!_data.Is2DMode)
                Render3DMode();
            
            else
            {
                EditorGUIUtility.wideMode = true;
                EditorGUIUtility.SetIconSize(Vector2.one * 16);
                EditorGUIUtility.labelWidth = Mathf.Max(
                    EditorGUIUtility.currentViewWidth * StyleProperties.LabelWidthNormalized,
                    StyleProperties.LabelWidthMin);

                RenderToolbar();
                ScrollView(() =>
                {
                    RenderSelection();
                    IndentedBorderedSection(() =>
                    {
                        RenderTransform();
                        RenderWarnings();
                    });
                    RenderAlign();
                    RenderDistribute();
                    RenderSettings();
                    RenderSpriteInfo();
                });
            }

            if (Transform2D.ShowDebug)
                RenderDebug();
        }
    }
}
