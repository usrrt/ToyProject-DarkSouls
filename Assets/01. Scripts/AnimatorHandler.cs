using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SW
{
    public class AnimatorHandler : MonoBehaviour
    {
        // ###############################################
        //             NAME : HongSW
        //             MAIL : gkenfktm@gmail.com
        // ###############################################

        public Animator anim;
        public InputHandler inputHandler;
        public PlayerLocomotion locomotion;

        public bool canRotate;

        int horizontal;
        int vertical;

        public void Init()
        {
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            locomotion = GetComponentInParent<PlayerLocomotion>();

            // string���� ȣ���ϸ� ���������� �Ź� ���ڿ����� �ؽ÷� ��ȯ�ϱ⿡ ������ �������� ���� int������ ĳ���س��� ���°� ����
            horizontal = Animator.StringToHash("Horizontal");
            vertical = Animator.StringToHash("Vertical");
        }

        public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement)
        {
            #region Vertical
            float v = 0;

            if (verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (verticalMovement > 0.55f)
            {
                v = 1;
            }
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                v = 1;
            }
            else
            {
                v = 0;
            }

            #endregion

            #region Horizontal
            float h = 0;

            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = 1;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }
            #endregion

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("isInteracting", isInteracting);

            // �ִϸ��̼� ���°��� �ε巯�� ��ȯ�� �����ϰ� �Ѵ�(�ڿ������� �ִϸ��̼� ������)
            anim.CrossFade(targetAnim, 0.2f);
        }

        public void CanRotation()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }

        private void OnAnimatorMove()
        {
            if (inputHandler.isInteracting == false)
                return;

            float delta = Time.deltaTime;
            locomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 vel = deltaPosition / delta;
            locomotion.rigidbody.velocity = vel;
        }
    }
}
