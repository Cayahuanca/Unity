using UnityEngine;

namespace Praecipua.EE
{
    [CreateAssetMenu(fileName = "AssetsIconMapping", menuName = "Custom/AssetsIcon Mapping")]
    public class AssetsIconMapping : ScriptableObject
    {
        [System.Serializable]
        public struct IconMapping
        {
            public string guid;
            public Texture2D icon;
        }

        public IconMapping[] iconMappings;
    }
}
