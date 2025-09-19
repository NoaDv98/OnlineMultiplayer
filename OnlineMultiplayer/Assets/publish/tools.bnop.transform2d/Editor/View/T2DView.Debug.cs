//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using tools.bnop.Transform2d.Service;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private void RenderDebug()
        {
            if (GUILayout.Button("Test 1")) Test1();
            if (GUILayout.Button("Test 2")) Test2();
        }

        private void Test1()
        {
            T2DConsole.Log("Test 1");
        }
        
        private void Test2()
        {
            T2DConsole.Log("Test 2");
        } 
    }
}
