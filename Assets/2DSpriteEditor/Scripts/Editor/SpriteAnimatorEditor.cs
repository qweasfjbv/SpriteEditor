using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;

namespace SpriteEditor
{
    public class SpriteAnimatorEditor : EditorWindow
    {


        private int selectedTab = 0;
        private string[] tabs = new string[]
        {
            "Animation Maker", "Override Controller Setter"
        };


        private string characterName = "Name";
        private string storePath = "Assets/";

        private Texture2D[] spriteSheets = new Texture2D[5];
        private AnimationClip[,] animationClips = new AnimationClip[5, 4];

        private int widthPx;
        private int heightPx;
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

        private void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 30;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            GUILayout.Space(20);
            GUILayout.Label("Sprite Editor", style);
            GUILayout.Space(10);
            GuiLine();

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
                    DrawAnimationMakerTab();
                    break;
                case 1:
                    DrawOverrideControllerSetterTab();
                    break;
            }


            characterName = EditorGUILayout.TextField("Sprite Name", characterName);
            baseController = (UnityEditor.Animations.AnimatorController)EditorGUILayout.ObjectField("Base Controller", baseController, typeof(UnityEditor.Animations.AnimatorController), false);


            GUILayout.Space(20);

            if (GUILayout.Button("Set Store path"))
            {
                storePath = EditorUtility.OpenFolderPanel("Store path", "", "");
                string[] paths = System.IO.Directory.GetFiles(storePath);
            }

            GUILayout.Label("Store Path : " + storePath);
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Width px ");
            widthPx = EditorGUILayout.IntSlider(widthPx, 1, 1024);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Height px ");
            heightPx = EditorGUILayout.IntSlider(heightPx, 1, 1024);
            GUILayout.EndHorizontal();

            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty property = so.FindProperty("animatorStructs");

