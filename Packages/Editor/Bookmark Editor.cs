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
        private string extensionVersion = "6.3.50";
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
        private List<GameObject> childrenToAdd = new List<GameObject>();
        private List<BookmarkDataObjs> notFoundObjsList = new List<BookmarkDataObjs>();
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

            Rect jsonDropArea = GUILayoutUtility.GetRect(0.0f, 40.0f, GUILayout.ExpandWidth(true));
            GUI.Box(jsonDropArea, "JSON File Drag & Drop Area");

            EditorGUILayout.LabelField("JSON File Path: " +  jsonFilePath);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Mode");
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
                                    if (root != null)
                                    {
                                        children = ReadBookmarkObjects(jsonFilePath, bookmarkIndex, root, selectionMethod);
                                    }
                                }
                            }
                        }

                        if (GUILayout.Button("Create New", GUILayout.MaxWidth(100)))
                        {
                            if (bookmarkIndices.Count > 0)
                            {
                                bookmarkIndex = bookmarkIndices.Max() + 1;
                            }
                            else
                            {
                                bookmarkIndex = 1;
                            }
                        }

                        bookmarkIndex = EditorGUILayout.IntField(bookmarkIndex, GUILayout.MaxWidth(100));

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

                    if (GUILayout.Button("Load Child Objects", GUILayout.MaxWidth(205)))
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
                    selectionMethod = GUILayout.Toolbar(selectionMethod, new string[] { "By Relative Path", "By Instance ID", "By Object Name (Not Recommend)" });

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Registered Child Objects");

                    if (GUILayout.Button("Rewrite All", GUILayout.MaxWidth(100)))
                    {
                        if (root != null && !string.IsNullOrEmpty(jsonFilePath))
                        {
                            bool success = BookmarkAddEditObjects(root, children, jsonFilePath, bookmarkIndex);
                            if (success)
                            {
                                Debug.Log("Objects added to bookmark successfully.");
                                ReadBookmarkObjects(jsonFilePath, bookmarkIndex, root, selectionMethod);
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

                    if (GUILayout.Button("Delete All", GUILayout.MaxWidth(100)))
                    {
                        if (root != null && !string.IsNullOrEmpty(jsonFilePath))
                        {
                            bool success = BookmarkRemoveObjects(root, children, jsonFilePath, bookmarkIndex);
                            if (success)
                            {
                                Debug.Log("Objects deleted from bookmark successfully.");
                                ReadBookmarkObjects(jsonFilePath, bookmarkIndex, root, selectionMethod);
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

                EditorGUI.indentLevel++;

                if (children.Count > 0)
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                            children[i] = EditorGUILayout.ObjectField(children[i], typeof(GameObject), true) as GameObject;

                            if (GUILayout.Button("Delete", GUILayout.MaxWidth(70)))
                            {
                                List<GameObject> removeChildList = new List<GameObject> { children[i] };
                                BookmarkRemoveObjects(root, removeChildList, jsonFilePath, bookmarkIndex);

                                children.RemoveAt(i);
                                i--; // Adjust index after removing element
                            }

                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUI.indentLevel--;

                if (notFoundObjsList.Count > 0)
                {

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Registered but not foud Child Objects");

                        if (GUILayout.Button("Delete All", GUILayout.MaxWidth(100)))
                        {
                            List<string> deleteObjectIds = new List<string>();
                            foreach (var notFoundObj in notFoundObjsList)
                            {
                                deleteObjectIds.Add(notFoundObj.bookmarkObjectId);
                            }
                            BookmarkRemoveObjectsGuid(deleteObjectIds, jsonFilePath, bookmarkIndex);

                            notFoundObjsList.Clear();
                        }

                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel++;

                    for (int i = 0; i < notFoundObjsList.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField(notFoundObjsList[i].objectName);
                            EditorGUILayout.LabelField(root.name + "/" + notFoundObjsList[i].relativePath);
                            if (GUILayout.Button("Delete", GUILayout.MaxWidth(70)))
                            {
                                List<string> bookmarkObjectIds = new List<string> { notFoundObjsList[i].bookmarkObjectId };
                                BookmarkRemoveObjectsGuid(bookmarkObjectIds, jsonFilePath, bookmarkIndex);

                                notFoundObjsList.RemoveAt(i);
                                i--; // Adjust index after removing element
                            }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Add Child Objects");

                    if (GUILayout.Button("Add Objects to Bookmark", GUILayout.MaxWidth(205)))
                    {
                        if (root != null && !string.IsNullOrEmpty(jsonFilePath))
                        {
                            bool success = BookmarkAddEditObjects(root, childrenToAdd, jsonFilePath, bookmarkIndex);
                            if (success)
                            {
                                Debug.Log("Objects added to bookmark successfully.");
                                ReadBookmarkObjects(jsonFilePath, bookmarkIndex, root, selectionMethod);
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

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                if (childrenToAdd.Count > 0)
                {
                    for (int i = 0; i < childrenToAdd.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        childrenToAdd[i] = EditorGUILayout.ObjectField(childrenToAdd[i], typeof(GameObject), true) as GameObject;

                        if (GUILayout.Button("Remove", GUILayout.MaxWidth(70)))
                        {
                            childrenToAdd.RemoveAt(i);
                            i--; // Adjust index after removing element
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUI.indentLevel--;

                Rect childObjDropArea = GUILayoutUtility.GetRect(0.0f, 40.0f, GUILayout.ExpandWidth(true));

                GUI.Box(childObjDropArea, "Child Objects Drag & Drop Area (Don't drop root object and prefab that doesn't exist in scene)");

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
                                        childrenToAdd.Add(draggedObject);
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
            if (root == null)
            {
                Debug.LogError("Root object must be specified.");
                return false;
            }

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
                    switch (selectionMethod)
                    {
                        default:
                        case 0: // Relative Path
                        case 3: // Relative Path -> (if not found) -> Instance ID
                            BookmarkDataObjs existingObjRelative = bookmarkDataObjsList.Find(data => data.relativePath == GetRelativePath(root.transform, child.transform));
                            if (existingObjRelative == null)
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
                                // 既存のオブジェクトデータを更新
                                existingObjRelative.objectId = child.GetInstanceID();
                                existingObjRelative.rootId = root.GetInstanceID();

                                if (existingObjRelative.bookmarkIndices.Contains(bookmarkIndex))
                                {
                                    continue;
                                }

                                existingObjRelative.bookmarkIndices.Add(bookmarkIndex);
                                bookmarkDataBooks.linkedObjectIds.Add(existingObjRelative.bookmarkObjectId);
                            }
                            break;
                        case 1: // Instance ID
                        case 4: // Instance ID -> (if not found) -> Object Name
                            BookmarkDataObjs existingObjInstanceId = bookmarkDataObjsList.Find(data => data.objectId == child.GetInstanceID());
                            if (existingObjInstanceId == null)
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
                                // 既存のオブジェクトデータを更新
                                existingObjInstanceId.rootId = root.GetInstanceID();
                                existingObjInstanceId.relativePath = GetRelativePath(root.transform, child.transform);
                                existingObjInstanceId.objectName = child.name;

                                if (existingObjInstanceId.bookmarkIndices.Contains(bookmarkIndex))
                                {
                                    continue;
                                }

                                existingObjInstanceId.bookmarkIndices.Add(bookmarkIndex);
                                bookmarkDataBooks.linkedObjectIds.Add(existingObjInstanceId.bookmarkObjectId);
                            }
                            break;
                        case 2: // Object Name
                            BookmarkDataObjs existingObjName = bookmarkDataObjsList.Find(data => data.objectName == child.name);
                            if (existingObjName == null)
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
                                // 既存のオブジェクトデータを更新
                                existingObjName.objectId = child.GetInstanceID();
                                existingObjName.rootId = root.GetInstanceID();
                                existingObjName.relativePath = GetRelativePath(root.transform, child.transform);

                                if (existingObjName.bookmarkIndices.Contains(bookmarkIndex))
                                {
                                    continue;
                                }

                                existingObjName.bookmarkIndices.Add(bookmarkIndex);
                                bookmarkDataBooks.linkedObjectIds.Add(existingObjName.bookmarkObjectId);
                            }
                            break;
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
                extensionVersion = extensionVersion,
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
            if (root == null)
            {
                Debug.LogError("Root object must be specified.");
                return false;
            }

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

            foreach (GameObject child in children)
            {
                if (child != null)
                {
                    // 一致するデータを削除
                    switch (selectionMethod)
                    {
                        default:
                        case 0: // Relative Path
                        case 3: // Relative Path -> (if not found) -> Instance ID
                            List<BookmarkDataObjs> matchsRelative = bookmarkDataObjsList.Where(data => data.relativePath == GetRelativePath(root.transform, child.transform) && data.bookmarkIndices.Contains(bookmarkIndex)).ToList();
                            foreach (var match in matchsRelative)
                            {
                                if (match.bookmarkIndices.Count <= 1)
                                {
                                    bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex).linkedObjectIds.Remove(match.bookmarkObjectId);
                                    bookmarkDataObjsList.Remove(match);
                                }
                                else
                                {
                                    bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex).linkedObjectIds.Remove(match.bookmarkObjectId);
                                    match.bookmarkIndices.Remove(bookmarkIndex);
                                }
                            }
                            break;
                        case 1: // Instance ID
                        case 4: // Instance ID -> (if not found) -> Object Name
                            List<BookmarkDataObjs> matchsInstanceId = bookmarkDataObjsList.Where(data => data.objectId == child.GetInstanceID() && data.bookmarkIndices.Contains(bookmarkIndex)).ToList();
                            foreach (var match in matchsInstanceId)
                            {
                                if (match.bookmarkIndices.Count <= 1)
                                {
                                    bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex).linkedObjectIds.Remove(match.bookmarkObjectId);
                                    bookmarkDataObjsList.Remove(match);
                                }
                                else
                                {
                                    bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex).linkedObjectIds.Remove(match.bookmarkObjectId);
                                    match.bookmarkIndices.Remove(bookmarkIndex);
                                }
                            }
                            break;
                        case 2: // Object Name
                            List<BookmarkDataObjs> matchsObjectName = bookmarkDataObjsList.Where(data => data.objectName == child.name && data.bookmarkIndices.Contains(bookmarkIndex)).ToList();
                            foreach (var match in matchsObjectName)
                            {
                                if (match.bookmarkIndices.Count <= 1)
                                {
                                    bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex).linkedObjectIds.Remove(match.bookmarkObjectId);
                                    bookmarkDataObjsList.Remove(match);
                                }
                                else
                                {
                                    bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex).linkedObjectIds.Remove(match.bookmarkObjectId);
                                    match.bookmarkIndices.Remove(bookmarkIndex);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    Debug.LogError("GameObjects must be specified.");
                    return false;
                }
            }

            string unityMajorVersion = Application.unityVersion.Split('.')[0];

            BookmarkDataInfos bookmarkDataInfos = new BookmarkDataInfos
            {
                unityVersion = int.Parse(unityMajorVersion),
                extensionVersion = extensionVersion,
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

        public bool BookmarkRemoveObjectsGuid(List<string> bookmarkObjectIds, string jsonFilePath, int bookmarkIndex)
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

            foreach (string bookmarkObjectId in bookmarkObjectIds)
            {
                if (!string.IsNullOrEmpty(bookmarkObjectId))
                {
                    BookmarkDataObjs toBeDeleted = bookmarkDataObjsList.Find(data => data.bookmarkObjectId == bookmarkObjectId && data.bookmarkIndices.Contains(bookmarkIndex));
                    if (toBeDeleted != null)
                    {
                        if (toBeDeleted.bookmarkIndices.Count <= 1)
                        {
                            bookmarkDataObjsList.Remove(toBeDeleted);
                            bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex).linkedObjectIds.Remove(bookmarkObjectId);
                        }
                        else
                        {
                            toBeDeleted.bookmarkIndices.Remove(bookmarkIndex);
                            bookmarkDataBooksList.Find(b => b.bookmarkIndex == bookmarkIndex).linkedObjectIds.Remove(bookmarkObjectId);
                        }
                    }
                    else
                    {
                        Debug.LogError("There are no objects that match the specified bookmark object ID and bookmark index.");
                        return false;
                    }
                }
                else
                {
                    Debug.LogError("Bookmark object ID must be specified.");
                    return false;
                }
            }

            string unityMajorVersion = Application.unityVersion.Split('.')[0];

            BookmarkDataInfos bookmarkDataInfos = new BookmarkDataInfos
            {
                unityVersion = int.Parse(unityMajorVersion),
                extensionVersion = extensionVersion,
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
                extensionVersion = extensionVersion,
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
                extensionVersion = extensionVersion,
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
                extensionVersion = extensionVersion,
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
                extensionVersion = extensionVersion,
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

            notFoundObjsList.Clear();

            foreach (var bookmarkDataObjs in bookmarkDataObjsList)
            {
                if (bookmarkDataObjs.bookmarkIndices.Contains(bookmarkIndex))
                {
                    switch (selectionMethod)
                    {
                        default:
                        case 0: // Relative Path
                            Transform child = root.transform.Find(bookmarkDataObjs.relativePath);
                            if (child != null)
                            {
                                bookmarkedObjects.Add(child.gameObject);
                            }
                            else
                            {
                                notFoundObjsList.Add(bookmarkDataObjs);
                            }
                            break;
                        case 1: // Instance ID
                            GameObject obj = EditorUtility.InstanceIDToObject(bookmarkDataObjs.objectId) as GameObject;
                            if (obj != null)
                            {
                                bookmarkedObjects.Add(obj);
                            }
                            else
                            {
                                notFoundObjsList.Add(bookmarkDataObjs);
                            }
                            break;
                        case 2: // Object Name
                            Transform child4 = root.transform.Find(bookmarkDataObjs.objectName);
                            if (child4 != null)
                            {
                                bookmarkedObjects.Add(child4.gameObject);
                            }
                            else
                            {
                                notFoundObjsList.Add(bookmarkDataObjs);
                            }
                            break;
                        case 3: // Relative Path -> (if not found) -> Instance ID
                            Transform child2 = root.transform.Find(bookmarkDataObjs.relativePath);
                            if (child2 != null)
                            {
                                bookmarkedObjects.Add(child2.gameObject);
                            }
                            else
                            {
                                GameObject obj2 = EditorUtility.InstanceIDToObject(bookmarkDataObjs.objectId) as GameObject;
                                if (obj2 != null)
                                {
                                    bookmarkedObjects.Add(obj2);
                                }
                                else
                                {
                                    notFoundObjsList.Add(bookmarkDataObjs);
                                }
                            }
                            break;
                        case 4: // Instance ID -> (if not found) -> Relative Path
                            GameObject obj3 = EditorUtility.InstanceIDToObject(bookmarkDataObjs.objectId) as GameObject;
                            if (obj3 != null)
                            {
                                bookmarkedObjects.Add(obj3);
                            }
                            else
                            {
                                Transform child3 = root.transform.Find(bookmarkDataObjs.relativePath);
                                if (child3 != null)
                                {
                                    bookmarkedObjects.Add(child3.gameObject);
                                }
                                else
                                {
                                    notFoundObjsList.Add(bookmarkDataObjs);
                                }
                            }
                            break;
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