using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HierarchyLines : MonoBehaviour
{
    static HierarchyLines()
    {
        EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyLine;
    }

    private static void DrawHierarchyLine(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        var objTransform = obj.transform;

        string unityVersion = UnityEngine.Application.unityVersion;
        int majorVersion = int.Parse(unityVersion.Split('.')[0]);

        if (objTransform.parent != null)
        {
            Handles.BeginGUI();

            var start = new Vector2(selectionRect.xMin - 22, selectionRect.yMin);
            var end = new Vector2(selectionRect.xMin - 22, selectionRect.yMax);
            var middle = new Vector2(selectionRect.xMin - 22, selectionRect.yMin + selectionRect.height / 2);
            var toobj = new Vector2(selectionRect.xMin - 14, selectionRect.yMin + selectionRect.height / 2);

            Handles.color = EditorGUIUtility.isProSkin ? new Color( 0.75f, 0.75f, 0.75f, 1.0f ) : new Color( 0.3f, 0.3f, 0.3f, 1.0f );

            // オブジェクトがさらに子を持つか
            if (objTransform.childCount == 0)
            {
                // さらに子を持たない場合、横線の長さを延長
                toobj.x += 8;
            }

            // オブジェクトが、最後の子か
            if (objTransform.GetSiblingIndex() == objTransform.parent.childCount - 1)
            {
                // True の場合、L 字型に
                Handles.DrawLine(start, middle);
                Handles.DrawLine(middle, toobj);
            }
            else
            {
                // False の場合、T 字型に
                Handles.DrawLine(start, end);
                Handles.DrawLine(middle, toobj);
            }

            // 親オブジェクト(階層)の数を取得
            int hierarchyLevel = 0;
            Transform parentTransform = objTransform.parent;
            while (parentTransform != null) {
                hierarchyLevel++;
                parentTransform = parentTransform.parent;
            }

            // 親オブジェクト(階層)の数だけ、処理を繰り返す
            for (int i = 1; hierarchyLevel >= i; i++) {
                Transform currentParent = objTransform.parent;
                for (int j = 1; i > j; j++) {
                    currentParent = currentParent.parent;
                }

                // 親オブジェクトが、その階層での最後の子でない場合に、点線を描画
                if (currentParent.parent != null && currentParent.GetSiblingIndex() != currentParent.parent.childCount - 1) {
                    var dotlinestart = new Vector2(selectionRect.xMin - 22 - i * 14, selectionRect.yMin);
                    var dotlineend = new Vector2(selectionRect.xMin - 22 - i * 14, selectionRect.yMax);
                    if (majorVersion <= 2019)
                    {
                        Handles.DrawDottedLine(dotlinestart, dotlineend, 0.5f);
                    }
                    else
                    {
                        Handles.DrawLine(dotlinestart, dotlineend);
                    }
                }
            }
            Handles.EndGUI();
        }
    }
}