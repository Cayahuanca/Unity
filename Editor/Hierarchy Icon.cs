using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CustomIconInHierarchy
{
    private static bool EditorThemeColor = true;
    private static Color backgroundColor = new Color32(194, 194, 194, 255);

    static CustomIconInHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if (!(obj is GameObject go))
        {
            return;
        }

        var icon = EditorGUIUtility.ObjectContent(go, go.GetType()).image as Texture2D;
        var color = EditorPrefs.GetInt("Obj" + instanceID + "Color", -1);

        var rect = new Rect(selectionRect);
        rect.x += 0;
        rect.y += 0;
        rect.width = 16;
        rect.height = 16;


        if (EditorThemeColor)
        {
            var backgroundColor = EditorGUIUtility.isProSkin ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255);
            EditorGUI.DrawRect(rect, backgroundColor);

            if (icon != null)
            {
                GUI.DrawTexture(rect, icon);
            }
        }
        else
        {
            EditorGUI.DrawRect(rect, backgroundColor);
        }
    }

    [PreferenceItem("Praecipua/Hierarchy Icon")]
    private static void OnPreferencesGUI()
    {
        EditorThemeColor = EditorGUILayout.Toggle("Editor Theme Color", EditorThemeColor);
        backgroundColor = EditorGUILayout.ColorField("Icon Background Color", backgroundColor);
    }
}

