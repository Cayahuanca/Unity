using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Praecipua.EE
{
    public class BookmarkPicker : EditorWindow
    {
        private GameObject targetObject;
        private string jsonFilePath;
        private List<BookmarkDataBooks> bookmarkDataBooksList = new List<BookmarkDataBooks>();
        private List<BookmarkDataObjs> bookmarkDataObjsList = new List<BookmarkDataObjs>();
        private Texture backgroundTexture;
        private int selectedBookmarkIndex = -1;
        private int selectionMethod = 0;

        private Vector2 dragStartPos;
        private Vector2 dragEndPos;
        private Vector2 positionDiff;
        private bool isDragging = false;
        private bool isBookmarkLabelExist = false;
        private int draggingBookmarkIndex = -1;
        private BookmarkDataBooks editedBookmark;

        [MenuItem("Window/Praecipua/Bookmark Picker")]
        public static void ShowWindow()
        {
            GetWindow<BookmarkPicker>("Bookmark Picker");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Bookmark Picker", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            GUILayout.Label("Select the file in popup window or Right click the Json file in project window and select Copy Path, then paste it here.");

            EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Create Json File"))
                {
                    string jsonDirPath = "Assets/Praecipua/Bookmarks";
	                if (!Directory.Exists(jsonDirPath))
	                {
	                    Directory.CreateDirectory(jsonDirPath);
	                    // AssetDatabase.Refresh();
	                }

                    string newJsonFilePath = EditorUtility.SaveFilePanel("Save JSON File", "Assets/Praecipua/Bookmarks", "NewBookmark", "json");
                    if (!string.IsNullOrEmpty(newJsonFilePath))
                    {
                        BookmarkEditor bookmarkEditor = ScriptableObject.CreateInstance<BookmarkEditor>();
                        bool createSuccess = bookmarkEditor.CreateJsonFile(newJsonFilePath);
                        if (createSuccess )
                        {
                            jsonFilePath = newJsonFilePath;
                            LoadJson();
                        }
                    }
                }

                if (GUILayout.Button("Open Json File"))
                {
                    string jsonDirPath = "Assets/Praecipua/Bookmarks";
                    if (!Directory.Exists(jsonDirPath))
	                {
	                    jsonDirPath = Application.dataPath;
	                }

                    string newJsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", jsonDirPath, "json");
                    if (!string.IsNullOrEmpty(newJsonFilePath))
                    {
                        jsonFilePath = newJsonFilePath;
                        LoadJson();
                    }
                }

                if (GUILayout.Button("Reload JSON"))
                {
                    LoadJson();
                }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("JSON File Path: " +  jsonFilePath);

            EditorGUILayout.Space();

            targetObject = EditorGUILayout.ObjectField("Root Object", targetObject, typeof(GameObject), true) as GameObject;

            EditorGUILayout.Space();

            Rect backgroundTextureFieldRect = EditorGUILayout.GetControlRect();
            backgroundTexture = (Texture)EditorGUI.ObjectField(new Rect(backgroundTextureFieldRect.position, new Vector2(backgroundTextureFieldRect.width, EditorGUIUtility.singleLineHeight)), "Background Texture", backgroundTexture, typeof(Texture), false);


            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Selection Method");
                selectionMethod = GUILayout.SelectionGrid(selectionMethod, new string[] { "By Relative Path", "By Instance ID"}, 2);
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Bookmarks:");

            EditorGUILayout.Space();

            GUILayout.Label("Left click to select a bookmark.");
            GUILayout.Label("Right click to edit a bookmark.");
            GUILayout.Label("Ctrl + Right click to add a bookmark.");
            GUILayout.Label("Ctrl + Left click to drag a bookmark.");

            // ウィンドウの高さと幅が最大値
            Rect bookmarksArea = GUILayoutUtility.GetRect(100, 500, position.width, position.width);

            // 背景画像を描画
            if (backgroundTexture != null)
            {
                // backgroundTexture の透明度を設定
                Color backgroundColor = GUI.color;
                backgroundColor.a = 0.5f;
                GUI.color = backgroundColor;

                Rect backgroundPos = new Rect(bookmarksArea.xMin, bookmarksArea.yMin, backgroundTexture.width, backgroundTexture.height);
                GUI.DrawTexture(backgroundPos, backgroundTexture, ScaleMode.ScaleToFit);
            }
            // グリッド線を描画
            DrawBackgroundGrid(bookmarksArea, 50f, 0.2f, Color.gray);

            foreach (var bookmark in bookmarkDataBooksList)
            {
                EditorGUILayout.Space();

                // 座標情報を取得
                int xCoordinate = bookmark.xCoordinate;
                int yCoordinate = bookmark.yCoordinate;

                // ウィンドウ内の座標に変換
                float radius = 10f;
                Rect rect = new Rect(xCoordinate, yCoordinate, radius, radius);
                rect.position += bookmarksArea.position;

                // ラベルの表示設定
                // if (bookmark.showLabel == 2)
                // {
                    GUIContent labelContent = new GUIContent(bookmark.labelText);
                    GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                    labelStyle.normal.textColor = GetContrastColor(bookmark.labelColor);
                    labelStyle.fontStyle = FontStyle.Bold;
                    float labelWidth = labelStyle.CalcSize(labelContent).x;
                    // rect.size = new Vector2(labelWidth + 10f, rect.size.y);
                    labelStyle.alignment = TextAnchor.MiddleCenter;
                    // EditorGUI.LabelField(rect, labelContent, labelStyle);
                // }

                switch (bookmark.labelType)
                {
                    case 0: // 初期の未指定の場合, 1つのオブジェクトにリンクされている場合は Circle, それ以外は Square
                        rect.size = new Vector2(radius * 2, radius * 2);
                        if (bookmark.linkedObjectIds.Count == 1)
                        {
                            DrawCircle(rect, bookmark.labelColor, radius);
                        }
                        else
                        {
                            EditorGUI.DrawRect(rect, bookmark.labelColor);
                        }
                        break;
                    case 1: // Small Circle
                        rect.size = new Vector2(radius, radius);
                        DrawCircle(rect, bookmark.labelColor, radius / 2);
                        break;
                    case 2: // Medium Circle
                        rect.size = new Vector2(radius * 2, radius * 2);
                        DrawCircle(rect, bookmark.labelColor, radius);
                        break;
                    case 3: // Large Circle
                        rect.size = new Vector2(radius * 4, radius * 4);
                        DrawCircle(rect, bookmark.labelColor, radius * 2);
                        break;
                    case 4: // Small Square
                        rect.size = new Vector2(radius, radius);
                        EditorGUI.DrawRect(rect, bookmark.labelColor);
                        break;
                    case 5: // Medium Square
                        rect.size = new Vector2(radius * 2, radius * 2);
                        EditorGUI.DrawRect(rect, bookmark.labelColor);
                        break;
                    case 6: // Large Square
                        rect.size = new Vector2(radius * 4, radius * 4);
                        EditorGUI.DrawRect(rect, bookmark.labelColor);
                        break;
                    case 7: // Small Diamond
                        rect.size = new Vector2(radius, radius);
                        DrawDiamond(rect, bookmark.labelColor);
                        break;
                    case 8: // Medium Diamond
                        rect.size = new Vector2(radius * 2, radius * 2);
                        DrawDiamond(rect, bookmark.labelColor);
                        break;
                    case 9: // Large Diamond
                        rect.size = new Vector2(radius * 4, radius * 4);
                        DrawDiamond(rect, bookmark.labelColor);
                        break;
                    case 10: // Small Triangle
                        rect.size = new Vector2(radius, radius);
                        DrawTriangle(rect, bookmark.labelColor);
                        break;
                    case 11: // Medium Triangle
                        rect.size = new Vector2(radius * 2, radius * 2);
                        DrawTriangle(rect, bookmark.labelColor);
                        break;
                    case 12: // Large Triangle
                        rect.size = new Vector2(radius * 4, radius * 4);
                        DrawTriangle(rect, bookmark.labelColor);
                        break;
                    case 13: // Square with Text
                        if (labelWidth + 10f > radius * 2)
                        {
                            rect.size = new Vector2(labelWidth + 10f, radius * 2);
                        }
                        else
                        {
                            rect.size = new Vector2(radius * 2, radius * 2);
                        }
                        EditorGUI.DrawRect(rect, bookmark.labelColor);
                        EditorGUI.LabelField(rect, labelContent, labelStyle);
                        break;
                    default:
                        rect.size = new Vector2(radius * 2, radius * 2);
                        if (bookmark.linkedObjectIds.Count == 1)
                        {
                            DrawCircle(rect, bookmark.labelColor, radius);
                        }
                        else
                        {
                            EditorGUI.DrawRect(rect, bookmark.labelColor);
                        }
                        break;
                }

                // EditorGUI.DrawRect(rect, bookmark.labelColor); // 当たり判定の領域確認

                // ブックマークの選択
                if (Event.current.isMouse && Event.current.type == EventType.MouseDown)
                {
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        isBookmarkLabelExist = true;

                        if((Event.current.modifiers & EventModifiers.Control) == 0)
                        {
                            // 左クリックで、ブックマークのオブジェクトを選択
                            if (Event.current.button == 0)
                            {
                                selectedBookmarkIndex = bookmark.bookmarkIndex;
                                Debug.Log("Selected Bookmark: " + selectedBookmarkIndex);
                                SelectObjectByGUID(bookmark.linkedObjectIds.ToArray());
                            }
                            // 右クリックで、ブックマークの編集
                            else if (Event.current.button == 1)
                            {
                                BookmarkEditor.ShowWindowFilled(
                                    root: targetObject,
                                    jsonFilePath: jsonFilePath,
                                    bookmarkIndex: bookmark.bookmarkIndex,
                                    labelType: bookmark.labelType,
                                    showLabel: bookmark.showLabel,
                                    labelText: bookmark.labelText,
                                    // children: 省略
                                    labelColorR: bookmark.labelColor.r,
                                    labelColorG: bookmark.labelColor.g,
                                    labelColorB: bookmark.labelColor.b,
                                    labelColorA: bookmark.labelColor.a,
                                    labelPositionX: bookmark.xCoordinate,
                                    labelPositionY: bookmark.yCoordinate
                                );
                            }
                        }
                        // 左クリック + Ctrl でドラッグを開始
                        else if ((Event.current.modifiers & EventModifiers.Control) != 0)
                        {
                            if (Event.current.button == 0)
                            {
                                isDragging = true;
                                draggingBookmarkIndex = bookmark.bookmarkIndex;
                                dragStartPos = Event.current.mousePosition;
                                positionDiff = dragStartPos - rect.position;
                            }
                        }
                    }
                }

                // マウスを離したときの処理
                if (Event.current.isMouse && Event.current.type == EventType.MouseUp)
                {
                    // ブックマークのドラッグ中の処理
                    if (isDragging)
                    {
                        if (dragEndPos != dragStartPos)
                        {
                            isBookmarkLabelExist = false;
                            isDragging = false;
                            draggingBookmarkIndex = -1;
                            continue;
                        }

                        if (bookmarksArea.Contains(Event.current.mousePosition))
                        {
                            dragEndPos = Event.current.mousePosition;
                        }
                        else
                        {
                            dragEndPos = dragStartPos;
                        }

                        Vector2 movedPosition = dragEndPos - dragStartPos;
                        // Debug.Log("Dragged: " + movedPosition + ", " + "Drag Start: " + dragStartPos + ", " + "Drag End: " + dragEndPos);

                        Vector2 editedPosition = dragEndPos - positionDiff - bookmarksArea.position;
                        Vector2Int editedPositionInt = new Vector2Int((int)editedPosition.x, (int)editedPosition.y);
                        // Debug.Log("最初のずれ: " + positionDiff + ", " + "編集後の座標: " + editedPositionInt + ", " + "移動するブックマーク: " + draggingBookmarkIndex);

                        foreach (var draggingBookmark in bookmarkDataBooksList)
                        {
                            if (draggingBookmark.bookmarkIndex == draggingBookmarkIndex)
                            {
                                Debug.Log("Dragged Bookmark labelColor: " + draggingBookmark.labelColor);
                                Debug.Log("Dragged Bookmark labelText: " + draggingBookmark.labelText);
                                editedBookmark = draggingBookmark;
                                break; // 目的のブックマークを見つけたらループを終了します
                            }
                        }
                        //Debug.Log(editedBookmark.labelColor + ", " + editedBookmark.labelText + ", " + editedBookmark.labelType + ", " + editedBookmark.showLabel + ", " + editedBookmark.xCoordinate + ", " + editedBookmark.yCoordinate + ", " + editedBookmark.linkedObjectIds.Count);

                        BookmarkEditor bookmarkEditor = ScriptableObject.CreateInstance<BookmarkEditor>();
                        bool editSuccess = bookmarkEditor.BookmarkAddEditBooks(draggingBookmarkIndex, editedBookmark.labelType, editedBookmark.showLabel, editedBookmark.labelText, editedBookmark.labelColor, editedPositionInt, jsonFilePath);
                        if (editSuccess && jsonFilePath != null)
                        {
                            LoadJson();
                        }

                        isBookmarkLabelExist = false;
                        isDragging = false;
                        draggingBookmarkIndex = -1;
                    }
                    else
                    {
                        isBookmarkLabelExist = false;
                    }
                }
            }

            // ブックマークの追加
            // 右クリック + Ctrl でブックマークを追加
            if (Event.current.isMouse && Event.current.type == EventType.MouseDown)
            {
                if (bookmarksArea.Contains(Event.current.mousePosition) && !isBookmarkLabelExist)
                {
                    if (Event.current.button == 1 && (Event.current.modifiers & EventModifiers.Control) != 0)
                    {
                        BookmarkEditor bookmarkEditor = ScriptableObject.CreateInstance<BookmarkEditor>();
                        // 現在存在する BookmarkIndex の最大値を取得
                        int maxBookmarkIndex = 0;
                        foreach (var bookmark in bookmarkDataBooksList)
                        {
                            if (bookmark.bookmarkIndex > maxBookmarkIndex)
                            {
                                maxBookmarkIndex = bookmark.bookmarkIndex;
                            }
                        }

                        Vector2 createPosition = Event.current.mousePosition - bookmarksArea.position;
                        Vector2Int createPositionInt = new Vector2Int((int)createPosition.x, (int)createPosition.y);
                        bookmarkEditor.BookmarkAddEditBooks(maxBookmarkIndex + 1, 0, 0, "", Color.white, createPositionInt, jsonFilePath);
                        LoadJson();
                    }
                }
            }
        }

        private void LoadJson()
        {
            if (string.IsNullOrEmpty(jsonFilePath) || targetObject == null)
            {
                Debug.LogError("Please specify both JSON file path and target object.");
                return;
            }

            if (!File.Exists(jsonFilePath))
            {
                Debug.LogError("JSON file does not exist.");
                return;
            }

            string json = File.ReadAllText(jsonFilePath);
            BookmarkFile bookmarkFile = JsonUtility.FromJson<BookmarkFile>(json);

            if (bookmarkFile != null && bookmarkFile.bookmarks != null)
            {
                bookmarkDataBooksList = bookmarkFile.bookmarks;
            }
            else
            {
                Debug.LogError("Invalid JSON file.");
            }
            if (bookmarkFile != null && bookmarkFile.objects != null)
            {
                bookmarkDataObjsList = bookmarkFile.objects;
            }
            else
            {
                Debug.LogError("Invalid JSON file.");
            }
        }
        private void DrawCircle(Rect rect, Color color, float radius)
        {
            Vector2 center = rect.center;
            Handles.color = color;
            Handles.DrawSolidDisc(center, Vector3.forward, radius);
        }

        private void DrawDiamond(Rect rect, Color color)
        {
            Vector3[] diamondVertices = new Vector3[4];
            diamondVertices[0] = new Vector3(rect.center.x, rect.y);
            diamondVertices[1] = new Vector3(rect.x, rect.center.y);
            diamondVertices[2] = new Vector3(rect.center.x, rect.yMax);
            diamondVertices[3] = new Vector3(rect.xMax, rect.center.y);
            Handles.color = color;
            Handles.DrawSolidRectangleWithOutline(diamondVertices, color, color);
        }

        private void DrawTriangle(Rect rect, Color color)
        {
            Vector3[] triangleVertices = new Vector3[3];
            triangleVertices[0] = new Vector3(rect.center.x, rect.y);
            triangleVertices[1] = new Vector3(rect.x, rect.yMax);
            triangleVertices[2] = new Vector3(rect.xMax, rect.yMax);
            Handles.color = color;
            Handles.DrawAAConvexPolygon(triangleVertices);
        }

        private void SelectObjectByGUID(string[] bookmarkObjectIds)
        {
            List<GameObject> foundObjects = new List<GameObject>();

            if (bookmarkObjectIds == null || bookmarkObjectIds.Length == 0)
            {
                Debug.LogWarning("No object IDs provided.");
                return;
            }

            if (targetObject == null)
            {
                Debug.LogError("Target object is not specified.");
                return;
            }

            foreach (string bookmarkObjectId in bookmarkObjectIds)
            {
                    // Debug.Log(bookmarkObjectId);
                foreach (var bookmarkObject in bookmarkDataObjsList)
                {
                    if (bookmarkObject.bookmarkObjectId == bookmarkObjectId)
                    {
                        if (selectionMethod != 1)
                        {
                            string relativePath = bookmarkObject.relativePath;
                            Transform currentTransform = targetObject.transform;

                            string[] pathComponents = relativePath.Split('/');

                            foreach (string pathComponent in pathComponents)
                            {
                                currentTransform = currentTransform.Find(pathComponent);
                                if (currentTransform == null)
                                {
                                    Debug.LogWarning("Object not found: " + relativePath + " (Relative Path)");
                                    break;
                                }
                            }

                            if (currentTransform != null)
                            {
                                GameObject foundObject  = currentTransform.gameObject;
                                foundObjects.Add(foundObject);
                            }

                            continue;
                        }

                        if (selectionMethod == 1)
                        {
                            GameObject foundObject = EditorUtility.InstanceIDToObject(bookmarkObject.objectId) as GameObject;

                            if (foundObject != null)
                            {
                                foundObjects.Add(foundObject);
                            }
                            else
                            {
                                Debug.LogWarning("Object not found: " + bookmarkObject.objectId + " (Instance ID)");
                            }

                            continue;
                        }
                    }
                }
            }

            Selection.objects = foundObjects.ToArray();
        }

        private void DrawBackgroundGrid(Rect rect, float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(rect.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(rect.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            // 縦線
            for (int x = 0; x < widthDivs; x++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * x, rect.yMin, 0), new Vector3(gridSpacing * x, rect.height + rect.yMin, 0));
            }

            // 横線
            for (int y = 0; y < heightDivs; y++)
            {
                Handles.DrawLine(new Vector3(0, gridSpacing * y + rect.yMin, 0), new Vector3(rect.width, gridSpacing * y + rect.yMin, 0));
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private Color GetContrastColor(Color color)
        {
            float y = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
            return y >= 0.5f ? Color.black : Color.white;
        }
    }
}