            EditorGUILayout.PropertyField(property, true);
            so.ApplyModifiedProperties();

            
            if (GUILayout.Button("Create Animator"))
            {
                CheckColumnCounts(6, 7, 48, 48, animatorStructs[0].sprite);
            }

        }

        private void CreateAnimator()
        {
            // 예외처리. 시트는 전부 지정되어있어야 합니다.
            for (int i = 0; i < spriteSheets.Length; i++)
            {
                if (spriteSheets[i] == null)
                {
                    Debug.LogError("Sprite Sheet is null.");
                    return;
                }
            }

            /*
            for (int i = 0; i < Enum.GetValues(typeof(AnimName)).Length; i++)
            {
                SliceSpriteSheet(spriteSheets[i], i);
            }
            */

            overrideController = new AnimatorOverrideController(baseController);
            clipOverrides = new AnimationClipOverrides(overrideController.overridesCount);


            overrideController.GetOverrides(clipOverrides);

            /*
            for (int i = 0; i < Enum.GetValues(typeof(AnimName)).Length; i++)
            {
                if (i == 1)
                {
                    clipOverrides["Human_DieSoul"] = animationClips[1, 0];
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {

                        clipOverrides["Human_" + Enum.GetName(typeof(AnimName), i) + "_" + Enum.GetName(typeof(AnimDir), j)] = animationClips[i, j];
                        Debug.Log(animationClips[i, j].name);
                    }
                }
            }

            */
            overrideController.ApplyOverrides(clipOverrides);

            AssetDatabase.CreateAsset(overrideController, storePath + "/" + characterName + "_Animator.overrideController");
            AssetDatabase.SaveAssets();


            Debug.Log("Animator and Override Controller created successfully.");
        }
        public void DrawAnimationMakerTab()
        {

        }
        public void DrawOverrideControllerSetterTab()
        {

        }


        // 각 애니메이션 스프라이트 당 한 번 호출됩니다.
        // 각 row를 애니메이션 클립으로 만듭니다.
        private void SliceSpriteSheet(Texture2D texture, int sheetIdx)
        {

            List<SpriteMetaData> mData = new List<SpriteMetaData>();
            Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(
                texture, Vector2.zero, new Vector2(widthPx, heightPx), Vector2.zero);


            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

            ti.isReadable = true;
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Multiple;
            ti.spritePixelsPerUnit = 8;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;


            for (int i = 0; i < rects.Length; i++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.rect = rects[i];
                smd.alignment = (int)SpriteAlignment.Custom;
                smd.pivot = new Vector2(0.5f, 0.4f);
                smd.name = texture.name + "_" + i;
                mData.Add(smd);
            }



            ti.spritesheet = mData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

            Array.Sort(assets, (a, b) =>
            {
                var tmp1 = a.name.Split('_');
                var tmp2 = b.name.Split('_');

                int part1, part2;
                bool check1 = int.TryParse(System.IO.Path.GetFileNameWithoutExtension(tmp1[tmp1.Length - 1]), out part1);
                bool check2 = int.TryParse(System.IO.Path.GetFileNameWithoutExtension(tmp2[tmp2.Length - 1]), out part2);

                if (!check1) part1 = -1;
                if (!check2) part2 = -1;

                if (part1 == part2) return 0;
                else if (part1 < part2) return -1;
                else return 1;
            });


            List<Sprite> sprites = new List<Sprite>();

            // column 개수. 애니메이션 당 들어가야 할 이미지 개수를 셉니다.
            int animFrameCount = texture.width / widthPx;

            int dir = 0;
            int cnt = 0;
            for (int i = 1; i < assets.Length; i++)
            {
                if (assets[i] is Sprite)
                {
                    sprites.Add(assets[i] as Sprite);
                    cnt++;

                    if (cnt % animFrameCount == 0)
                    {
                        CreateAnimationClip(sprites, sheetIdx, dir++);
                        sprites.Clear();
                    }
                }
            }

            return;


        }

        private void CreateAnimationClip(List<Sprite> sprites, int sheetIdx, int dir)
        {
            AnimationClip clip = new AnimationClip();
            string clipName = "";
            clipName = characterName;

            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.type = typeof(SpriteRenderer);
            curveBinding.path = "";
            curveBinding.propertyName = "m_Sprite";

            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Count + 1];

            for (int i = 0; i < sprites.Count; i++)
            {
                keyFrames[i] = new ObjectReferenceKeyframe();
                keyFrames[i].time = i / 6f;
                keyFrames[i].value = sprites[i];

            }
            keyFrames[keyFrames.Length - 1] = new ObjectReferenceKeyframe();
            keyFrames[keyFrames.Length - 1].time = (keyFrames.Length - 1) / 6f;
            keyFrames[keyFrames.Length - 1].value = sprites[0];


            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

            /*
            if (sheetIdx == (int)AnimName.Idle || sheetIdx == (int)AnimName.Walk)
            {
                AnimationClipSettings setting = AnimationUtility.GetAnimationClipSettings(clip);
                setting.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(clip, setting);
            }
            animationClips[sheetIdx, (sheetIdx == (int)AnimName.DieSoul) ? 0 : dir] = clip;
            */

            // 에셋으로 생성 후 저장
            AssetDatabase.CreateAsset(clip, storePath + "/" + clipName + ".anim");
            AssetDatabase.SaveAssets();

            return;
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

        private void MakeTextureReadable(Texture2D texture)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);

            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate); 
        }

        private void CheckColumnCounts(int rows, int columns, int cellWidth, int cellHeight, Texture2D texture)
        {

            if (!texture.isReadable)
            {
                MakeTextureReadable(texture);
            }

            for (int row = 0; row < rows; row++)
            {
                int validColumns = 0;

                for (int col = 0; col < columns; col++)
                {
                    int x = col * (int)cellWidth;
                    int y = row * (int)cellHeight;

                    if (!IsCellEmpty(texture, x, y, (int)cellWidth, (int)cellHeight))
                    {
                        validColumns++;
                    }

                }

                // TODO : sprite (has diff col count) Test needed
                Debug.Log($"ROW : {row} , VALID : {validColumns}");
            }
        }

        private bool IsCellEmpty(Texture2D texture, int x, int y, int width, int height)
        {
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    if (i < texture.width && j < texture.height)
                    {
                        Color pixelColor = texture.GetPixel(i, j);
                        if (pixelColor.a > 0) return false;
                    }
                }
            }
            return true;
        }

    }
}