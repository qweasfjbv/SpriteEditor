using UnityEditor;
using UnityEngine;


namespace AutoAnimaker.Editor
{
    [CustomEditor(typeof(AnimOptionSO))]
    public class AnimOptionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spriteName"), new GUIContent("Sprite Name"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseController"), new GUIContent("Base Controller"));

            EditorGUILayout.Space(10);
            EditorUtil.GuiLine(1);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gridBy"), new GUIContent("Grid By"));

            switch ((GridByEnum)serializedObject.FindProperty("gridBy").enumValueIndex)
            {
                case GridByEnum.CellSize:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("widthPx"), new GUIContent("Width (px)"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("heightPx"), new GUIContent("Height (px)"));
                    break;
                case GridByEnum.CellCount:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("colCount"), new GUIContent("Column Count"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("rowCount"), new GUIContent("Row Count"));
                    break;
            }

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("pivotX"), new GUIContent("Pivot X"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pivotY"), new GUIContent("Pivot Y"));

            EditorGUILayout.Space(10);
            EditorUtil.GuiLine(1);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nameConvention"), new GUIContent("Name Convention"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frameTime"), new GUIContent("Frame Time"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isLoop"), new GUIContent("Is Loop"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("animatorStructs"), new GUIContent("Animator Structs"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("storePath"), new GUIContent("Store Path"));

            // SerializedObject Àû¿ë
            serializedObject.ApplyModifiedProperties();
        }
    }
}