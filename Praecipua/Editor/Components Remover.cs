using UnityEngine;
using UnityEditor;

public class ComponentRemoverWindow : EditorWindow
{
    private GameObject targetObject;

    [MenuItem("Window/Component Remover")]
    public static void ShowWindow()
    {
        var window = GetWindow<ComponentRemoverWindow>();
        window.titleContent = new GUIContent("Collider Remover");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Target Object", EditorStyles.boldLabel);
        targetObject = EditorGUILayout.ObjectField(targetObject, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("Remove Colliders"))
        {
            if (targetObject != null)
            {
                RemoveColliders(targetObject);
                Debug.Log("Colliders removed from the target object and its children.");
            }
            else
            {
                Debug.LogError("No target object selected.");
            }
        }
    }

    private void RemoveColliders(GameObject obj)
    {
        var colliders = obj.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            DestroyImmediate(collider);
        }
    }
}
