using System.Collections.Generic;
using UnityEngine;

namespace AutoAnimaker.Demo
{
    [RequireComponent(typeof(Animator))]
    public class BaseController : MonoBehaviour
    {
        protected Animator animator;
        protected List<string> animParams = new List<string>();

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            for (int i = 1; i <= animParams.Count; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    ConvertAnimState(i);
                }
            }
        }

        private void ConvertAnimState(int id)
        {
            for(int i=0; i< animParams.Count; i++)
            {
                animator.SetBool(animParams[i], false);
            }

            animator.SetBool(animParams[id - 1], true);
        }

    }
}