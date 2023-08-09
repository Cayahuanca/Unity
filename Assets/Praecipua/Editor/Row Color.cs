using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Globalization;

namespace Praecipua.EE
{
	[InitializeOnLoad]
	internal static class RowColor
	{
	    private static bool RCHEnabled;
	    private static bool RCPEnabled;
	    private static bool ForceEnglish;
	
	    private const string RCHEnabledKey = "RowColor_Hierarchy_Enabled";
	    private const string RCPEnabledKey = "RowColor_Project_Enabled:";
	
	    private static string HierarchyColor_R_Str;
	    private static string HierarchyColor_G_Str;
	    private static string HierarchyColor_B_Str;
	    private static string HierarchyColor_A_Str;
	        
	    private static string ProjectColor_R_Str;
	    private static string ProjectColor_G_Str;
	    private static string ProjectColor_B_Str;
	    private static string ProjectColor_A_Str;
	
	    private const string RCH_R = "RowColor_Hierarchy_R";
	    private const string RCH_G = "RowColor_Hierarchy_G";
	    private const string RCH_B = "RowColor_Hierarchy_B";
	    private const string RCH_A = "RowColor_Hierarchy_A";
	
	    private const string RCP_R = "RowColor_Project_R";
	    private const string RCP_G = "RowColor_Project_G";
	    private const string RCP_B = "RowColor_Project_B";
	    private const string RCP_A = "RowColor_Project_A";
	    
	    private static Color HierarchyColor = new Color( 0, 0, 0, 0.08f );
	    private static Color ProjectColor = new Color( 0, 0, 0, 0.08f );
	
	    static RowColor()
	    {
	        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
	        EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
	
	        LoadSettings();
	    }
	
	    // Hierarchy に色を付ける
	    private static void HierarchyWindowItemOnGUI( int instanceID, Rect rect )
	    {
	        if (!RCHEnabled) return;
	
	        var index = ( int ) ( rect.y - 4 ) / 16;
	
	        if ( index % 2 == 0 ) return;
	
	        var xMax = rect.xMax;
	
	        rect.x    = 32;
	        rect.xMax = xMax + 16;
	
	        EditorGUI.DrawRect( rect, HierarchyColor );
	    }
	
	    // Project に色を付ける
	    private static void ProjectWindowItemOnGUI(string guid, Rect rect)
	    {
	        if (!RCPEnabled) return;
	
	        var index = ( int ) ( rect.y ) / 16;
	
	        if ( index % 2 == 0 ) return;
	
	        var xMax = rect.xMax;
	
	        rect.x    = 16;
	        rect.xMax = xMax + 16;
	
	        EditorGUI.DrawRect( rect, ProjectColor );
	    }
	
	    // Project Settings に設定画面を表示
	    public class RowColorSettingsProvider : SettingsProvider
	    {
	        private const string SettingPath = "Project/Praecipua/Row Color";
	        private const string RCHEnabledKey = "RowColor_Hierarchy_Enabled";
	        private const string RCPEnabledKey = "RowColor_Project_Enabled";
	
	        public RowColorSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
	            : base(path, scopes)
	        {
	        }
	        [SettingsProvider]
	        public static SettingsProvider Create()
	        {
	            var provider = new RowColorSettingsProvider(SettingPath, SettingsScope.Project);
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
	
	            string RCHEnableText = "Enable Hierarchy Background Color";
	            string RCPEnableText = "Enable Project Background Color";
	            string RCHColorText  = "Hierarchy Background Color";
	            string RCPColorText = "Project Background Color";
	            string SaveGlobalText = "Save settings (shared with other project)";
	            string LoadGlobalText = "Load settings from shared settings.";
	            
	            if (ForceEnglish == false && lang == "ja-JP")
	            {
	                RCHEnableText = "ヒエラルキーの背景色を有効にする";
	                RCPEnableText = "プロジェクトの背景色を有効にする";
	                RCHColorText  = "ヒエラルキーの背景色";
	                RCPColorText = "プロジェクトの背景色";
	                SaveGlobalText = "他のプロジェクトと共通の設定ファイルに書き込む";
	                LoadGlobalText = "他のプロジェクトと共通の設定ファイルから読み込む";
	            }
	
				EditorGUILayout.BeginHorizontal();
	            	RCHEnabled = EditorGUILayout.Toggle(RCHEnabled, GUILayout.Width(50));
					EditorGUILayout.LabelField(RCHEnableText, GUILayout.ExpandWidth(true));
				EditorGUILayout.EndHorizontal();

	            EditorGUILayout.BeginHorizontal();
					HierarchyColor = EditorGUILayout.ColorField(HierarchyColor, GUILayout.Width(50));
					EditorGUILayout.LabelField(RCHColorText, GUILayout.ExpandWidth(true));
				EditorGUILayout.EndHorizontal();
	
	            EditorGUILayout.BeginHorizontal();
					RCPEnabled = EditorGUILayout.Toggle(RCPEnabled, GUILayout.Width(50));
					EditorGUILayout.LabelField(RCPEnableText, GUILayout.ExpandWidth(true));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
	            	ProjectColor = EditorGUILayout.ColorField(ProjectColor, GUILayout.Width(50));
					EditorGUILayout.LabelField(RCPColorText, GUILayout.ExpandWidth(true));
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
	        RCHEnabled = EditorUserSettings.GetConfigValue(RCHEnabledKey) == "1";
	        RCPEnabled = EditorUserSettings.GetConfigValue(RCPEnabledKey) == "1";
	
	        ForceEnglish = EditorPrefs.GetBool("Praecipua_English");
	
	        HierarchyColor_R_Str = EditorUserSettings.GetConfigValue(RCH_R);
	            float.TryParse(HierarchyColor_R_Str, out HierarchyColor.r);
	        HierarchyColor_G_Str = EditorUserSettings.GetConfigValue(RCH_G);
	            float.TryParse(HierarchyColor_G_Str, out HierarchyColor.g);
	        HierarchyColor_B_Str = EditorUserSettings.GetConfigValue(RCH_B);
	            float.TryParse(HierarchyColor_B_Str, out HierarchyColor.b);
	        HierarchyColor_A_Str = EditorUserSettings.GetConfigValue(RCH_A);
	            float.TryParse(HierarchyColor_A_Str, out HierarchyColor.a);
	
	        HierarchyColor = new Color(HierarchyColor.r, HierarchyColor.g, HierarchyColor.b, HierarchyColor.a);
	
	        ProjectColor_R_Str = EditorUserSettings.GetConfigValue(RCP_R);
	            float.TryParse(ProjectColor_R_Str, out ProjectColor.r);
	        ProjectColor_G_Str = EditorUserSettings.GetConfigValue(RCP_G);
	            float.TryParse(ProjectColor_G_Str, out ProjectColor.g);
	        ProjectColor_B_Str = EditorUserSettings.GetConfigValue(RCP_B);
	            float.TryParse(ProjectColor_B_Str, out ProjectColor.b);
	        ProjectColor_A_Str = EditorUserSettings.GetConfigValue(RCP_A);
	            float.TryParse(ProjectColor_A_Str, out ProjectColor.a);
	
	        ProjectColor = new Color(ProjectColor.r, ProjectColor.g, ProjectColor.b, ProjectColor.a);
	    }
	
