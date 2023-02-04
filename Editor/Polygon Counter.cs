using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshFilter))]
public class PolygonCounter : Editor
{
    public override void OnInspectorGUI()
    {
        MeshFilter meshFilter = (MeshFilter)target;
        Mesh mesh = meshFilter.sharedMesh;

        if (mesh != null)
        {
            EditorGUILayout.LabelField("Polygon Count:", mesh.triangles.Length / 3 + " triangles");
        }

        base.OnInspectorGUI();
    }
}