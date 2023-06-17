using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Globalization;

[InitializeOnLoad]
public class ObjectActivationToggle
{
    private static bool HOAEnabled;

    private static int HOAOffset;
    
    private const string HOAEnabledKey = "Hierarchy_Object_Icon_Enabled";
    private const string HOAOffsetKey = "Hierarchy_Object_Icon_Offset";
    private static bool ForceEnglish;

    static ObjectActivationToggle()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;

        LoadSettings();
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject GameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (GameObject == null)
        {
            return;
        }

        var rect = new Rect(selectionRect);
        rect.x = selectionRect.xMax - 16 - (HOAOffset * 16);
        rect.y = selectionRect.yMin;
        rect.width = 16;
        rect.height = 16;

        if (HOAEnabled)
        {
        EditorGUI.BeginChangeCheck();

        bool active = GUI.Toggle(rect, GameObject.activeSelf, "");

        if (active != GameObject.activeSelf)
        {
            GameObject.SetActive(active);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(GameObject, "Toggle Active");
            GameObject.SetActive(active);
        }
        }
    }

    // Project Settings に設定画面を表示
    public class HierarchyObjectActiveSettingsProvider : SettingsProvider
    {
        private const string SettingPath = "Project/Praecipua/Hierarchy Object Active";

        public HierarchyObjectActiveSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
            : base(path, scopes)
        {
        }
        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new HierarchyObjectActiveSettingsProvider(SettingPath, SettingsScope.Project);
            return provider;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            LoadSettings();
        }

        public override void OnGUI(string searchContext)
        {
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string lang = ci.Name;

            string HOAEnableText = "Enable Hierarchy Object Active";
            string HOAOffsetText = "Icon Offset";
            
            if (ForceEnglish == false && lang == "ja-JP")
            {
                HOAEnableText = "Hierarchy Object Active を有効にする";
                HOAOffsetText = "アイコンの位置を調整する";
            }
            
            HOAEnabled = EditorGUILayout.Toggle(HOAEnableText, HOAEnabled);
            HOAOffset = EditorGUILayout.IntField(HOAOffsetText, HOAOffset);

            SaveSettings();
        }
    }

    // 設定を読み込み
    private static void LoadSettings()
    {
        HOAEnabled = EditorUserSettings.GetConfigValue(HOAEnabledKey) == "1";
        string HOAOffsetStr = EditorUserSettings.GetConfigValue(HOAOffsetKey);
        HOAOffset = HOAOffsetStr == null ? 0 : System.Int32.Parse(HOAOffsetStr);

        ForceEnglish = EditorPrefs.GetBool("Praecipua_English");
    }

    // 設定を保存
    private static void SaveSettings()
    {
        EditorUserSettings.SetConfigValue(HOAEnabledKey, HOAEnabled ? "1" : "0");
        EditorUserSettings.SetConfigValue(HOAOffsetKey, HOAOffset.ToString());
    }
}
