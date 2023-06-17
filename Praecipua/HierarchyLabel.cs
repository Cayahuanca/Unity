#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Globalization;

public class HierarchyLabel : MonoBehaviour
{
    public Color labelColor;
    public Color textColor;
    public Color customLabelColor;
    public Color customTextColor;
    public bool textBold;
    public bool textItalic;
    public int textPosition;
    public int presetID;
    public int themeID;
    public string HLThemeKey = "Hierarchy_Label_Theme";
    public string HLThemeStr;
}

[CustomEditor(typeof(HierarchyLabel))]
[CanEditMultipleObjects]
public class HierarchyLabelScriptEditor : Editor
{
    // 有効化時に呼ばれる
    private void OnEnable()
    {
        HierarchyLabel HierarchyLabelScript = (HierarchyLabel)target;
        LoadSettings(HierarchyLabelScript);
        SetColor(HierarchyLabelScript);
    }

    // Inspector の表示内容
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        HierarchyLabel HierarchyLabelScript = (HierarchyLabel)target;

        // テーマの設定
        EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                //HierarchyLabelScript.themeID = EditorGUILayout.IntField("Theme", HierarchyLabelScript.themeID);
        
                    if (GUILayout.Button("Theme 0"))
                    {
                        HierarchyLabelScript.themeID = 0;
                    }
                    if (GUILayout.Button("Theme 1"))
                    {
                        HierarchyLabelScript.themeID = 1;
                    }
                    if (GUILayout.Button("Theme 2"))
                    {
                        HierarchyLabelScript.themeID = 2;
                    }
                if (EditorGUI.EndChangeCheck())
                {
                    SetColor(HierarchyLabelScript);
                    SaveSettings(HierarchyLabelScript);
                }
            EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        
        // プリセットカラーの設定
        EditorGUILayout.LabelField("Color Presets");
        string color0 = "Red";
        string color1 = "Green";
        string color2 = "Blue";
        string color3 = "Yellow";
        string color4 = "Cyan";
        string color5 = "Magenta";
        string color6 = "White";
        string color7 = "Gray";
        string color8 = "Black";
        string color9 = "Custom";

        if (HierarchyLabelScript.themeID == 1)
        {
            color0 = "Red1";
            color1 = "Green1";
            color2 = "Blue1";
            color3 = "Yellow1";
            color4 = "Cyan1";
            color5 = "Magenta1";
            color6 = "White1";
            color7 = "Gray1";
            color8 = "Black1";
        }

        EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(color0))
                {
                    HierarchyLabelScript.presetID = 0;
                    SetColor(HierarchyLabelScript);
                }
                if (GUILayout.Button(color1))
                {
                    HierarchyLabelScript.presetID = 1;
                    SetColor(HierarchyLabelScript);
                }
                if (GUILayout.Button(color2))
                {
                    HierarchyLabelScript.presetID = 2;
                    SetColor(HierarchyLabelScript);
                }
                if (GUILayout.Button(color3))
                {
                    HierarchyLabelScript.presetID = 3;
                    SetColor(HierarchyLabelScript);
                }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(color4))
                {
                    HierarchyLabelScript.presetID = 4;
                    SetColor(HierarchyLabelScript);
                }
                if (GUILayout.Button(color5))
                {
                    HierarchyLabelScript.presetID = 5;
                    SetColor(HierarchyLabelScript);
                }
                if (GUILayout.Button(color6))
                {
                    HierarchyLabelScript.presetID = 6;
                    SetColor(HierarchyLabelScript);
                }
                if (GUILayout.Button(color7))
                {
                    HierarchyLabelScript.presetID = 7;
                    SetColor(HierarchyLabelScript);
                }
                if (GUILayout.Button(color8))
                {
                    HierarchyLabelScript.presetID = 8;
                    SetColor(HierarchyLabelScript);
                }
            EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // カスタムカラーの設定
        EditorGUILayout.LabelField("Custom Color");
        EditorGUI.BeginChangeCheck();
        HierarchyLabelScript.customLabelColor = EditorGUILayout.ColorField("Label Color", HierarchyLabelScript.customLabelColor);
        HierarchyLabelScript.customTextColor = EditorGUILayout.ColorField("Text Color", HierarchyLabelScript.customTextColor);
        if (EditorGUI.EndChangeCheck())
        {
            HierarchyLabelScript.customLabelColor.a = 1.0f;
            HierarchyLabelScript.customTextColor.a = 1.0f;
            HierarchyLabelScript.presetID = 9;
            SetColor(HierarchyLabelScript);
        }

        EditorGUILayout.Space();

        //文字の位置、装飾の設定
        EditorGUILayout.LabelField("Text Presets");
        EditorGUILayout.BeginVertical();
        // 文字の装飾
            EditorGUILayout.BeginHorizontal();
                if (GUILayout.Toggle(HierarchyLabelScript.textBold, "Bold"))
                {
                    HierarchyLabelScript.textBold = true;
                }
                else
                {
                    HierarchyLabelScript.textBold = false;
                }
                if (GUILayout.Toggle(HierarchyLabelScript.textItalic, "Italic"))
                {
                    HierarchyLabelScript.textItalic = true;
                }
                else
                {
                    HierarchyLabelScript.textItalic = false;
                }
            EditorGUILayout.EndHorizontal();
            
            // 文字の位置
            EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Left"))
                {
                    HierarchyLabelScript.textPosition = 0;
                }
                if (GUILayout.Button("Center"))
                {
                    HierarchyLabelScript.textPosition = 1;
                }
                if (GUILayout.Button("Right"))
                {
                    HierarchyLabelScript.textPosition = 2;
                }
            EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
        EditorApplication.RepaintHierarchyWindow();
    }

    // テーマ、プリセットに応じた色の適用
    private void SetColor(HierarchyLabel HierarchyLabelScript)
    {
        // テーマ0
        if (HierarchyLabelScript.themeID == 0)
        {
            if (HierarchyLabelScript.presetID == 0)
            {
                HierarchyLabelScript.labelColor = Color.red;
                HierarchyLabelScript.textColor = Color.white;
            }
            else if (HierarchyLabelScript.presetID == 1)
            {
                HierarchyLabelScript.labelColor = Color.green;
                HierarchyLabelScript.textColor = Color.black;
            }
            else if (HierarchyLabelScript.presetID == 2)
            {
                HierarchyLabelScript.labelColor = Color.blue;
                HierarchyLabelScript.textColor = Color.white;
            }
            else if (HierarchyLabelScript.presetID == 3)
            {
                HierarchyLabelScript.labelColor = Color.yellow;
                HierarchyLabelScript.textColor = Color.black;
            }
            else if (HierarchyLabelScript.presetID == 4)
            {
                HierarchyLabelScript.labelColor = Color.cyan;
                HierarchyLabelScript.textColor = Color.black;
            }
            else if (HierarchyLabelScript.presetID == 5)
            {
                HierarchyLabelScript.labelColor = Color.magenta;
                HierarchyLabelScript.textColor = Color.white;
            }
            else if (HierarchyLabelScript.presetID == 6)
            {
                HierarchyLabelScript.labelColor = Color.white;
                HierarchyLabelScript.textColor = Color.black;
            }
            else if (HierarchyLabelScript.presetID == 7)
            {
                HierarchyLabelScript.labelColor = Color.gray;
                HierarchyLabelScript.textColor = Color.black;
            }
            else if (HierarchyLabelScript.presetID == 8)
            {
                HierarchyLabelScript.labelColor = Color.black;
                HierarchyLabelScript.textColor = Color.white;
            }
            else if (HierarchyLabelScript.presetID == 9)
            {
                HierarchyLabelScript.labelColor = HierarchyLabelScript.customLabelColor;
                HierarchyLabelScript.textColor = HierarchyLabelScript.customTextColor;
            }
            else
            {
                HierarchyLabelScript.labelColor = Color.white;
                HierarchyLabelScript.textColor = Color.black;
            }
        }
        // テーマ1
        else if (HierarchyLabelScript.themeID == 1)
        {
            if (HierarchyLabelScript.presetID == 0)
            {
                HierarchyLabelScript.labelColor = Color.red;
                HierarchyLabelScript.textColor = Color.black;
            }
            else if (HierarchyLabelScript.presetID == 1)
            {
                HierarchyLabelScript.labelColor = Color.green;
                HierarchyLabelScript.textColor = Color.white;
            }
            else if (HierarchyLabelScript.presetID == 2)
            {
                HierarchyLabelScript.labelColor = Color.blue;
                HierarchyLabelScript.textColor = Color.black;
            }
            else if (HierarchyLabelScript.presetID == 3)
            {
                HierarchyLabelScript.labelColor = Color.yellow;
                HierarchyLabelScript.textColor = Color.white;
            }
            else if (HierarchyLabelScript.presetID == 4)
            {
                HierarchyLabelScript.labelColor = Color.cyan;
                HierarchyLabelScript.textColor = Color.white;
            }
            else if (HierarchyLabelScript.presetID == 5)
            {
                HierarchyLabelScript.labelColor = Color.magenta;
                HierarchyLabelScript.textColor = Color.black;
            }
            else if (HierarchyLabelScript.presetID == 6)
            {
                HierarchyLabelScript.labelColor = Color.white;
                HierarchyLabelScript.textColor = Color.red;
            }
            else if (HierarchyLabelScript.presetID == 7)
            {
                HierarchyLabelScript.labelColor = Color.gray;
                HierarchyLabelScript.textColor = Color.white;
            }
            else if (HierarchyLabelScript.presetID == 8)
            {
                HierarchyLabelScript.labelColor = Color.black;
                HierarchyLabelScript.textColor = Color.red;
            }
            else if (HierarchyLabelScript.presetID == 9)
            {
                HierarchyLabelScript.labelColor = HierarchyLabelScript.customLabelColor;
                HierarchyLabelScript.textColor = HierarchyLabelScript.customTextColor;
            }
            else
            {
                HierarchyLabelScript.labelColor = Color.black;
                HierarchyLabelScript.textColor = Color.white;
            }
        }
        // 存在しないテーマ番号の場合
        else
        {
            if (HierarchyLabelScript.presetID == 9)
            {
                HierarchyLabelScript.labelColor = HierarchyLabelScript.customLabelColor;
                HierarchyLabelScript.textColor = HierarchyLabelScript.customTextColor;
            }
            else
            {
                HierarchyLabelScript.labelColor = Color.magenta;
                HierarchyLabelScript.textColor = Color.magenta;
            }
        }
    }

    // テーマ番号を読み込み
    private static void LoadSettings(HierarchyLabel HierarchyLabelScript)
    {
        //string ForceEnglish = EditorPrefs.GetBool("Praecipua_English");

        HierarchyLabelScript.HLThemeStr = EditorUserSettings.GetConfigValue(HierarchyLabelScript.HLThemeKey);
        HierarchyLabelScript.themeID = HierarchyLabelScript.HLThemeStr == null ? 0 : System.Int32.Parse(HierarchyLabelScript.HLThemeStr);
    }

    // テーマ番号を保存
    private static void SaveSettings(HierarchyLabel HierarchyLabelScript)
    {
        EditorUserSettings.SetConfigValue(HierarchyLabelScript.HLThemeKey, HierarchyLabelScript.themeID.ToString());
    }
}

