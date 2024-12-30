using System.Collections.Generic;
using UnityEditor;

namespace AutoAnimaker.Core
{
    public static class PresetLoader
    {
        public static List<AnimOptionSO> LoadScriptableObjects()
        {
            List<AnimOptionSO> scriptableObjects = new List<AnimOptionSO>();

            // type : AnimOptionSO
            string[] guids = AssetDatabase.FindAssets("t:AnimOptionSO", new[] { Constants.PATH_PRESET });
            scriptableObjects.Clear();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AnimOptionSO asset = AssetDatabase.LoadAssetAtPath<AnimOptionSO>(assetPath);
                if (asset != null)
                {
                    scriptableObjects.Add(asset);
                }
            }

            return scriptableObjects;
        }
    }
}