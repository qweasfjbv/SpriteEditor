using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

        public static AnimOptionSO CreateNewPreset(string name)
        {
            if (HasFilesInFolder(name + ".asset")) {
                return null;
            }

            AnimOptionSO newAsset = ScriptableObject.CreateInstance<AnimOptionSO>();
            AssetDatabase.CreateAsset(newAsset, Constants.PATH_PRESET + "/" + name + ".asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newAsset;
            return newAsset;
        }

        private static bool HasFilesInFolder(string name)
        {
            string[] assetGuids = AssetDatabase.FindAssets("", new[] { Constants.PATH_PRESET });

            foreach (string guid in assetGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (System.IO.Path.GetFileName(assetPath.ToUpper()) == name.ToUpper())
                {
                    return true; // File exists
                }
            }

            return false; // No file exists
        }

        public static void RemovePreset(string name)
        {
            AssetDatabase.DeleteAsset(Constants.PATH_PRESET + "/" + name + ".asset"); 
            AssetDatabase.SaveAssets();              
        }
    }
}