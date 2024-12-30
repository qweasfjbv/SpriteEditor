using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;


namespace AutoAnimaker
{

    [System.Serializable]
    public class SpriteAnimatorStruct
    {
        public Texture2D sprite;
        [SerializeField]
        public List<string> animationNames = new List<string>();

        public SpriteAnimatorStruct GetDeepCopy()
        {
            SpriteAnimatorStruct tmpStruct = new SpriteAnimatorStruct();
            tmpStruct.animationNames = new List<string>(this.animationNames);
            tmpStruct.sprite = this.sprite;
            return tmpStruct;
        }
    }
    
    [System.Serializable]
    public struct SpriteSliceOptions
    {
        public int widthPx, heightPx;
        public float pivotX, pivotY;
        
        // TODO: Optional (not implemented yet)
        public int paddingX, paddingY;
        public int offsetX, offsetY;
    }

    [System.Serializable]
    public struct AnimClipOptions {
        public float frameTime;
        public bool isLoop;
    }

    [System.Serializable]
    public struct AnimatorOverrideOptions
    {
        public AnimationOptions animOpt;
        public AnimatorController baseController;
    }

    [System.Serializable]
    public struct AnimationOptions
    {
        public string spriteName;
        public string savePath;
        public List<string> animNames;
        public FileNameConventionEnum fileNameConvention;
        public SpriteSliceOptions sliceOptions;
        public AnimClipOptions clipOptions;
    }


}