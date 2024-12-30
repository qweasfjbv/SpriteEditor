using UnityEditor.Animations;
using UnityEngine;

namespace AutoAnimaker
{
    [CreateAssetMenu(fileName = "AnimakerOption", menuName = "Animaker/AnimakerOption")]
    public class AnimOptionSO : ScriptableObject
    {
        public string spriteName;
        public AnimatorController baseController;

        [Header("Sprite Setting")]
        /** Sprite Setting **/
        public GridByEnum gridBy;
        public int widthPx;
        public int heightPx;
        public int colCount;
        public int rowCount;
        public float pivotX;
        public float pivotY;

        [Header("Animation Setting")]
        /** Animation Setting **/
        public FileNameConventionEnum nameConvention;
        public float frameTime;
        public bool isLoop;
        public SpriteAnimatorStruct[] animatorStructs;
        public string storePath;
    }

}