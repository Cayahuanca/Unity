using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Globalization;
using Praecipua.EE.RT;

namespace Praecipua.EE
{	
	[InitializeOnLoad]
	public class ObjectActivationToggle
	{
	    private static bool HOAEnabled;
	    private static bool lastSelectionToggleState;
	    private static bool ForceEnglish;
	
	    private static int HOAOffset;
	    
	    private const string HOAEnabledKey = "Hierarchy_Object_Icon_Enabled";
	    private const string HOAOffsetKey = "Hierarchy_Object_Icon_Offset";
	
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
	            if (GameObject.GetComponent<HierarchyLabel>() != null) return;
	
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
	            string SaveGlobalText = "Save settings (shared with other project)";
	            string LoadGlobalText = "Load settings from shared settings.";
	            
	            if (ForceEnglish == false && lang == "ja-JP")
	            {
	                HOAEnableText = "Hierarchy Object Active を有効にする";
	                HOAOffsetText = "アイコンの位置を調整する";
	                SaveGlobalText = "他のプロジェクトと共通の設定ファイルに書き込む";
	                LoadGlobalText = "他のプロジェクトと共通の設定ファイルから読み込む";
	            }
	            
				EditorGUILayout.BeginHorizontal();
	            	HOAEnabled = EditorGUILayout.Toggle(HOAEnabled, GUILayout.Width(30));
					EditorGUILayout.LabelField(HOAEnableText, GUILayout.ExpandWidth(true));
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
	            	HOAOffset = EditorGUILayout.IntField(HOAOffset, GUILayout.Width(30));
					EditorGUILayout.LabelField(HOAOffsetText, GUILayout.ExpandWidth(true));
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
	    
	    // 他のプロジェクトと設定を共有
	    private static void SaveSettingsGlobal()
	    {
	        EditorPrefs.SetInt(HOAOffsetKey, HOAOffset);
	    }
	
	    private static void LoadSettingsGlobal()
	    {
	        HOAOffset = EditorPrefs.GetInt(HOAOffsetKey);
	    }
	}
}