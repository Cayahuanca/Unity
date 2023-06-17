using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Globalization;

[CustomEditor(typeof(Transform))]
public class TransformEditor : Editor
{   
    private bool isWorldPositionEditable = false;

    private static bool TEEnabled;
    private static bool ForceEnglish;

    private const string ForceEnglishKey = "Praecipua_English";
    private const string TEEnabledKey = "TransformEditor_Enabled";
    
    public override void OnInspectorGUI()
    {
        CultureInfo ci = CultureInfo.InstalledUICulture;
        string lang = ci.Name;

        Transform transform = (Transform)target;
        var LocalPosition = transform.localPosition;
        var LocalRotation = transform.localRotation;
        var LocalScale = transform.localScale;
        var WorldPosition = transform.position;
        var WorldRotation = transform.rotation;

        string toggleText = "Edit World Position and Rotation";
        string toggleDescription = "Using World Position may cause the numbers to continuously change.";
        string PositionLabel = "Position";
        string localPositionLabel = "Position (Local)";
        string worldPositionLabel = "Position (World)";
        string RotationLabel = "Rotation";
        string localRotationLabel = "Rotation (Local)";
        string worldRotationLabel = "Rotation (World)";
        string ScaleLabel = "Scale";
        string localScaleLabel = "Scale (Local)";
        string worldScaleLabel = "Scale (World)";

        if (ForceEnglish == false && lang == "ja-JP")
        {
            toggleText = "ワールド座標と回転を編集";
            toggleDescription = "ワールド座標を使用すると、数値が入力していないときに変動し続ける場合があります。";
            PositionLabel = "位置";
            localPositionLabel = "座標 (ローカル)";
            worldPositionLabel = "座標 (ワールド)";
            RotationLabel = "回転";
            localRotationLabel = "回転 (ローカル)";
            worldRotationLabel = "回転 (ワールド)";
            ScaleLabel = "スケール";
            localScaleLabel = "スケール (ローカル)";
            worldScaleLabel = "スケール (ワールド)";
        }

        LoadSettings();

        if (TEEnabled)
        {
            isWorldPositionEditable = EditorGUILayout.Toggle(toggleText, isWorldPositionEditable);
            EditorGUILayout.LabelField(toggleDescription);

            //ローカル座標を編集
            EditorGUI.BeginChangeCheck();
            LocalPosition  = EditorGUILayout.Vector3Field(localPositionLabel, LocalPosition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(transform, "Local Position Change");
                transform.localPosition = LocalPosition;
            }
            
            if (!isWorldPositionEditable)
            {
                // ワールド座標を表示
                EditorGUILayout.LabelField(worldPositionLabel, WorldPosition.ToString("F4"));
            }
            else
            {
                // ワールド座標を編集
                EditorGUI.BeginChangeCheck();
                WorldPosition = EditorGUILayout.Vector3Field(worldPositionLabel, WorldPosition);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(transform, "World Position Change");
                    transform.position = WorldPosition;
                }
            }

            EditorGUILayout.LabelField("");

            // ローカル回転を編集
            EditorGUI.BeginChangeCheck();
            LocalRotation = Quaternion.Euler(EditorGUILayout.Vector3Field(localRotationLabel, LocalRotation.eulerAngles));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(transform, "Local Rotation Change");
                transform.localRotation = LocalRotation;
            }

            if (!isWorldPositionEditable)
            {
                // ワールド回転を表示
                EditorGUILayout.LabelField(worldRotationLabel, WorldRotation.eulerAngles.ToString("F4"));
            }
            else
            {
                // ワールド回転を編集
                EditorGUI.BeginChangeCheck();
                WorldRotation = Quaternion.Euler(EditorGUILayout.Vector3Field(worldRotationLabel, WorldRotation.eulerAngles));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(transform, "World Rotation Change");
                    transform.rotation = WorldRotation;
                }
            }

            EditorGUILayout.LabelField("");

            // ローカルスケールを編集
            EditorGUI.BeginChangeCheck();
            LocalScale = EditorGUILayout.Vector3Field(localScaleLabel, LocalScale);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(transform, "Local Scale Change");
                transform.localScale = LocalScale;
            }
            EditorGUILayout.LabelField(worldScaleLabel, GetAbsoluteScale(transform).ToString("F4"));
        }
        else
        {
            // DrawDefaultInspector(); を使用すると、Rotation に W 欄が追加され、表示がおかしくなるため、デフォルトと同様のものを以下のコードで再現しています。
            EditorGUI.BeginChangeCheck();
            
            LocalPosition = EditorGUILayout.Vector3Field(PositionLabel, LocalPosition);
            LocalRotation = Quaternion.Euler(EditorGUILayout.Vector3Field(RotationLabel, LocalRotation.eulerAngles));
            LocalScale = EditorGUILayout.Vector3Field(ScaleLabel, LocalScale);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(transform, "Transform Change");

                transform.localPosition = LocalPosition;
                transform.localRotation = LocalRotation;
                transform.localScale = LocalScale;
            }
        }
    }

    private Vector3 GetAbsoluteScale(Transform transform)
    {
        {
            Vector3 absoluteScale = transform.localScale;
            Transform parent = transform.parent;
            while (parent != null)
            {
                absoluteScale = Vector3.Scale(absoluteScale, parent.localScale);
                parent = parent.parent;
            }
            return absoluteScale;
        }
    }

    // Project Settings に設定画面を表示
    public class TransformEditorSettingsProvider : SettingsProvider
    {
        private const string SettingPath = "Project/Praecipua/Transform Editor";

        public TransformEditorSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
            : base(path, scopes)
        {
        }

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new TransformEditorSettingsProvider(SettingPath, SettingsScope.Project);
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

            string TEEnableText = "Transform Editor Enabled";
            if (ForceEnglish == false && lang == "ja-JP")
            {
                TEEnableText = "Transform Editor を有効にする";
            }

            TEEnabled = EditorGUILayout.Toggle(TEEnableText, TEEnabled);

            SaveSettings();
        }
    }

    private static void LoadSettings()
    {
        ForceEnglish = EditorPrefs.GetBool(ForceEnglishKey);
        TEEnabled = EditorUserSettings.GetConfigValue(TEEnabledKey) == "1";
    }

    private static void SaveSettings()
    {
        EditorUserSettings.SetConfigValue(TEEnabledKey, TEEnabled ? "1" : "0");
    }
}

