//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using tools.bnop.Transform2d.Data;
using tools.bnop.Transform2d.Service;
using tools.bnop.Transform2d.View;
using UnityEditor;

namespace tools.bnop.Transform2d
{
    public static class Transform2D
    {
        public static readonly string Name = "Transform2D";
        public static readonly bool ShowDebug = false;

        private static T2DEditorWindow _editorEditorWindow;
        public static T2DView View;
        public static T2DModel Model;
        public static T2DGuidesView GuidesView;

        public static bool Initialized { get; private set; }

        public static T2DEditorWindow EditorWindow
        {
            get => _editorEditorWindow;
            set
            {
                if (value != null)
                {
                    if (_editorEditorWindow) EditorWindow = null;
                    
                    _editorEditorWindow = value;
                    _editorEditorWindow.wantsMouseMove = true;
                    if (View != null)
                        View.RepaintRequest += _editorEditorWindow.Repaint;
                }
                else
                {
                    if (_editorEditorWindow && View != null)
                        View.RepaintRequest -= _editorEditorWindow.Repaint;

                    _editorEditorWindow = null;
                }
            }
        }

        public static void Init()
        {
            if (Initialized) return;
            Initialized = true;

            
            T2DMigrations.DeleteObsoleteFiles();

            View = new T2DView();
            View.Click += OnViewClick;
            View.Change += OnViewChange;


            GuidesView = new T2DGuidesView();
            GuidesView.Change += T2DGuides.OnGuidesViewChange;

            Model = new T2DModel();
            Model.Change += OnModelChange;
            Model.Init();
            
            SceneView.duringSceneGui += OnSceneGUI;

            T2DGuides.Init();
        }

        private static void OnViewChange(T2DViewChangeEvent viewChange)
        {
            T2DConsole.Log("OnViewChange: " + viewChange.Type);

            switch (viewChange.Type)
            {
                case T2DViewChangeEventType.Position:
                    Model.SetPosition(viewChange.Data);
                    break;
                case T2DViewChangeEventType.Size:
                    Model.SetSize(viewChange.Data);
                    break;
                case T2DViewChangeEventType.Scale:
                    Model.SetScale(viewChange.Data);
                    break;
                case T2DViewChangeEventType.Rotation:
                    Model.SetRotation(viewChange.Data);
                    break;
                case T2DViewChangeEventType.Settings:
                    Model.Settings = viewChange.Data;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void OnViewClick(T2DViewClickEvent viewClick)
        {
            T2DConsole.Log("OnViewClick: " + viewClick.Type);

            switch (viewClick.Type)
            {
                case T2DViewClickEventType.Align:
                    Model.Selection.Align(viewClick.Data);
                    break;
                case T2DViewClickEventType.Distribute:
                    Model.Selection.Distribute(viewClick.Data);
                    break;
                case T2DViewClickEventType.FixSpritePpu:
                    Model.FixSpriteAssetPpu();
                    break;
                case T2DViewClickEventType.FixSpritePivot:
                    Model.FixSpriteAssetPivot();
                    break;
                case T2DViewClickEventType.OpenSpriteEditor:
                    EditorApplication.ExecuteMenuItem("Window/2D/Sprite Editor");
                    break;
                case T2DViewClickEventType.FindSpriteAsset:
                    T2DDSpriteAsset.FindSpriteAsset(Model.Selection.First?.Target);
                    break;
            }
        }

        private static void OnModelChange()
        {
            T2DConsole.Log("OnModelChange");

            T2DScreen.PPU = Model.Settings.ProjectPpu;

            var viewData = new T2DViewData
            {
                Is2DMode = Model.Is2DMode,
                Settings = Model.Settings
            };

            if (Model.Settings.IsEnabled)
            {
                viewData.ObjectCount = Model.Selection.Count;
                viewData.HasPrefab = Model.Selection.HasPrefab;
                viewData.HasNesting = Model.Selection.HasNesting;
                viewData.ActiveGameObjectName = Model.Selection.IsEmpty ? "" : Model.Selection.First.Target.name;
                viewData.Transform = Model.Transform;
                viewData.UnscaledSize = Model.Selection.UnscaledSize;
                viewData.SpritePivot = Model.SpritePivot;
                viewData.SpritePPU = Model.SpritePpu;
                viewData.SpritePPUSetting = Model.SpritePPUSetting;
                viewData.SpriteIsDownscaled = Model.SpriteIsDownscaled;
                viewData.ActiveBoundsSource = Model.Selection.IsEmpty ? Model.Settings.BoundsSource : Model.Selection.BoundsSource;
                viewData.HasRenderer = Model.HasRenderer;
                viewData.HasCollider = Model.HasCollider;
                viewData.HasText = Model.HasText;
                viewData.CameraCount = Model.CameraCount;
                viewData.TextCount = Model.TextCount;
                viewData.IsMatchingPPU = Model.IsMatchingPPU;
                viewData.PivotIsPixelPerfect = Model.IsPivotPixelPerfect;
            }

            View.Data = viewData;
            EditorWindow?.Repaint();

            SceneView.lastActiveSceneView?.Repaint();
        }

        internal static void OnGUI()
        {
            if (!Initialized) return;

            View.Render();

        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!Model.Settings.IsEnabled || !Model.Is2DMode) return;

            if (!Model.Settings.ShowGuides) return;
            GuidesView.Data = new T2DGuidesViewData
            {
                Guides = T2DGuides.ListAll,
                UnitType = Model.Settings.UnitType,
                SelectionBounds = Model.Selection.IsEmpty ? null : Model.Selection.Bounds
            };
            GuidesView.Render();
        }

        public static void Destroy()
        {
            if (!Initialized) return;
            Initialized = false;

            SceneView.duringSceneGui -= OnSceneGUI;
            
            EditorWindow = null;

            View.Click -= OnViewClick;
            View.Change -= OnViewChange;
            Model.Change -= OnModelChange;

            GuidesView.Change -= T2DGuides.OnGuidesViewChange;

            Model.Destroy();

            Model = null;
            View = null;
            GuidesView = null;
        }

        internal static void ToggleEnabled()
        {
            var settings = Model.Settings;
            settings.IsEnabled = !settings.IsEnabled;
            Model.Settings = settings;
            OnModelChange();
        }
    }
}
