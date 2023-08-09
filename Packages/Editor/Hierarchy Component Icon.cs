using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Globalization;
using Praecipua.EE.RT;

namespace Praecipua.EE
{
	[InitializeOnLoad]
	public static class ComponentIconDrawer
	{
	    private static bool CIEnabled;
	    private static bool AREnabled;
	    private static bool ForceEnglish;
	    
	    private static int ICON_SIZE = 14;
	    private static int MAX_ICONS;
	    private static int CIICons = 0;
	    private static int CIOffset = 0;
	    
	    private const string CIEnabledKey = "Component_Icon_Enabled";
	    private const string AREnabledKey = "Component_Icon_Align_Right";
	    private const string CIIConsKey = "Component_Icon_Max_Icons";
	    private const string CIOffsetKey = "Component_Icon_Offset";

	    private const string SettingPath = "Project/Praecipua/Hierarchy Component Icon";

	    static ComponentIconDrawer()
	    {
	        EditorApplication.hierarchyWindowItemOnGUI += DrawComponentIcons;

	        LoadSettings();
	    }

	    private static void DrawComponentIcons(int instanceID, Rect selectionRect)
	    {
	        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

	        if (obj != null)
	        {
	            int iconCount = 0;
	            MAX_ICONS = 4 + CIICons;

	            // アイコンを表示するための位置を計算する
	            Rect iconRect = new Rect(selectionRect);
	            iconRect.width = ICON_SIZE;
	            iconRect.height = ICON_SIZE;
	            iconRect.y = selectionRect.yMin + 1;
	            
	            // 右寄せの場合のアイコン表示位置
	            if(AREnabled)
	            {
	                int componentCount = obj.GetComponents<Component>().Length;
	                if(componentCount>MAX_ICONS)
	                {
	                    componentCount = MAX_ICONS;
	                }
	                else
	                {
	                    componentCount = componentCount - 1;
	                }

	                int totalIconWidth = ICON_SIZE * componentCount;
	                iconRect.x = selectionRect.xMax - totalIconWidth - 16 - 16 - (CIOffset * 16);

	            }
	            // 左寄せの場合のアイコン表示位置
	            else
	            {
	                iconRect.x = selectionRect.xMax - iconRect.width * (MAX_ICONS ) - 16 - 16 - (CIOffset * 16);
	            }

	            // Component Icon が有効な場合のみ、アイコンを描画する
	            if (CIEnabled)
	            {
	                // オブジェクトに含まれるコンポーネントのアイコンを表示する
	                foreach (Component component in obj.GetComponents<Component>())
	                {
	                    if (iconCount >= MAX_ICONS) break;

	                    // Transform コンポーネント、Hierarchy Label のアイコンは除外する
	                    if (component is Transform) continue;
	                    if (component is HierarchyLabel) continue;

	                    Texture2D icon = AssetPreview.GetMiniThumbnail(component);

	                    if (icon != null)
	                    {
	                        // アイコンを描画する
	                        GUI.DrawTexture(iconRect, icon);

	                        // 次のアイコンの描画位置を計算する
	                        iconRect.x += ICON_SIZE;
	                        iconCount++;
	                    }
	                }
	            }
	        }
	    }

	    // Project Settings に設定画面を表示
	    public class ComponentIconSettingsProvider : SettingsProvider
	    {
	        public ComponentIconSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
	            : base(path, scopes)
	            {
	            }

	        [SettingsProvider]
	        public static SettingsProvider Create()
	        {
	            var provider = new ComponentIconSettingsProvider(SettingPath, SettingsScope.Project);
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

	            string CIEnableText = "Enable Hierarchy Component Icon";
	            string AREnableText = "Align icon right.";
	            string CIIConsText = "Increase the number of icons";
	            string CIOffsetText = "Icon Offset";
	            string SaveGlobalText = "Save settings (shared with other project)";
	            string LoadGlobalText = "Load settings from shared settings.";
	            
	            if (ForceEnglish == false && lang == "ja-JP")
	            {
	                CIEnableText = "ヒエラルキーに、コンポーネントのアイコンを表示する";
	                AREnableText = "アイコンを右寄せにする";
	                CIIConsText = "アイコンの数を増やす";
	                CIOffsetText = "アイコンの位置を調整する";
	                SaveGlobalText = "他のプロジェクトと共通の設定ファイルに書き込む";
	                LoadGlobalText = "他のプロジェクトと共通の設定ファイルから読み込む";
	            }

				EditorGUILayout.BeginHorizontal();;
	            	CIEnabled = EditorGUILayout.Toggle(CIEnabled, GUILayout.Width(30));
					EditorGUILayout.LabelField(CIEnableText, GUILayout.ExpandWidth(true));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
	            	AREnabled = EditorGUILayout.Toggle(AREnabled, GUILayout.Width(30));
					EditorGUILayout.LabelField(AREnableText, GUILayout.ExpandWidth(true));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
	            	CIICons = EditorGUILayout.IntField(CIICons, GUILayout.Width(30));
					EditorGUILayout.LabelField(CIIConsText, GUILayout.ExpandWidth(true));
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
	            	CIOffset = EditorGUILayout.IntField(CIOffset, GUILayout.Width(30));
					EditorGUILayout.LabelField(CIOffsetText, GUILayout.ExpandWidth(true));
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

	    // 設定を読み込み
	    private static void LoadSettings()
	    {
	        CIEnabled = EditorUserSettings.GetConfigValue(CIEnabledKey) == "1";
	        ForceEnglish = EditorPrefs.GetBool("Praecipua_English");
	    
	        AREnabled = EditorUserSettings.GetConfigValue(AREnabledKey) == "1";

	        string CIIConstr = EditorUserSettings.GetConfigValue(CIIConsKey);
	        CIICons = CIIConstr == null ? 0 : System.Int32.Parse(CIIConstr);

	        string CIOffsetStr = EditorUserSettings.GetConfigValue(CIOffsetKey);
	        CIOffset = CIOffsetStr == null ? 0 : System.Int32.Parse(CIOffsetStr);
	    }

	    // 設定を保存
	    private static void SaveSettings()
	    {
	        EditorUserSettings.SetConfigValue(CIEnabledKey, CIEnabled ? "1" : "0");
	        EditorUserSettings.SetConfigValue(AREnabledKey, AREnabled ? "1" : "0");

	        EditorUserSettings.SetConfigValue(CIIConsKey, CIICons.ToString());
	        EditorUserSettings.SetConfigValue(CIOffsetKey, CIOffset.ToString());
	    }

	    // 他のプロジェクトと設定を共有
	    private static void SaveSettingsGlobal()
	    {
	        EditorPrefs.SetBool(AREnabledKey, AREnabled);

	        EditorPrefs.SetInt(CIIConsKey, CIICons);
	        EditorPrefs.SetInt(CIOffsetKey, CIOffset);
	    }

	    private static void LoadSettingsGlobal()
	    {
	        AREnabled = EditorPrefs.GetBool(AREnabledKey);

	        CIICons = EditorPrefs.GetInt(CIIConsKey);
	        CIOffset = EditorPrefs.GetInt(CIOffsetKey);
	    }
	}
}