using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class OpenInVSCode : Editor {
    [MenuItem("Assets/Open in Visual Studio Code", false)]
    public static void OpenSelectedFileInVSCode() {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(path)) {
            Process proc = new Process();
            proc.StartInfo.FileName = "code";
            proc.StartInfo.Arguments = "-g \"" + path + "\"";
            proc.Start();
        }
    }

    [MenuItem("Assets/Open in Visual Studio Code", true)]
    private static bool ValidateSelectedFileInVSCode() {
        return Selection.activeObject != null && AssetDatabase.Contains(Selection.activeObject);
    }
}