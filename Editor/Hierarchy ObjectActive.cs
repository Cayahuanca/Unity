using UnityEngine;
using UnityEditor;

public static class HierarchyGUI
{
    private const int WIDTH = 16;
    private const int OFFSET = 10;

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
    }
    
    private static void OnGUI(int instanceID, Rect selectionRect)
    {
        var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null)
        {
            return;
        }

        var pos = selectionRect;
        pos.x = pos.xMax - OFFSET;
        pos.width = WIDTH;

        bool active = GUI.Toggle(pos, go.activeSelf, string.Empty);
        if (active == go.activeSelf)
        {
            return;
        }

        Undo.RecordObject(go, $"{(active ? "Activate" : "Deactivate")} GameObject '{go.name}'");
        go.SetActive(active);
        EditorUtility.SetDirty(go);
    }
}
