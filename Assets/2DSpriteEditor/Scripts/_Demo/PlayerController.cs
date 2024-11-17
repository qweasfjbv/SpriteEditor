using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

namespace SpriteEditor.Demo
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
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                ConvertAnimState(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ConvertAnimState(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ConvertAnimState(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ConvertAnimState(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ConvertAnimState(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ConvertAnimState(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                ConvertAnimState(6);
            }
        }

        private void ConvertAnimState(int id)
        {
            animator.SetBool(Constants.ANIM_PARAM_WALK, false);
            animator.SetBool(Constants.ANIM_PARAM_JUMP, false);
            animator.SetBool(Constants.ANIM_PARAM_FALL, false);
            animator.SetBool(Constants.ANIM_PARAM_ATK, false);
            animator.SetBool(Constants.ANIM_PARAM_DAMAGED, false);
            animator.SetBool(Constants.ANIM_PARAM_DEAD, false);
            switch (id)
            {
                case 1:
                    animator.SetBool(Constants.ANIM_PARAM_WALK, true); break;
                case 2:
                    animator.SetBool(Constants.ANIM_PARAM_JUMP, true); break;
                case 3:
                    animator.SetBool(Constants.ANIM_PARAM_FALL, true); break;
                case 4:
                    animator.SetBool(Constants.ANIM_PARAM_ATK, true); break;
                case 5:
                    animator.SetBool(Constants.ANIM_PARAM_DAMAGED, true); break;
                case 6:
                    animator.SetBool(Constants.ANIM_PARAM_DEAD, true); break;
            }
        }
    }
}