[InitializeOnLoad]
public class HierarchyColor
{
    private static GUIStyle textStyle;
    private static GUIStyleState textStyleState;

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
            HierarchyLabel HierarchyLabelScript = gameObject.GetComponent<HierarchyLabel>();
            if (HierarchyLabelScript != null)
            {
                textStyle = new GUIStyle();
                textStyleState = new GUIStyleState();
                textStyleState.textColor = HierarchyLabelScript.textColor;
                textStyle.normal = textStyleState;

                Rect textRect = new Rect(rect);
                Vector2 labelSize = textStyle.CalcSize(new GUIContent(gameObject.name));

                if (HierarchyLabelScript.textBold && HierarchyLabelScript.textItalic)
                {
                    textStyle.fontStyle = FontStyle.BoldAndItalic;
                }
                else if (HierarchyLabelScript.textBold && !HierarchyLabelScript.textItalic)
                {
                    textStyle.fontStyle = FontStyle.Bold;
                }
                else if (!HierarchyLabelScript.textBold && HierarchyLabelScript.textItalic)
                {
                    textStyle.fontStyle = FontStyle.Italic;
                }
                else
                {
                    textStyle.fontStyle = FontStyle.Normal;
                }

                if (HierarchyLabelScript.textPosition == 0)
                {
                    textStyle.alignment = TextAnchor.MiddleLeft;
                    textRect.x = rect.x + 8;
                }
                else if (HierarchyLabelScript.textPosition == 1)
                {
                    textStyle.alignment = TextAnchor.MiddleCenter;
                }
                else if (HierarchyLabelScript.textPosition == 2)
                {
                    textStyle.alignment = TextAnchor.MiddleRight;
                    textRect.x = rect.x - 8;
                }

                EditorGUI.DrawRect(rect, HierarchyLabelScript.labelColor);
                GUI.Label(textRect, gameObject.name, textStyle);
            }
        }
    }
}
#endif
