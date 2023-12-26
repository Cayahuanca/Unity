using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Praecipua.EE
{
    [InitializeOnLoad]
    public class SceneAutoSave
    {
        private static bool saveEnabled = true;
        private static bool backupEnabled = true;
        private static bool alreadyNotified = false;
        private static bool saveSkip = false;
        private static bool ForceEnglish;

        private static float saveInterval = 300.0f;  // 保存間隔（秒）
        private static float notificationOffset = 15.0f; // 通知表示時間（秒）
        private static float lastNotificationTime = 0.0f;   // 最後に通知を表示した時間
        private static float lastSaveTime = 0.0f;   // 最後に保存した時間

        private static Thread saveThread;
        private static string saveInterval_Str;
        private static string notificationOffset_Str;
        private static string lang = CultureInfo.InstalledUICulture.Name;

        private const string SaveEnabledKey = "SceneAutoSave_Enabled";
        private const string BackupEnabledKey = "SceneAutoSave_BackupEnabled";
        private const string SaveIntervalKey = "SceneAutoSave_Interval";
        private const string NotificationOffsetKey = "SceneAutoSave_NotificationOffset";

        private static string NotSaveInPlayModeText = "Scene is not automatically saved while in play mode.";
        private static string NotSavedYetText = "Scene is not saved yet. Please save scene manually first to run auto save.";
        private static string SceneSaveSkippedText = "Scene Auto Save Skipped";
        private static string SceneSaveFailedText = "Scene Auto Save Failed";
        private static string SceneBackupFailedText = "Scene Backup Failed";

        private static string saveEnabledText = "Enable Scene Auto Save";
        private static string backupEnabledText = "Enable Scene Backup";
        private static string excludeAskText = "Exclude backed up Scene files from Git management.";
        private static string excludeSetupText = "Exclude (add *.unity to .gitignore)";
        private static string excludeRemoveText = "Not Exclude (remove *.unity from .gitignore)";
        private static string saveIntervalText = "Scene Auto Save Interval (sec) (min: 60s)";
        private static string notificationOffsetText = "Scene Auto Save Notification Offset (sec) (min: 10s)";

        static SceneAutoSave()
        {
            // 設定値を読み込む
            LoadSettings();
            LangSetting();

            // Unityエディタが起動した際に実行
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            // シーンを自動保存するためのカウンターを更新
            if (saveInterval > EditorApplication.timeSinceStartup - lastNotificationTime && EditorApplication.timeSinceStartup - lastNotificationTime >= saveInterval - notificationOffset && !alreadyNotified)
            {
                // シーンが一度も保存されたことがいない場合は、保存しない
                if (string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))
                {
                    Debug.Log(NotSavedYetText);
                    lastNotificationTime = (float)EditorApplication.timeSinceStartup + notificationOffset;
                    return;
                }

                // シーンが変更されていない場合は、保存しない
                if (!EditorSceneManager.GetActiveScene().isDirty)
                {
                    lastNotificationTime = (float)EditorApplication.timeSinceStartup + notificationOffset;
                    lastSaveTime = (float)EditorApplication.timeSinceStartup;
                    return;
                }

                Debug.Log("Scene will be saved in " + (int)notificationOffset + " seconds. \n Last saved at " + (EditorApplication.timeSinceStartup - lastSaveTime).ToString("F0") + " seconds ago.");
                var saveNotification = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.SceneView");
                EditorWindow.GetWindow(saveNotification).ShowNotification(new GUIContent("Scene will be saved in " + notificationOffset.ToString("F0") + " seconds"));

                alreadyNotified = true;
            }

            else if (EditorApplication.timeSinceStartup - lastNotificationTime >= saveInterval)
            {
                // 処理性能が非常に高い PC や、非常に軽量なシーンの場合、ここが同時に何回も呼ばれることがあるが、調整が面倒なので、利用者には諦めてもらう。
                lastNotificationTime = (float)EditorApplication.timeSinceStartup;
                alreadyNotified = false;

                // 保存スキップフラグが立っている場合は、保存せずにフラグを下ろす
                if (saveSkip)
                {
                    Debug.Log(SceneSaveSkippedText);
                    saveSkip = false;
                    return;
                }

                // Play モード中は、保存しない
                if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
                {
                    Debug.Log(NotSaveInPlayModeText);
                    return;
                }

                // 保存スレッドが動作していないか確認し、起動
                if (saveThread == null || !saveThread.IsAlive)
                {
                    saveThread = new Thread(SaveScene);
                    saveThread.Start();
                }

                if (backupEnabled)
                {
                    BackupScene();
                }
            }
        }

        // 今回のみ保存をスキップする
        [MenuItem("File/Scene Auto Save Skip &#S", priority = 175)]
        private static void SkipSave()
        {
            bool isChecked = Menu.GetChecked("File/Scene Auto Save Skip &#S");
            Menu.SetChecked("File/Scene Auto Save Skip &#S", !isChecked);
            saveSkip = !isChecked;
        }

        // シーンを保存する
        private static void SaveScene()
        {
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
                {
                    Debug.Log(NotSaveInPlayModeText);
                    return;
                }

                if (SceneManager.sceneCount > 0)
                {
                    for (int i=0; i < SceneManager.sceneCount; i++)
                    {
                        Scene scene = SceneManager.GetSceneAt(i);
                        string scenePath = scene.path;

                        // シーンが一度も保存されたことがいない場合は、バックアップしない
                        if (string.IsNullOrEmpty(scenePath))
                        {
                            Debug.Log(NotSavedYetText);
                            continue;
                        }

                        // シーンが変更されていない場合は、保存しない
                        if (!scene.isDirty)
                        {
                            continue;
                        }

                        // シーンを保存
                        bool saveSucceed = EditorSceneManager.SaveScene(scene);

                        // 保存に失敗した場合は、エラーを表示
                        if (!saveSucceed)
                        {
                            Debug.LogError(SceneSaveFailedText);
                            saveThread = null;
                            return;
                        }
                    }
                }

                // 保存完了を通知
                Scene ActiveScene = EditorSceneManager.GetActiveScene();
                Debug.Log("Scene Auto Saved: " + ActiveScene.path);
                var saveFinNotification = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.SceneView");
                EditorWindow.GetWindow(saveFinNotification).ShowNotification(new GUIContent("Scene " + ActiveScene.name + " is Saved."));

                // カウンターをリセット
                lastSaveTime = (float)EditorApplication.timeSinceStartup;
            };

            // saveThread が終了したため、null を代入
            saveThread = null;
        }

        // シーンをバックアップする
        private static void BackupScene()
        {
            // 今開いているシーンのパスを取得
            // string scenePath = EditorSceneManager.GetActiveScene().path; では、複数のシーンをロードしている場合に、対応できない

            // バックアップ先のフォルダが無ければ作成
            string backupFolderPath = Application.dataPath + "/../SceneBackup";
            if (!Directory.Exists(backupFolderPath))
            {
                Directory.CreateDirectory(backupFolderPath);
            }

            if (SceneManager.sceneCount > 0)
            {
                for (int i=0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    string scenePath = scene.path;

                    // シーンが一度も保存されたことがいない場合は、バックアップしない
                    if (string.IsNullOrEmpty(scenePath))
                    {
                        continue;
                    }

                    string sceneFullPath = Application.dataPath + "/../" + scenePath;

                    // scene のファイル名に、現在の日時を付けて、バックアップ先にコピー
                    string sceneName = scene.name;
                    string backupScenePath = backupFolderPath + "/" + sceneName + "_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".unity";

                    // すでに同名のファイルが存在する場合は、Scene ファイルの GUID を付けて、上書きを避ける
                    if (File.Exists(backupScenePath))
                    {
                        string sceneGUID = AssetDatabase.AssetPathToGUID(scenePath);
                        backupScenePath = backupFolderPath + "/" + sceneName + "_" + sceneGUID + ") " + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".unity";
                    }

                    // バックアップ先にコピー
                    File.Copy(sceneFullPath, backupScenePath);

                    // 失敗した場合は、エラーを表示
                    if (!File.Exists(backupScenePath))
                    {
                        Debug.LogError(SceneBackupFailedText);
                        continue;
                    }
                }
            }
        }

        private static void BackupSceneGitignore(bool ignore)
    {
        // バックアップ先のフォルダが無ければ作成
        string backupFolderPath = Application.dataPath + "/../SceneBackup";
        if (!Directory.Exists(backupFolderPath))
        {
            Directory.CreateDirectory(backupFolderPath);
        }

        string gitignorePath = backupFolderPath + "/.gitignore";

        if (ignore)
        {
            // .gitignore が無ければ作成
            if (!File.Exists(gitignorePath))
            {
                File.Create(gitignorePath).Close(); // ファイルを作成してからクローズ
            }

            // .gitignore に、*.unity の行がなければ、その行を追加
            string unityIgnoreLine = "*.unity";
            string[] lines = File.ReadAllLines(gitignorePath);
            if (Array.IndexOf(lines, unityIgnoreLine) == -1)
            {
                using (StreamWriter sw = File.AppendText(gitignorePath))
                {
                    sw.WriteLine(sw.NewLine + unityIgnoreLine);
                }
            }
        }
        else
        {
            // .gitignore が存在し、*.unity の行があれば、その行を削除
            if (File.Exists(gitignorePath))
            {
                string[] lines = File.ReadAllLines(gitignorePath);
                int unityIgnoreIndex = Array.IndexOf(lines, "*.unity");
                if (unityIgnoreIndex != -1)
                {
                    // 指定した行を削除
                    string[] newLines = new string[lines.Length - 1];
                    Array.Copy(lines, 0, newLines, 0, unityIgnoreIndex);
                    Array.Copy(lines, unityIgnoreIndex + 1, newLines, unityIgnoreIndex, lines.Length - unityIgnoreIndex - 1);
                    File.WriteAllLines(gitignorePath, newLines);
                }
            }
        }
    }

        private class SceneAutoSaveSettingsProvider : SettingsProvider
        {
            private const string SettingPath = "Project/Praecipua/Scene Auto Save";

            public SceneAutoSaveSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
                : base(path, scopes) { }

            [SettingsProvider]
            public static SettingsProvider Create()
            {
                var provider = new SceneAutoSaveSettingsProvider(SettingPath, SettingsScope.Project);
                return provider;
            }

            public override void OnActivate(string searchContext, VisualElement rootElement)
            {
                LoadSettings();
            }

            public override void OnGUI(string searchContext)
            {
                EditorGUILayout.BeginHorizontal();
                    saveEnabled = EditorGUILayout.Toggle(saveEnabled, GUILayout.Width(50));
                    EditorGUILayout.LabelField(saveEnabledText, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                    backupEnabled = EditorGUILayout.Toggle(backupEnabled, GUILayout.Width(50));
                    EditorGUILayout.LabelField(backupEnabledText, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();

                if (backupEnabled)
                {
                EditorGUILayout.LabelField(excludeAskText);
                EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(excludeSetupText))
                    {
                        BackupSceneGitignore(true);
                    }
                    if (GUILayout.Button(excludeRemoveText))
                    {
                        BackupSceneGitignore(false);
                    }
                EditorGUILayout.EndHorizontal();
                }

                // 保存間隔
                EditorGUILayout.LabelField(saveIntervalText);
                saveInterval = EditorGUILayout.FloatField(saveInterval);
                if (saveInterval < 60.0f) saveInterval = 60.0f;

                // 通知表示時間
                EditorGUILayout.LabelField(notificationOffsetText);
                notificationOffset = EditorGUILayout.FloatField(notificationOffset);
                if (notificationOffset < 10.0f) notificationOffset = 10.0f;

                SaveSettings();
            }
        }

        // Load Settings
        private static void LoadSettings()
        {
            saveEnabled = EditorUserSettings.GetConfigValue(SaveEnabledKey) == "1";
            if (saveEnabled == null) saveEnabled = true;
            backupEnabled = EditorUserSettings.GetConfigValue(BackupEnabledKey) == "1";
            if (backupEnabled == null) backupEnabled = true;

            saveInterval_Str = EditorUserSettings.GetConfigValue(SaveIntervalKey);
                float.TryParse(saveInterval_Str, out saveInterval);
                if (saveInterval < 60.0f) saveInterval = 60.0f;
            notificationOffset_Str = EditorUserSettings.GetConfigValue(NotificationOffsetKey);
                float.TryParse(notificationOffset_Str, out notificationOffset);
                if (notificationOffset < 10.0f) notificationOffset = 10.0f;

            ForceEnglish = EditorPrefs.GetBool("Praecipua_English");
        }

        // Save Settings
        private static void SaveSettings()
        {
            EditorUserSettings.SetConfigValue(SaveEnabledKey, saveEnabled ? "1" : "0");
            EditorUserSettings.SetConfigValue(BackupEnabledKey, backupEnabled ? "1" : "0");

            saveInterval_Str = saveInterval.ToString();
                EditorUserSettings.SetConfigValue(SaveIntervalKey, saveInterval_Str);
            notificationOffset_Str = notificationOffset.ToString();
                EditorUserSettings.SetConfigValue(NotificationOffsetKey, notificationOffset_Str);
        }

        // 言語設定
        private static void LangSetting()
        {
            if (ForceEnglish == false && lang == "ja-JP")
            {
                NotSaveInPlayModeText = "プレイモード中は、シーンの自動保存は行われません。";
                NotSavedYetText = "シーンが一度も保存されたことがいない場合は、シーンの自動保存は行われません。";
                SceneSaveSkippedText = "シーンの自動保存をスキップしました";
                SceneSaveFailedText = "シーンの自動保存に失敗しました";
                SceneBackupFailedText = "シーンのバックアップに失敗しました";
                saveEnabledText = "シーンの自動保存を有効にする";
                backupEnabledText = "シーンのバックアップを有効にする";
                excludeAskText = "バックアップしたシーンファイルを Git 管理から除外するか";
                excludeSetupText = "除外する ( *.unity を .gitignore に追加)";
                excludeRemoveText = "除外しない ( *.unity を .gitignore から削除)";
                saveIntervalText = "シーンの自動保存間隔 (秒) (最小: 60秒)";
                notificationOffsetText = "シーン自動保存通知表示時間 (秒) (最小: 10秒)";
            }
        }
    }
}