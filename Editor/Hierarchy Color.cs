using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class RowColor
{
    private const int ROW_HEIGHT = 16;
    private const int OFFSET_Y   = -4;

    private static bool EnabledInProjectWindow = true;
    private static bool EnabledInHierarchyWindow = true;
    private static Color Color = new Color( 0, 0, 0, 0.08f );

    static RowColor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
    }

    private static void HierarchyWindowItemOnGUI( int instanceID, Rect rect )
    {
        if (!EnabledInHierarchyWindow) return;

        var index = ( int ) ( rect.y + OFFSET_Y ) / ROW_HEIGHT;

        if ( index % 2 == 0 ) return;

        var xMax = rect.xMax;

        rect.x    = 32;
        rect.xMax = xMax + 16;

        EditorGUI.DrawRect( rect, Color );
    }

    private static void ProjectWindowItemOnGUI(string guid, Rect rect)
    {
        if (!EnabledInProjectWindow) return;

        var index = ( int ) ( rect.y + OFFSET_Y ) / ROW_HEIGHT;

        if ( index % 2 == 0 ) return;

        var xMax = rect.xMax;

        rect.x    = 32;
        rect.xMax = xMax + 16;

        EditorGUI.DrawRect( rect, Color );
    }

    [PreferenceItem("Hierarchy Customization")]
    private static void OnPreferencesGUI()
    {
        EnabledInHierarchyWindow = EditorGUILayout.Toggle("Enabled in Hierarchy Window", EnabledInHierarchyWindow);
        EnabledInProjectWindow = EditorGUILayout.Toggle("Enabled in Project Window", EnabledInProjectWindow);
        Color = EditorGUILayout.ColorField("Background Color", Color);
    }
}
