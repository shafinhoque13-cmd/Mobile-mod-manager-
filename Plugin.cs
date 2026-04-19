using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using UnityEngine;

namespace MobileConfigManager
{
    [BepInPlugin("com.shafin.universal.config", "Mobile Config Manager", "1.3.1")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _showMenu = false;
        private Vector2 _scrollPos;
        private Rect _winRect = new Rect(50, 50, 600, 750); 

        void Awake()
        {
            Logger.LogInfo("!!! MOBILE CONFIG MANAGER AWAKE !!!");
        }

        void OnGUI()
        {
            // Forces the menu to the front
            GUI.depth = -1001; 

            float scale = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1));

            // Small button in the top right to open/close
            if (GUI.Button(new Rect(Screen.width / scale - 220, 20, 200, 80), "MOD SETTINGS")) 
            {
                _showMenu = !_showMenu;
            }

            if (_showMenu)
            {
                _winRect = GUI.Window(1, _winRect, DrawManager, "Mod Manager (Drag Handle Top)");
            }
        }

        void DrawManager(int windowID)
        {
            // Make the window draggable
            GUI.DragWindow(new Rect(0, 0, 10000, 60));

            GUILayout.BeginVertical();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(600));

            // Automatically list all loaded mods and their settings
            foreach (var pluginInfo in Chainloader.PluginInfos.Values)
            {
                GUILayout.Space(10);
                GUILayout.Label($"<b><color=yellow>[ {pluginInfo.Metadata.Name} ]</color></b>");
                
                foreach (var configKey in pluginInfo.Instance.Config.Keys)
                {
                    var entry = pluginInfo.Instance.Config[configKey];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(entry.Definition.Key, GUILayout.Width(350));
                    
                    if (entry.SettingType == typeof(bool))
                    {
                        entry.BoxedValue = GUILayout.Toggle((bool)entry.BoxedValue, "");
                    }
                    else
                    {
                        GUILayout.Label(entry.BoxedValue.ToString(), GUILayout.Width(100));
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            if (GUILayout.Button("CLOSE", GUILayout.Height(80))) _showMenu = false;
            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }
}
