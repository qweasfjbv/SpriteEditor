using System.Collections.Generic;
using UnityEngine;


namespace SpriteEditor
{
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
        [SerializeField]
        public string name = "";
        [SerializeField]
        public int column;
    }

    [System.Serializable]
    public class SpriteAnimatorStruct
    {
        public Texture2D sprite;
        [SerializeField]
        public List<string> animationNames = new List<string>();
    }
    
    [System.Serializable]
    public struct SpriteSliceOptions
    {
        public int widthPx, heightPx;
        public float pivotX, pivotY;
        
        // TODO: Optional (not implemented yet) **/
        public int paddingX, paddingY;
        public int offsetX, offsetY;
    }

    [System.Serializable]
    public struct AnimClipOptions {
        public float frameGap;
        public bool isLoop;
    }

    [System.Serializable]
    public struct AnimatorOverrideOptions
    {
        public AnimationOptions animOptions;

    }

    [System.Serializable]
    public struct AnimationOptions
    {
        public string spriteName;
        public string savePath;
        public List<string> animNames;
        public SpriteSliceOptions sliceOptions;
        public AnimClipOptions clipOptions;
    }


}