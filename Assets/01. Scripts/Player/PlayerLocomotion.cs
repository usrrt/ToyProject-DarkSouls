using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    CameraHandler cameraHandler;
    InputHandler inputHandler;
    PlayerManager manager;

    public Vector3 moveDirection;

    [HideInInspector]
    public Transform myTransform; // 나(플레이어)의 위치를 저장할 변수;

    [HideInInspector]
    public PlayerAnimator playerAnim;

    public new Rigidbody rigidbody;
    public GameObject normalCamera; // 나중에 락온카메라의 역할을 한다

    [Header("Ground & Air Detection Stats")]
    [SerializeField]
    float groundDetectionRayStartPoint = 0f;

    [SerializeField]
    float minimumDistanceNeededBeginFall = 1f;

    [SerializeField]
    float groundDirectionRayDistance = 0.5f;

    [SerializeField]
    float groundSphereCastRadius = 0.3f;

    [SerializeField]
    float lerfTime;
    LayerMask ignoreForGroundCheck;
    public float inAirTimer;

    [Header("Stats")]
    [SerializeField]
    float walkingSpeed = 3f;

    [SerializeField]
    float movementSpeed = 5f;

    [SerializeField]
    float sprintSpeed = 7f;

    [SerializeField]
    float rotationSpeed = 10f;

    [SerializeField]
    float fallingSpeed = 45f;

    private void Awake()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
        rigidbody = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        playerAnim = GetComponentInChildren<PlayerAnimator>();
        manager = GetComponent<PlayerManager>();
        myTransform = transform;
    }

    private void Start()
    {
        playerAnim.Init();

        ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        manager.isGrounded = true;
    }

    #region Movement
    Vector3 normalVector;
    Vector3 targetPosition;

    private void HandleRotation(float delta)
    {
        Vector3 targetDir;
        if (inputHandler.lockOnFlag)
        {
            if (inputHandler.sprintFlag || inputHandler.rollFlag)
            {
                targetDir = cameraHandler.cameraTransform.forward * inputHandler.vertical;
                targetDir += cameraHandler.cameraTransform.right * inputHandler.horizontal;
            }
            else
            {
                targetDir = cameraHandler.currentLockOnTarget.position - myTransform.position;
            }
            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(
                myTransform.rotation,
                tr,
                rotationSpeed * delta
            );

            myTransform.rotation = targetRotation;
        }
        else
        {
            targetDir = cameraHandler.cameraTransform.forward * inputHandler.vertical;
            targetDir += cameraHandler.cameraTransform.right * inputHandler.horizontal; // 캐릭터가 카메라에 따라 회전함
            targetDir.Normalize();
            targetDir.y = 0;

            // 사용자의 입력값이 없으면 방향은 캐릭터 자신이 바라보는 방향으로 설정
            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            // Quaternion.LookRotation 함수는 주어진 벡터 방향을 바라보는 회전상태를 쿼터니언으로 반환
            // 캐릭터가 입력한 방향을 바라보게 함
            Quaternion tr = Quaternion.LookRotation(targetDir);

            // 자연스러운 연출을 위해 Slerp를 사용해 서서히 바라보게함
            Quaternion targetRotation = Quaternion.Slerp(
                myTransform.rotation,
                tr,
                rotationSpeed * delta
            );

            // 회전 상태를 목표로 설정하여 원하는 방향을 바라보게 한다
            myTransform.rotation = targetRotation;
        }
    }

    public void HandleMovement(float delta)
    {
        if (inputHandler.rollFlag)
            return;

        if (manager.isInteracting)
            return;

        // 카메라의 전방향으로 입력값을 곱한뒤 저장
        // 캐릭터가 카메라 방향으로 움직일수있다
        moveDirection = cameraHandler.cameraTransform.forward * inputHandler.vertical;

        // 카메라 오른쪽방향으로 입력값을 곱함
        // 캐릭터가 카메라 방향에따라 좌우로 움직일수있다
        moveDirection += cameraHandler.cameraTransform.right * inputHandler.horizontal;

        // 방향만을 나타내는 정규화 작업
        // 대각선 이동시 속도가 빨라지는것 방지
        moveDirection.Normalize();
        moveDirection.y = 0; // 캐릭터가 카메라를 바라보면서 다가올때 공중부양하는 현상 방지

        float speed = movementSpeed;
        if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f)
        {
            speed = sprintSpeed;
            manager.isSprinting = true;
            moveDirection *= speed;
        }
        else
        {
            if (inputHandler.moveAmount < 0.5)
            {
                moveDirection *= walkingSpeed;
                manager.isSprinting = false;
            }
            else
            {
                moveDirection *= speed;
                manager.isSprinting = false;
            }
        }

        // 평면에 벡터를 투영해 이동속도를 평면에 맞게 조정하는 작업
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rigidbody.velocity = projectedVelocity;

        if (inputHandler.lockOnFlag && inputHandler.sprintFlag == false)
        {
            playerAnim.UpdateAnimatorValues(
                inputHandler.horizontal,
                inputHandler.vertical,
                manager.isSprinting
            );
        }
        else
        {
            playerAnim.UpdateAnimatorValues(0, inputHandler.moveAmount, manager.isSprinting);
        }

        if (playerAnim.canRotate)
        {
            HandleRotation(delta);
        }
    }

    public void HandleRollAndSprintAnim(float delta)
    {
        if (playerAnim.anim.GetBool("isInteracting"))
            return;

        if (inputHandler.rollFlag)
        {
            moveDirection = cameraHandler.cameraTransform.forward * inputHandler.vertical;
            moveDirection += cameraHandler.cameraTransform.right * inputHandler.horizontal;

            if (inputHandler.moveAmount > 0)
            {
                playerAnim.PlayTargetAnimation("Rolling", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotation;
            }
            else
            {
                playerAnim.PlayTargetAnimation("Backstep", true);
            }
        }
    }

    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        manager.isGrounded = false;
        RaycastHit hit;
        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;
        Vector3 adjustedMoveDir = moveDirection;
        if (
            Physics.SphereCast(
                origin,
                groundSphereCastRadius,
                Vector3.down,
                out hit,
                minimumDistanceNeededBeginFall
            )
        )
        {
            adjustedMoveDir = Vector3.zero;
        }

        if (manager.isInAir)
        {
            rigidbody.AddForce(Vector3.down * fallingSpeed);
            rigidbody.AddForce(adjustedMoveDir * fallingSpeed / 8f);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin += dir * groundDirectionRayDistance;

        targetPosition = myTransform.position;

        if (
            Physics.SphereCast(
                origin,
                groundSphereCastRadius,
                Vector3.down,
                out hit,
                minimumDistanceNeededBeginFall,
                ignoreForGroundCheck
            )
        )
        {
            normalVector = hit.normal;
            manager.isGrounded = true;
            targetPosition.y = hit.point.y;

            if (manager.isInAir)
            {
                if (inAirTimer > 0.5f)
                {
                    playerAnim.PlayTargetAnimation("Land", true);
                    inAirTimer = 0;
                }
                else
                {
                    playerAnim.PlayTargetAnimation("Empty", false);
                    inAirTimer = 0;
                }

                manager.isInAir = false;
            }
        }
        else
        {
            if (manager.isGrounded)
            {
                manager.isGrounded = false;
            }

            if (manager.isInAir == false)
            {
                if (manager.isInteracting == false)
                {
                    playerAnim.PlayTargetAnimation("Falling", true);
                }

                rigidbody.velocity = moveDirection.normalized * (movementSpeed / 2);
                manager.isInAir = true;
            }
        }

        if (manager.isInteracting || inputHandler.moveAmount > 0)
        {
            myTransform.position = Vector3.Lerp(
                myTransform.position,
                targetPosition,
                lerfTime * Time.deltaTime
            );
        }
        else
        {
            myTransform.position = targetPosition;
        }
    }

    public void HandleJumping() // 제자리에서만 점프하는 문제 발생
    {
        if (manager.isInteracting)
            return;

        if (inputHandler.jump_Input)
        {
            if (inputHandler.moveAmount > 0)
            {
                playerAnim.PlayTargetAnimation("Jump", true);
                moveDirection = cameraHandler.cameraTransform.forward * inputHandler.vertical;
                moveDirection += cameraHandler.cameraTransform.right * inputHandler.horizontal;
                //rigidbody.AddForce(moveDirection);
                moveDirection.y = 0;
                myTransform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
    }

    #endregion
}
