using UnityEditor;
using UnityEngine;

public class HierarchyLine : MonoBehaviour
{
    public Color objectColor;
    public Color textColor;
    public bool textBold;
    public bool textItalic;
    public int textPosition;
}

[CustomEditor(typeof(HierarchyLine))]
public class ColorScriptEditor : Editor
{
    void OnInspectorUpdate()
    {
        Repaint();
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        HierarchyLine colorScript = (HierarchyLine)target;

        EditorGUILayout.Space();
        colorScript.objectColor = EditorGUILayout.ColorField("Object Color", colorScript.objectColor);
        colorScript.textColor = EditorGUILayout.ColorField("Text Color", colorScript.textColor);

        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Color Presets");
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Red"))
            {
                colorScript.objectColor = Color.red;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Button("Green"))
            {
                colorScript.objectColor = Color.green;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Button("Blue"))
            {
                colorScript.objectColor = Color.blue;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Button("Yellow"))
            {
                colorScript.objectColor = Color.yellow;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Button("Cyan"))
            {
                colorScript.objectColor = Color.cyan;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Button("Magenta"))
            {
                colorScript.objectColor = Color.magenta;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Button("White"))
            {
                colorScript.objectColor = Color.white;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Button("Gray"))
            {
                colorScript.objectColor = Color.gray;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Button("Black"))
            {
                colorScript.objectColor = Color.black;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Button("Clear"))
            {
                colorScript.objectColor = Color.clear;
                EditorApplication.RepaintHierarchyWindow();
            }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Text Presets");
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
            if (GUILayout.Toggle(colorScript.textBold, "Bold"))
            {
                colorScript.textBold = true;
                EditorApplication.RepaintHierarchyWindow();
            }
            else
            {
                colorScript.textBold = false;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Toggle(colorScript.textItalic, "Italic"))
            {
                colorScript.textItalic = true;
                EditorApplication.RepaintHierarchyWindow();
            }
            else
            {
                colorScript.textItalic = false;
                EditorApplication.RepaintHierarchyWindow();
            }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Left"))
            {
                colorScript.textPosition = 0;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Button("Center"))
            {
                colorScript.textPosition = 1;
                EditorApplication.RepaintHierarchyWindow();
            }
            if (GUILayout.Button("Right"))
            {
                colorScript.textPosition = 2;
                EditorApplication.RepaintHierarchyWindow();
            }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

[InitializeOnLoad]
public class HierarchyColor
{
    private static GUIStyle textStyle;
    private static GUIStyleState textStyleState;
    //private static Color textColor = Color.white;

    static HierarchyColor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect rect)
    {
        rect.x = 32;
        rect.xMax = rect.xMax + 48;

        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject != null)
        {
            HierarchyLine colorScript = gameObject.GetComponent<HierarchyLine>();
            if (colorScript != null)
            {
                textStyle = new GUIStyle();
                textStyleState = new GUIStyleState();
                textStyleState.textColor = colorScript.textColor;
                textStyle.normal = textStyleState;


                if (colorScript.textBold && colorScript.textItalic);
                {
                    textStyle.fontStyle = FontStyle.BoldAndItalic;
                }
                if (colorScript.textBold && !colorScript.textItalic)
                {
                    textStyle.fontStyle = FontStyle.Bold;
                }
                if (!colorScript.textBold && colorScript.textItalic)
                {
                    textStyle.fontStyle = FontStyle.Italic;
                }
                if (!colorScript.textBold && !colorScript.textItalic)
                {
                    textStyle.fontStyle = FontStyle.Normal;
                }

                if (colorScript.textPosition == 0)
                {
                    textStyle.alignment = TextAnchor.MiddleLeft;
                }
                if (colorScript.textPosition == 1)
                {
                    textStyle.alignment = TextAnchor.MiddleCenter;
                }
                if (colorScript.textPosition == 2)
                {
                    textStyle.alignment = TextAnchor.MiddleRight;
                }

                EditorGUI.DrawRect(rect, colorScript.objectColor);
                GUI.Label(rect, gameObject.name, textStyle);
            }
        }
    }
}
