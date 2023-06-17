using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Globalization;

[InitializeOnLoad]
public class ObjectActivationToggle
{
    private static bool HOAEnabled;
    private static bool lastSelectionToggleState;
    private static bool SaveGlobal;
    private static bool ForceEnglish;

    private static int HOAOffset;
    
    private const string HOAEnabledKey = "Hierarchy_Object_Icon_Enabled";
    private const string HOAOffsetKey = "Hierarchy_Object_Icon_Offset";
    private const string SaveGlobalKey = "Hierarchy_Custom_Icon_Global";

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
            if (Event.current.alt && GUI.Toggle(rect, false, "") && Selection.gameObjects.Length > 1)
            {
                MultiObjects(Selection.gameObjects);
            }

            EditorGUI.BeginChangeCheck();

            bool active = GUI.Toggle(rect, GameObject.activeSelf, "");

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(GameObject, "Toggle Active");
                GameObject.SetActive(active);
            }
        }
    }

    private static void MultiObjects(GameObject[] selectedGameObjects)
    {
        if (selectedGameObjects == null || selectedGameObjects.Length == 0)
        {
            return;
        }

        bool newState = !lastSelectionToggleState;

        foreach (GameObject GameObjects in selectedGameObjects)
        {
            Undo.RecordObject(GameObjects, "Toggle Active");
            GameObjects.SetActive(newState);
        }

        lastSelectionToggleState = newState;
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
            string SaveGlobalText = "Apply positions to all project";
            
            if (ForceEnglish == false && lang == "ja-JP")
            {
                HOAEnableText = "Hierarchy Object Active を有効にする";
                HOAOffsetText = "アイコンの位置を調整する";
                SaveGlobalText = "全プロジェクトに配置を適用する";
            }
            
            HOAEnabled = EditorGUILayout.Toggle(HOAEnableText, HOAEnabled);
            HOAOffset = EditorGUILayout.IntField(HOAOffsetText, HOAOffset);
            SaveGlobal = EditorGUILayout.Toggle(SaveGlobalText, SaveGlobal);

            SaveSettings();
        }
    }

    // 設定を読み込み
    private static void LoadSettings()
    {
        HOAEnabled = EditorUserSettings.GetConfigValue(HOAEnabledKey) == "1";

        SaveGlobal = EditorPrefs.GetBool(SaveGlobalKey);
        if (!SaveGlobal)
        {
            SaveGlobal = EditorUserSettings.GetConfigValue(SaveGlobalKey) == "1";
        }
        ForceEnglish = EditorPrefs.GetBool("Praecipua_English");

        // SetGlobal が True のときは、全プロジェクト共通の設定ファイルから読み込み
        if (SaveGlobal == true)
        {
            HOAOffset = EditorPrefs.GetInt(HOAOffsetKey);
        }
        // SetGlobal が False のときは、プロジェクト内の設定ファイルから読み込み
        else
        {
            string HOAOffsetStr = EditorUserSettings.GetConfigValue(HOAOffsetKey);
            HOAOffset = HOAOffsetStr == null ? 0 : System.Int32.Parse(HOAOffsetStr);
        }
    }

    // 設定を保存
    private static void SaveSettings()
    {
        // SetGlobal が True のときは、全プロジェクト共通の設定ファイルに保存
        if (SaveGlobal == true)
        {
            EditorPrefs.SetInt(HOAOffsetKey, HOAOffset);

        }
        // SetGlobal の値にかかわらず、プロジェクト内の設定ファイルにも保存
        EditorUserSettings.SetConfigValue(HOAEnabledKey, HOAEnabled ? "1" : "0");
        EditorUserSettings.SetConfigValue(HOAOffsetKey, HOAOffset.ToString());
    }
}
