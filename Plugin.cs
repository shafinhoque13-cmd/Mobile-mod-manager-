using BepInEx;
using BepInEx.Bootstrap;
using UnityEngine;

namespace MobileConfigManager
{
    [BepInPlugin("com.shafin.universal.config", "MobileConfigManager", "1.4.4")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _show = false;
        private Rect _winRect = new Rect(300, 100, 500, 600);
        private Vector2 _scroll;
        private bool _dragging = false;
        private Vector2 _lastMouse;

        void OnGUI()
        {
            GUI.depth = -1001;
            float s = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1));

            if (GUI.Button(new Rect(Screen.width/s - 210, 10, 200, 60), "SETTINGS")) _show = !_show;

            if (!_show) return;

            // Drag Logic
            Vector2 mouse = new Vector2(Input.mousePosition.x / s, (Screen.height - Input.mousePosition.y) / s);
            if (Input.GetMouseButtonDown(0) && new Rect(_winRect.x, _winRect.y, _winRect.width, 50).Contains(mouse)) _dragging = true;
            if (_dragging && Input.GetMouseButton(0)) _winRect.position += (mouse - _lastMouse);
            if (Input.GetMouseButtonUp(0)) _dragging = false;
            _lastMouse = mouse;

            _winRect = GUI.Window(1, _winRect, (id) => {
                _scroll = GUILayout.BeginScrollView(_scroll);
                foreach (var p in Chainloader.PluginInfos.Values)
                {
                    GUILayout.Label($"<b>{p.Metadata.Name}</b>");
                    foreach (var key in p.Instance.Config.Keys)
                    {
                        var entry = p.Instance.Config[key];
                        if (entry.SettingType == typeof(bool))
                            entry.BoxedValue = GUILayout.Toggle((bool)entry.BoxedValue, $" {entry.Definition.Key}");
                    }
                }
                GUILayout.EndScrollView();
                if (GUILayout.Button("CLOSE", GUILayout.Height(50))) _show = false;
            }, "Mod Settings");
        }
    }
}
