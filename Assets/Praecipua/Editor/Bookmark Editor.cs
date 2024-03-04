using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace Praecipua.EE
{
    public class BookmarkEditor : EditorWindow
    {
        // 両方
        private string jsonFilePath;
        private int editTab = 0;
        private int bookmarkIndex;

        // ブックマーク側
        private int labelType;
        private int showLabel;
        private string labelText = "";
        private Color labelColor = Color.white;
        private Vector2Int labelPosition;

        // オブジェクト側
        private GameObject root;
        private List<GameObject> children = new List<GameObject>();
        private int childCount;
        private int selectionMethod = 0;

        [MenuItem("Window/Praecipua/Bookmark Editor")]
        public static void ShowWindow()
        {
            GetWindow<BookmarkEditor>("Bookmark Editor");
        }

        public static void ShowWindowFilled(GameObject root, string jsonFilePath, int bookmarkIndex, int labelType = 0, int showLabel = 0, string labelText = "", List<GameObject> children = null, float labelColorR = 1, float labelColorG = 1, float labelColorB = 1, float labelColorA = 1, int labelPositionX = 0, int labelPositionY = 0)
        {
            if (root == null || string.IsNullOrEmpty(jsonFilePath) || bookmarkIndex <= 0)
            {
                Debug.LogError("Some of the required fields are empty. Please fill in all the required fields.");
                return;
            }

            if (children == null)
            {
                children = new List<GameObject>();
            }

            BookmarkEditor window = EditorWindow.GetWindow<BookmarkEditor>("Bookmark Editor");
            window.root = root;
            window.children = children;
            window.jsonFilePath = jsonFilePath;
            window.bookmarkIndex = bookmarkIndex;
            window.labelType = labelType;
            window.showLabel = showLabel;
            window.labelText = labelText;
            window.labelColor = new Color(labelColorR, labelColorG, labelColorB, labelColorA);
            window.labelPosition = new Vector2Int(labelPositionX, labelPositionY);
        }

        private void OnGUI()
        {
            GUILayout.Label("Bookmark Editor", EditorStyles.boldLabel);

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
                    }
                }

                if (GUILayout.Button("Reload Json"))
                {
                    if (jsonFilePath != null && jsonFilePath != "")
                    {
                        AssetDatabase.ImportAsset(jsonFilePath);
                    }
                }

            EditorGUILayout.EndHorizontal();

            Rect jsonDropArea = GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(true));
            GUI.Box(jsonDropArea, "JSON File Drag & Drop Area");

            EditorGUILayout.LabelField("JSON File Path: " +  jsonFilePath);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Select Mode: ");
                editTab = GUILayout.SelectionGrid(editTab, new string[] { "Edit Bookmark Label", "Edit Bookmarked Objects"}, 2);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Bookmark Index");
            if (!string.IsNullOrEmpty(jsonFilePath))
            {
                List<int> bookmarkIndices = GetBookmarkIndices(jsonFilePath);

                if (bookmarkIndices != null)
                {
                    EditorGUILayout.BeginHorizontal();

                        bookmarkIndex = EditorGUILayout.IntField(bookmarkIndex, GUILayout.Width(50), GUILayout.MaxWidth(100));

                        if (bookmarkIndices.Count == 0)
                        {
                            EditorGUILayout.LabelField("No bookmarks found.");
                        }
                        else
                        {
                            foreach (var index in bookmarkIndices)
                            {
                                if (GUILayout.Button(index.ToString()))
                                {
                                    bookmarkIndex = index;
                                    ReadBookmarkBooks(jsonFilePath, bookmarkIndex);
                                }
                            }
                        }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space();

            // Bookmark Label Edit
            if (editTab == 0)
            {
                EditorGUILayout.LabelField("Mode: Bookmark Label Edit", EditorStyles.boldLabel);

                EditorGUILayout.Space();

                EditorGUI.indentLevel++;

                    string[] labelTypeOptions = new string[] { "Auto", "Circle/Small", "Circle/Medium", "Circle/Large", "Square/Small", "Square/Medium", "Square/Large", "Diamond/Small", "Diamond/Medium", "Diamond/Large", "Triangle/Small", "Triangle/Medium", "Triangle/Large", "Square with Text" };
                    labelType = EditorGUILayout.Popup("Label Type", labelType, labelTypeOptions);
                    showLabel = EditorGUILayout.Popup("Show Label", showLabel, new string[] { "Auto", "Hide", "Show" });
                    labelText = EditorGUILayout.TextField("Label Text", labelText);
                    labelColor = EditorGUILayout.ColorField("Label Color", labelColor);
                    labelPosition = EditorGUILayout.Vector2IntField("Label Position", labelPosition);

                EditorGUI.indentLevel--;

                EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Add/Edit Bookmark"))
                    {
                        bool success = BookmarkAddEditBooks(bookmarkIndex, labelType, showLabel, labelText, labelColor, labelPosition, jsonFilePath);
                        if (success)
                        {
                            Debug.Log("Bookmark added/edited successfully.");
                        }
                        else
                        {
                            Debug.LogError("Failed to add/edit bookmark.");
                        }
                    }
                    if (GUILayout.Button("Remove Bookmark"))
                    {
                        bool success = BookmarkRemoveBooks(bookmarkIndex, jsonFilePath);
                        if (success)
                        {
                            Debug.Log("Bookmark removed successfully.");
                        }
                        else
                        {
                            Debug.LogError("Failed to remove bookmark.");
                        }
                    }

                EditorGUILayout.EndHorizontal();
            }
            // Bookmark Objects Edit
            else if (editTab == 1)
            {
                EditorGUILayout.LabelField("Mode: Bookmarked Objects Edit", EditorStyles.boldLabel);

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                    root = EditorGUILayout.ObjectField("Root Object", root, typeof(GameObject), true) as GameObject;

                    if (GUILayout.Button("Remove", GUILayout.MaxWidth(70)))
                    {
                        root = null;
                    }

                    if (GUILayout.Button("Read Objects", GUILayout.MaxWidth(100)))
                    {
                        if (root != null && !string.IsNullOrEmpty(jsonFilePath))
                        {
                            children = ReadBookmarkObjects(jsonFilePath, bookmarkIndex, root, selectionMethod);
                        }
                        else
                        {
                            Debug.LogError("Root object and JSON file path must be specified.");
                        }
                    }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Selection Method");
                    selectionMethod = GUILayout.SelectionGrid(selectionMethod, new string[] { "By Relative Path", "By Instance ID"}, 2);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Child Objects");

                EditorGUI.indentLevel++;

                if (children.Count > 0)
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        children[i] = EditorGUILayout.ObjectField(children[i], typeof(GameObject), true) as GameObject;

                        if (GUILayout.Button("Remove", GUILayout.MaxWidth(70)))
                        {
                            children.RemoveAt(i);
                            i--; // Adjust index after removing element
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUI.indentLevel--;

                Rect childObjDropArea = GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(true));

                GUI.Box(childObjDropArea, "Child Objects Drag & Drop Area (Don't drop root object and prefab that doesn't exist in scene here)");

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Add Objects to Bookmark"))
                    {
                        if (root != null && !string.IsNullOrEmpty(jsonFilePath))
                        {
                            bool success = BookmarkAddEditObjects(root, children, jsonFilePath, bookmarkIndex);
                            if (success)
                            {
                                Debug.Log("Objects added to bookmark successfully.");
                            }
                            else
                            {
                                Debug.LogError("Failed to add objects to bookmark.");
                            }
                        }
                        else
                        {
                            Debug.LogError("Root object and JSON file path must be specified.");
                        }
                    }

                    if (GUILayout.Button("Delete Objects from Bookmark"))
                    {
                        if (root != null && !string.IsNullOrEmpty(jsonFilePath))
                        {
                            bool success = BookmarkRemoveObjects(root, children, jsonFilePath, bookmarkIndex);
                            if (success)
                            {
                                Debug.Log("Objects deleted from bookmark successfully.");
                            }
                            else
                            {
                                Debug.LogError("Failed to delete objects from bookmark.");
                            }
                        }
                        else
                        {
                            Debug.LogError("Root object and JSON file path must be specified.");
                        }
                    }

                EditorGUILayout.EndHorizontal();

                Event evt = Event.current;
                switch (evt.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (childObjDropArea.Contains(evt.mousePosition))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            if (evt.type == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();

                                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                                {
                                    GameObject draggedObject = DragAndDrop.objectReferences[i] as GameObject;

                                    if (draggedObject != null && !children.Contains(draggedObject))
                                    {
                                        children.Add(draggedObject);
                                    }

                                }
                            }
                        }
                        else if (jsonDropArea.Contains(evt.mousePosition))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            if (evt.type == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();

                                for (int i = 0; i < DragAndDrop.paths.Length; i++)
                                {
                                    string draggedPath = DragAndDrop.paths[i];
                                    if (draggedPath.EndsWith(".json"))
                                    {
                                        jsonFilePath = draggedPath;
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }

                        Event.current.Use();
                        break;
                }
            }
        }

        public bool BookmarkAddEditObjects(GameObject root, List<GameObject> children, string jsonFilePath, int bookmarkIndex)
        {
            if (root == null || string.IsNullOrEmpty(jsonFilePath))
            {
                Debug.LogError("Root object and JSON file path must be specified.");
                return false;
            }

            List<BookmarkDataInfos> bookmarkDataInfoList = new List<BookmarkDataInfos>();
            List<BookmarkDataObjs> bookmarkDataObjsList = new List<BookmarkDataObjs>();
            List<BookmarkDataBooks> bookmarkDataBooksList = new List<BookmarkDataBooks>();
            BookmarkFile bookmarkFile = new BookmarkFile();

            // 既存のJsonファイルを読み込み、既存データを保持
            if (!File.Exists(jsonFilePath))
            {
                bool success = CreateJsonFile(jsonFilePath);
                if (!success)
                {
                    Debug.LogError("Failed to create JSON file.");
                    return false;
                }
            }

            string existingJson = File.ReadAllText(jsonFilePath);
            BookmarkFile existingBookmarkFile = JsonUtility.FromJson<BookmarkFile>(existingJson);
            if (existingBookmarkFile.objects != null)
            {
                bookmarkDataObjsList.AddRange(existingBookmarkFile.objects);
            }
            if (existingBookmarkFile.bookmarks != null)
            {
                bookmarkDataBooksList.AddRange(existingBookmarkFile.bookmarks);
            }

            // 新しいブックマークを作成
            BookmarkDataBooks bookmarkDataBooks = bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex);
            if (bookmarkDataBooks == null)
            {
                bookmarkDataBooks = new BookmarkDataBooks
                {
                    bookmarkIndex = bookmarkIndex,
                    linkedObjectIds = new List<string>(),
                    labelType = 0,
                    showLabel = 0,
                    labelText = "",
                    labelColor = Color.white,
                    xCoordinate = 0,
                    yCoordinate = 0,
                    zCoordinate = 0
                };
                bookmarkDataBooksList.Add(bookmarkDataBooks);
            }

            foreach (GameObject child in children)
            {
                if (child != null)
                {
                    BookmarkDataObjs existingObj = bookmarkDataObjsList.Find(data => data.relativePath == GetRelativePath(root.transform, child.transform));

                    if (existingObj == null)
                    {
                        // 新しいオブジェクトデータを作成
                        BookmarkDataObjs bookmarkDataObjs = new BookmarkDataObjs
                        {
                            objectId = child.GetInstanceID(),
                            rootId = root.GetInstanceID(),
                            relativePath = GetRelativePath(root.transform, child.transform),
                            objectName = child.name,
                            bookmarkObjectId = Guid.NewGuid().ToString(),
                            bookmarkIndices = new List<int>() { bookmarkIndex }
                        };
                        bookmarkDataObjsList.Add(bookmarkDataObjs);
                        bookmarkDataBooks.linkedObjectIds.Add(bookmarkDataObjs.bookmarkObjectId);
                    }
                    else
                    {
                        if (existingObj.bookmarkIndices.Contains(bookmarkIndex))
                        {
                            continue;
                        }

                        // 既存のオブジェクトデータにブックマークインデックスを追加
                        existingObj.bookmarkIndices.Add(bookmarkIndex);
                        bookmarkDataBooks.linkedObjectIds.Add(existingObj.bookmarkObjectId);
                    }
                }
                else
                {
                    Debug.LogError("Child object must be specified.");
                    return false;
                }
            }

            string unityMajorVersion = Application.unityVersion.Split('.')[0];

            BookmarkDataInfos bookmarkDataInfos = new BookmarkDataInfos
            {
                unityVersion = int.Parse(unityMajorVersion),
                extensionVersion = "6.3.50",
                lastSave = DateTime.Now.ToString()
            };

            bookmarkDataInfoList.Add(bookmarkDataInfos);

            bookmarkFile.info = bookmarkDataInfoList;
            bookmarkFile.objects = bookmarkDataObjsList;
            bookmarkFile.bookmarks = bookmarkDataBooksList;

            string json = JsonUtility.ToJson(bookmarkFile, true);
            File.WriteAllText(jsonFilePath, json);

            Debug.Log("Objects bookmarked successfully.");
            return true;
        }

        public bool BookmarkRemoveObjects(GameObject root, List<GameObject> children, string jsonFilePath, int bookmarkIndex)
        {
            if (root == null || string.IsNullOrEmpty(jsonFilePath))
            {
                Debug.LogError("Root object and JSON file path must be specified.");
                return false;
            }

            if (!File.Exists(jsonFilePath))
            {
                Debug.LogError("JSON file does not exist.");
                return false;
            }

            List<BookmarkDataInfos> bookmarkDataInfoList = new List<BookmarkDataInfos>();
            List<BookmarkDataObjs> bookmarkDataObjsList = new List<BookmarkDataObjs>();
            List<BookmarkDataBooks> bookmarkDataBooksList = new List<BookmarkDataBooks>();
            BookmarkFile bookmarkFile = new BookmarkFile();

            // 既存のJsonファイルを読み込み、既存データを保持
            if (File.Exists(jsonFilePath))
            {
                string existingJson = File.ReadAllText(jsonFilePath);
                BookmarkFile existingBookmarkFile = JsonUtility.FromJson<BookmarkFile>(existingJson);
                if (existingBookmarkFile.objects != null || existingBookmarkFile.bookmarks != null)
                {
                    bookmarkDataObjsList.AddRange(existingBookmarkFile.objects);
                    bookmarkDataBooksList.AddRange(existingBookmarkFile.bookmarks);
                }
                else
                {
                    Debug.LogError("Invalid JSON file.");
                    return false;
                }
            }
            else
            {
                Debug.LogError("JSON file does not exist.");
                return false;
            }

            List<BookmarkDataObjs> toBeDeleted = new List<BookmarkDataObjs>(); // 削除対象のリストを作成

            foreach (GameObject child in children)
            {
                if (child != null)
                {
                    // 一致するデータを削除対象リストに追加
                    toBeDeleted.AddRange(bookmarkDataObjsList.Where(data => data.relativePath == GetRelativePath(root.transform, child.transform) && data.bookmarkIndices.Contains(bookmarkIndex)));
                }
                else
                {
                    Debug.LogError("Child object must be specified.");
                    return false;
                }
            }

            // 削除対象リストに含まれる要素を元のリストから削除
            foreach (var item in toBeDeleted)
            {
                bookmarkDataObjsList.Remove(item);
                bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex).linkedObjectIds.Remove(item.bookmarkObjectId);
            }

            string unityMajorVersion = Application.unityVersion.Split('.')[0];

            BookmarkDataInfos bookmarkDataInfos = new BookmarkDataInfos
            {
                unityVersion = int.Parse(unityMajorVersion),
                extensionVersion = "6.3.50",
                lastSave = DateTime.Now.ToString()
            };

            bookmarkDataInfoList.Add(bookmarkDataInfos);

            bookmarkFile.info = bookmarkDataInfoList;
            bookmarkFile.objects = bookmarkDataObjsList;
            bookmarkFile.bookmarks = bookmarkDataBooksList;

            string json = JsonUtility.ToJson(bookmarkFile, true);
            File.WriteAllText(jsonFilePath, json);

            Debug.Log("Objects deleted from bookmark successfully.");
            return true;
        }

        private string GetRelativePath(Transform root, Transform child)
        {
            List<string> pathParts = new List<string>();
            Transform current = child;

            while (current != null && current != root)
            {
                pathParts.Insert(0, current.name);
                current = current.parent;
            }

            return string.Join("/", pathParts);
        }

        public bool BookmarkAddEditBooks(int bookmarkIndex, int labelType, int showLabel, string labelText, Color labelColor, Vector2Int labelPosition, string jsonFilePath)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                Debug.LogError("JSON file path must be specified.");
                return false;
            }

            List<BookmarkDataInfos> bookmarkDataInfoList = new List<BookmarkDataInfos>();
            List<BookmarkDataObjs> bookmarkDataObjsList = new List<BookmarkDataObjs>();
            List<BookmarkDataBooks> bookmarkDataBooksList = new List<BookmarkDataBooks>();
            BookmarkFile bookmarkFile = new BookmarkFile();

            if (!File.Exists(jsonFilePath))
            {
                bool success = CreateJsonFile(jsonFilePath);
                if (!success)
                {
                    Debug.LogError("Failed to create JSON file.");
                    return false;
                }
            }

            // 既存のJsonファイルを読み込み、既存データを保持
            string existingJson = File.ReadAllText(jsonFilePath);
            BookmarkFile existingBookmarkFile = JsonUtility.FromJson<BookmarkFile>(existingJson);
            if (existingBookmarkFile.objects != null)
            {
                bookmarkDataObjsList.AddRange(existingBookmarkFile.objects);
            }
            if (existingBookmarkFile.bookmarks != null)
            {
                bookmarkDataBooksList.AddRange(existingBookmarkFile.bookmarks);
            }

            BookmarkDataBooks bookmarkDataBooks = bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex);
            if (bookmarkDataBooks == null)
            {
                bookmarkDataBooks = new BookmarkDataBooks
                {
                    bookmarkIndex = bookmarkIndex,
                    linkedObjectIds = new List<string>(),
                    labelType = labelType,
                    showLabel = showLabel,
                    labelText = labelText,
                    labelColor = labelColor,
                    xCoordinate = labelPosition.x,
                    yCoordinate = labelPosition.y,
                    zCoordinate = 0
                };
                bookmarkDataBooksList.Add(bookmarkDataBooks);
            }
            else
            {
                bookmarkDataBooks.labelType = labelType;
                bookmarkDataBooks.showLabel = showLabel;
                bookmarkDataBooks.labelText = labelText;
                bookmarkDataBooks.labelColor = labelColor;
                bookmarkDataBooks.xCoordinate = labelPosition.x;
                bookmarkDataBooks.yCoordinate = labelPosition.y;
                bookmarkDataBooks.zCoordinate = 0;
            }

            string unityMajorVersion = Application.unityVersion.Split('.')[0];

            BookmarkDataInfos bookmarkDataInfos = new BookmarkDataInfos
            {
                unityVersion = int.Parse(unityMajorVersion),
                extensionVersion = "6.3.50",
                lastSave = DateTime.Now.ToString()
            };

            bookmarkDataInfoList.Add(bookmarkDataInfos);

            bookmarkFile.info = bookmarkDataInfoList;
            bookmarkFile.objects = bookmarkDataObjsList;
            bookmarkFile.bookmarks = bookmarkDataBooksList;

            string json = JsonUtility.ToJson(bookmarkFile, true);
            File.WriteAllText(jsonFilePath, json);

            Debug.Log("Bookmark added/edited successfully.");
            return true;
        }

        public bool BookmarkRemoveBooks(int bookmarkIndex, string jsonFilePath)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                Debug.LogError("JSON file path must be specified.");
                return false;
            }

            List<BookmarkDataInfos> bookmarkDataInfoList = new List<BookmarkDataInfos>();
            List<BookmarkDataObjs> bookmarkDataObjsList = new List<BookmarkDataObjs>();
            List<BookmarkDataBooks> bookmarkDataBooksList = new List<BookmarkDataBooks>();
            BookmarkFile bookmarkFile = new BookmarkFile();

            // 既存のJsonファイルを読み込み、既存データを保持
            if (File.Exists(jsonFilePath))
            {
                string existingJson = File.ReadAllText(jsonFilePath);
                BookmarkFile existingBookmarkFile = JsonUtility.FromJson<BookmarkFile>(existingJson);
                if (existingBookmarkFile.objects != null)
                {
                    bookmarkDataObjsList.AddRange(existingBookmarkFile.objects);
                }
                if (existingBookmarkFile.bookmarks != null)
                {
                    bookmarkDataBooksList.AddRange(existingBookmarkFile.bookmarks);
                }
                else
                {
                    Debug.LogError("Invalid JSON file.");
                    return false;
                }
            }
            else
            {
                Debug.LogError("JSON file does not exist.");
                return false;
            }

            BookmarkDataBooks toBeDeletedBooks = bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex);
            if (toBeDeletedBooks != null)
            {
                List<string> linkedObjectIds = toBeDeletedBooks.linkedObjectIds;
                foreach (var linkedObjectId in linkedObjectIds)
                {
                    BookmarkDataObjs bookmarkDataObj = bookmarkDataObjsList.Find(o => o.bookmarkObjectId == linkedObjectId);
                    if (bookmarkDataObj != null)
                    {
                        bookmarkDataObj.bookmarkIndices.Remove(bookmarkIndex);

                        if (bookmarkDataObj.bookmarkIndices.Count == 0)
                        {
                            bookmarkDataObjsList.Remove(bookmarkDataObj);
                        }
                    }
                }

                bookmarkDataBooksList.Remove(toBeDeletedBooks);
            }

            string unityMajorVersion = Application.unityVersion.Split('.')[0];

            BookmarkDataInfos bookmarkDataInfos = new BookmarkDataInfos
            {
                unityVersion = int.Parse(unityMajorVersion),
                extensionVersion = "6.3.50",
                lastSave = DateTime.Now.ToString()
            };

            bookmarkDataInfoList.Add(bookmarkDataInfos);

            bookmarkFile.info = bookmarkDataInfoList;
            bookmarkFile.objects = bookmarkDataObjsList;
            bookmarkFile.bookmarks = bookmarkDataBooksList;

            string json = JsonUtility.ToJson(bookmarkFile, true);
            File.WriteAllText(jsonFilePath, json);

            Debug.Log("Bookmark removed successfully.");
            return true;
        }

        public bool BookmarkInfoWrite(string jsonFilePath)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                Debug.LogError("JSON file path must be specified.");
                return false;
            }

            List<BookmarkDataInfos> bookmarkDataInfoList = new List<BookmarkDataInfos>();
            List<BookmarkDataObjs> bookmarkDataObjsList = new List<BookmarkDataObjs>();
            List<BookmarkDataBooks> bookmarkDataBooksList = new List<BookmarkDataBooks>();
            BookmarkFile bookmarkFile = new BookmarkFile();

            // 既存のJsonファイルを読み込み、既存データを保持
            if (File.Exists(jsonFilePath))
            {
                string existingJson = File.ReadAllText(jsonFilePath);
                BookmarkFile existingBookmarkFile = JsonUtility.FromJson<BookmarkFile>(existingJson);
                if (existingBookmarkFile.objects != null)
                {
                    bookmarkDataObjsList.AddRange(existingBookmarkFile.objects);
                }
                if (existingBookmarkFile.bookmarks != null)
                {
                    bookmarkDataBooksList.AddRange(existingBookmarkFile.bookmarks);
                }
            }
            else
            {
                Debug.LogError("JSON file does not exist.");
                return false;
            }

            string unityMajorVersion = Application.unityVersion.Split('.')[0];

            BookmarkDataInfos bookmarkDataInfos = new BookmarkDataInfos
            {
                unityVersion = int.Parse(unityMajorVersion),
                extensionVersion = "6.3.50",
                lastSave = DateTime.Now.ToString()
            };

            bookmarkDataInfoList.Add(bookmarkDataInfos);

            bookmarkFile.info = bookmarkDataInfoList;
            bookmarkFile.objects = bookmarkDataObjsList;
            bookmarkFile.bookmarks = bookmarkDataBooksList;

            string json = JsonUtility.ToJson(bookmarkFile, true);
            File.WriteAllText(jsonFilePath, json);
            return true;
        }

        public bool CreateJsonFile(string jsonFilePath)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                Debug.LogError("JSON file path must be specified.");
                return false;
            }

            if (File.Exists(jsonFilePath))
            {
                Debug.LogError("JSON file already exists.");
                return false;
            }

            BookmarkFile bookmarkFile = new BookmarkFile();

            string unityMajorVersion = Application.unityVersion.Split('.')[0];

            BookmarkDataInfos bookmarkDataInfos = new BookmarkDataInfos
            {
                unityVersion = int.Parse(unityMajorVersion),
                extensionVersion = "6.3.50",
                lastSave = DateTime.Now.ToString()
            };

            bookmarkFile.info = new List<BookmarkDataInfos> { bookmarkDataInfos };
            bookmarkFile.objects = new List<BookmarkDataObjs>();
            bookmarkFile.bookmarks = new List<BookmarkDataBooks>();

            string json = JsonUtility.ToJson(bookmarkFile, true);
            File.WriteAllText(jsonFilePath, json);
            return true;
        }

        public List<int> GetBookmarkIndices(string jsonFilePath)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                Debug.LogError("JSON file path must be specified.");
                return null;
            }

            if (!File.Exists(jsonFilePath))
            {
                Debug.LogError("JSON file does not exist.");
                return null;
            }

            List<BookmarkDataBooks> bookmarkDataBooksList = new List<BookmarkDataBooks>();

            string existingJson = File.ReadAllText(jsonFilePath);
            BookmarkFile existingBookmarkFile = JsonUtility.FromJson<BookmarkFile>(existingJson);
            if (existingBookmarkFile.bookmarks != null)
            {
                bookmarkDataBooksList.AddRange(existingBookmarkFile.bookmarks);
            }
            else
            {
                Debug.LogError("Invalid JSON file.");
                return null;
            }

            List<int> bookmarkIndices = new List<int>();
            foreach (var bookmarkDataBooks in bookmarkDataBooksList)
            {
                bookmarkIndices.Add(bookmarkDataBooks.bookmarkIndex);
            }

            return bookmarkIndices;
        }

        public void ReadBookmarkBooks(string jsonFilePath, int bookmarkIndex)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                Debug.LogError("JSON file path must be specified.");
                return;
            }

            if (!File.Exists(jsonFilePath))
            {
                Debug.LogError("JSON file does not exist.");
                return;
            }

            List<BookmarkDataBooks> bookmarkDataBooksList = new List<BookmarkDataBooks>();

            string existingJson = File.ReadAllText(jsonFilePath);
            BookmarkFile existingBookmarkFile = JsonUtility.FromJson<BookmarkFile>(existingJson);
            if (existingBookmarkFile.bookmarks != null)
            {
                bookmarkDataBooksList.AddRange(existingBookmarkFile.bookmarks);
            }
            else
            {
                Debug.LogError("Invalid JSON file.");
                return;
            }

            BookmarkDataBooks bookmarkDataBooks = bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex);
            if (bookmarkDataBooks != null)
            {
                labelType = bookmarkDataBooks.labelType;
                showLabel = bookmarkDataBooks.showLabel;
                labelText = bookmarkDataBooks.labelText;
                labelColor = bookmarkDataBooks.labelColor;
                labelPosition = new Vector2Int(bookmarkDataBooks.xCoordinate, bookmarkDataBooks.yCoordinate);
            }
            else
            {
                Debug.LogError("Bookmark not found.");
            }
        }

        public List<GameObject> ReadBookmarkObjects(string jsonFilePath, int bookmarkIndex, GameObject root, int selectionMethod = 0)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                Debug.LogError("JSON file path must be specified.");
                return null;
            }

            if (!File.Exists(jsonFilePath))
            {
                Debug.LogError("JSON file does not exist.");
                return null;
            }

            List<BookmarkDataObjs> bookmarkDataObjsList = new List<BookmarkDataObjs>();
            List<GameObject> bookmarkedObjects = new List<GameObject>();

            string existingJson = File.ReadAllText(jsonFilePath);
            BookmarkFile existingBookmarkFile = JsonUtility.FromJson<BookmarkFile>(existingJson);
            if (existingBookmarkFile.objects != null)
            {
                bookmarkDataObjsList.AddRange(existingBookmarkFile.objects);
            }
            else
            {
                Debug.LogError("Invalid JSON file.");
                return null;
            }

            foreach (var bookmarkDataObjs in bookmarkDataObjsList)
            {
                if (bookmarkDataObjs.bookmarkIndices.Contains(bookmarkIndex) && bookmarkDataObjs.rootId == root.GetInstanceID())
                {
                    if (selectionMethod != 1)
                    {
                        Transform child = root.transform.Find(bookmarkDataObjs.relativePath);
                        if (child != null)
                        {
                            bookmarkedObjects.Add(child.gameObject);
                        }
                    }
                    else
                    {
                        GameObject obj = EditorUtility.InstanceIDToObject(bookmarkDataObjs.objectId) as GameObject;
                        if (obj != null)
                        {
                            bookmarkedObjects.Add(obj);
                        }
                    }
                }
            }

            return bookmarkedObjects;
        }
    }


    // Json のデータ
    [Serializable]
    public class BookmarkFile
    {
        public List<BookmarkDataInfos> info;
        public List<BookmarkDataObjs> objects;
        public List<BookmarkDataBooks> bookmarks;
    }

    // Json の情報部分
    [Serializable]
    public class BookmarkDataInfos
    {
        public int unityVersion;
        public string extensionVersion;
        public string lastSave;
    }

    // Json のオブジェクト部分
    [Serializable]
    public class BookmarkDataObjs
    {
        public int objectId; // Scene 内でのオブジェクトのInstanceID
        public int rootId; // Scene 内でのルートオブジェクトのInstanceID
        public string relativePath; // スクリプトをアタッチしたオブジェクトから登録するオブジェクトへの相対パス
        public string objectName; // 登録するオブジェクトの名前
        public string bookmarkObjectId; // オブジェクトの識別番号(GUIDを生成)
        public List<int> bookmarkIndices;// 紐づけるブックマークの識別番号
    }

    // Json のブックマーク部分
    [Serializable]
    public class BookmarkDataBooks
    {
        public int bookmarkIndex; // ブックマークの識別番号
        public List<string> linkedObjectIds; // 紐付けるオブジェクトの識別番号(生成したGUID)
        public int labelType; // ラベルの種類
        public int showLabel; // ラベルを表示するかどうか
        public string labelText; // ブックマークのラベルのテキスト
        public Color labelColor; // ブックマークのラベルの色
        public int xCoordinate;
        public int yCoordinate;
        public int zCoordinate;
    }
}