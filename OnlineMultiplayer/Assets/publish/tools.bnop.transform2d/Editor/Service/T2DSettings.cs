//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using tools.bnop.Transform2d.Data;
using UnityEditor;
using UnityEditor.SettingsManagement;

namespace tools.bnop.Transform2d.Service
{
    /// See https://docs.unity.cn/Packages/com.unity.settings-manager@2.0
    public static class T2DSettings
    {
        private const string PackageName = "co.bnop.Transform2D";

        private static Settings _instance;
        private static Settings Instance => _instance ??= new Settings(PackageName);

        public static void Save(T2DSettingsData data)
        {
            Set("IsEnabled", data.IsEnabled);
            Set("SnapToPixel", data.SnapToPixel);
            Set("ShowGuides", data.ShowGuides);
            Set("SnapToGuidesIfShown", data.SnapToGuidesIfShown);
            Set("ConstrainProportions", data.ConstrainProportions);
            Set("ProjectPpu", data.ProjectPpu);
            Set("PreferencesPanelIsOpen", data.PreferencesPanelIsOpen);
            Set("SpriteInfoPanelIsOpen", data.SpriteInfoPanelIsOpen);
            Set("AlignPanelIsOpen", data.AlignPanelIsOpen);
            Set("DistributePanelIsOpen", data.DistributePanelIsOpen);
            Set("PivotType", data.PivotType);
            Set("UnitType", data.UnitType);
            Set("UnitSpace", data.UnitSpace);
            Set("BoundsSource", data.BoundsSource);
            Instance.Save();
            T2DConsole.Log("User prefs saved");
        }

        public static T2DSettingsData Load()
        {
            var data = new T2DSettingsData
            {
                IsEnabled = Get("IsEnabled", T2DSettingsData.Default.IsEnabled),
                SnapToPixel = Get("SnapToPixel", T2DSettingsData.Default.SnapToPixel),
                ShowGuides = Get("ShowGuides", T2DSettingsData.Default.ShowGuides),
                SnapToGuidesIfShown = Get("SnapToGuides", T2DSettingsData.Default.SnapToGuidesIfShown),
                ConstrainProportions = Get("ConstrainProportions", T2DSettingsData.Default.ConstrainProportions),
                ProjectPpu = Get("ProjectPpu", T2DSettingsData.Default.ProjectPpu),
                PreferencesPanelIsOpen = Get("PreferencesPanelIsOpen", T2DSettingsData.Default.PreferencesPanelIsOpen),
                SpriteInfoPanelIsOpen = Get("SpriteInfoPanelIsOpen", T2DSettingsData.Default.SpriteInfoPanelIsOpen),
                AlignPanelIsOpen = Get("AlignPanelIsOpen", T2DSettingsData.Default.AlignPanelIsOpen),
                DistributePanelIsOpen = Get("DistributePanelIsOpen", T2DSettingsData.Default.DistributePanelIsOpen),
                PivotType = Get("PivotType", T2DSettingsData.Default.PivotType),
                UnitType = Get("UnitType", T2DSettingsData.Default.UnitType),
                UnitSpace = Get("UnitSpace", T2DSettingsData.Default.UnitSpace),
                BoundsSource = Get("BoundsSource", T2DSettingsData.Default.BoundsSource)
            };
            T2DConsole.Log("User prefs loaded");
            return data;
        }

        private static T Get<T>(string key, T fallback = default(T), SettingsScope scope = SettingsScope.Project)
        {
            return Instance.Get<T>(key, scope, fallback);
        }

        private static void Set<T>(string key, T value, SettingsScope scope = SettingsScope.Project)
        {
            Instance.Set<T>(key, value, scope);
        }

        public static bool ContainsKey<T>(string key, SettingsScope scope = SettingsScope.Project)
        {
            return Instance.ContainsKey<T>(key, scope);
        }
    }
}
