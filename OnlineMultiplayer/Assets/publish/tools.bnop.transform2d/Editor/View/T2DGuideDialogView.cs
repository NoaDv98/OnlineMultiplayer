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
    public class T2DGuideDialogView : EditorWindow
    {
        private T2DGuide _guide;
        private UnitType _unitType;
        private Action<T2DGuide> _callback;

        public enum Operation
        {
            Create,
            Edit
        }
        
        public static void ShowDialog(Operation operation, T2DGuide guide, Action<T2DGuide> onSubmit)
        {
            var window = CreateInstance<T2DGuideDialogView>();
            window.titleContent = new GUIContent(operation == Operation.Create ? "Create New Guide" : "Edit Guide");
            window._unitType = Transform2D.Model.Settings.UnitType;
            window._guide = new T2DGuide(guide);
            window._callback = onSubmit;
            window.position = new Rect((T2DSceneViewScreen.ScreenRect.width - 250) / 2f,
                (T2DSceneViewScreen.ScreenRect.height - 100) / 2f, 250, 100);
            window.ShowModalUtility(); 
        }

        private void OnGUI()
        {
            var e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    case KeyCode.Escape:
                        Close();
                        break;
                    case KeyCode.Return or KeyCode.KeypadEnter:
                        Submit();
                        break;
                }
            }

            GUILayout.BeginVertical(new GUIStyle()
            {
                padding = new RectOffset(8, 8, 8, 8)
            });

            GUILayout.BeginHorizontal();
            GUILayout.Label("Orientation");
            _guide.Axis = (Axis)GUILayout.Toolbar((int)_guide.Axis, Enum.GetNames(typeof(Axis)));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(4);
            
            var label = _unitType == UnitType.Pixels ? "Position (pixels)" : "Position (units)";
            var pos = _unitType == UnitType.WorldUnits
                ? _guide.Position
                : T2DScreen.ToPixels(_guide.Position);
            GUI.SetNextControlName("PositionField");
            pos = EditorGUILayout.FloatField(label, pos);
            _guide.Position = _unitType == UnitType.WorldUnits ? pos : T2DScreen.ToUnits(pos);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(80)))
            {
                Close();
            }

            if (GUILayout.Button("OK", GUILayout.Width(80)))
            {
                Submit();
            }

            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            
            EditorGUI.FocusTextInControl("PositionField");
        }

        private void Submit()
        {
            _callback?.Invoke(_guide);
            Close();
        }
    }
}
