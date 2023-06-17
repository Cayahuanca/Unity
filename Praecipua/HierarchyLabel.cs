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
    public int textStyle = 1;
    public int textPosition = 1;
    public int presetID;
    public int themeID;
    public string HLThemeKey = "Hierarchy_Label_Theme";
    public string HLThemeStr;
}

[CustomEditor(typeof(HierarchyLabel))]
public class HierarchyLabelScriptEditor : Editor
{
    // Inspector 表示時またはオブジェクト選択時に呼ばれる
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

        
        EditorGUILayout.BeginVertical();

            EditorGUI.BeginChangeCheck();

                // テーマの設定
                EditorGUILayout.LabelField("Theme");

                EditorGUILayout.BeginHorizontal();
                    switch (GUILayout.Toolbar(HierarchyLabelScript.themeID, new string[] { "Default", "Pastel", "CUD" }))
                    {
                        case 0:
                            HierarchyLabelScript.themeID = 0;
                            break;
                        case 1:
                            HierarchyLabelScript.themeID = 1;
                            break;
                        case 2:
                            HierarchyLabelScript.themeID = 2;
                            break;
                        default:
                            HierarchyLabelScript.themeID = 0;
                            break;
                    }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                // プリセットカラーの設定
                EditorGUILayout.LabelField("Color Presets");

                string[] colors;
                switch (HierarchyLabelScript.themeID)
                {
                    case 0:
                        colors = new string[] { "Red", "Green", "Blue", "Yellow", "Cyan", "Magenta", "White", "Gray", "Black", "None" };
                        break;
                    case 1:
                        colors = new string[] { "Yellow", "Orange", "Rose", "Pink", "Purple", "Blue", "Osian", "Blue", "Aqua", "Green" };
                        break;
                    case 2:
                        colors = new string[] { "White", "Black", "Light Gray", "Dark Gray", "Red", "Yellow", "Green", "Blue", "Orange", "Brown" };
                        break;
                    default:
                        colors = new string[] { "Error", "Error", "Error", "Error", "Error", "Error", "Error", "Error", "Error", "Error" };
                        break;
                }

