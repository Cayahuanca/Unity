using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ComponentRemoverWindow : EditorWindow
{
    private List<GameObject> targetObjects;
    private Dictionary<GameObject, Component[]> componentsDict;
    private Dictionary<GameObject, bool[]> removeFlagsDict;
    private Vector2 scrollPosition;

    [MenuItem("Window/Component Remover")]
    public static void ShowWindow()
    {
        var window = GetWindow<ComponentRemoverWindow>();
        window.titleContent = new GUIContent("Component Remover");
        window.Show();
    }

    private void OnEnable()
    {
        targetObjects = new List<GameObject>();
        componentsDict = new Dictionary<GameObject, Component[]>();
        removeFlagsDict = new Dictionary<GameObject, bool[]>();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Target Objects", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Target Object", GUILayout.Width(120f)))
        {
            AddTargetObject();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5f);

        if (targetObjects.Count == 0)
        {
            AddTargetObject();
        }

        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < targetObjects.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GameObject newTargetObject = EditorGUILayout.ObjectField(targetObjects[i], typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("Remove", GUILayout.Width(70f)) && targetObjects[i] != null)
            {
                RemoveTargetObject(i);
                break;
            }

            EditorGUILayout.EndHorizontal();

            // 重複したオブジェクトを選択した場合は追加しない
            if (newTargetObject != null && !targetObjects.Contains(newTargetObject))
            {
                targetObjects[i] = newTargetObject;
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            ListComponents();
        }

        EditorGUILayout.Space(10f);

        if (GUILayout.Button("Show Components"))
        {
            ListComponents();
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (componentsDict != null)
        {
            EditorGUILayout.Space(10f);

            foreach (var entry in componentsDict)
            {
                GameObject targetObject = entry.Key;
                Component[] components = entry.Value;
                bool[] removeFlags = removeFlagsDict[targetObject];

                if (targetObject != null)
                {
                    EditorGUILayout.LabelField(targetObject.name, EditorStyles.boldLabel);
                }
                for (int i = 0; i < components.Length; i++)
                {
                    string componentName;
                    if (components[i] != null)
                    {
                        componentName = ObjectNames.NicifyVariableName(components[i].GetType().Name);
                    }
                    else
                    {
                        componentName = "Missing Script (Can't remove))";
                    }
                    removeFlags[i] = EditorGUILayout.Toggle(componentName, removeFlags[i]);
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10f);

            if (GUILayout.Button("Remove Components"))
            {
                RemoveSelectedComponents();
            }

            if (GUILayout.Button("Reset Window"))
            {
                ResetWindow();
            }
        }
    }

    private void AddTargetObject()
    {
        targetObjects.Add(null);
    }

    private void RemoveTargetObject(int index)
    {
        GameObject targetObject = targetObjects[index];
        targetObjects.RemoveAt(index);
        componentsDict.Remove(targetObject);
        removeFlagsDict.Remove(targetObject);
    }

    private void ListComponents()
    {
        componentsDict.Clear();
        removeFlagsDict.Clear();

        foreach (var targetObject in targetObjects)
        {
            if (targetObject != null)
            {
                Component[] components = targetObject.GetComponents<Component>();
                bool[] removeFlags = new bool[components.Length];

                List<Component> filteredComponents = new List<Component>();
                foreach (var component in components)
                {
                    if (!(component is Transform))
                    {
                        filteredComponents.Add(component);
                    }
                }

                componentsDict.Add(targetObject, filteredComponents.ToArray());
                removeFlagsDict.Add(targetObject, removeFlags);
            }
        }
    }

    private void RemoveSelectedComponents()
    {
        foreach (var entry in componentsDict)
        {
            GameObject targetObject = entry.Key;
            Component[] components = entry.Value;
            bool[] removeFlags = removeFlagsDict[targetObject];

            for (int i = 0; i < components.Length; i++)
            {
                if (removeFlags[i])
                {
                    if (components[i] != null)
                    {
                        RemoveComponent(targetObject, components[i]);
                        Debug.Log("Component removed: " + ObjectNames.NicifyVariableName(components[i].GetType().Name));
                    }
                }
            }
        }
    }

    private void RemoveComponent(GameObject obj, Component component)
{
    if (component != null)
    {
        var componentsInChildren = obj.GetComponentsInChildren(component.GetType());
        foreach (var childComponent in componentsInChildren)
        {
            if (childComponent != component)
            {
                Undo.DestroyObjectImmediate(childComponent);
            }
        }
        Undo.DestroyObjectImmediate(component);
    }
}


    private void ResetWindow()
    {
        targetObjects.Clear();
        componentsDict.Clear();
        removeFlagsDict.Clear();
        scrollPosition = Vector2.zero;
    }
}
