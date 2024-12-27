using UnityEngine;

namespace AutoAnimaker.Demo
{
    /// <summary>
    /// Get Input by `Input.GetKeyDown`
    /// Enhanced Input isnt supported.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class KnightController : BaseController
    {
        private void Start()
        {
            animParams.Add(Constants.ANIM_PARAM_IDLE);
            animParams.Add(Constants.ANIM_PARAM_WALK);
            animParams.Add(Constants.ANIM_PARAM_RUN);
            animParams.Add(Constants.ANIM_PARAM_ROLL);
            animParams.Add(Constants.ANIM_PARAM_HIT);
            animParams.Add(Constants.ANIM_PARAM_DIE);
        }
    }
}