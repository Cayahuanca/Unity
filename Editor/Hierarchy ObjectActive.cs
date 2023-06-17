using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class ObjectActivationToggle
{
    static ObjectActivationToggle()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
    }

    private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject == null)
        {
            return;
        }

        Rect toggleRect = new Rect(selectionRect.xMax, selectionRect.yMin, 15, 15);
        bool active = GUI.Toggle(toggleRect, gameObject.activeSelf, "");

        if (active != gameObject.activeSelf)
        {
            gameObject.SetActive(active);
        }
    }
}
