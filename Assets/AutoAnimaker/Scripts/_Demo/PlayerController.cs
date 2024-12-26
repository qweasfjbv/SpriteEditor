using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

namespace AutoAnimaker.Demo
{
    /// <summary>
    /// Get Input by `Input.GetKeyDown`
    /// Enhanced Input isnt supported.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            for (int i = 1; i <= 3; i++) 
            {
                if(Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    ConvertAnimState(i);
                }
            }
        }

        private void ConvertAnimState(int id)
        {
            animator.SetBool(Constants.ANIM_PARAM_IDLE, false);
            animator.SetBool(Constants.ANIM_PARAM_SPAWN, false);
            animator.SetBool(Constants.ANIM_PARAM_HIT, false);
            switch (id)
            {
                case 1:
                    animator.SetBool(Constants.ANIM_PARAM_IDLE, true); break;
                case 2:
                    animator.SetBool(Constants.ANIM_PARAM_SPAWN, true); break;
                case 3:
                    animator.SetBool(Constants.ANIM_PARAM_HIT, true); break;
            }
        }
    }
}