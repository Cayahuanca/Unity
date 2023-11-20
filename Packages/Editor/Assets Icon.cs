using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Praecipua.EE
{
    [InitializeOnLoad]
    public class AssetsIconDraw
    {
        static AssetsIconDraw()
        {
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
        }

        private static void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            // AssetsIconMapping.asset をロード
            AssetsIconMapping iconMapping = AssetDatabase.LoadAssetAtPath<AssetsIconMapping>("Assets/Praecipua/AssetsIconMapping.asset");

            // アイコンをマッピングから検索
            Texture2D icon = null;
            if (iconMapping != null && iconMapping.iconMappings != null)
            {
                foreach (var mapping in iconMapping.iconMappings)
                {
                    if (mapping.guid == guid)
                    {
                        icon = mapping.icon;
                        break;
                    }
                }
            }

            // アイコンを描画
            if (icon != null)
            {
                float iconMaxSize = selectionRect.height;
                float iconBackSize = iconMaxSize;
                float iconSize = iconMaxSize;
                if (iconMaxSize > 32)
                {
                    iconSize = iconBackSize - 16;
                }
                if (selectionRect.x == 14)
                {
                    selectionRect.x = selectionRect.x + 2;
                }

                // アイコンのサイズを表示サイズに合わせて調整
                Rect iconRect = new Rect(selectionRect.x, selectionRect.y, iconSize, iconSize);
                GUI.DrawTexture(iconRect, icon);
            }
        }
    }
}
