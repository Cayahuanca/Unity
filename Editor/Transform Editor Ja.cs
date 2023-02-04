using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformEditorJa : Editor
{
    public override void OnInspectorGUI()
    {
        Transform transform = (Transform)target;

        /* EditorGUILayout.LabelField("Relative Position", transform.localPosition.ToString()); */
        transform.localPosition = EditorGUILayout.Vector3Field("座標 (ローカル)", transform.localPosition);

        /* EditorGUILayout.LabelField("Absolute Position", transform.position.ToString()); */
        transform.position = EditorGUILayout.Vector3Field("座標 (ワールド)", transform.position);

        EditorGUILayout.LabelField("");

        /* EditorGUILayout.LabelField("Relative Rotation", transform.localRotation.eulerAngles.ToString()); */
        transform.localRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("回転 (ローカル)", transform.localRotation.eulerAngles));

        /* EditorGUILayout.LabelField("Absolute Rotation", transform.rotation.eulerAngles.ToString()); */
        transform.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("回転 (ワールド)", transform.rotation.eulerAngles));

        EditorGUILayout.LabelField("");

        /* EditorGUILayout.LabelField("Relative Scale", transform.localScale.ToString()); */
        transform.localScale = EditorGUILayout.Vector3Field("スケール (ローカル)", transform.localScale);

        EditorGUILayout.LabelField("スケール (ワールド)", GetAbsoluteScale(transform).ToString());
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
