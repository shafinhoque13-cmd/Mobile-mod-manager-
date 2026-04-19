using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using UnityEngine;

namespace MobileConfigManager
{
    [BepInPlugin("com.shafin.universal.config", "Mobile Config Manager", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _showMenu = false;
        private Vector2 _scrollPos;
        // Medium size window for settings list
        private Rect _winRect = new Rect(50, 50, 600, 700); 

        void OnGUI()
        {
            float scale = Screen.width / 1920f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1));

            // Small Toggle Button to open the manager
            if (GUI.Button(new Rect(Screen.width - 250, 20, 200, 80), "MODS")) _showMenu = !_showMenu;

            if (_showMenu)
            {
                _winRect = GUI.Window(1, _winRect, DrawManager, "Mod Manager (Drag Me)");
            }
        }

        void DrawManager(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 60)); // Drag handle at top

            GUILayout.BeginVertical();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(500));

            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                GUILayout.Label($"<b>{plugin.Metadata.Name}</b>");
                foreach (var key in plugin.Instance.Config.Keys)
                {
                    var entry = plugin.Instance.Config[key];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(entry.Definition.Key, GUILayout.Width(300));
                    if (entry.SettingType == typeof(bool))
                        entry.BoxedValue = GUILayout.Toggle((bool)entry.BoxedValue, "");
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            if (GUILayout.Button("CLOSE", GUILayout.Height(80))) _showMenu = false;
            GUILayout.EndVertical();

            GUI.DragWindow(); // Allow dragging from empty spaces
        }
    }
}
