using UnityEngine;
using UnityEditor;

public class FileInfoWindow : EditorWindow
{
    [MenuItem("Window/File Info")]
    public static void ShowWindow()
    {
        GetWindow<FileInfoWindow>("File Info");
    }

    private void OnSelectionChange()
    {
        Repaint();
    }

    private void OnGUI()
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }
            
            var fileInfo = new System.IO.FileInfo(path);
            long length = fileInfo.Length;

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (length >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                length = length / 1024;
            }

            string result = string.Format("{0:0.##} {1}", length, sizes[order]);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Name: ");
            EditorGUILayout.LabelField(obj.name + fileInfo.Extension);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Extension: ");
            EditorGUILayout.LabelField(fileInfo.Extension);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size: ");
            EditorGUILayout.LabelField(result);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Last Updated: ");
            EditorGUILayout.LabelField(fileInfo.LastWriteTime.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path: ");
            EditorGUILayout.TextField(path);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
    }
}
