using AutoAnimaker.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AutoAnimaker.Editor
{
    public class SpriteAnimatorEditor : EditorWindow
    {

        private List<AnimOptionSO> scriptableObjects = new List<AnimOptionSO>();
        private AnimOptionSO selectedObject;

        /** Editor Tab **/
        private int selectedTabIndex = 0;
        private string[] tabs = new string[]
        {
            "Anim Clip Maker",
            "Animator Override Controller"
        };

        private GridByEnum gridByEnum = 0;
        private FileNameConventionEnum fileNameConventionEnum = 0;

        /** Preset Settings **/
        private bool isPresetFold;

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

        private string[] presetNames;

        /// <summary>
        /// Currently initialization code is here.
        /// TODO : Load code is needed
        /// </summary>
        private void InitOrLoad()
        {
            /** Layout Values **/
            halfRatio = 0.45f;
            halfLabelRatio = halfRatio * 0.33f;
            halfContentRatio = halfRatio * 0.66f;

            baseController = null;

            // TODO - Save/Load Needed
            selectedTabIndex = 0;
            gridByEnum = 0;

            LoadPreset();

            isPresetFold = false;
        }

        [MenuItem("Window/Auto Animaker/Helper Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<SpriteAnimatorEditor>("AutoAnimaker");
            window.InitOrLoad();
            window.minSize = new Vector2(600, 600);
            window.maxSize = new Vector2(600, 600);
            Rect tmpRect = window.position;
            window.position = tmpRect;

        }

        private void OnEnable()
        {
            scriptableObjects = PresetLoader.LoadScriptableObjects();
            if (scriptableObjects != null && scriptableObjects.Count > 0 )
            {
                selectedObject = scriptableObjects[0];
            }
        }

        private void OnDisable()
        {
            ClosePreviewPopup();
        }

        private Vector2 scrollPosition = Vector2.zero;
        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(600));
            {

                DrawHeader();

                DrawSpriteSetting();
                GUILayout.Space(10);
                EditorUtil.GuiLine();
                GUILayout.Space(10);

                DrawAnimationSetting();

                DrawPreviewButtons();
                GUILayout.Space(20);

                DrawFooter();
                GUILayout.Space(20);


            }
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
            GUILayout.Label("Auto Animaker", style);
            GUILayout.Space(10);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, tabs, GUILayout.Width(Screen.width / 1.5f), GUILayout.Height(25));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            isPresetFold = EditorGUILayout.Foldout(isPresetFold, "Preset Settings");

            GUILayout.Space(10);
            EditorUtil.GuiLine(1);
            GUILayout.Space(10);
            
            if (isPresetFold)
            {
                DrawPresetLoadTab();

                GUILayout.Space(10);
                EditorUtil.GuiLine(3);
                GUILayout.Space(10);
            }

        }

        private string presetName;
        private void DrawPresetLoadTab()
        {
            GUILayout.Label("Create Preset", EditorUtil.GetH3LabelStyle());

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width * 0.4f));
                GUILayout.Label("Preset Name");
                presetName = EditorGUILayout.TextField("", presetName);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width * 0.3f));
                if (GUILayout.Button("Save as new preset..."))
                {
                    if (presetName == null || presetName.Length == 0)
                    {
                        EditorUtility.DisplayDialog("Denied",
                            "Preset name cannot be empty", "OK");
                    }
                    else
                    {
                        AnimOptionSO tmpSo = PresetLoader.CreateNewPreset(presetName);
                        if (tmpSo == null)
                        {
                            EditorUtility.DisplayDialog("Denied",
                                "The preset with the same name already exists.", "OK");
                        }
                        else
                        {
                            SavePreset(tmpSo);
                            scriptableObjects = PresetLoader.LoadScriptableObjects();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            EditorUtil.GuiLine(1);
            GUILayout.Space(10);

            GUILayout.Label("Save/Load Preset", EditorUtil.GetH3LabelStyle());

            // ScriptableObject Selection Dropdown
            if (scriptableObjects.Count > 0)
            {
                presetNames = scriptableObjects.Select(obj => obj.name).ToArray();
                int selectedIndex = scriptableObjects.IndexOf(selectedObject);
                int newSelectedIndex = EditorGUILayout.Popup("Select Preset", selectedIndex, presetNames, GUILayout.Width(position.width * 0.6f));

                if (newSelectedIndex != selectedIndex)
                {
                    selectedObject = scriptableObjects[newSelectedIndex];
                    Debug.Log("Selected ScriptableObject: " + selectedObject.name);
                }
            }

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            {
                Color saveColor = GUI.backgroundColor;

                if (GUILayout.Button("Save Preset"))
                    SavePreset(selectedObject);

                if (GUILayout.Button("Load Preset"))
                    LoadPreset();

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Remove Preset"))
                    RemovePreset();

                GUI.backgroundColor = saveColor;
            }
            EditorGUILayout.EndHorizontal();
        }


        private void DrawSpriteSetting()
        {
            GUILayout.Label("Sprite Setting", EditorUtil.GetH3LabelStyle());

            spriteName = EditorGUILayout.TextField("Sprite Name", spriteName);
            if (selectedTabIndex == 1)
                baseController = (UnityEditor.Animations.AnimatorController)EditorGUILayout.ObjectField("Base Controller", baseController, typeof(UnityEditor.Animations.AnimatorController), false);

            GUILayout.Space(10);

            GUILayout.BeginHorizontal(GUILayout.Width(position.width * halfRatio));
            {
                GUILayout.Label("Grid By : ");
                gridByEnum = (GridByEnum)EditorGUILayout.EnumPopup(gridByEnum);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
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
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
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
            }
            GUILayout.EndHorizontal();

            // TODO - Offset, Padding needed
            // -> drawline in preview popup should be modified

        }
        private void DrawAnimationSetting()
        {

            GUILayout.Label("Animation Setting", EditorUtil.GetH3LabelStyle());

            GUILayout.BeginHorizontal();
            {
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
            }
            GUILayout.EndHorizontal();
            
            // 
            if (animatorStructs.Length > 0)
            {
                foreach (var value in animatorStructs)
                {
                    if (value.sprite == null) continue;
                    UpdateRowColumn(gridByEnum, new Vector2Int(value.sprite.width, value.sprite.height),
                        ref widthPx, ref heightPx, ref columnCount, ref rowCount);

                    for (int i = value.animationNames.Count; i < rowCount; i++)
                    {
                        // HACK - default value : can be changed by string[]
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
                        previewPopup.position = new Rect(position.x, position.y, position.width, position.height);
                        previewPopup.Show();
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

            if (previewPopup != null)
            {
                Rect rect = new Rect(0, 0, animatorStructs[0].sprite.width, animatorStructs[0].sprite.height);
                previewPopup.PreviewSprite = Sprite.Create(animatorStructs[0].sprite, rect, new Vector2(0.5f, 0.5f));
                previewPopup.RowCount = rowCount;
                previewPopup.ColumnCount = columnCount;
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

        /// <summary>
        /// Function to update row, column info
        ///     with considering empty column.
        /// </summary>
        private void UpdateRowColumn(GridByEnum gridByEnum, Vector2Int textureSize, 
            ref int widthPx, ref int heightPx, ref int colCount, ref int rowCount)
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


        public void SavePreset(AnimOptionSO optionSO)
        {
            optionSO.spriteName = spriteName;
            optionSO.baseController = baseController;

            optionSO.gridBy = gridByEnum;
            optionSO.widthPx = widthPx;
            optionSO.heightPx = heightPx;
            optionSO.colCount = columnCount;
            optionSO.rowCount = rowCount;
            optionSO.pivotX = pivotX;
            optionSO.pivotY = pivotY;

            optionSO.nameConvention = fileNameConventionEnum;
            optionSO.frameTime = frameTime;
            optionSO.isLoop = isLoop;

            // NOT REF. Deep Copy
            Array.Clear(optionSO.animatorStructs, 0, optionSO.animatorStructs.Count());
            optionSO.animatorStructs = new SpriteAnimatorStruct[animatorStructs.Length];
            for (int i = 0; i < animatorStructs.Length; i++)
            {
                optionSO.animatorStructs[i] = animatorStructs[i].GetDeepCopy();
            }

            optionSO.storePath = new String(storePath);

            EditorUtility.SetDirty(optionSO);
        }

        public void LoadPreset()
        {
            spriteName = selectedObject.spriteName;
            baseController = selectedObject.baseController;

            gridByEnum = selectedObject.gridBy;
            widthPx = selectedObject.widthPx;
            heightPx = selectedObject.heightPx;
            columnCount = selectedObject.colCount;
            rowCount = selectedObject.rowCount;
            pivotX = selectedObject.pivotX;
            pivotY = selectedObject.pivotY;

            fileNameConventionEnum = selectedObject.nameConvention;
            frameTime = selectedObject.frameTime;
            isLoop = selectedObject.isLoop;

            Array.Clear(animatorStructs, 0, animatorStructs.Count());
            animatorStructs = new SpriteAnimatorStruct[selectedObject.animatorStructs.Length];
            for (int i = 0; i < selectedObject.animatorStructs.Length; i++)
            {
                animatorStructs[i] = selectedObject.animatorStructs[i].GetDeepCopy();
            }

            storePath = new String(selectedObject.storePath);
        }


        private void RemovePreset()
        {
            if (scriptableObjects.Count <= 1)
            {
                EditorUtility.DisplayDialog("Denied",
                    "You cannot delete this asset because it is the only one remaining.",
                    "OK");
                return;
            }

            PresetLoader.RemovePreset(selectedObject.name);
            scriptableObjects = PresetLoader.LoadScriptableObjects();
            selectedObject = scriptableObjects[0];
        }

    }
}