using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace MobileManager
{
    [BepInPlugin("com.shafin.unified.manager", "Mobile Unified Manager", "1.5.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _showMenu = false;
        private bool _benchBypass = true;
        private Rect _winRect = new Rect(100, 100, 600, 800);
        private Vector2 _scroll;

        // Dragging
        private bool _isDragging = false;
        private Vector2 _lastMouse;

        void Awake()
        {
            // This prevents the mod from disappearing when moving from Menu to Game
            DontDestroyOnLoad(this.gameObject);
            Logger.LogInfo("Unified Manager Loaded: Path Detection Active.");
        }

        void OnGUI()
        {
            GUI.depth = -1001;
            float s = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1));

            // Main Toggle Button (Stays visible)
            if (GUI.Button(new Rect(20, 20, 200, 80), _showMenu ? "CLOSE UI" : "MOD MENU"))
            {
                _showMenu = !_showMenu;
            }

            if (!_showMenu) return;

            // --- Manual Drag Logic ---
            Vector2 mouse = new Vector2(Input.mousePosition.x / s, (Screen.height - Input.mousePosition.y) / s);
            if (Input.GetMouseButtonDown(0) && new Rect(_winRect.x, _winRect.y, _winRect.width, 60).Contains(mouse))
                _isDragging = true;
            
            if (_isDragging && Input.GetMouseButton(0))
            {
                _winRect.position += (mouse - _lastMouse);
            }
            if (Input.GetMouseButtonUp(0)) _isDragging = false;
            _lastMouse = mouse;
            // -------------------------

            _winRect = GUI.Window(1, _winRect, DrawInterface, "MOD MANAGER v1.5");
        }

        void DrawInterface(int id)
        {
            _scroll = GUILayout.BeginScrollView(_scroll);

            // SECTION 1: THE BRIDGE (Bench Bypass)
            GUILayout.Label("<color=cyan><b>[ SYSTEM TOOLS ]</b></color>");
            _benchBypass = GUILayout.Toggle(_benchBypass, " FORCE BENCH BYPASS (Active)");
            GUILayout.Space(10);

            // SECTION 2: FOLDER DETECTION
            GUILayout.Label("<color=yellow><b>[ FOLDER DETECTION ]</b></color>");
            GUILayout.Label($"Plugins: {Path.Combine(Paths.BepInExRootPath, "plugins")}");
            GUILayout.Label($"Configs: {Path.Combine(Paths.BepInExRootPath, "config")}");
            
            if (GUILayout.Button("REFRESH FOLDERS", GUILayout.Height(40))) {
                // Trigger BepInEx to look for new files
                Chainloader.Initialize(); 
            }
            GUILayout.Space(10);

            // SECTION 3: DETECTED MODS
            GUILayout.Label("<color=lime><b>[ DETECTED MODS ]</b></color>");
            foreach (var p in Chainloader.PluginInfos.Values)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label($"<b>{p.Metadata.Name}</b> (v{p.Metadata.Version})");
                foreach (var configKey in p.Instance.Config.Keys)
                {
                    var entry = p.Instance.Config[configKey];
                    if (entry.SettingType == typeof(bool))
                        entry.BoxedValue = GUILayout.Toggle((bool)entry.BoxedValue, $" {entry.Definition.Key}");
                }
                GUILayout.EndVertical();
            }

            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, 10000, 60));
        }

        void Update()
        {
            if (!_benchBypass) return;

            // Every frame, we tell the game we are at a bench
            GameObject gm = GameObject.Find("GameManager");
            if (gm != null) gm.SendMessage("SetAtBench", true, SendMessageOptions.DontRequireReceiver);

            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null) {
                pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
                pd.SendMessage("SetBool", new object[] { "canEquip", true }, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
