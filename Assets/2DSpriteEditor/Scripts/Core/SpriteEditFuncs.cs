using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SpriteEditor
{

    public static class SpriteEditFuncs
    {

        /// <summary>
        /// Be called for each sprite.
        /// create animation clip of each row
        /// </summary>
        private static void SliceAndCreateClip(Texture2D texture, SpriteSliceOptions sliceOpt)
        {
            List<SpriteMetaData> mData = new List<SpriteMetaData>();
            Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(
                texture, Vector2.zero, new Vector2(sliceOpt.widthPx, sliceOpt.heightPx), Vector2.zero);


            string texturePath = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            /* Sprite Setting / Slice */
            ti.isReadable = true;
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Multiple;
            ti.filterMode = FilterMode.Point;       
            ti.textureCompression = TextureImporterCompression.Uncompressed;

            for (int i = 0; i < rects.Length; i++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.rect = rects[i];
                smd.alignment = (int)SpriteAlignment.Custom;
                smd.pivot = new Vector2(sliceOpt.pivotX, sliceOpt.pivotY);
                smd.name = texture.name + "_" + i;
                mData.Add(smd);
            }

            ti.spritesheet = mData.ToArray();
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);


            /* */
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(texturePath);

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

            // TODO :´Ù½Ã ¼¼¾ßµÊ
            int animFrameCount = texture.width / sliceOpt.widthPx;

            int cnt = 0;
            for (int i = 1; i < assets.Length; i++)
            {
                if (assets[i] is Sprite)
                {
                    sprites.Add(assets[i] as Sprite);
                    cnt++;

                    if (cnt % animFrameCount == 0)
                    {
                        AssetDatabase.CreateAsset(CreateAnimationClip(sprites, new AnimClipOptions()), "PATH + NAME");
                        AssetDatabase.SaveAssets();
                        sprites.Clear();
                    }
                }
            }

            return;
        }

        private static AnimationClip CreateAnimationClip(List<Sprite> sprites, AnimClipOptions clipOpt)
        {
            AnimationClip clip = new AnimationClip();

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

            AnimationClipSettings setting = AnimationUtility.GetAnimationClipSettings(clip);
            setting.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, setting);

            return clip;
        }


        public static void CreateAnimator(AnimatorOverrideController overrideController, UnityEditor.Animations.AnimatorController baseController,
            AnimationClipOverrides clipOverrides, Texture2D[] spriteSheets, string storePath, string characterName)
        {
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



        private static void MakeTextureReadable(Texture2D texture)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);

            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

        private static void CheckColumnCounts(int rows, int columns, int cellWidth, int cellHeight, Texture2D texture)
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
            }
        }

        private static bool IsCellEmpty(Texture2D texture, int x, int y, int width, int height)
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