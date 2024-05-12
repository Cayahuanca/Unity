using UnityEditor;
using UnityEngine;

namespace Praecipua.EE
{
    public class ThreeSideView : EditorWindow
    {
        private Camera[] cameras;
        private int horizontalCount = 3; // 横
        private int verticalCount = 1; // 縦
        private int renderSize = 512;
        private int cameraResolution = 2048;

        [MenuItem("Window/Praecipua/Three Side View")]
        public static void ShowWindow()
        {
            var window = GetWindow<ThreeSideView>("Three Side View");
            window.position = new Rect(0, 0, 1540, 570);
        }

        private void OnEnable()
        {
            cameras = new Camera[25];
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            horizontalCount = EditorGUILayout.IntSlider("Horizontal Count", horizontalCount, 1, 6, GUILayout.MaxWidth(300));
            verticalCount = EditorGUILayout.IntSlider("Vertical Count", verticalCount, 1, 3, GUILayout.MaxWidth(300));
            cameraResolution = EditorGUILayout.IntPopup("Camera Resolution", cameraResolution, new string[] { "Auto", "512", "1024", "2048", "4096" }, new int[] { renderSize * 2, 512, 1024, 2048, 4096 }, GUILayout.MaxWidth(300));
			/*
			if (GUILayout.Button("Save All", GUILayout.MaxWidth(100)))
			{
				SaveAllViews();
			}
			*/

            EditorGUILayout.EndHorizontal();

			if ((position.width / horizontalCount) <= ((position.height - 30 - verticalCount * 20) / verticalCount))
			{
				renderSize = (((int)position.width - 10) / horizontalCount) - 2;
			}
			else
			{
				renderSize = (((int)position.height - 35 - verticalCount * 20) / verticalCount) - 2;
			}

            for (int i = 0; i < verticalCount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < horizontalCount; j++)
                {
                    int index = i * horizontalCount + j;
                    cameras[index] = (Camera)EditorGUILayout.ObjectField("Camera " + (index + 1), cameras[index], typeof(Camera), true, GUILayout.MaxWidth(300));
					if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
					{
						cameras[index] = null;
					}

					if (index == 0)
					{
						if (GUILayout.Button("Scene", GUILayout.MaxWidth(50)))
						{
							if (SceneView.lastActiveSceneView != null)
							{
								cameras[index] = SceneView.lastActiveSceneView.camera;
							}
						}
					}

					if (GUILayout.Button("Save", GUILayout.MaxWidth(40)))
					{
						if (cameras[index] != null)
						{
							SaveCameraView(cameras[index]);
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			for (int i = 0; i < verticalCount; i++)
			{
				EditorGUILayout.BeginHorizontal();
				for (int j = 0; j < horizontalCount; j++)
				{
					if (cameras[i * horizontalCount + j] == null) continue;
					RenderPreview(cameras[i * horizontalCount + j], j + 1, i + 1);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void RenderPreview(Camera srcCamera, int posx = 0, int posy = 0)
        {
            if (srcCamera == null) return;
			if (posx == 0 || posx > horizontalCount) return;
			if (posy == 0 || posy > verticalCount) return;

            RenderTexture drawTexture = RenderTexture.GetTemporary(cameraResolution, cameraResolution, 24);
            srcCamera.targetTexture = drawTexture;
            srcCamera.Render();

            Rect drawRect = new Rect(5 + (posx - 1) * (renderSize + 2), 30 + verticalCount * 20 + (posy - 1) * (renderSize + 2), renderSize, renderSize);

            EditorGUI.DrawPreviewTexture(drawRect, drawTexture, null, ScaleMode.ScaleToFit);

			if (Event.current.isMouse && Event.current.type == EventType.MouseUp && Event.current.button == 1 && drawRect.Contains(Event.current.mousePosition))
			{
				Selection.activeGameObject = srcCamera.gameObject;
			}

            RenderTexture.ReleaseTemporary(drawTexture);
            srcCamera.targetTexture = null;
			RenderTexture.active = null;
        }

		private void SaveCameraView(Camera srcCamera)
		{
			if (srcCamera == null) return;

			RenderTexture drawTexture = RenderTexture.GetTemporary(cameraResolution, cameraResolution, 24, RenderTextureFormat.ARGB32);
			srcCamera.targetTexture = drawTexture;
			srcCamera.Render();

			Texture2D texture = new Texture2D(cameraResolution, cameraResolution, TextureFormat.ARGB32, false);
			RenderTexture.active = drawTexture;
			texture.ReadPixels(new Rect(0, 0, cameraResolution, cameraResolution), 0, 0);
			texture.Apply();

			byte[] pngData = texture.EncodeToPNG();

			string path = EditorUtility.SaveFilePanel("Save Camera View", "", "CameraView.png", "png");

			if (!string.IsNullOrEmpty(path))
			{
				System.IO.File.WriteAllBytes(path, pngData);
				Debug.Log($"Saved: {path}");
			}

			RenderTexture.ReleaseTemporary(drawTexture);
			srcCamera.targetTexture = null;
			RenderTexture.active = null;
			DestroyImmediate(texture);
		}

		private void SaveAllViews()
		{
    		if (cameras == null || cameras.Length == 0) return;

		    int width = renderSize * horizontalCount;
    		int height = renderSize * verticalCount;
    		Texture2D combinedTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);

		    for (int i = 0; i < verticalCount; i++)
    		{
        		for (int j = 0; j < horizontalCount; j++)
        		{
            		Camera srcCamera = cameras[i * horizontalCount + j];
		            if (srcCamera != null)
        		    {
		                RenderTexture drawTexture = RenderTexture.GetTemporary(cameraResolution, cameraResolution, 24, RenderTextureFormat.ARGB32);
                		srcCamera.targetTexture = drawTexture;
                		srcCamera.Render();

		                RenderTexture.active = drawTexture;
		                Texture2D tempTexture = new Texture2D(cameraResolution, cameraResolution, TextureFormat.ARGB32, false);
        		        tempTexture.ReadPixels(new Rect(0, 0, cameraResolution, cameraResolution), 0, 0);
                		tempTexture.Apply();

						/* 単体の動作確認用
						byte[] pngDataTest = tempTexture.EncodeToPNG();
						string pathTest = EditorUtility.SaveFilePanel("Save Camera View", "", "CameraView.png", "png");
						if (!string.IsNullOrEmpty(pathTest))
						{
							System.IO.File.WriteAllBytes(pathTest, pngDataTest);
						}
						*/

                		combinedTexture.SetPixels(renderSize * j, renderSize * i, renderSize, renderSize, tempTexture.GetPixels());
						combinedTexture.Apply();

	                	RenderTexture.ReleaseTemporary(drawTexture);
    		            srcCamera.targetTexture = null;
            		    RenderTexture.active = null;
		                DestroyImmediate(tempTexture);
        		    }
		        }
    		}

		    combinedTexture.Apply();

    		byte[] pngData = combinedTexture.EncodeToPNG();

		    string path = EditorUtility.SaveFilePanel("Save Combined Camera View", "", "CombinedCameraView.png", "png");

    		if (!string.IsNullOrEmpty(path))
    		{
		        System.IO.File.WriteAllBytes(path, pngData);
        		Debug.Log($"Combined View Saved: {path}");
    		}

    		DestroyImmediate(combinedTexture);
		}
    }
}
