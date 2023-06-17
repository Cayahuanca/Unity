using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Globalization;

[InitializeOnLoad]
public static class CustomIconInHierarchy
{
    private static Color backgroundColor;

    private static bool HCIEnabled;
    private static bool SaveGlobal;
    private static bool ForceEnglish;

    private static int HCIOffset;
    
    private const string HCIEnabledKey = "Hierarchy_Custom_Icon_Enabled";
    private static string HCIOffsetKey = "Hierarchy_Custom_Icon_Offset";
    private const string SaveGlobalKey = "Hierarchy_Custom_Icon_Global";

    private const string SettingPath = "Project/Praecipua/Hierarchy Custom Icon";

    static CustomIconInHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        LoadSettings();
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect rect)
    {
        
        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if (!(obj is GameObject go))
        {
            return;
        }

        var icon = EditorGUIUtility.ObjectContent(go, go.GetType()).image as Texture2D;
        var color = EditorPrefs.GetInt("Obj" + instanceID + "Color", -1);

        rect.x = 32 + (HCIOffset * 16);;
        rect.y += 1;
        rect.width = 14;
        rect.height = 14;

        var backgroundColor = EditorGUIUtility.isProSkin ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255);
        if (HCIEnabled)
        {
            if (!EditorGUIUtility.isProSkin)
            {
                if (icon != null && icon.name != "GameObject Icon" && icon.name != "Prefab Icon" && icon.name != "PrefabModel Icon" && icon.name != "PrefabVariant Icon")
                {

                    EditorGUI.DrawRect(rect, backgroundColor);
                    GUI.DrawTexture(rect, icon);

                }
            }
            else
                if (icon != null && icon.name != "d_GameObject Icon" && icon.name != "d_Prefab Icon" && icon.name != "d_PrefabModel Icon" && icon.name != "d_PrefabVariant Icon")
                {
                    EditorGUI.DrawRect(rect, backgroundColor);
                    GUI.DrawTexture(rect, icon);
                }
        }    
    }

    // Project Settings に設定画面を表示
    public class HierarchyCustomtIconSettingsProvider : SettingsProvider
    {
        public HierarchyCustomtIconSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
            : base(path, scopes)
            {
            }

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new HierarchyCustomtIconSettingsProvider(SettingPath, SettingsScope.Project);
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

            string HCIEnableText = "Enable Hierarchy Object Custom Icon";
            string HCIOffsetText = "Icon Offset";
            
            if (ForceEnglish == false && lang == "ja-JP")
            {
                HCIEnableText = "ヒエラルキーに、オブジェクトのアイコンを表示する";
                HCIOffsetText = "アイコンの位置を調整する";
            }

            HCIEnabled = EditorGUILayout.Toggle(HCIEnableText, HCIEnabled);
            HCIOffset = EditorGUILayout.IntField(HCIOffsetText, HCIOffset);

            //SaveGlobal = EditorGUILayout.Toggle(SaveGlobalText, SaveGlobal);

            SaveSettings();
        }
    }

    private static void LoadSettings()
    {
        HCIEnabled = EditorUserSettings.GetConfigValue(HCIEnabledKey) == "1";
        string HCIOffsetStr = EditorUserSettings.GetConfigValue(HCIOffsetKey);
        HCIOffset = HCIOffsetStr == null ? 0 : System.Int32.Parse(HCIOffsetStr);

        SaveGlobal = EditorPrefs.GetBool(SaveGlobalKey);
        if (!SaveGlobal)
        {
            SaveGlobal = EditorUserSettings.GetConfigValue(SaveGlobalKey) == "1";
        }
        ForceEnglish = EditorPrefs.GetBool("Praecipua_English");

        // SetGlobal が True のときは、全プロジェクト共通の設定ファイルから読み込み
        if (SaveGlobal == true)
        {

        }
        // SetGlobal が False のときは、プロジェクト内の設定ファイルから読み込み
        else
        {

        }
    }

    // 設定を保存
    private static void SaveSettings()
    {
        // SetGlobal が True のときは、全プロジェクト共通の設定ファイルに保存
        if (SaveGlobal == true)
        {
        }
        EditorUserSettings.SetConfigValue(HCIEnabledKey, HCIEnabled ? "1" : "0");
        EditorUserSettings.SetConfigValue(HCIOffsetKey, HCIOffset.ToString());
    }
}