using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SW
{
    public class PlayerLocomotion : MonoBehaviour
    {
        // ###############################################
        //             NAME : HongSW
        //             MAIL : gkenfktm@gmail.com
        // ###############################################

        Transform cameraObject; // main ī�޶��� ��ġ���� ����
        InputHandler inputHandler;
        PlayerManager manager;

        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform; // ��(�÷��̾�)�� ��ġ�� ������ ����;

        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera; // ���߿� ����ī�޶��� ������ �Ѵ�

        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5f;

        [SerializeField]
        float sprintSpeed = 7f;

        [SerializeField]
        float rotationSpeed = 10f;

        private void Awake()
        {
            cameraObject = Camera.main.transform;
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            animatorHandler.Init();
            rigidbody = GetComponent<Rigidbody>();
            manager = GetComponent<PlayerManager>();
            myTransform = transform;
        }

        #region Movement
        Vector3 normalVector; // �̸��� �ָ������� ���� �˰Եȴٰ���
        Vector3 targetPosition;

        // ȸ�� ���� �Լ�
        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveAmountOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical; // ĳ���� ī�޶� ���� �ٶ󺸰���
            targetDir += cameraObject.right * inputHandler.horizontal; // ĳ���Ͱ� ī�޶� ���� ȸ����
            targetDir.Normalize();
            targetDir.y = 0;

            // ������� �Է°��� ������ ������ ĳ���� �ڽ��� �ٶ󺸴� �������� ����
            if (targetDir == Vector3.zero)
            {
                targetDir = myTransform.forward;
            }

            // �ٶ󺸴� �������� ȸ���ϱ����� ����
            float rs = rotationSpeed;

            // Quaternion.LookRotation �Լ��� �־��� ���� ������ �ٶ󺸴� ȸ�����¸� ���ʹϾ����� ��ȯ
            // ĳ���Ͱ� �Է��� ������ �ٶ󺸰� ��
            Quaternion tr = Quaternion.LookRotation(targetDir);

            // �ڿ������� ������ ���� Slerp�� ����� ������ �ٶ󺸰���
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            // ȸ�� ���¸� ��ǥ�� �����Ͽ� ���ϴ� ������ �ٶ󺸰� �Ѵ�
            myTransform.rotation = targetRotation;
        }

        public void HandleMovement(float delta)
        {
            if (inputHandler.rollFlag)
                return;

            // ī�޶��� ���������� �Է°��� ���ѵ� ����
            // ĳ���Ͱ� ī�޶� �������� �����ϼ��ִ�
            moveDirection = cameraObject.forward * inputHandler.vertical;

            // ī�޶� �����ʹ������� �Է°��� ����
            // ĳ���Ͱ� ī�޶� ���⿡���� �¿�� �����ϼ��ִ�
            moveDirection += cameraObject.right * inputHandler.horizontal;

            // ���⸸�� ��Ÿ���� ����ȭ �۾�
            // �밢�� �̵��� �ӵ��� �������°� ����
            moveDirection.Normalize();
            moveDirection.y = 0; // ĳ���Ͱ� ī�޶� �ٶ󺸸鼭 �ٰ��ö� ���ߺξ��ϴ� ���� ����

            float speed = movementSpeed;

            if (inputHandler.sprintFlag)
            {
                speed = sprintSpeed;
                manager.isSprinting = true;
            }
            moveDirection *= speed;

            // ��鿡 ���͸� ������ �̵��ӵ��� ��鿡 �°� �����ϴ� �۾�
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(0, inputHandler.moveAmount, manager.isSprinting); // ����Ʈ�� value�� ��ȭ

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (animatorHandler.anim.GetBool("isInteracting"))
                return;

            if (inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if (inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }

        #endregion
    }
}
