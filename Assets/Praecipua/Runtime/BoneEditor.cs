#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
namespace Praecipua.EE.RT
{
    public class BoneEditor : MonoBehaviour
    {
        private SkinnedMeshRenderer skinMesh;
        private Transform[] bones;
        private Transform[] weightedBones;

        private GUIStyle boneGizmoStyle = new GUIStyle();

        private Color boneColor = Color.white;
        private Color textColor = Color.black;

        private int drawType = 0;
        private int textSize = 12;
        private bool drawWeightedBones = true;
        private bool showBoneName = false;

        void OnDrawGizmos()
        {
            skinMesh = GetComponent<SkinnedMeshRenderer>();
            bones = skinMesh.bones;

            if (skinMesh == null || bones == null)
            {
                return;
            }

            if (drawWeightedBones)
            {
                if (weightedBones == null)
                {
                    weightedBones = ExtractWeightedBones(bones);
                }
                bones = weightedBones;
            }

            foreach (Transform bone in bones)
            {
                // 親オブジェクトがない場合、処理をしない
                if (bone == null)
                {
                    continue;
                }

                // 親オブジェクトが Armature、親オブジェクトとオブジェクトが同名の場合、処理しない
                if (bone.parent.name == bone.name || bone.parent.name == "Armature")
                {
                    continue;
                }

                boneGizmoStyle.normal.textColor = textColor;
                boneGizmoStyle.fontSize = textSize;
                boneGizmoStyle.alignment = TextAnchor.MiddleCenter;
                Gizmos.color = boneColor;

                Vector3 basepoint = Vector3.Lerp(bone.parent.position, bone.position, 0.125f);
                Vector3 direction = (bone.parent.position - bone.position).normalized;

                float distance = Vector3.Distance(bone.parent.position, bone.position);
                float arrowSize = distance * 0.15f;

                if (bone.parent.childCount >= 4)
                {
                    basepoint = Vector3.Lerp(bone.parent.position, bone.position, 0.2f);
                    arrowSize = distance * 0.05f;
                }

                Vector3 upVector = Vector3.up;
                Vector3 rightVector = Vector3.Cross(direction, upVector).normalized;
                if (rightVector == Vector3.zero)
                {
                    rightVector = Vector3.forward;
                }

                rightVector = Quaternion.AngleAxis(45f, direction) * rightVector;

                upVector = Vector3.Cross(rightVector, direction).normalized;

                // DrawCircle(basepoint, upVector, rightVector, arrowSize, 16);

                if (showBoneName)
                {
                Handles.Label(bone.position, bone.name, boneGizmoStyle);
                }

                switch (drawType)
                {
                    case 0:
                        Gizmos.DrawWireSphere(bone.position, arrowSize * 0.5f);
                        DrawOctahedral(basepoint, upVector, rightVector, bone.parent.position, bone.position, arrowSize);
                        break;
                    case 1:
                        Gizmos.DrawLine(bone.parent.position, bone.position);
                        break;
                    case 2:
                        Gizmos.DrawWireSphere(bone.position, arrowSize * 0.5f);
                        break;
                    case 3:
                        Gizmos.DrawWireSphere(bone.position, arrowSize * 0.1f);
                        Gizmos.DrawLine(bone.parent.position, bone.position);
                        break;
                    default:
                        break;
                }
            }
        }

