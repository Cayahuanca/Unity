using UnityEngine;
using UnityEditor;
using System.Globalization;

namespace Praecipua.EE
{
    [CustomEditor(typeof(MeshFilter))]
    public class PolygonCounter : Editor
    {
        private static bool ForceEnglish;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LoadSettings();

            string PolygonText = "Polygon Count";

            CultureInfo ci = CultureInfo.InstalledUICulture;
            string lang = ci.Name;

            if (ForceEnglish == false && lang == "ja-JP")
            {
                PolygonText = "ポリゴン数";
            }

            MeshFilter meshFilter = (MeshFilter)target;
            Mesh mesh = meshFilter.sharedMesh;

            if (mesh != null)
            {
                EditorGUILayout.LabelField(PolygonText, (mesh.triangles.Length / 3).ToString());
            }
            else
            {
                EditorGUILayout.LabelField(PolygonText, "null");
            }
        }
    
        private static void LoadSettings()
        {
            ForceEnglish = EditorPrefs.GetBool("Praecipua_English");
        }
    }
}