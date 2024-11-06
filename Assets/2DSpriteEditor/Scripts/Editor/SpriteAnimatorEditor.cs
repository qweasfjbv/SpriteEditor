using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace SpriteEditor
{
    public class SpriteAnimatorEditor : EditorWindow
    {


        private int selectedTab = 0;
        private string[] tabs = new string[]
        {
        "Sprite Cutter", "Animation Maker", "Override Controller Setter"
        };


        private string characterName = "Name";
        private string storePath = "Assets/";

        private AnimationClip[,] animationClips = new AnimationClip[5, 4];

        private int widthPx = 16;
        private int heightPx = 16;
        private float pivotX = 0.5f;
        private float pivotY = 0.5f;
        private AnimatorOverrideController overrideController;
        private UnityEditor.Animations.AnimatorController baseController;

        private AnimationClipOverrides clipOverrides;

        public SpriteAnimatorStruct[] animatorStructs = new SpriteAnimatorStruct[0];


        [MenuItem("Window/Sprite Editor/Helper Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<SpriteAnimatorEditor>("Sprite Editor");
            window.minSize = new Vector2(600, 600);
        }
        Vector2 scrollPosition = Vector2.zero;
        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(700));

            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 30;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            GUILayout.Space(20);
            GUILayout.Label("Sprite Editor", style);
            GUILayout.Space(10);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            selectedTab = GUILayout.Toolbar(selectedTab, tabs, GUILayout.Width(Screen.width / 1.5f), GUILayout.Height(25));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GuiLine(1);
            GUILayout.Space(10);

            switch (selectedTab)
            {
                case 0:
                    DrawSpriteCutterTab();
                    break;
                case 1:
                    DrawAnimationMakerTab();
                    break;
                case 2:
                    DrawOverrideControllerSetterTab();
                    break;
            }



            characterName = EditorGUILayout.TextField("Sprite Name", characterName);
            baseController = (UnityEditor.Animations.AnimatorController)EditorGUILayout.ObjectField("Base Controller", baseController, typeof(UnityEditor.Animations.AnimatorController), false);


            /** ----------------------- Sprite Setting ----------------------- **/


            GUILayout.Space(20);
            GUILayout.Label("Sprite Setting", GetH3LabelStyle());
            GUILayout.BeginHorizontal();
            GUILayout.Label("Width px ");
            widthPx = EditorGUILayout.IntSlider(widthPx, 16, 1024);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Pivot X ");
            pivotX = EditorGUILayout.Slider(pivotX, 0f, 1f);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Height px ");
            heightPx = EditorGUILayout.IntSlider(heightPx, 16, 1024);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Pivot Y ");
            pivotY = EditorGUILayout.Slider(pivotY, 0f, 1f);
            GUILayout.EndHorizontal();

            // TODO : Offset, Padding needed
            GUILayout.Space(10);
            GuiLine();
            GUILayout.Space(10);
            GUILayout.Label("Animation Setting", GetH3LabelStyle());

            if (animatorStructs.Length > 0)
            {
                foreach(var value in animatorStructs)
                {
                    if (value.sprites == null) continue;
                    int cnt = value.sprites.height / heightPx;
                    for(int i=value.settings.Count; i<cnt; i++)
                    {
                        value.settings.Add(new RowPair());
                    }
                }
            }

            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty property = so.FindProperty("animatorStructs");

            EditorGUILayout.PropertyField(property, true);
            so.ApplyModifiedProperties();

            GUILayout.Space(20);
            GUILayout.Label("Store Path : " + storePath);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Store path"))
            {
                storePath = EditorUtility.OpenFolderPanel("Store path", "", "");
                string[] paths = System.IO.Directory.GetFiles(storePath);
            }

            if (GUILayout.Button("Create Animation"))
            {
                //SpriteEditFuncs.CreateAnimationClip;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }

        public void DrawSpriteCutterTab()
        {

        }
        public void DrawAnimationMakerTab()
        {

        }
        public void DrawOverrideControllerSetterTab()
        {

        }


        private void GuiLine(int height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        private GUIStyle GetH1LabelStyle()
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = 25;
            labelStyle.normal.textColor = Color.white;
            return labelStyle;
        }
        private GUIStyle GetH2LabelStyle()
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

        private GUIStyle GetH3LabelStyle()
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


    }
}