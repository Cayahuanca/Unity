using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Globalization;

namespace Praecipua.EE
{
    public class PraecipuaSettingsProvider : SettingsProvider
    {
        private static bool ForceEnglish;

        private const string SettingPath = "Project/Praecipua";
        private const string ForceEnglishKey = "Praecipua_English";

        public PraecipuaSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
            : base(path, scopes) {}
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
    
        // Load Settings
        private static void LoadSettings()
        {
            ForceEnglish = EditorPrefs.GetBool(ForceEnglishKey);
        }
    
        // Save Settings
        private static void SaveSettings()
        {
            EditorPrefs.SetBool(ForceEnglishKey, ForceEnglish);
        }
    }
}
