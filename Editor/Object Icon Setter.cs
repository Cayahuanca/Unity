using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class IconSetter : EditorWindow
{
    private static bool ForceEnglish;

    private int IconMode = 1;
    private int objectCount = 1;

    private string searchText = "";

    private GameObject[] targetObjects = new GameObject[1];
    private Texture2D icon;
    private Vector2 scrollPosIcn;
    private Vector2 scrollPosObj;

    [MenuItem("Window/Object Icon Setter")]
    public static void ShowWindow()
    {
        GetWindow<IconSetter>("Icon Setter");
    }

public static class GUILayoutEx
    {
        public static GUILayoutOption[] GetOptionsForIconSelection(Texture2D[] textures)
        {
            int columnCount = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / 70f);
            int rowCount = Mathf.CeilToInt(textures.Length / (float)columnCount);

            var options = new GUILayoutOption[]
            {
                GUILayout.Width(columnCount * 70f),
                GUILayout.Height(rowCount * 70f),
            };

            return options;
        }
    }

    private void OnGUI()
    {
        LoadSettings();
    
        string SelectIconText = "Select the icon you want to use";
        string SelectObjectText = "Select the object you want to apply the icon to";
        string ObjectCountText = "Number of objects";
        string ResetText = "Reset";
        string ApplyText = "Apply Icon";
        string ExportBuiltInIconsText = "Export Built-in Icons (Generate about 1500 icon files). It needs some times.";

        string folderPath = "Assets/Praecipua/Icons";

        CultureInfo ci = CultureInfo.InstalledUICulture;
        string lang = ci.Name;

        if (ForceEnglish == false && lang == "ja-JP")
        {
            SelectIconText = "アイコンを選択";
            SelectObjectText = "アイコンを設定するオブジェクト";
            ObjectCountText = "アイコンを適用するオブジェクトの数";
            ResetText = "オブジェクトの選択を解除";
            ApplyText = "適用";
            ExportBuiltInIconsText = "ビルトインアイコン(約1500枚)を書き出す 少し時間がかかります";
        }

        GUILayout.Label(SelectIconText, EditorStyles.boldLabel);

        string[] IconModeText = { "Main Folder", "Builtin Icon", "Folder 1", "Folder 2", "Folder 3" };
        IconMode = GUILayout.SelectionGrid(IconMode, IconModeText,  5);
    
        if(IconMode == 1)
        {
            if (GUILayout.Button(ExportBuiltInIconsText))
            {
                WriteIcon();
                WriteIcon2();
            }
        }
    
        Texture2D[] icons = new Texture2D[0];
        if(IconMode == 0)
        {
            folderPath = "Assets/Praecipua/Icons";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }
            string[] iconsPaths = AssetDatabase.FindAssets("t:Texture2D", new[] {folderPath})
                                                .Select(AssetDatabase.GUIDToAssetPath).ToArray();
            icons = iconsPaths.Select(AssetDatabase.LoadAssetAtPath<Texture2D>).ToArray();
        }

        else if(IconMode == 1)
        {
            string[] iconFolders = { "Assets/Praecipua/UnityIcons/1", "Assets/Praecipua/UnityIcons/2" };
            foreach (string iconFolders1 in iconFolders)
            {
                if (!Directory.Exists(iconFolders1))
                {
                    Directory.CreateDirectory(iconFolders1);
                    AssetDatabase.Refresh();
                }
            }
            List<string> iconsPathsList = new List<string>();

            foreach (string iconFolder in iconFolders)
            {
                string[] paths = AssetDatabase.FindAssets("t:Texture2D", new[] {iconFolder})
                                            .Select(AssetDatabase.GUIDToAssetPath)
                                            .ToArray();
                iconsPathsList.AddRange(paths);
            }

            string[] iconsPaths = iconsPathsList.ToArray();
            icons = iconsPaths.Select(AssetDatabase.LoadAssetAtPath<Texture2D>).ToArray();
        }

        else if(IconMode == 2)
        {
            folderPath = "Assets/Praecipua/Icons/1";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }
            string[] iconsPaths = AssetDatabase.FindAssets("t:Texture2D", new[] {folderPath})
                                                .Select(AssetDatabase.GUIDToAssetPath).ToArray();
            icons = iconsPaths.Select(AssetDatabase.LoadAssetAtPath<Texture2D>).ToArray();
        }

        else if(IconMode == 3)
        {
            folderPath = "Assets/Praecipua/Icons/2";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }
            string[] iconsPaths = AssetDatabase.FindAssets("t:Texture2D", new[] {folderPath})
                                                .Select(AssetDatabase.GUIDToAssetPath).ToArray();
            icons = iconsPaths.Select(AssetDatabase.LoadAssetAtPath<Texture2D>).ToArray();
        }

        else if(IconMode == 4)
        {
            folderPath = "Assets/Praecipua/Icons/3";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }
            string[] iconsPaths = AssetDatabase.FindAssets("t:Texture2D", new[] {folderPath})
                                                .Select(AssetDatabase.GUIDToAssetPath).ToArray();
            icons = iconsPaths.Select(AssetDatabase.LoadAssetAtPath<Texture2D>).ToArray();

        }

        // Texture2D を検索
        GUILayout.Space(10);
        searchText = EditorGUILayout.TextField("Search Icons", searchText);
        Texture2D[] filteredIcons = icons.Where(x => x.name.ToLower().Contains(searchText.ToLower())).ToArray();

        // それらの Texture2D をスクロール表示
        scrollPosIcn = GUILayout.BeginScrollView(scrollPosIcn);
        GUILayout.BeginHorizontal();
        int count = 0;
        foreach (Texture2D tex in filteredIcons)
        {
            if (GUILayout.Button(tex, GUILayout.Width(60), GUILayout.Height(60)))
            {
                icon = tex;
            }

            // 10枚ごとに改行
            count++;
            if (count % 10 == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();

        GUILayout.Space(10);
        if (icon != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Selected Icon: " + icon.name);
            GUILayout.Label("Path: " + AssetDatabase.GetAssetPath(icon));
            GUILayout.EndVertical();
            GUILayout.Box(icon, GUILayout.Width(60), GUILayout.Height(60));
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        GUILayout.Label(SelectObjectText, EditorStyles.boldLabel);

        // 適用するオブジェクトの数を変更、リセット
        GUILayout.BeginHorizontal();
        GUILayout.Label(ObjectCountText);
        objectCount = EditorGUILayout.IntField(objectCount, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        if (objectCount < 1)
        {
            objectCount = 1;
        }
        if (objectCount != targetObjects.Length)
        {

            targetObjects = new GameObject[objectCount];
        }
        scrollPosObj = EditorGUILayout.BeginScrollView(scrollPosObj, GUILayout.Height(100));
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetObjects[i] = (GameObject)EditorGUILayout.ObjectField(targetObjects[i], typeof(GameObject), true);
        }
        EditorGUILayout.EndScrollView();
        
        if (GUILayout.Button(ResetText))
        {
            targetObjects = new GameObject[1];
            objectCount = 1;
        }


        GUILayout.Space(20);

        if (GUILayout.Button(ApplyText))
        {
            ApplyIcon();
        }
        GUILayout.Space(20);
    }

    private void ApplyIcon()
    {
        if (icon == null || targetObjects == null || targetObjects.Length == 0) return;

        var iconPath = AssetDatabase.GetAssetPath(icon);
        var iconGUID = AssetDatabase.AssetPathToGUID(iconPath);

        foreach (var targetObject in targetObjects)
        {
            if (targetObject == null) continue;

            SerializedObject serializedObject = new SerializedObject(targetObject);
            SerializedProperty serializedProperty = serializedObject.FindProperty("m_Icon");

            if (serializedProperty != null && serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                serializedProperty.objectReferenceValue = AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture2D));
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }

    private static void WriteIcon()
    {
        // 保存先のフォルダパス
        string folderPath = "Assets/Praecipua/UnityIcons/1";
        // フォルダがなければ作成
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        Dictionary<string, bool> foldouts = new Dictionary<string, bool>();
        // アイコンを取得
        Texture2D[] icons;
        icons = Resources.FindObjectsOfTypeAll(typeof(Texture2D))
            .Where(x => AssetDatabase.GetAssetPath(x) == "Library/unity editor resources") //アイコンのAssetPathを取得すると全てこれ
            .Select(x => x.name)    //同一名で複数のアイコンが存在する場合があるので
            .Distinct()             //重複を除去
            .OrderBy(x => x)
            .Select(x => EditorGUIUtility.Load(x) as Texture2D)
            .Where(x => x)          //FontTextureなど、ロードできないものを除外
            .ToArray();

        // 各アイコンに対して
        foreach (Texture2D icon in icons)
        {
            // PNG形式に変換
            var pngData = new Texture2D(icon.width, icon.height, icon.format, icon.mipmapCount > 1);
                        Graphics.CopyTexture(icon, pngData);

            if (pngData != null)
            {
                if(!icon.name.Contains("@"))
                {
                    if(!icon.name.Contains(".sml"))
                    {
                        if(!icon.name.Contains("d_"))
                        {
                            if(!icon.name.Contains(" On"))
                            {
                                // ファイルパスを生成
                                string filePath = Path.Combine(folderPath, "zzz_icn_" + icon.name + ".png");
                                // ファイルに書き込み
                                File.WriteAllBytes(filePath, pngData.EncodeToPNG());
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        // アセットの更新
        //AssetDatabase.Refresh();

        //Debug.Log("All icons have been exported successfully.");
    }

    private static void WriteIcon2()
    {
        // 保存先のフォルダパス
        string folderPath = "Assets/Praecipua/UnityIcons/2";
        // フォルダがなければ作成
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // アイコン名をファイルから読み込み
        string filePath = "Assets/Praecipua/Editor/IconName.txt";
        string[] iconNames = File.ReadAllLines(filePath);

        foreach (string iconName in iconNames)
        {
            if (string.IsNullOrEmpty(iconName))
            {
                continue;
            }

            // アイコンの取得
            var icons = EditorGUIUtility.IconContent(iconName);

            // アイコンが存在する場合
            if (icons != null && icons.image != null)
            {
                // アイコンの取得
                var content = icons.image;
                var iconTexture = (Texture2D)content;

                if (iconTexture != null)
                {
                    // PNG形式に変換
                    var pngData = new Texture2D(iconTexture.width, iconTexture.height, iconTexture.format, iconTexture.mipmapCount > 1);
                    Graphics.CopyTexture(iconTexture, pngData);

                    // ファイルパスを生成
                    string iconFilePath = Path.Combine(folderPath, "zzz_icn_" + iconName + ".png");

                    // ファイルに書き込み
                    File.WriteAllBytes(iconFilePath, pngData.EncodeToPNG());
                }
            }
        }

        // アセットの更新
        AssetDatabase.Refresh();

        Debug.Log("All icons have been exported successfully.");
    }

    private static void LoadSettings()
    {
        ForceEnglish = EditorPrefs.GetBool("Praecipua_English");
    }
}
