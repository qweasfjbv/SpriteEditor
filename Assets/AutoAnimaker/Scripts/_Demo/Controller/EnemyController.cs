using UnityEngine;

namespace AutoAnimaker.Demo
{
    /// <summary>
    /// Get Input by `Input.GetKeyDown`
    /// Enhanced Input isnt supported.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class EnemyController : BaseController
    {
        private void Start()
        {
            animParams.Add(Constants.ANIM_PARAM_IDLE);
            animParams.Add(Constants.ANIM_PARAM_SPAWN);
            animParams.Add(Constants.ANIM_PARAM_HIT);
        }
    }
}