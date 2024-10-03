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

        // string���� ȣ���ϸ� ���������� �Ź� ���ڿ����� �ؽ÷� ��ȯ�ϱ⿡ ������ �������� ���� int������ ĳ���س��� ���°� ����
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

    public void EnableCombo()
    {
        anim.SetBool("canDoCombo", true);
    }

    public void DisableCombo()
    {
        anim.SetBool("canDoCombo", false);
    }

    // Animator������Ʈ�� ���� Root Motion�� ó���Ǵ� ��� �����ӿ� ȣ��
    // Apply Root Motion üũ�ڽ� Ȱ��ȭ���¿��� ����
    private void OnAnimatorMove()
    {
        // �ִϸ��̼ǿ� ���� ĳ���� ������ ����� ��ȣ�ۿ� ���϶��� �����ϰ� �Ѵ�
        if (manager.isInteracting == false)
            return;

        float delta = Time.deltaTime;

        // drag�� 0���� �����Ͽ� �����̳� �������� ���� �ܺ� ������ ĳ���� �����ӿ� ������ �ִ°� ����
        locomotion.rigidbody.drag = 0;

        // �ִϸ��̼ǿ� ���� �̷���� �̵��Ÿ� ����
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;

        // �̵��� �Ÿ��� ��ŸŸ������ ���� �����Ӵ� ĳ���Ͱ� ��� �� �ӵ��� ����Ѵ�
        Vector3 vel = deltaPosition / delta;

        // �ӵ��� ĳ���� rigidbody�� ������� �ִϸ��̼ǿ� ���� �̵��� ���� ĳ������ ���������� �ݿ��ǰ� �Ѵ�
        locomotion.rigidbody.velocity = vel;
    }
}