	    // 設定を保存
	    private static void SaveSettings()
	    {
	        EditorUserSettings.SetConfigValue(RCHEnabledKey, RCHEnabled ? "1" : "0");
	        EditorUserSettings.SetConfigValue(RCPEnabledKey, RCPEnabled ? "1" : "0");
	
	        HierarchyColor_R_Str = HierarchyColor.r.ToString();
	            EditorUserSettings.SetConfigValue(RCH_R, HierarchyColor_R_Str);
	        HierarchyColor_G_Str = HierarchyColor.g.ToString();
	            EditorUserSettings.SetConfigValue(RCH_G, HierarchyColor_G_Str);
	        HierarchyColor_B_Str = HierarchyColor.b.ToString();
	            EditorUserSettings.SetConfigValue(RCH_B, HierarchyColor_B_Str);
	        HierarchyColor_A_Str = HierarchyColor.a.ToString();
	            EditorUserSettings.SetConfigValue(RCH_A, HierarchyColor_A_Str);
	        
	        ProjectColor_R_Str = ProjectColor.r.ToString();
	            EditorUserSettings.SetConfigValue(RCP_R, ProjectColor_R_Str);
	        ProjectColor_G_Str = ProjectColor.g.ToString();
	            EditorUserSettings.SetConfigValue(RCP_G, ProjectColor_G_Str);
	        ProjectColor_B_Str = ProjectColor.b.ToString();
	            EditorUserSettings.SetConfigValue(RCP_B, ProjectColor_B_Str);
	        ProjectColor_A_Str = ProjectColor.a.ToString();
	            EditorUserSettings.SetConfigValue(RCP_A, ProjectColor_A_Str);
	    }
	    
	    // 他のプロジェクトと設定を共有
	    private static void SaveSettingsGlobal()
	    {
	        EditorPrefs.SetFloat(RCH_R, HierarchyColor.r);
	        EditorPrefs.SetFloat(RCH_G, HierarchyColor.g);
	        EditorPrefs.SetFloat(RCH_B, HierarchyColor.b);
	        EditorPrefs.SetFloat(RCH_A, HierarchyColor.a);
	
	        EditorPrefs.SetFloat(RCP_R, ProjectColor.r);
	        EditorPrefs.SetFloat(RCP_G, ProjectColor.g);
	        EditorPrefs.SetFloat(RCP_B, ProjectColor.b);
	        EditorPrefs.SetFloat(RCP_A, ProjectColor.a);
	    }
	
	    private static void LoadSettingsGlobal()
	    {
	        HierarchyColor = new Color(
	            EditorPrefs.GetFloat(RCH_R, HierarchyColor.r),
	            EditorPrefs.GetFloat(RCH_G, HierarchyColor.g),
	            EditorPrefs.GetFloat(RCH_B, HierarchyColor.b),
	            EditorPrefs.GetFloat(RCH_A, HierarchyColor.a));
	
	        ProjectColor = new Color(
	            EditorPrefs.GetFloat(RCP_R, ProjectColor.r),
	            EditorPrefs.GetFloat(RCP_G, ProjectColor.g),
	            EditorPrefs.GetFloat(RCP_B, ProjectColor.b),
	            EditorPrefs.GetFloat(RCP_A, ProjectColor.a));
	    }
	}
}