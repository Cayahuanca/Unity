using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformViewer : Editor
{
    public override void OnInspectorGUI()
    {
        Transform transform = (Transform)target;
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Relative (相対, ローカル)");
        EditorGUILayout.LabelField("Absolute (絶対, ワールド)");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Position");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(transform.localPosition.ToString());
        EditorGUILayout.LabelField(transform.position.ToString());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Rotation");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(transform.localRotation.ToString());
        EditorGUILayout.LabelField(transform.rotation.ToString());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Scale");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(transform.localScale.ToString());
        EditorGUILayout.LabelField(GetAbsoluteScale(transform).ToString());
        EditorGUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }

    private Vector3 GetAbsoluteScale(Transform transform)
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
