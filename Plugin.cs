using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap; // Needed to see other mods
using UnityEngine;
using System.Collections.Generic;

namespace MobileConfigManager
{
    [BepInPlugin("com.shafin.universal.config", "Mobile Config Manager", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _showMenu = false;
        private Vector2 _scrollPos;
        private Rect _windowRect = new Rect(100, 100, 800, 1000);

        void OnGUI()
        {
            // Scale UI for high-res mobile screens
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 1920f, Screen.height / 1080f, 1));

            if (GUI.Button(new Rect(50, 50, 250, 80), "MOD SETTINGS"))
            {
                _showMenu = !_showMenu;
            }

            if (_showMenu)
            {
                _windowRect = GUI.Window(0, _windowRect, DrawManager, "Universal Mod Manager");
            }
        }

        void DrawManager(int windowID)
        {
            GUILayout.BeginVertical();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Width(750), GUILayout.Height(850));

            // Automatically find every mod installed on your phone
            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                GUILayout.Label($"<b><color=cyan>[ {plugin.Metadata.Name} ]</color></b>", GUILayout.Height(50));
                
                // Show every setting for this specific mod
                foreach (var entry in plugin.Instance.Config.Keys)
                {
                    DrawSetting(plugin.Instance.Config[entry]);
                }
                GUILayout.Space(30);
            }

            GUILayout.EndScrollView();
            if (GUILayout.Button("CLOSE MENU", GUILayout.Height(100))) _showMenu = false;
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        void DrawSetting(ConfigEntryBase entry)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(entry.Definition.Key, GUILayout.Width(400));

            // Handles Toggle switches for Bench Bypass etc.
            if (entry.SettingType == typeof(bool))
            {
                entry.BoxedValue = GUILayout.Toggle((bool)entry.BoxedValue, "", GUILayout.Width(50), GUILayout.Height(50));
            }
            // Handles numbers (Speed, Damage, etc.)
            else
            {
                GUILayout.Label(entry.BoxedValue.ToString(), GUILayout.Width(150));
            }
            
            GUILayout.EndHorizontal();
        }
    }
}