                for (int i = 0; i < 2; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = i * 5; j < i * 5 + 5; j++)
                    {
                        if (GUILayout.Button(colors[j]))
                        {
                            HierarchyLabelScript.presetID = j;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

            if (EditorGUI.EndChangeCheck())
            {
                SetColor(HierarchyLabelScript);
            }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // カスタムカラーの設定
        EditorGUILayout.LabelField("Custom Color");
        
        HierarchyLabelScript.customLabelColor = HierarchyLabelScript.labelColor;
        HierarchyLabelScript.customTextColor = HierarchyLabelScript.textColor;

        EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel++;
                HierarchyLabelScript.customLabelColor = EditorGUILayout.ColorField("Label Color", HierarchyLabelScript.customLabelColor);
                HierarchyLabelScript.customTextColor = EditorGUILayout.ColorField("Text Color", HierarchyLabelScript.customTextColor);
            EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
        {
            HierarchyLabelScript.customLabelColor.a = 1.0f;
            HierarchyLabelScript.customTextColor.a = 1.0f;
            HierarchyLabelScript.presetID = 11;
            SetColor(HierarchyLabelScript);
        }

        EditorGUILayout.Space();

        //文字の書体、位置の設定
        EditorGUILayout.LabelField("Text Style & Position");

        EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

                switch (GUILayout.Toolbar(HierarchyLabelScript.textStyle, new string[] { "Normal", "Bold", "Italic", "Bold Italic" }))
                {
                    case 0:
                        HierarchyLabelScript.textStyle = 0;
                        break;
                    case 1:
                        HierarchyLabelScript.textStyle = 1;
                        break;
                    case 2:
                        HierarchyLabelScript.textStyle = 2;
                        break;
                    case 3:
                        HierarchyLabelScript.textStyle = 3;
                        break;
                    default:
                        HierarchyLabelScript.textStyle = 0;
                        break;
                }

                switch (GUILayout.Toolbar(HierarchyLabelScript.textPosition, new string[] { "Left", "Center", "Right" }))
                {
                    case 0:
                        HierarchyLabelScript.textPosition = 0;
                        break;
                    case 1:
                        HierarchyLabelScript.textPosition = 1;
                        break;
                    case 2:
                        HierarchyLabelScript.textPosition = 2;
                        break;
                }

            EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
        EditorApplication.RepaintHierarchyWindow();
    }

    // テーマ、プリセットに応じた色の適用
    private void SetColor(HierarchyLabel HierarchyLabelScript)
    {
        switch (HierarchyLabelScript.themeID)
        {
            // テーマ0: Default
            case 0:
                switch (HierarchyLabelScript.presetID)
                {
                    case 0:
                        HierarchyLabelScript.labelColor = Color.red;
                        HierarchyLabelScript.textColor = Color.white;
                        break;
                    case 1:
                        HierarchyLabelScript.labelColor = Color.green;
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 2:
                        HierarchyLabelScript.labelColor = Color.blue;
                        HierarchyLabelScript.textColor = Color.white;
                        break;
                    case 3:
                        HierarchyLabelScript.labelColor = Color.yellow;
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 4:
                        HierarchyLabelScript.labelColor = Color.cyan;
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 5:
                        HierarchyLabelScript.labelColor = Color.magenta;
                        HierarchyLabelScript.textColor = Color.white;
                        break;
                    case 6:
                        HierarchyLabelScript.labelColor = Color.white;
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 7:
                        HierarchyLabelScript.labelColor = Color.gray;
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 8:
                        HierarchyLabelScript.labelColor = Color.black;
                        HierarchyLabelScript.textColor = Color.white;
                        break;
                    case 9:
                        //HierarchyLabelScript.labelColor = Color.;
                        //HierarchyLabelScript.textColor = Color.;
                        break;
                    case 11:
                        HierarchyLabelScript.labelColor = HierarchyLabelScript.customLabelColor;
                        HierarchyLabelScript.textColor = HierarchyLabelScript.customTextColor;
                        break;
                    default:
                        HierarchyLabelScript.labelColor = Color.white;
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                }
                break;

            // テーマ1
            //Color Palette from this website. https://coolors.co/palette/fbf8cc-fde4cf-ffcfd2-f1c0e8-cfbaf0-a3c4f3-90dbf4-8eecf5-98f5e1-b9fbc0
            case 1:
                switch (HierarchyLabelScript.presetID)
                {
                    case 0:
                        HierarchyLabelScript.labelColor = new Color(251f / 255f, 248f / 255f, 204f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 1:
                        HierarchyLabelScript.labelColor = new Color(253f / 255f, 228f / 255f, 207f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 2:
                        HierarchyLabelScript.labelColor = new Color(255f / 255f, 207f / 255f, 210f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 3:
                        HierarchyLabelScript.labelColor = new Color(241f / 255f, 192f / 255f, 232f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 4:
                        HierarchyLabelScript.labelColor = new Color(207f / 255f, 186f / 255f, 240f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 5:
                        HierarchyLabelScript.labelColor = new Color(163f / 255f, 196f / 255f, 243f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 6:
                        HierarchyLabelScript.labelColor = new Color(144f / 255f, 219f / 255f, 244f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 7:
                        HierarchyLabelScript.labelColor = new Color(142f / 255f, 236f / 255f, 245f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 8:
                        HierarchyLabelScript.labelColor = new Color(152f / 255f, 245f / 255f, 225f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 9:
                        HierarchyLabelScript.labelColor = new Color(185f / 255f, 251f / 255f, 192f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 11:
                        HierarchyLabelScript.labelColor = HierarchyLabelScript.customLabelColor;
                        HierarchyLabelScript.textColor = HierarchyLabelScript.customTextColor;
                        break;
                    default:
                        HierarchyLabelScript.labelColor = Color.white;
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                }
                break;

            // テーマ2
            //Color Palette from CUD Color GuideBook v4.
            case 2:
                switch (HierarchyLabelScript.presetID)
                {
                    case 0:
                        HierarchyLabelScript.labelColor = Color.white;
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 1:
                        HierarchyLabelScript.labelColor = Color.black;
                        HierarchyLabelScript.textColor = Color.white;
                        break;
                    case 2:
                        HierarchyLabelScript.labelColor = new Color(200f / 255f, 200f / 255f, 203f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 3:
                        HierarchyLabelScript.labelColor = new Color(132f / 255f, 145f / 255f, 158f / 255f);
                        HierarchyLabelScript.textColor = Color.white;
                        break;
                    case 4:
                        HierarchyLabelScript.labelColor = new Color(255f / 255f, 75f / 255f, 0f / 255f);
                        HierarchyLabelScript.textColor = Color.white;
                        break;
                    case 5:
                        HierarchyLabelScript.labelColor = new Color(255f / 255f, 241f / 255f, 0f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 6:
                        HierarchyLabelScript.labelColor = new Color(3f / 255f, 175f / 255f, 122f / 255f);
                        HierarchyLabelScript.textColor = Color.white;
                        break;
                    case 7:
                        HierarchyLabelScript.labelColor = new Color(0f / 255f, 90f / 255f, 255f / 255f);
                        HierarchyLabelScript.textColor = Color.white;
                        break;
                    case 8:
                        HierarchyLabelScript.labelColor = new Color(246f / 255f, 170f / 255f, 0f / 255f);
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                    case 9:
                        HierarchyLabelScript.labelColor = new Color(128f / 255f, 64f / 255f, 0f / 255f);
                        HierarchyLabelScript.textColor = Color.white;
                        break;
                    case 11:
                        HierarchyLabelScript.labelColor = HierarchyLabelScript.customLabelColor;
                        HierarchyLabelScript.textColor = HierarchyLabelScript.customTextColor;
                        break;
                    default:
                        HierarchyLabelScript.labelColor = Color.white;
                        HierarchyLabelScript.textColor = Color.black;
                        break;
                }
                break;

            // 存在しないテーマ番号の場合
            default:
                switch (HierarchyLabelScript.presetID)
                {
                    case 11:
                        HierarchyLabelScript.labelColor = HierarchyLabelScript.customLabelColor;
                        HierarchyLabelScript.textColor = HierarchyLabelScript.customTextColor;
                        break;
                    default:
                        HierarchyLabelScript.labelColor = Color.magenta;
                        HierarchyLabelScript.textColor = Color.magenta;
                        break;
                }
                break;
        }

        SaveSettings(HierarchyLabelScript);
    }

    // テーマ番号を読み込み
    private static void LoadSettings(HierarchyLabel HierarchyLabelScript)
    {
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

        // GameObject 及び Hierarchy Label のコンポーネントがない場合は、何もしない
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject == null) return;
        HierarchyLabel HierarchyLabelScript = gameObject.GetComponent<HierarchyLabel>();
        if (HierarchyLabelScript == null) return;

        // 親オブジェクトの数に応じて、ラベルの左端位置を調整
        int hierarchyLevel = 0;
        Transform parentTransform = gameObject.transform.parent;
        while (parentTransform != null)
        {
            hierarchyLevel++;
            parentTransform = parentTransform.parent;
        }

        if (hierarchyLevel != 0)
        {
            rect.x = rect.x + 24 + (hierarchyLevel * 16);
        }    

        // 文字の書体、位置を設定
        textStyle = new GUIStyle();
        textStyleState = new GUIStyleState();
        textStyleState.textColor = HierarchyLabelScript.textColor;
        textStyle.normal = textStyleState;

        Rect textRect = new Rect(rect);
        Vector2 labelSize = textStyle.CalcSize(new GUIContent(gameObject.name));

        switch (HierarchyLabelScript.textStyle)
        {
            case 0:
                textStyle.fontStyle = FontStyle.Normal;
                break;
            case 1:
                textStyle.fontStyle = FontStyle.Bold;
                break;
            case 2:
                textStyle.fontStyle = FontStyle.Italic;
                break;
            case 3:
                textStyle.fontStyle = FontStyle.BoldAndItalic;
                break;
            default:
                textStyle.fontStyle = FontStyle.Normal;
                break;
        }

        switch (HierarchyLabelScript.textPosition)
        {
            case 0:
                textStyle.alignment = TextAnchor.MiddleLeft;
                textRect.x = rect.x + 8;
                break;
            case 1:
                textStyle.alignment = TextAnchor.MiddleCenter;
                break;
            case 2:
                textStyle.alignment = TextAnchor.MiddleRight;
                textRect.x = rect.x - 8;
                break;
            default:
                textStyle.alignment = TextAnchor.MiddleCenter;
                break;
        }

        // ラベルを描画
        EditorGUI.DrawRect(rect, HierarchyLabelScript.labelColor);
        GUI.Label(textRect, gameObject.name, textStyle);
    }
}
#endif