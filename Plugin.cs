using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;
using System.IO;
using System.Linq;

namespace MobileManager
{
    [BepInPlugin("com.shafin.unified.manager", "Mobile Unified Manager", "1.5.2")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _showMenu = false;
        private bool _benchBypass = true;
        private Rect _winRect = new Rect(100, 100, 600, 850);
        private Vector2 _scroll;

        private bool _isDragging = false;
        private Vector2 _lastMouse;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Logger.LogInfo("Unified Manager v1.5.2 - Config loop fixed.");
        }

        void OnGUI()
        {
            GUI.depth = -1001;
            float scale = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1));

            if (GUI.Button(new Rect(20, 20, 220, 75), _showMenu ? "CLOSE MENU" : "MOD MENU"))
            {
                _showMenu = !_showMenu;
            }

            if (!_showMenu) return;

            Vector2 mousePos = new Vector2(Input.mousePosition.x / scale, (Screen.height - Input.mousePosition.y) / scale);
            Rect dragHandle = new Rect(_winRect.x, _winRect.y, _winRect.width, 70);

            if (Input.GetMouseButtonDown(0) && dragHandle.Contains(mousePos))
            {
                _isDragging = true;
                _lastMouse = mousePos;
            }

            if (_isDragging && Input.GetMouseButton(0))
            {
                _winRect.position += (mousePos - _lastMouse);
                _lastMouse = mousePos;
            }

            if (Input.GetMouseButtonUp(0)) _isDragging = false;

            _winRect = GUI.Window(1, _winRect, DrawInterface, "MOBILE UNIFIED MANAGER");
        }

        void DrawInterface(int id)
        {
            _scroll = GUILayout.BeginScrollView(_scroll);

            GUILayout.BeginVertical("box");
            GUILayout.Label("<color=cyan><b>[ SYSTEM TOOLS ]</b></color>");
            _benchBypass = GUILayout.Toggle(_benchBypass, " FORCE BENCH BYPASS (Active)");
            GUILayout.EndVertical();
            GUILayout.Space(10);

            GUILayout.Label("<color=lime><b>[ ACTIVE MODS ]</b></color>");
            
            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                if (plugin.Metadata.GUID == "com.shafin.unified.manager") continue;

                GUILayout.BeginVertical("box");
                GUILayout.Label($"<b>{plugin.Metadata.Name}</b> <color=grey>v{plugin.Metadata.Version}</color>");
                
                // FIXED LOOP: Using Keys to get the entries
                if (plugin.Instance != null && plugin.Instance.Config != null)
                {
                    foreach (var key in plugin.Instance.Config.Keys)
                    {
                        var entry = plugin.Instance.Config[key];
                        if (entry.SettingType == typeof(bool))
                        {
                            entry.BoxedValue = GUILayout.Toggle((bool)entry.BoxedValue, $" {entry.Definition.Key}");
                        }
                    }
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }

            GUILayout.EndScrollView();
            if (GUILayout.Button("HIDE MENU", GUILayout.Height(70))) _showMenu = false;
        }

        void Update()
        {
            if (!_benchBypass) return;

            GameObject gm = GameObject.Find("GameManager");
            if (gm != null) gm.SendMessage("SetAtBench", true, SendMessageOptions.DontRequireReceiver);

            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null)
            {
                pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
                pd.SendMessage("SetBool", new object[] { "canEquip", true }, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
