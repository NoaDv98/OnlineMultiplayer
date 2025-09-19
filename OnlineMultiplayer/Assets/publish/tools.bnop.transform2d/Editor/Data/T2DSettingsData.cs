//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

namespace tools.bnop.Transform2d.Data
{
    public struct T2DSettingsData
    {
        public bool IsEnabled;
        public bool SnapToPixel;
        public bool ShowGuides;
        public bool SnapToGuidesIfShown;
        public bool ConstrainProportions;
        public float ProjectPpu;
        public bool PreferencesPanelIsOpen;
        public bool SpriteInfoPanelIsOpen;
        public bool AlignPanelIsOpen;
        public bool DistributePanelIsOpen;
        public PivotType PivotType;
        public UnitType UnitType;
        public UnitSpace UnitSpace;
        public BoundsSource BoundsSource;

        public bool SnapToGuides => SnapToGuidesIfShown && ShowGuides;
        public bool Snapping => SnapToPixel || SnapToGuides;
        
        public static T2DSettingsData Default = new()
        {
            IsEnabled = true,
            PivotType = PivotType.Center,
            UnitType = UnitType.Pixels,
            UnitSpace = UnitSpace.Global,
            BoundsSource = BoundsSource.None,
            ProjectPpu = 100,
            PreferencesPanelIsOpen = true,
            ShowGuides = true,
            SnapToGuidesIfShown = true
        };
        
        public const int FloatPrecisionPixels = 2;
        public const int FloatPrecisionUnits = 6;
        public const float SnapToGuidesForce = 0.1f;
    }
}
