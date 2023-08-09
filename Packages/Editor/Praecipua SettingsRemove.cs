using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Praecipua.EE
{
	public class PraecipuaSettingsRemoveProvider : SettingsProvider
	{
	    private const string SettingPath = "Project/Praecipua/Settings Remove";

	    public PraecipuaSettingsRemoveProvider(string path, SettingsScope scopes = SettingsScope.Project)
	        : base(path, scopes)
	    {
	    }

	    [SettingsProvider]
	    public static SettingsProvider Create()
	    {
	        var provider = new PraecipuaSettingsRemoveProvider(SettingPath, SettingsScope.Project);
	        return provider;
	    }

	    public override void OnActivate(string searchContext, VisualElement rootElement)
	    {
	    }

	    public override void OnGUI(string searchContext)
	    {
	        EditorGUILayout.LabelField("ここで、キーを削除する前に、そのキーを使用するエディタ拡張のファイルを削除してください。");
			EditorGUILayout.LabelField("Unitypackage 版では、/Assets/Praecipua/Editor/エディタ拡張名).cs に保存されています。");
	        EditorGUILayout.LabelField("UPM や VPM 版では、/Packages/org.praecipua.ee/Editor/エディタ拡張名).cs に保存されています。");
	        EditorGUILayout.Space();
	        EditorGUILayout.LabelField("この画面からは、他のプロジェクトと共通の設定ファイルの設定のみ消去します。");
	        EditorGUILayout.LabelField("このプロジェクト内のみの設定は、消去できません。");
	        EditorGUILayout.Space();

	    // Hierarchy Component Icon
	        EditorGUILayout.LabelField("Hierarchy Component Icon が使用するキー");
	        string AREnabledKey = "Component_Icon_Align_Right";
	        string CIIConsKey = "Component_Icon_Max_Icons";
	        string CIOffsetKey = "Component_Icon_Offset";

	        if (GUILayout.Button("Delete " + AREnabledKey))
	        {
	            if (EditorPrefs.HasKey(AREnabledKey))
	            {
	                EditorPrefs.DeleteKey(AREnabledKey);
	            }
	        }
	        if (GUILayout.Button("Delete " + CIIConsKey))
	        {
	            if (EditorPrefs.HasKey(CIIConsKey))
	            {
	                EditorPrefs.DeleteKey(CIIConsKey);
	            }
	        }
	        if (GUILayout.Button("Delete " + CIOffsetKey))
	        {
	            if (EditorPrefs.HasKey(CIOffsetKey))
	            {
	                EditorPrefs.DeleteKey(CIOffsetKey);
	            }
	        }

	        if (GUILayout.Button("Delete All"))
	        {
	            if (EditorPrefs.HasKey(AREnabledKey))
	            {
	                EditorPrefs.DeleteKey(AREnabledKey);
	            }
	            if (EditorPrefs.HasKey(CIIConsKey))
	            {
	                EditorPrefs.DeleteKey(CIIConsKey);
	            }
	            if (EditorPrefs.HasKey(CIOffsetKey))
	            {
	                EditorPrefs.DeleteKey(CIOffsetKey);
	            }
	        }
	        EditorGUILayout.Space();
		
		// Hierarchy Custom Icon
			EditorGUILayout.LabelField("Hierarchy Custom Icon が使用するキー");
			string HCICenterKey = "Hierarchy_Custom_Icon_Center";
			string HCIOffsetKey = "Hierarchy_Custom_Icon_Offset";

			if (GUILayout.Button("Delete " + HCICenterKey))
	        {
	            if (EditorPrefs.HasKey(HCICenterKey))
	            {
	                EditorPrefs.DeleteKey(HCICenterKey);
	            }
	        }
			if (GUILayout.Button("Delete " + HCIOffsetKey))
	        {
	            if (EditorPrefs.HasKey(HCIOffsetKey))
	            {
	                EditorPrefs.DeleteKey(HCIOffsetKey);
	            }
	        }

			if (GUILayout.Button("Delete All"))
	        {
	            if (EditorPrefs.HasKey(HCICenterKey))
	            {
	                EditorPrefs.DeleteKey(HCICenterKey);
	            }
	            if (EditorPrefs.HasKey(HCIOffsetKey))
	            {
	                EditorPrefs.DeleteKey(HCIOffsetKey);
	            }
	        }
			EditorGUILayout.Space();

		// Hierarchy Object Active
			EditorGUILayout.LabelField("Hierarchy Object Active が使用するキー");
			string HOAOffsetKey = "Hierarchy_Object_Icon_Offset";
			if (GUILayout.Button("Delete " + HOAOffsetKey))
	        {
	            if (EditorPrefs.HasKey(HOAOffsetKey))
	            {
	                EditorPrefs.DeleteKey(HOAOffsetKey);
	            }
	        }
			EditorGUILayout.Space();

	    // Row Color
	        EditorGUILayout.LabelField("Row Color が使用するキー");
	        string RCH_R = "RowColor_Hierarchy_R";
	        string RCH_G = "RowColor_Hierarchy_G";
	        string RCH_B = "RowColor_Hierarchy_B";
	        string RCH_A = "RowColor_Hierarchy_A";

	        string RCP_R = "RowColor_Project_R";
	        string RCP_G = "RowColor_Project_G";
	        string RCP_B = "RowColor_Project_B";
	        string RCP_A = "RowColor_Project_A";

	        if (GUILayout.Button("Delete " + RCH_R))
	        {
	            if (EditorPrefs.HasKey(RCH_R))
	            {
	                EditorPrefs.DeleteKey(RCH_R);
	            }
	        }
	        if (GUILayout.Button("Delete " + RCH_G))
	        {
	            if (EditorPrefs.HasKey(RCH_G))
	            {
	                EditorPrefs.DeleteKey(RCH_G);
	            }
	        }
	        if (GUILayout.Button("Delete " + RCH_B))
	        {
	            if (EditorPrefs.HasKey(RCH_B))
	            {
	                EditorPrefs.DeleteKey(RCH_B);
	            }
	        }
	        if (GUILayout.Button("Delete " + RCH_A))
	        {
	            if (EditorPrefs.HasKey(RCH_A))
	            {
	                EditorPrefs.DeleteKey(RCH_A);
	            }
	        }
	        if (GUILayout.Button("Delete " + RCP_R))
	        {
	            if (EditorPrefs.HasKey(RCP_R))
	            {
	                EditorPrefs.DeleteKey(RCP_R);
	            }
	        }
	        if (GUILayout.Button("Delete " + RCP_G))
	        {
	            if (EditorPrefs.HasKey(RCP_G))
	            {
	                EditorPrefs.DeleteKey(RCP_G);
	            }
	        }
	        if (GUILayout.Button("Delete " + RCP_B))
	        {
	            if (EditorPrefs.HasKey(RCP_B))
	            {
	                EditorPrefs.DeleteKey(RCP_B);
	            }
	        }
	        if (GUILayout.Button("Delete " + RCP_A))
	        {
	            if (EditorPrefs.HasKey(RCP_A))
	            {
	                EditorPrefs.DeleteKey(RCP_A);
	            }
	        }

	        if (GUILayout.Button("Delete All"))
	        {
	            if (EditorPrefs.HasKey(RCH_R))
	            {
	                EditorPrefs.DeleteKey(RCH_R);
	            }
	            if (EditorPrefs.HasKey(RCH_G))
	            {
	                EditorPrefs.DeleteKey(RCH_G);
	            }
	            if (EditorPrefs.HasKey(RCH_B))
	            {
	                EditorPrefs.DeleteKey(RCH_B);
	            }
	            if (EditorPrefs.HasKey(RCH_A))
	            {
	                EditorPrefs.DeleteKey(RCH_A);
	            }
	            if (EditorPrefs.HasKey(RCP_R))
	            {
	                EditorPrefs.DeleteKey(RCP_R);
	            }
	            if (EditorPrefs.HasKey(RCP_G))
	            {
	                EditorPrefs.DeleteKey(RCP_G);
	            }
	            if (EditorPrefs.HasKey(RCP_B))
	            {
	                EditorPrefs.DeleteKey(RCP_B);
	            }
	            if (EditorPrefs.HasKey(RCP_A))
	            {
	                EditorPrefs.DeleteKey(RCP_A);
	            }
	        }
	        EditorGUILayout.Space();

	    // Praecipua 共通
	        EditorGUILayout.LabelField("Praecipua のエディタ拡張全てが使用するキー");
	        string ForceEnglishKey = "Praecipua_English";
	        EditorGUILayout.LabelField("Praecipua のエディタ拡張で英語を使用する設定:");
	        if (GUILayout.Button("Delete " + ForceEnglishKey))
	        {
	            if (EditorPrefs.HasKey(ForceEnglishKey))
	            {
	                EditorPrefs.DeleteKey(ForceEnglishKey);
	            }
	        }
	        EditorGUILayout.Space();

	    // ユーザーが個別に削除するキーを指定
	        /*
	        string CustomPref = "";
	        CustomPref = EditorGUILayout.TextField("Custom Key Name:", CustomPref);
	        if (GUILayout.Button("Delete"))
	        {
	            if (EditorPrefs.HasKey(CustomPref))
	            {
	                    EditorPrefs.DeleteKey(CustomPref);
	            }
	            else
	            {
	                EditorUtility.DisplayDialog("Could not find the Key", "Could not find the Key " + CustomPref, "OK");
	            }
	        }
	        EditorGUILayout.Space();
	        */
	    }
	}
}