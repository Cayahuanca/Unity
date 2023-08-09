using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Globalization;

namespace Praecipua.EE
{
	[InitializeOnLoad]
	public static class CustomIconInHierarchy
	{
	    private static bool HCIEnabled;
	    private static bool HCICenter;
	    private static bool ForceEnglish;

	    private static int HCIOffset;
	    
	    private const string HCIEnabledKey = "Hierarchy_Custom_Icon_Enabled";
	    private const string HCICenterKey = "Hierarchy_Custom_Icon_Center";
	    private static string HCIOffsetKey = "Hierarchy_Custom_Icon_Offset";

	    private static Color backgroundColor;

	    static CustomIconInHierarchy()
	    {
	        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
	        LoadSettings();
	    }

	    private static void HierarchyWindowItemOnGUI(int instanceID, Rect rect)
	    {
	        var obj = EditorUtility.InstanceIDToObject(instanceID);
	        if (!(obj is GameObject go)) return;

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
	            string SaveGlobalText = "Save settings (shared with other project)";
	            string LoadGlobalText = "Load settings from shared settings.";
	            
	            if (ForceEnglish == false && lang == "ja-JP")
	            {
	                HCIEnableText = "ヒエラルキーに、オブジェクトのアイコンを表示する";
	                HCICenterText = "アイコンの位置をデフォルトのアイコンの位置に合わせる";
	                HCIOffsetText = "アイコンの位置を調整する";
	                SaveGlobalText = "他のプロジェクトと共通の設定ファイルに書き込む";
	                LoadGlobalText = "他のプロジェクトと共通の設定ファイルから読み込む";
	            }

				EditorGUILayout.BeginHorizontal();
	            	HCIEnabled = EditorGUILayout.Toggle(HCIEnabled, GUILayout.Width(30));
					EditorGUILayout.LabelField(HCIEnableText, GUILayout.ExpandWidth(true));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
					HCICenter = EditorGUILayout.Toggle(HCICenter, GUILayout.Width(30));
					EditorGUILayout.LabelField(HCICenterText, GUILayout.ExpandWidth(true));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
	            	HCIOffset = EditorGUILayout.IntField(HCIOffset, GUILayout.Width(30));
					EditorGUILayout.LabelField(HCIOffsetText, GUILayout.ExpandWidth(true));
				EditorGUILayout.EndHorizontal();

	            if (GUILayout.Button(SaveGlobalText))
	            {
	                SaveSettingsGlobal();
	            }

	            if (GUILayout.Button(LoadGlobalText))
	            {
	                LoadSettingsGlobal();
	            }

	            SaveSettings();
	        }
	    }

	    private static void LoadSettings()
	    {
	        HCIEnabled = EditorUserSettings.GetConfigValue(HCIEnabledKey) == "1";
	        HCICenter = EditorUserSettings.GetConfigValue(HCICenterKey) == "1";
	        string HCIOffsetStr = EditorUserSettings.GetConfigValue(HCIOffsetKey);
	        HCIOffset = HCIOffsetStr == null ? 0 : System.Int32.Parse(HCIOffsetStr);

	        ForceEnglish = EditorPrefs.GetBool("Praecipua_English");
	    }

	    // 設定を保存
	    private static void SaveSettings()
	    {
	        EditorUserSettings.SetConfigValue(HCIEnabledKey, HCIEnabled ? "1" : "0");
	        EditorUserSettings.SetConfigValue(HCICenterKey, HCICenter ? "1" : "0");
	        EditorUserSettings.SetConfigValue(HCIOffsetKey, HCIOffset.ToString());
	    }

	    // 他のプロジェクトと設定を共有
	    private static void SaveSettingsGlobal()
	    {
	        EditorPrefs.SetBool(HCICenterKey, HCICenter);
	        EditorPrefs.SetInt(HCIOffsetKey, HCIOffset);
	    }

	    private static void LoadSettingsGlobal()
	    {
	        HCICenter = EditorPrefs.GetBool(HCICenterKey);
	        HCIOffset = EditorPrefs.GetInt(HCIOffsetKey);
	    }
	}
}