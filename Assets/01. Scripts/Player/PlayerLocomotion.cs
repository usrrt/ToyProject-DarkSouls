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
    public Transform myTransform; // ��(�÷��̾�)�� ��ġ�� ������ ����;

    [HideInInspector]
    public PlayerAnimator playerAnim;

    public new Rigidbody rigidbody;
    public GameObject normalCamera; // ���߿� ����ī�޶��� ������ �Ѵ�

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
            targetDir += cameraHandler.cameraTransform.right * inputHandler.horizontal; // ĳ���Ͱ� ī�޶� ���� ȸ����
            targetDir.Normalize();
            targetDir.y = 0;

            // ������� �Է°��� ������ ������ ĳ���� �ڽ��� �ٶ󺸴� �������� ����
            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            // Quaternion.LookRotation �Լ��� �־��� ���� ������ �ٶ󺸴� ȸ�����¸� ���ʹϾ����� ��ȯ
            // ĳ���Ͱ� �Է��� ������ �ٶ󺸰� ��
            Quaternion tr = Quaternion.LookRotation(targetDir);

            // �ڿ������� ������ ���� Slerp�� ����� ������ �ٶ󺸰���
            Quaternion targetRotation = Quaternion.Slerp(
                myTransform.rotation,
                tr,
                rotationSpeed * delta
            );

            // ȸ�� ���¸� ��ǥ�� �����Ͽ� ���ϴ� ������ �ٶ󺸰� �Ѵ�
            myTransform.rotation = targetRotation;
        }
    }

    public void HandleMovement(float delta)
    {
        if (inputHandler.rollFlag)
            return;

        if (manager.isInteracting)
            return;

        // ī�޶��� ���������� �Է°��� ���ѵ� ����
        // ĳ���Ͱ� ī�޶� �������� �����ϼ��ִ�
        moveDirection = cameraHandler.cameraTransform.forward * inputHandler.vertical;

        // ī�޶� �����ʹ������� �Է°��� ����
        // ĳ���Ͱ� ī�޶� ���⿡���� �¿�� �����ϼ��ִ�
        moveDirection += cameraHandler.cameraTransform.right * inputHandler.horizontal;

        // ���⸸�� ��Ÿ���� ����ȭ �۾�
        // �밢�� �̵��� �ӵ��� �������°� ����
        moveDirection.Normalize();
        moveDirection.y = 0; // ĳ���Ͱ� ī�޶� �ٶ󺸸鼭 �ٰ��ö� ���ߺξ��ϴ� ���� ����

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

        // ��鿡 ���͸� ������ �̵��ӵ��� ��鿡 �°� �����ϴ� �۾�
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

    public void HandleJumping() // ���ڸ������� �����ϴ� ���� �߻�
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
