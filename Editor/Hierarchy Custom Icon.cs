using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Globalization;

[InitializeOnLoad]
public static class CustomIconInHierarchy
{
    private static Color backgroundColor;

    private static bool HCIEnabled;
    private static bool HCICenter;
    private static bool SaveGlobal;
    private static bool ForceEnglish;

    private static int HCIOffset;
    
    private const string HCIEnabledKey = "Hierarchy_Custom_Icon_Enabled";
    private const string HCICenterKey = "Hierarchy_Custom_Icon_Center";
    private static string HCIOffsetKey = "Hierarchy_Custom_Icon_Offset";
    private const string SaveGlobalKey = "Hierarchy_Custom_Icon_Global";

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

        if (HCICenter)
        {
            rect.x = rect.xMin + (HCIOffset * 16);
        }
        else
        {
            rect.x = 32 + (HCIOffset * 16);
        }
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
        private const string SettingPath = "Project/Praecipua/Hierarchy Custom Icon";

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
            string HCICenterText = "Superimpose the icon position on the default icon position";
            string HCIOffsetText = "Icon Offset";
            string SaveGlobalText = "Apply positions to all project";
            
            if (ForceEnglish == false && lang == "ja-JP")
            {
                HCIEnableText = "ヒエラルキーに、オブジェクトのアイコンを表示する";
                HCICenterText = "アイコンの位置をデフォルトのアイコンの位置に合わせる";
                HCIOffsetText = "アイコンの位置を調整する";
                SaveGlobalText = "全プロジェクトに配置を適用する";
            }

            HCIEnabled = EditorGUILayout.Toggle(HCIEnableText, HCIEnabled);
            HCICenter = EditorGUILayout.Toggle(HCICenterText, HCICenter);
            HCIOffset = EditorGUILayout.IntField(HCIOffsetText, HCIOffset);
            SaveGlobal = EditorGUILayout.Toggle(SaveGlobalText, SaveGlobal);

            //SaveGlobal = EditorGUILayout.Toggle(SaveGlobalText, SaveGlobal);

            SaveSettings();
        }
    }

    private static void LoadSettings()
    {
        HCIEnabled = EditorUserSettings.GetConfigValue(HCIEnabledKey) == "1";


        SaveGlobal = EditorPrefs.GetBool(SaveGlobalKey);
        if (!SaveGlobal)
        {
            SaveGlobal = EditorUserSettings.GetConfigValue(SaveGlobalKey) == "1";
        }
        ForceEnglish = EditorPrefs.GetBool("Praecipua_English");

        // SetGlobal が True のときは、全プロジェクト共通の設定ファイルから読み込み
        if (SaveGlobal == true)
        {
            HCICenter = EditorPrefs.GetBool(HCICenterKey);
            HCIOffset = EditorPrefs.GetInt(HCIOffsetKey);

        }
        // SetGlobal が False のときは、プロジェクト内の設定ファイルから読み込み
        else
        {
            HCICenter = EditorUserSettings.GetConfigValue(HCICenterKey) == "1";
            string HCIOffsetStr = EditorUserSettings.GetConfigValue(HCIOffsetKey);
            HCIOffset = HCIOffsetStr == null ? 0 : System.Int32.Parse(HCIOffsetStr);
        }
    }

    // 設定を保存
    private static void SaveSettings()
    {
        EditorUserSettings.SetConfigValue(HCIEnabledKey, HCIEnabled ? "1" : "0");

        // SetGlobal が True のときは、全プロジェクト共通の設定ファイルに保存
        if (SaveGlobal == true)
        {
            EditorPrefs.SetBool(HCICenterKey, HCICenter);
            EditorPrefs.SetInt(HCIOffsetKey, HCIOffset);
        }

        // SetGlobal の値にかかわらず、プロジェクト内の設定ファイルにも保存
        EditorUserSettings.SetConfigValue(HCICenterKey, HCICenter ? "1" : "0");
        EditorUserSettings.SetConfigValue(HCIOffsetKey, HCIOffset.ToString());
    }
}