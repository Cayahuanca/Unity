using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformEditorJa : Editor
{   
    private bool isWorldPositionEditable = false;
    private static bool EnabledInHierarchyWindow = true;
    private static bool LanguageInHierarchyWindow = true;

    private static string TransformEditorEnable = "Transform Editor Enabled";
    
    public override void OnInspectorGUI()
    {
        Transform transform = (Transform)target;

        TransformEditorEnable = "Transform Editor Enabled";
        string toggleText = "Edit World Position";
        string toggleDescription = "Using World Position may cause the numbers to continuously change.";
        string localPositionLabel = "Position (Local)";
        string worldPositionLabel = "Position (World)";
        string localRotationLabel = "Rotation (Local)";
        string worldRotationLabel = "Rotation (World)";
        string localScaleLabel = "Scale (Local)";
        string worldScaleLabel = "Scale (World)";

        if (LanguageInHierarchyWindow)
        {
            TransformEditorEnable = "Transform Editor を有効にする";
            toggleText = "ワールド座標を編集";
            toggleDescription = "ワールド座標を使用すると、数値が入力していないときに変動し続ける場合があります。";
            localPositionLabel = "座標 (ローカル)";
            worldPositionLabel = "座標 (ワールド)";
            localRotationLabel = "回転 (ローカル)";
            worldRotationLabel = "回転 (ワールド)";
            localScaleLabel = "スケール (ローカル)";
            worldScaleLabel = "スケール (ワールド)";
        }

        if (EnabledInHierarchyWindow)
        {
            isWorldPositionEditable = EditorGUILayout.Toggle(toggleText, isWorldPositionEditable);
            EditorGUILayout.LabelField(toggleDescription);

            if (!isWorldPositionEditable)
            {
                transform.localPosition = EditorGUILayout.Vector3Field(localPositionLabel, transform.localPosition);
                EditorGUILayout.LabelField(worldPositionLabel, transform.position.ToString());
            }
            else
            {
                transform.localPosition = EditorGUILayout.Vector3Field(localPositionLabel, transform.localPosition);
                transform.position = EditorGUILayout.Vector3Field(worldPositionLabel, transform.position);
            }

            EditorGUILayout.LabelField("");

            transform.localRotation = Quaternion.Euler(EditorGUILayout.Vector3Field(localRotationLabel, transform.localRotation.eulerAngles));

            transform.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field(worldRotationLabel, transform.rotation.eulerAngles));

            EditorGUILayout.LabelField("");

            transform.localScale = EditorGUILayout.Vector3Field(localScaleLabel, transform.localScale);

            EditorGUILayout.LabelField(worldScaleLabel, GetAbsoluteScale(transform).ToString());
        }
        else
        {
            /* DrawDefaultInspector(); を使用すると、Rotation に W 欄が追加され、表示がおかしくなるため、デフォルトと同様のものを以下のコードで再現しています。*/
            transform.localPosition = EditorGUILayout.Vector3Field("Position", transform.localPosition);
            transform.localRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", transform.localRotation.eulerAngles));
            transform.localScale = EditorGUILayout.Vector3Field("Scale", transform.localScale);
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

    [PreferenceItem("Praecipua/Transform Editor")]
    private static void OnPreferences()
    {
        EditorGUILayout.LabelField("Transform Editor");
        EnabledInHierarchyWindow = EditorGUILayout.Toggle(TransformEditorEnable, EnabledInHierarchyWindow);
        LanguageInHierarchyWindow = EditorGUILayout.Toggle("English / Japanese", LanguageInHierarchyWindow);
    }
}
