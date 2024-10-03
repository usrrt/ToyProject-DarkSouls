using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    PlayerManager manager;
    PlayerLocomotion locomotion;

    public Animator anim;

    public bool canRotate;

    int horizontal;
    int vertical;

    public void Init()
    {
        anim = GetComponent<Animator>();
        locomotion = GetComponentInParent<PlayerLocomotion>();
        manager = GetComponentInParent<PlayerManager>();

        // string으로 호출하면 간편하지만 매번 문자열에서 해시로 변환하기에 성능이 떨어진다 따라서 int값으로 캐싱해놓고 쓰는게 좋음
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorValues(
        float horizontalMovement,
        float verticalMovement,
        bool isSprinting
    )
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

        if (isSprinting)
        {
            v = 2;
            h = horizontalMovement;
        }

        anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);

        // 애니메이션 상태간의 부드러운 전환을 가능하게 한다(자연스러운 애니메이션 시퀀스)
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

    public void EnableCombo()
    {
        anim.SetBool("canDoCombo", true);
    }

    public void DisableCombo()
    {
        anim.SetBool("canDoCombo", false);
    }

    // Animator컴포넌트에 의해 Root Motion이 처리되는 모든 프레임에 호출
    // Apply Root Motion 체크박스 활성화상태에서 동작
    private void OnAnimatorMove()
    {
        // 애니메이션에 의해 캐릭터 움직임 제어는 상호작용 중일때만 가능하게 한다
        if (manager.isInteracting == false)
            return;

        float delta = Time.deltaTime;

        // drag를 0으로 설정하여 마찰이나 공기저항 같은 외부 요인이 캐릭터 움직임에 영항을 주는것 방지
        locomotion.rigidbody.drag = 0;

        // 애니메이션에 의해 이루어진 이동거리 저장
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;

        // 이동한 거리를 델타타임으로 나눠 프레임당 캐릭터가 얻게 될 속도를 계산한다
        Vector3 vel = deltaPosition / delta;

        // 속도를 캐릭터 rigidbody에 적용시켜 애니메이션에 의한 이동이 실제 캐릭터의 움직임으로 반영되게 한다
        locomotion.rigidbody.velocity = vel;
    }
}
