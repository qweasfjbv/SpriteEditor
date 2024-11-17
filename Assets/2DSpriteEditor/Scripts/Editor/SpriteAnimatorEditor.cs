using UnityEngine;
using UnityEditor;
using SpriteEditor.Core;
using System.Collections.Generic;

namespace SpriteEditor.Editor
{
    public class SpriteAnimatorEditor : EditorWindow
    {

        /** Editor Tab **/
        private int selectedTabIndex = 0;
        private string[] tabs = new string[]
        {
            "Animation Clip Maker",
            "Override Controller Setter"
        };

        private GridByEnum gridByEnum = 0;
        private FileNameConventionEnum fileNameConventionEnum = 0;

        /** Basic Settings **/
        private string spriteName;
        private string storePath;

        /** Sprite Settings **/
        private int widthPx;
        private int heightPx;
        private int rowCount;
        private int columnCount;
        private float pivotX;
        private float pivotY;

        /** Animation Clip Settings **/
        private bool isLoop;
        private float frameTime;
        private UnityEditor.Animations.AnimatorController baseController;
        public SpriteAnimatorStruct[] animatorStructs = new SpriteAnimatorStruct[0];


        /** Editor Layout Settings **/
        private bool isShowPreview = false;

        private SpritePreviewPopup previewPopup;
        private float halfRatio;
        private float halfLabelRatio;
        private float halfContentRatio;

        private void InitOrLoad()
        {
            /** Layout Values **/
            halfRatio = 0.45f;
            halfLabelRatio = halfRatio * 0.33f;
            halfContentRatio = halfRatio * 0.66f;

            baseController = null;

            // TODO - Save/Load Needed
            spriteName = "sample";
            storePath = StringUtils.PreprocessPath(Constants.PATH_BASIC);

            selectedTabIndex = 0;
            gridByEnum = 0;

            widthPx = 8; heightPx = 8;
            rowCount = 1; columnCount = 1;
            pivotX = 0.5f; pivotY = 0.5f;

            frameTime = 60f;
            isLoop = false;
        }

        [MenuItem("Window/Sprite Editor/Helper Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<SpriteAnimatorEditor>("Sprite Editor");
            window.InitOrLoad();
            window.minSize = new Vector2(600, 600);
            window.maxSize = new Vector2(600, 600);
            Rect tmpRect = window.position;
            window.position = tmpRect;
        }

        private Vector2 scrollPosition = Vector2.zero;
        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(600));

            DrawHeader();
            GUILayout.Space(20);

            DrawSpriteSetting();
            GUILayout.Space(10);
            EditorUtil.GuiLine();
            GUILayout.Space(10);

            DrawAnimationSetting();

            DrawPreviewButtons();
            GUILayout.Space(20);

            DrawFooter();
            GUILayout.Space(20);
            GUILayout.EndScrollView();
        }
        
        private void DrawHeader()
        {

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
            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, tabs, GUILayout.Width(Screen.width / 1.5f), GUILayout.Height(25));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorUtil.GuiLine(1);
            GUILayout.Space(10);

            switch (selectedTabIndex)
            {
                case 0:
                    DrawAnimationMakerTab();
                    break;
                case 1:
                    DrawOverrideControllerSetterTab();
                    break;
            }

