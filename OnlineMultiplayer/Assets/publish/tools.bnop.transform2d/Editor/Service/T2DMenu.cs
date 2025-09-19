//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.Service
{
    public static class T2DMenu
    {
        private const string MenuPath = "Tools/Transform2D/";
        private const string MenuPathOpen = MenuPath + "Open Transform2D";
        private const string MenuPathEnable = MenuPath + "Enable Transform2D";
        private const string MenuPathManual = MenuPath + "Open User Manual";
        private const string MenuPathDiscord = MenuPath + "Join our Discord";
        
        private const string UserManualUrl = "https://drive.google.com/file/d/1lfEp7GIBcvIbFINMgga0nBUQe0U_AaWW/view?usp=drive_link";
        private const string DiscordUrl = "https://discord.gg/TKvmCU9rxV";
        
        [MenuItem(MenuPathOpen, false, 1)]
        private static void Open()
        {
            T2DEditorWindow.ShowWindow();
        }

        
        [MenuItem(MenuPathManual, false, 1000)]
        private static void JoinDiscord()
        {
            Application.OpenURL(UserManualUrl);
        }
        
        [MenuItem(MenuPathDiscord, false, 1001)]
        private static void OpenManual()
        {
            Application.OpenURL(DiscordUrl);
        }
        
        
        [MenuItem(MenuPathEnable, true)]
        private static bool ValidateEnabled()
        {
            if (Transform2D.Model == null)
                return false;

            Menu.SetChecked(MenuPathEnable, Transform2D.Model.Settings.IsEnabled);
            return true;
        }

        [MenuItem(MenuPathEnable, false, 2000)]
        private static void ToggleEnabled()
        {
            Transform2D.ToggleEnabled();
        }
    }
}
