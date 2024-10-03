using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    Transform cameraObject; // main ī�޶��� ��ġ���� ����
    InputHandler inputHandler;
    PlayerManager manager;

    public Vector3 moveDirection;

    [HideInInspector]
    public Transform myTransform; // ��(�÷��̾�)�� ��ġ�� ������ ����;

    [HideInInspector]
    public AnimatorHandler animatorHandler;

    public new Rigidbody rigidbody;
    public GameObject normalCamera; // ���߿� ����ī�޶��� ������ �Ѵ�

    [Header("Ground & Air Detection Stats")]
    [SerializeField]
    float groundDetectionRayStartPoint = 0.5f;

    [SerializeField]
    float minimumDistanceNededBeginFall = 1f;

    [SerializeField]
    float groundDirectionRayDistance = 0.2f;

    [SerializeField]
    float groundSphereCastRadius = 0.3f;
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
        rigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        myTransform = transform;
    }

    private void Start()
    {
        manager = GetComponent<PlayerManager>();
        animatorHandler.Init();

        ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        manager.isGrounded = true;
    }

    #region Movement
    Vector3 normalVector;
    Vector3 targetPosition;

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

        if (manager.isInteracting)
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

        animatorHandler.UpdateAnimatorValues(0, inputHandler.moveAmount, manager.isSprinting); // ����Ʈ�� value�� ��ȭ

        if (animatorHandler.canRotate)
        {
            HandleRotation(delta);
        }
    }

    public void HandleRollAndSprintAnim(float delta)
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

    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        manager.isGrounded = false;
        RaycastHit hit;
        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;

        if (
            Physics.SphereCast(
                origin,
                groundSphereCastRadius,
                (myTransform.position - origin).normalized,
                out hit,
                0.4f
            )
        )
        {
            moveDirection = Vector3.zero;
        }

        if (manager.isInAir)
        {
            rigidbody.AddForce(Vector3.down * fallingSpeed);
            rigidbody.AddForce(moveDirection * fallingSpeed / 8f);
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
                minimumDistanceNededBeginFall,
                ignoreForGroundCheck
            )
        )
        {
            normalVector = hit.normal;
            Vector3 tp = hit.point;
            manager.isGrounded = true;
            targetPosition.y = tp.y;

            if (manager.isInAir)
            {
                if (inAirTimer > 0.5f)
                {
                    animatorHandler.PlayTargetAnimation("Land", true);
                    inAirTimer = 0;
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Empty", false);
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
                    animatorHandler.PlayTargetAnimation("Falling", true);
                }

                Vector3 vel = rigidbody.velocity;
                vel.Normalize();
                rigidbody.velocity = vel * (movementSpeed / 2);
                manager.isInAir = true;
            }
        }

        if (manager.isInteracting || inputHandler.moveAmount > 0)
        {
            myTransform.position = Vector3.Lerp(
                myTransform.position,
                targetPosition,
                Time.deltaTime
            );
        }
        else
        {
            myTransform.position = targetPosition;
        }
    }

    #endregion
}