        private Transform[] ExtractWeightedBones(Transform[] inputBones)
        {
            bool[] isWeighted = new bool[inputBones.Length];
            BoneWeight[] weights = skinMesh.sharedMesh.boneWeights;

            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i].weight0 > 0f || weights[i].weight1 > 0f || weights[i].weight2 > 0f || weights[i].weight3 > 0f)
                {
                    isWeighted[weights[i].boneIndex0] = true;
                    isWeighted[weights[i].boneIndex1] = true;
                    isWeighted[weights[i].boneIndex2] = true;
                    isWeighted[weights[i].boneIndex3] = true;
                }
            }

            int weightedBoneCount = isWeighted.Count(b => b);

            Transform[] weightedBones = new Transform[weightedBoneCount];
            for (int i = 0, j = 0; i < isWeighted.Length; i++)
            {
                if (isWeighted[i])
                {
                    weightedBones[j++] = inputBones[i];
                }
            }

            // weightedBones の子に、1つだけオブジェクトが存在する場合、その子オブジェクトを weightedBones に追加する。
            for (int i = 0; i < weightedBones.Length; i++)
            {
                if (weightedBones[i].childCount == 1)
                {
                    Transform child = weightedBones[i].GetChild(0);
                    if (child.name != weightedBones[i].name)
                    {
                        Transform[] newWeightedBones = new Transform[weightedBones.Length + 1];
                        for (int j = 0; j < weightedBones.Length; j++)
                        {
                            newWeightedBones[j] = weightedBones[j];
                        }
                        newWeightedBones[weightedBones.Length] = child;
                        weightedBones = newWeightedBones;
                    }
                }
            }

            return weightedBones;
        }

        // 指定した頂点の数で、円を描画する
        private void DrawCircle(Vector3 basePoint, Vector3 upVector, Vector3 rightVector, float radius, int segments)   // basePoint: 円の中心座標, direction: 円の法線ベクトル, radius: 円の半径, segments: 円の分割数
        {
            float angle = 360f / segments;

            Vector3 previousPoint = basePoint + rightVector * radius;

            for (int i = 1; i <= segments; i++)
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * angle * i);
                float y = Mathf.Sin(Mathf.Deg2Rad * angle * i);

                Vector3 currentPoint = basePoint + rightVector * x * radius + upVector * y * radius;
                Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }
        }

        private void DrawOctahedral(Vector3 basePoint, Vector3 upVector, Vector3 rightVector, Vector3 startPoint, Vector3 endPoint, float radius)   // basePoint: 八面体の中心座標, direction: 八面体の法線ベクトル, radius: 八面体の半径
        {
            Vector3[] points = new Vector3[4];
            points[0] = basePoint + rightVector * radius;
            points[1] = basePoint + upVector * radius;
            points[2] = basePoint - rightVector * radius;
            points[3] = basePoint - upVector * radius;

            Gizmos.DrawLine(points[0], startPoint);
            Gizmos.DrawLine(points[0], endPoint);
            Gizmos.DrawLine(points[1], startPoint);
            Gizmos.DrawLine(points[1], endPoint);
            Gizmos.DrawLine(points[2], startPoint);
            Gizmos.DrawLine(points[2], endPoint);
            Gizmos.DrawLine(points[3], startPoint);
            Gizmos.DrawLine(points[3], endPoint);

            Gizmos.DrawLine(points[0], points[1]);
            Gizmos.DrawLine(points[1], points[2]);
            Gizmos.DrawLine(points[2], points[3]);
            Gizmos.DrawLine(points[3], points[0]);

            // Unity 2019 では、Gizmos.DrawLineList が使えないので、大量に Gizmos.DrawLine を呼び出している。
        }

        [CustomEditor(typeof(BoneEditor))]
        public class BoneEditorEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                // base.OnInspectorGUI();

                BoneEditor boneEditor = target as BoneEditor;

                boneEditor.drawType = EditorGUILayout.Popup("Draw Type", boneEditor.drawType, new string[] { "Octahedral", "Line", "Sphere", "Sphere + Line", "None" });

                boneEditor.drawWeightedBones = EditorGUILayout.Toggle("Draw Weighted Bones", boneEditor.drawWeightedBones);


                boneEditor.boneColor = EditorGUILayout.ColorField("Bone Color", boneEditor.boneColor);


                boneEditor.showBoneName = EditorGUILayout.Toggle("Show Bone Name", boneEditor.showBoneName);


                boneEditor.textColor = EditorGUILayout.ColorField("Text Color", boneEditor.textColor);

                boneEditor.textSize = EditorGUILayout.IntSlider("Text Size", boneEditor.textSize, 8, 32);
            }
        }
    }
}

#endif