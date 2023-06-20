using UnityEngine;
using UnityEditor;
using System.IO;

namespace Praecipua.EE
{
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

	    private Vector2 scrollPos;

	    private void OnGUI()
	    {
	        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

	        foreach (var obj in Selection.objects)
	        {
	            string path = AssetDatabase.GetAssetPath(obj);
				if (Directory.Exists(path))
				{
					continue;
				}
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

	            string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);

	            string result = string.Format("{0:0.##} {1}", length, sizes[order]);

	            EditorGUILayout.Space();

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
	            	EditorGUILayout.SelectableLabel(path);
				EditorGUILayout.EndHorizontal();

	            EditorGUILayout.LabelField("Full Path: ");
	            EditorGUILayout.SelectableLabel(projectPath + path);
	        }

	        EditorGUILayout.EndScrollView();
	    }
	}
}