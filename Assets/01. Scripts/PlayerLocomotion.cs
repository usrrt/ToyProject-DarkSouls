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

        Transform cameraObject; // main 카메라의 위치값을 저장
        InputHandler inputHandler;
        PlayerManager manager;

        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform; // 나(플레이어)의 위치를 저장할 변수;

        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera; // 나중에 락온카메라의 역할을 한다

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
        Vector3 normalVector; // 이름이 애매하지만 추후 알게된다고함
        Vector3 targetPosition;

        // 회전 조작 함수
        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveAmountOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical; // 캐릭터 카메라 방향 바라보게함
            targetDir += cameraObject.right * inputHandler.horizontal; // 캐릭터가 카메라에 따라 회전함
            targetDir.Normalize();
            targetDir.y = 0;

            // 사용자의 입력값이 없으면 방향은 캐릭터 자신이 바라보는 방향으로 설정
            if (targetDir == Vector3.zero)
            {
                targetDir = myTransform.forward;
            }

            // 바라보는 방향으로 회전하기위한 과정
            float rs = rotationSpeed;

            // Quaternion.LookRotation 함수는 주어진 벡터 방향을 바라보는 회전상태를 쿼터니언으로 반환
            // 캐릭터가 입력한 방향을 바라보게 함
            Quaternion tr = Quaternion.LookRotation(targetDir);

            // 자연스러운 연출을 위해 Slerp를 사용해 서서히 바라보게함
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            // 회전 상태를 목표로 설정하여 원하는 방향을 바라보게 한다
            myTransform.rotation = targetRotation;
        }

        public void HandleMovement(float delta)
        {
            if (inputHandler.rollFlag)
                return;

            // 카메라의 전방향으로 입력값을 곱한뒤 저장
            // 캐릭터가 카메라 방향으로 움직일수있다
            moveDirection = cameraObject.forward * inputHandler.vertical;

            // 카메라 오른쪽방향으로 입력값을 곱함
            // 캐릭터가 카메라 방향에따라 좌우로 움직일수있다
            moveDirection += cameraObject.right * inputHandler.horizontal;

            // 방향만을 나타내는 정규화 작업
            // 대각선 이동시 속도가 빨라지는것 방지
            moveDirection.Normalize();
            moveDirection.y = 0; // 캐릭터가 카메라를 바라보면서 다가올때 공중부양하는 현상 방지

            float speed = movementSpeed;

            if (inputHandler.sprintFlag)
            {
                speed = sprintSpeed;
                manager.isSprinting = true;
            }
            moveDirection *= speed;

            // 평면에 벡터를 투영해 이동속도를 평면에 맞게 조정하는 작업
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(0, inputHandler.moveAmount, manager.isSprinting); // 블랜드트리 value값 변화

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
