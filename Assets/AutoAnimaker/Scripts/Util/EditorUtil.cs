using UnityEditor;
using UnityEngine;

namespace AutoAnimaker
{
    public static class EditorUtil
    {
        public static void GuiLine(int height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
        public static GUIStyle GetH1LabelStyle()
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = 25;
            labelStyle.normal.textColor = Color.white;
            return labelStyle;
        }
        public static GUIStyle GetH2LabelStyle()
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.padding = new RectOffset
            {
                left = 10,
                bottom = 10,
            };
            labelStyle.fontSize = 20;
            labelStyle.normal.textColor = Color.white;
            return labelStyle;
        }
        public static GUIStyle GetH3LabelStyle()
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.padding = new RectOffset
            {
                left = 10,
                bottom = 10,
            };
            labelStyle.fontSize = 15;
            labelStyle.normal.textColor = Color.white;
            return labelStyle;
        }
        public static GUIStyle GetTruncateLabelStyle()
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.padding = new RectOffset
            {
                left = 5,
                bottom = 5,
            };
            labelStyle.fontSize = 11;
            labelStyle.normal.textColor = Color.white;
            labelStyle.wordWrap = true;
            return labelStyle;
        }
    }
}