using UnityEngine;
using UnityEditor;
using System.IO;

public class GameObjectIconWindow : EditorWindow
{
    private GameObject[] selectedGameObjects;
    private Texture2D[] icons;
    private int selectedIconIndex;

    private const string iconFolderPath = "Assets/Praecipua/Icons/Quick/";

    [MenuItem("GameObject/Quick Icon", false, 20)]
    private static void ShowWindow()
    {
        var selectedGameObjects = Selection.gameObjects;
        if (selectedGameObjects != null && selectedGameObjects.Length > 0)
        {
            var window = GetWindow<GameObjectIconWindow>();
            window.titleContent = new GUIContent("Quick Icon");
            window.Show();
        }
    }

    private void OnEnable()
    {
        if (!Directory.Exists(iconFolderPath))
        {
            Directory.CreateDirectory(iconFolderPath);
            AssetDatabase.Refresh();
        }

        icons = new Texture2D[10];
        for (int i = 0; i < 10; i++)
        {
            string iconPath = iconFolderPath + "zzz_" + (i + 1) + ".png";
            icons[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        }
    }

    private void OnGUI()
    {
        selectedGameObjects = Selection.gameObjects;

        if (selectedGameObjects != null)
        {
            GUILayout.Label("Select an icon to apply:");
            GUILayout.Space(10);

            int iconSize = 50;
            int iconsPerRow = Mathf.FloorToInt(position.width / iconSize);
            int iconRows = Mathf.CeilToInt(icons.Length / (float)iconsPerRow);

            for (int i = 0; i < iconRows; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < iconsPerRow; j++)
                {
                    int index = i * iconsPerRow + j;
                    if (index >= icons.Length) break;

                    Texture2D icon = icons[index];
                    if (icon == null) continue;
                    Rect iconRect = GUILayoutUtility.GetRect(iconSize, iconSize, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                    GUI.DrawTexture(iconRect, icon);

                    if (Event.current.type == EventType.MouseDown && iconRect.Contains(Event.current.mousePosition))
                    {
                        selectedIconIndex = index;
                        foreach (var selectedGameObject in selectedGameObjects)
                        {
                            ApplySelectedIcon(selectedGameObject);
                        }
                        Close();
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Reset Icon"))
            {
                foreach (var selectedGameObject in selectedGameObjects)
                {
                    ResetIcon(selectedGameObject);
                }
                Close();
            }
        }
        else
        {
            GUILayout.Label("Select a GameObject in the Hierarchy and open the Quick Icon window.");
        }
    }

    private void ApplySelectedIcon(GameObject selectedGameObject)
    {
        string iconPath = iconFolderPath + "zzz_" + (selectedIconIndex + 1) + ".png";
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

        if (icon != null && selectedGameObject != null)
        {
            var serializedObject = new SerializedObject(selectedGameObject);
            var iconProperty = serializedObject.FindProperty("m_Icon");
            iconProperty.objectReferenceValue = AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture2D));
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private void ResetIcon(GameObject selectedGameObject)
    {
        var serializedObject = new SerializedObject(selectedGameObject);
        var iconProperty = serializedObject.FindProperty("m_Icon");
        iconProperty.objectReferenceValue = null;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }
}
