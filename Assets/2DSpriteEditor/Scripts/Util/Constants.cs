
using UnityEngine;

namespace SpriteEditor
{
    public static class Constants
    {
        /** Animator Parameters **/
        public static readonly string ANIM_PARAM_WALK       = "IsWalk";
        public static readonly string ANIM_PARAM_JUMP       = "IsJump";
        public static readonly string ANIM_PARAM_FALL       = "IsFall";
        public static readonly string ANIM_PARAM_ATK        = "IsAttack";
        public static readonly string ANIM_PARAM_DAMAGED    = "IsDamaged";
        public static readonly string ANIM_PARAM_DEAD       = "IsDead";


        /** Basic Settings (could be changed) **/
        public static readonly string PATH_BASIC = Application.dataPath;

    }
}