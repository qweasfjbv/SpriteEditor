using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AutoAnimaker.Core
{
    public static class SpriteEditFuncs
    {
        /// <summary>
        /// Create animation clips from each row of sprites
        /// </summary>
        public static List<AnimationClip> CreateClipsFromSprite(Texture2D texture, in AnimationOptions animOpt)
        {
            UnityEngine.Object[] objects = SliceSprite(texture, animOpt.sliceOptions);
            List<Sprite> sprites = new List<Sprite>();
            List<AnimationClip> clips = new List<AnimationClip>();
            AnimationClip tmpClip;

            int spritePointer = 0;
            int rowCount = texture.height / animOpt.sliceOptions.heightPx;
            for (int i = 0; i < rowCount; i++)
            {
                int columnCount = GetColumnCounts(texture, rowCount - i - 1, animOpt.sliceOptions);

                // Empty column -> ignore
                if (columnCount == 0) continue;

                sprites.Clear();
                for (int j = 0; j < columnCount; j++)
                {
                    sprites.Add(objects[spritePointer++] as Sprite);
                }

                string clipName = StringUtils.GetConventionedName(new string[] {"Anim", animOpt.spriteName, animOpt.animNames[i] }, animOpt.fileNameConvention);

                tmpClip = CreateAnimationClip(sprites, animOpt.clipOptions);
                clips.Add(tmpClip);
                // Debug.Log("Created : " + clipName);
                AssetDatabase.CreateAsset(tmpClip, $"{animOpt.savePath}/{clipName}.anim");
                AssetDatabase.SaveAssets();
            }

            return clips;
        }

        /// <summary>
        /// Slice Sprite and return UnityEngine.Object[]
        /// </summary>
        private static UnityEngine.Object[] SliceSprite(Texture2D texture, in SpriteSliceOptions sliceOpt)
        {
            List<SpriteMetaData> mData = new List<SpriteMetaData>();
            Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(
                texture, Vector2.zero, new Vector2(sliceOpt.widthPx, sliceOpt.heightPx), Vector2.zero);


            string texturePath = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            /** Sprite Setting (can be customized) **/
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

            // HACK: Legacy code
            ti.spritesheet = mData.ToArray();
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);

            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(texturePath)
                .Where(sprite => !sprite.name.Equals(texture.name))
                .ToArray();

            // HACK: Is it necessary?
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

            return assets;
        }

        /// <summary>
        /// Create animation clip from List<Sprite> and options
        /// It returns clip object (asset saving needed)
        /// </summary>
        private static AnimationClip CreateAnimationClip(List<Sprite> sprites, in AnimClipOptions clipOpt)
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
                keyFrames[i].time = clipOpt.frameTime * i/1000;
                keyFrames[i].value = sprites[i];
            }
            keyFrames[keyFrames.Length - 1] = new ObjectReferenceKeyframe();
            keyFrames[keyFrames.Length - 1].time = clipOpt.frameTime * sprites.Count/1000;
            keyFrames[keyFrames.Length - 1].value = sprites[0];

            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

            AnimationClipSettings setting = AnimationUtility.GetAnimationClipSettings(clip);
            setting.loopTime = clipOpt.isLoop;
            AnimationUtility.SetAnimationClipSettings(clip, setting);

            return clip;
        }

        /// <summary>
        /// Create Override animator controller with Base anim controller and Clips
        /// </summary>
        public static void CreateOverrideAnimator(List<AnimationClip> clips, in AnimatorOverrideOptions overrideOpt)
        {
            AnimatorOverrideController overrideController = new AnimatorOverrideController(overrideOpt.baseController);
            List<KeyValuePair<AnimationClip, AnimationClip>> clipOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
            overrideController.GetOverrides(clipOverrides);

            var orderedOverrides = clipOverrides.OrderBy(item => item.Key.name).ToArray();
            var orderedClips = clips.OrderBy(item => item.name).ToArray();

            if (clips.Count() != orderedOverrides.Count())
            {
                Debug.LogError("Error: clips and clip overrides count mismatch!");
                return;
            }

            int count = 0;
            for (int i = 0; i < orderedOverrides.Length; i++) {
                orderedOverrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(orderedOverrides[i].Key, orderedClips[count++]);
            }
            clipOverrides = orderedOverrides.ToList();

            overrideController.ApplyOverrides(clipOverrides);
            string animatorName = StringUtils.GetConventionedName(
                new string[] { "Override", overrideOpt.baseController.name }, 
                overrideOpt.animOpt.fileNameConvention);
            AssetDatabase.CreateAsset(overrideController, overrideOpt.animOpt.savePath + animatorName + ".overrideController");
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// If there are nothing to slice, then except empty space for Loopable animation clip.
        /// So we need to count column.
        /// </summary>
        private static int GetColumnCounts(Texture2D texture, int rowIdx, in SpriteSliceOptions sliceOpt)
        {
            int columntCount = texture.width / sliceOpt.widthPx;
            int cellWidth = sliceOpt.widthPx;
            int cellHeight = sliceOpt.heightPx;

            if (!texture.isReadable)
            {
                MakeTextureReadable(texture);
            }

            int validColumns = 0;
            for (int col = 0; col < columntCount; col++)
            {
                int x = col * (int)cellWidth;
                int y = rowIdx * (int)cellHeight;

                if (!IsCellEmpty(texture, x, y, (int)cellWidth, (int)cellHeight))
                {
                    validColumns++;
                }
            }
            return validColumns;
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
        private static void MakeTextureReadable(Texture2D texture)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);

            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

    }

}