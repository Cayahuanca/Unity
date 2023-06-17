using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class MissingScriptWarning
{
    static MissingScriptWarning()
    {
        EditorApplication.hierarchyWindowItemOnGUI += CheckForMissingScript;
    }

    private static void CheckForMissingScript(int instanceID, Rect selectionRect)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject == null)
            return;

        var components = gameObject.GetComponents<Component>();

        foreach (var component in components)
        {
            if (component == null)
            {
                var iconRect = new Rect(selectionRect.xMax - 20, selectionRect.yMin, 20, 15);
                GUI.Label(iconRect, EditorGUIUtility.IconContent("d_console.warnicon.sml"));
                break;
            }
        }
    }
}