            spriteName = EditorGUILayout.TextField("Sprite Name", spriteName);
            if (selectedTabIndex == 1)
                baseController = (UnityEditor.Animations.AnimatorController)EditorGUILayout.ObjectField("Base Controller", baseController, typeof(UnityEditor.Animations.AnimatorController), false);

        }
        private void DrawAnimationMakerTab()
        {

        }
        private void DrawOverrideControllerSetterTab()
        {

        }

        private void DrawSpriteSetting()
        {

            GUILayout.Label("Sprite Setting", EditorUtil.GetH3LabelStyle());

            GUILayout.BeginHorizontal(GUILayout.Width(position.width * halfRatio));
            GUILayout.Label("Grid By : ");
            gridByEnum = (GridByEnum)EditorGUILayout.EnumPopup(gridByEnum);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            switch (gridByEnum)
            {
                case GridByEnum.CellCount:
                    GUILayout.Label("Col Count ", GUILayout.Width(position.width * halfLabelRatio));
                    columnCount = EditorGUILayout.IntSlider(columnCount, 1, 48, GUILayout.Width(position.width * halfContentRatio));
                    break;
                case GridByEnum.CellSize:
                    GUILayout.Label("Width px ", GUILayout.Width(position.width * halfLabelRatio));
                    widthPx = EditorGUILayout.IntSlider(widthPx, 8, 256, GUILayout.Width(position.width * halfContentRatio));
                    break;
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label("Pivot X ", GUILayout.Width(position.width * halfLabelRatio));
            pivotX = EditorGUILayout.Slider(pivotX, 0f, 1f, GUILayout.Width(position.width * halfContentRatio));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            switch (gridByEnum)
            {
                case GridByEnum.CellCount:
                    GUILayout.Label("Row Count ", GUILayout.Width(position.width * halfLabelRatio));
                    rowCount = EditorGUILayout.IntSlider(rowCount, 1, 48, GUILayout.Width(position.width * halfContentRatio));
                    break;
                case GridByEnum.CellSize:
                    GUILayout.Label("Height px ", GUILayout.Width(position.width * halfLabelRatio));
                    heightPx = EditorGUILayout.IntSlider(heightPx, 8, 256, GUILayout.Width(position.width * halfContentRatio));
                    break;
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label("Pivot Y ", GUILayout.Width(position.width * halfLabelRatio));
            pivotY = EditorGUILayout.Slider(pivotY, 0f, 1f, GUILayout.Width(position.width * halfContentRatio));
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            // TODO - Offset, Padding needed
            // -> drawline in preview popup should be modified

        }
        private void DrawAnimationSetting()
        {

            GUILayout.Label("Animation Setting", EditorUtil.GetH3LabelStyle());

            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(position.width * 0.4f));
            GUILayout.Label("Clip Name Convention : ");
            fileNameConventionEnum = (FileNameConventionEnum)EditorGUILayout.EnumPopup(fileNameConventionEnum);
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(GUILayout.Width(position.width * 0.2f));
            GUILayout.Label("Frame time(ms) ");
            frameTime = EditorGUILayout.FloatField(frameTime);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal(GUILayout.Width(position.width * 0.2f));
            GUILayout.Label("Is Loop ");
            isLoop = EditorGUILayout.Toggle(isLoop);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            

            if (animatorStructs.Length > 0)
            {
                foreach (var value in animatorStructs)
                {
                    if (value.sprite == null) continue;
                    UpdateRowColumn(gridByEnum, new Vector2Int(value.sprite.width, value.sprite.height),
                        ref widthPx, ref heightPx, ref columnCount, ref rowCount);


                    for (int i = value.animationNames.Count; i < rowCount; i++)
                    {
                        value.animationNames.Add($"motion{i}");
                    }

                    for (int i = value.animationNames.Count; i > rowCount; i--)
                    {
                        value.animationNames.RemoveAt(value.animationNames.Count - 1);
                    }
                }
            }


            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty property = so.FindProperty("animatorStructs");

            EditorGUILayout.PropertyField(property, true);
            so.ApplyModifiedProperties();

        }
        private void DrawPreviewButtons()
        {

            if (GUILayout.Button("Show Sprite Preview"))
            {

                if (animatorStructs[0].sprite != null)
                {
                    if (!isShowPreview && previewPopup == null)
                    {
                        isShowPreview = true;
                        previewPopup = CreateInstance<SpritePreviewPopup>();
                        Rect rect = new Rect(0, 0, animatorStructs[0].sprite.width, animatorStructs[0].sprite.height);
                        previewPopup.PreviewSprite = Sprite.Create(animatorStructs[0].sprite, rect, new Vector2(0.5f, 0.5f));
                        previewPopup.RowCount = rowCount;
                        previewPopup.ColumnCount = columnCount;
                        previewPopup.ShowAsDropDown(new Rect(position.x, position.y, position.width, position.height), new Vector2(600, 600));
                    }
                }
                else
                {
                    ClosePreviewPopup();
                }
            }
            else
            {
                isShowPreview = false;
            }
        }
        private void DrawFooter()
        {

            GUILayout.Label("Store Path : " + storePath, EditorUtil.GetTruncateLabelStyle());

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Store path"))
            {
                storePath = EditorUtility.OpenFolderPanel("Store path", "", "");
                if (storePath == "") storePath = Constants.PATH_BASIC;
                storePath = StringUtils.PreprocessPath(storePath);
            }

            if (selectedTabIndex == 0)
            {
                if (GUILayout.Button("Create Animation"))
                {
                    foreach (var animatorStruct in animatorStructs)
                    {
                        AnimationOptions animOpt = GetAnimOption();
                        for (int i = 0; i < animatorStruct.animationNames.Count; i++)
                        {
                            animOpt.animNames.Add(animatorStruct.animationNames[i]);
                        }
                        SpriteEditFuncs.CreateClipsFromSprite(animatorStruct.sprite, animOpt);
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Create Override Controller"))
                {
                    List<AnimationClip> clips = new List<AnimationClip>();
                    foreach (var animatorStruct in animatorStructs)
                    {
                        AnimationOptions animOpt = GetAnimOption();
                        for (int i = 0; i < animatorStruct.animationNames.Count; i++)
                        {
                            animOpt.animNames.Add(animatorStruct.animationNames[i]);
                        }
                        clips.AddRange(SpriteEditFuncs.CreateClipsFromSprite(animatorStruct.sprite, animOpt));
                    }

                    SpriteEditFuncs.CreateOverrideAnimator(clips, GetOverrideOption());
                }
            }

            GUILayout.EndHorizontal();

        }

        private void UpdateRowColumn(GridByEnum gridByEnum, Vector2Int textureSize, ref int widthPx, ref int heightPx, ref int colCount, ref int rowCount)
        {
            switch (gridByEnum)
            {
                case GridByEnum.CellCount:
                    widthPx = textureSize.x / colCount;
                    heightPx = textureSize.y / rowCount;
                    break;
                case GridByEnum.CellSize:
                    colCount = textureSize.x / widthPx;
                    rowCount = textureSize.y / heightPx;
                    break;
            }

            return;
        }
        private void ClosePreviewPopup()
        {
            if (previewPopup != null)
            {
                previewPopup.Close();
                previewPopup = null;
            }
        }

        private AnimationOptions GetAnimOption()
        {
            SpriteSliceOptions sliceOpt = new SpriteSliceOptions();
            sliceOpt.widthPx = widthPx;
            sliceOpt.heightPx = heightPx;
            sliceOpt.pivotX = pivotX;
            sliceOpt.pivotY = pivotY;

            AnimClipOptions clipOpt = new AnimClipOptions();
            clipOpt.frameTime = frameTime;
            clipOpt.isLoop = isLoop;

            AnimationOptions animOpt = new AnimationOptions();
            animOpt.spriteName = spriteName;
            animOpt.savePath = storePath;
            animOpt.animNames = new System.Collections.Generic.List<string>();
            animOpt.sliceOptions = sliceOpt;
            animOpt.clipOptions = clipOpt;
            animOpt.fileNameConvention = fileNameConventionEnum;

            return animOpt;
        }
        private AnimatorOverrideOptions GetOverrideOption()
        {
            AnimatorOverrideOptions overrideOpt = new AnimatorOverrideOptions();
            overrideOpt.baseController = baseController;
            overrideOpt.animOpt = GetAnimOption();

            return overrideOpt;
        }
    }
}