using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Globalization;

public class PraecipuaSettingsProvider : SettingsProvider
{
    private static bool ForceEnglish;

    private const string SettingPath = "Project/Praecipua";
    private const string ForceEnglishKey = "Praecipua_English";

    public PraecipuaSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
        : base(path, scopes)
        {
        }
    [SettingsProvider]
    public static SettingsProvider Create()
    {
        var provider = new PraecipuaSettingsProvider(SettingPath, SettingsScope.Project);
        return provider;
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        LoadSettings();
    }

    public override void OnGUI(string searchContext)
    {
        //Language
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string lang = ci.Name;

            string CurrentEEXLanguageText = "The language for this extension is set to English.";
            string ForceEnglishText = "Force English to All Praecipua Editor Extension";
            
            if (ForceEnglish == false && lang == "ja-JP")
            {
                CurrentEEXLanguageText = "この拡張機能は、日本語を使うように設定されています。";
                ForceEnglishText = "Force English to All Praecipua Editor Extension";
            }

            EditorGUILayout.LabelField(CurrentEEXLanguageText);
            ForceEnglish = EditorGUILayout.Toggle(ForceEnglishText, ForceEnglish);

        SaveSettings();
    }
    
    // 設定を読み込み
    private static void LoadSettings()
    {
        //localbool = EditorUserSettings.GetConfigValue(boolkey) == "1";
        //globalbool = EditorPrefs.GetBool(globalboolkey);
        ForceEnglish = EditorPrefs.GetBool(ForceEnglishKey);
    }
    
    // 設定を保存
    private static void SaveSettings()
    {
        //EditorUserSettings.SetConfigValue(localboolkey, localbool ? "1" : "0");
        //EditorPrefs.SetBool(globalboolkey, globalbool);
        EditorPrefs.SetBool(ForceEnglishKey, ForceEnglish);
    }
}
