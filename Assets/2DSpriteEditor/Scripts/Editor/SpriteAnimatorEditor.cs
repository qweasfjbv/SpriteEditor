using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;

public enum AnimDir { SE = 0, SW, NE, NW}
public enum AnimName { Attack = 0, DieSoul, Dmg, Idle, Walk}


public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}

[System.Serializable]
public class RowPair
{
    public string name;
    public int column;
}

[System.Serializable]
public class SpriteAnimatorStruct
{
    public Texture2D sprites;
    public RowPair[] settings = new RowPair[0];
}


public class SpriteAnimatorEditor : EditorWindow
{
    private string characterName = "NewAnimator";
    private string storePath;

    private Texture2D[] spriteSheets = new Texture2D[5];
    private AnimationClip[,] animationClips = new AnimationClip[5, 4];

    private int widthPx;
    private int heightPx;
    private AnimatorOverrideController overrideController;
    private UnityEditor.Animations.AnimatorController overriddenConroller;

    private AnimationClipOverrides clipOverrides;

    int arraySize;
    public SpriteAnimatorStruct[] animatorStructs = new SpriteAnimatorStruct[0];


    [MenuItem("Window/Auto Sprite Animator/Override Animator")]
    public static void ShowWindow()
    {
        GetWindow<SpriteAnimatorEditor>("Override Animator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Animator Settings", EditorStyles.boldLabel);
        GUILayout.Space(20);

        characterName = EditorGUILayout.TextField("Character Name", characterName);
        overriddenConroller = (UnityEditor.Animations.AnimatorController)EditorGUILayout.ObjectField("Overridden Controller", overriddenConroller, typeof(UnityEditor.Animations.AnimatorController), false);


        GUILayout.Space(20);

        if (GUILayout.Button("Set Store path"))
        {
            storePath = EditorUtility.OpenFolderPanel("Store path", "", "");
            storePath = storePath.Substring(storePath.IndexOf('A'));
        }

        GUILayout.Label("Store Path : " + storePath);
        GUILayout.Space(20);

        widthPx = EditorGUILayout.IntSlider(widthPx, 1, 1024);
        heightPx = EditorGUILayout.IntSlider(heightPx, 1, 1024);


        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty property = so.FindProperty("animatorStructs");

        EditorGUILayout.PropertyField(property, true);
        so.ApplyModifiedProperties();

        /*
        spriteSheets[0] = (Texture2D)EditorGUILayout.ObjectField("Attack Sprite", spriteSheets[0], typeof(Texture2D), false);
        spriteSheets[1] = (Texture2D)EditorGUILayout.ObjectField("Die Sprite", spriteSheets[1], typeof(Texture2D), false);
        spriteSheets[2] = (Texture2D)EditorGUILayout.ObjectField("Dmg Sprite", spriteSheets[2], typeof(Texture2D), false);
        spriteSheets[3] = (Texture2D)EditorGUILayout.ObjectField("Idle Sprite", spriteSheets[3], typeof(Texture2D), false);
        spriteSheets[4] = (Texture2D)EditorGUILayout.ObjectField("Walk Sprite", spriteSheets[4], typeof(Texture2D), false);
        */

        if (GUILayout.Button("Create Animator"))
        {
            CreateAnimator();
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

        for (int i = 0; i < Enum.GetValues(typeof(AnimName)).Length; i++)
        {
            SliceSpriteSheet(spriteSheets[i], i);
        }

        
        overrideController = new AnimatorOverrideController(overriddenConroller);
        clipOverrides = new AnimationClipOverrides(overrideController.overridesCount);


        overrideController.GetOverrides(clipOverrides);
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

        overrideController.ApplyOverrides(clipOverrides);

        AssetDatabase.CreateAsset(overrideController, storePath + "/" + characterName + "_Animator.overrideController");
        AssetDatabase.SaveAssets();


        Debug.Log("Animator and Override Controller created successfully.");
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

        Array.Sort(assets, (a, b)=>
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
        clipName = characterName + "_" + Enum.GetName(typeof(AnimName), sheetIdx) + "_" + (sheetIdx != 1 ? Enum.GetName(typeof(AnimDir), dir) : "");

        EditorCurveBinding curveBinding = new EditorCurveBinding();
        curveBinding.type = typeof(SpriteRenderer);
        curveBinding.path = "";
        curveBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Count+1];

        for (int i = 0; i < sprites.Count; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe();
            keyFrames[i].time = i / 6f;
            keyFrames[i].value = sprites[i];

        }
        keyFrames[keyFrames.Length-1] = new ObjectReferenceKeyframe();
        keyFrames[keyFrames.Length - 1].time = (keyFrames.Length - 1) / 6f;
        keyFrames[keyFrames.Length - 1].value = sprites[0];


        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

        if (sheetIdx == (int)AnimName.Idle || sheetIdx == (int)AnimName.Walk)
        {
            AnimationClipSettings setting = AnimationUtility.GetAnimationClipSettings(clip);
            setting.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, setting);
        }
        animationClips[sheetIdx, (sheetIdx == (int)AnimName.DieSoul) ? 0 : dir] = clip;

        // 에셋으로 생성 후 저장
        AssetDatabase.CreateAsset(clip, storePath + "/" + clipName + ".anim");
        AssetDatabase.SaveAssets();

        return;
    }

 
}