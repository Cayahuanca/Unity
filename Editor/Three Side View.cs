using UnityEditor;
using UnityEngine;

public class ThreeSideView : EditorWindow
{
    [MenuItem("Window/Three Side View")]
    public static void ShowWindow()
    {
        var window = GetWindow<ThreeSideView>("Three Side View");
        window.position = new Rect(0, 0, 1540, 570);
    }

    private Camera frontCamera;
    private Camera sideCamera;
    private Camera topCamera;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal ();
        frontCamera = (Camera)EditorGUILayout.ObjectField("Camera 1", frontCamera, typeof(Camera), true);
        sideCamera = (Camera)EditorGUILayout.ObjectField("Camera 2", sideCamera, typeof(Camera), true);
        topCamera = (Camera)EditorGUILayout.ObjectField("Camera 3", topCamera, typeof(Camera), true);
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.BeginHorizontal();

        if (frontCamera != null)
        {
            RenderPreviewf(frontCamera);
        }

        if (sideCamera != null)
        {
            RenderPreviews(sideCamera);
        }

        if (topCamera != null)
        {
            RenderPreviewt(topCamera); 
        }

        EditorGUILayout.EndHorizontal();
    }

    private void RenderPreviewf(Camera CF)
    {
        RenderTexture RTF = RenderTexture.GetTemporary(512, 512, 24);
        CF.targetTexture = RTF;
        CF.Render();

        if ((position.width / 3) <= (position.height - 30))
        {
            EditorGUI.DrawPreviewTexture(new Rect(5, 30, position.width / 3 - 5, position.width / 3 - 10), RTF);
        }
        else
        {
            EditorGUI.DrawPreviewTexture(new Rect(5, 30, position.height - 5, position.height - 10), RTF);
        }

        RenderTexture.ReleaseTemporary(RTF);
        CF.targetTexture = null;
    }
    private void RenderPreviews(Camera CS)
    {
        RenderTexture RTS = RenderTexture.GetTemporary(512, 512, 24);
        CS.targetTexture = RTS;
        CS.Render();
        if ((position.width / 3) <= (position.height - 30))
        {
            EditorGUI.DrawPreviewTexture(new Rect(position.width / 3 + 5, 30,  position.width / 3 - 5, position.width / 3 - 10), RTS);
        }
        else
        {
            EditorGUI.DrawPreviewTexture(new Rect(position.height + 5, 30, position.height - 5, position.height - 10), RTS);
        }
        RenderTexture.ReleaseTemporary(RTS);
        CS.targetTexture = null;
    }
    private void RenderPreviewt(Camera CT)
    {
        RenderTexture RTT = RenderTexture.GetTemporary(512, 512, 24);
        CT.targetTexture = RTT;
        CT.Render();
        if ((position.width / 3) <= (position.height - 30))
        {
            EditorGUI.DrawPreviewTexture(new Rect(position.width / 3 * 2 + 5, 30,  position.width / 3 - 5, position.width / 3 - 10), RTT);
        }
        else
        {
            EditorGUI.DrawPreviewTexture(new Rect(position.height * 2 + 5, 30, position.height - 5, position.height - 10), RTT);
        }
        RenderTexture.ReleaseTemporary(RTT);
        CT.targetTexture = null;
    }
}