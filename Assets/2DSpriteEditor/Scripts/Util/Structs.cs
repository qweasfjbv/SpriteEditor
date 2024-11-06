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
        public string name;
        public int column;
    }

    [System.Serializable]
    public class SpriteAnimatorStruct
    {
        public Texture2D sprite;
        public RowPair[] settings = new RowPair[0];
    }
}