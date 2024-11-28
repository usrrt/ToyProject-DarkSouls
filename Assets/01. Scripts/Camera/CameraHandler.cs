using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public static CameraHandler singleton;

    InputHandler inputHandler;
    PlayerManager playerManager;

    public Transform playerTransform; // ī�޶� ���� �÷��̾��� ��ġ
    public Transform cameraTransform; // ���� ī�޶��� ��ġ
    public Transform cameraPivotTransfrom; // pivot ���� ȸ���ϱ� ����
    private Transform holderTransform;

    public LayerMask ignoreLayers; // ī�޶� �ٸ� ��ü�� �浹ó����
    public LayerMask environmentLayers;
    private Vector3 cameraTransformPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 camHeightVel = Vector3.zero;

    public float lookSpeed = 0.1f,
        followSpeed = 0.1f,
        pivotSpeed = 0.03f,
        rotationSpeed = 10f,
        minPivot = -35,
        maxPivot = 35;

    private float defaultPosition,
        targetPosition,
        cameraCurrentVelocity,
        lookAngle, // �¿� �þ߰�
        pivotAngle; // �� �Ʒ� �þ߰�

    public float cameraSphereRadius = 0.2f,
        cameraCollisionOffset = 0.2f,
        minimumCollisionOffset = 0.2f;

    public float lockPivotPosition = 2.25f,
        unlockPivotPosition = 1.65f,
        smoothSpeed;

    [SerializeField]
    List<CharacterManager> availableTargets = new List<CharacterManager>();
    public Transform nearestLockOnTarget;
    public Transform currentLockOnTarget;
    public Transform leftLockTarget;
    public Transform rightLockTarget;
    public float maximumLockOnDist = 30f;

    private void Awake()
    {
        singleton = this;
        holderTransform = transform;
        defaultPosition = cameraTransform.localPosition.z; // �θ���ġ ����(local)���� z ����
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        environmentLayers = LayerMask.NameToLayer("Environment");

        // ���̾��Ű ���׷� �Ҵ�ȵɰ�� ���
        playerManager = FindObjectOfType<PlayerManager>();
        inputHandler = playerManager.GetComponent<InputHandler>();
        playerTransform = playerManager.transform;
    }

    public void FollowTarget(float delta)
    {
        // ref cameraFollowVelocity �� ������ ���޵Ǹ�, ���� �ӵ��� �����ϰ� �����ϴµ� ����Ѵ�
        Vector3 targetPosition = Vector3.SmoothDamp(
            holderTransform.position,
            playerTransform.position,
            ref cameraFollowVelocity,
            delta / followSpeed
        );
        holderTransform.position = targetPosition;
        HandleCameraCollisions(delta);
    }

    public void HandleCameraRotation(float delta, float mouseInputX, float mouseInputY)
    {
        if (inputHandler.lockOnFlag == false && currentLockOnTarget == null)
        {
            lookAngle += (mouseInputX * lookSpeed) / delta; // �¿�

            // -=�� ���콺�� ���� �������� ī�޶� ����, �Ʒ��� �������� ī�޶� �Ʒ���, +=�� ���Ͽ������� ������
            pivotAngle -= (mouseInputY * pivotSpeed) / delta; // ���Ʒ�
            pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);

            // y���� �������� ȸ���Ѵ� -> �� ��
            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;

            // Euler�Լ��� ����� ȸ�����͸� ���ʹϾ����� ��ȯ
            Quaternion targetRotation = Quaternion.Euler(rotation);
            holderTransform.rotation = targetRotation; // ��ü�� ����

            // x���� �������� ȸ���Ѵ� -> �� �Ʒ�
            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);

            // localRotation�� ����ϴ� ������ �θ� �߽����� ��������� ȸ���ϵ��� �ϱ� ����
            cameraPivotTransfrom.localRotation = targetRotation;
        }
        else
        {
            // Ÿ���� ��ġ�� ���ϴ� ���⺤�� ���
            Vector3 dir = currentLockOnTarget.position - holderTransform.position;
            dir.Normalize();

            // y������� ������ ������⸸ ����. �� ȸ�� ����� y���� �������θ� ȸ���ϵ��� �ϱ� ����
            dir.y = 0;

            Quaternion tr = Quaternion.LookRotation(dir); // ���⺤�� ������� ȸ��
            Quaternion targetRotation = Quaternion.Slerp(
                holderTransform.rotation,
                tr,
                rotationSpeed * delta
            );
            holderTransform.rotation = targetRotation; // ȸ�� ����

            dir = currentLockOnTarget.position - cameraPivotTransfrom.position;
            dir.Normalize();

            tr = Quaternion.LookRotation(dir);
            Vector3 eulerAngle = tr.eulerAngles;

            // y�� ������ �����Ͽ� pivot�� y���� �������� ȸ������ �ʰ� �Ѵ�. pivot�� ȸ���� x��(����)�������θ� �̷�������� �ϱ� ����
            eulerAngle.y = 0;
            cameraPivotTransfrom.localEulerAngles = eulerAngle;
        }
    }

    private void HandleCameraCollisions(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;

        // ���� ī�޶� ��ġ�� ī�޶� �Ǻ��� ��ġ ������ ���� ���
        Vector3 direction = cameraTransform.position - cameraPivotTransfrom.position;
        direction.Normalize();

        if (
            Physics.SphereCast(
                cameraPivotTransfrom.position,
                cameraSphereRadius,
                direction,
                out hit,
                Mathf.Abs(targetPosition),
                ignoreLayers
            )
        )
        {
            float dist = Vector3.Distance(cameraPivotTransfrom.position, hit.point);

            // ����� �Ÿ����ٰ� ������ ����� ���� ��ǥ��ġ�� �����Ѵ�
            targetPosition = -(dist - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            targetPosition = -minimumCollisionOffset;

        cameraTransformPosition.z = Mathf.SmoothDamp(
            cameraTransform.localPosition.z,
            targetPosition,
            ref cameraCurrentVelocity,
            delta / 0.2f
        );

        // ī�޶� ���� ��ġ�� �����Ͽ� ī�޶� ���� ��ġ�� �����Ѵ�
        cameraTransform.localPosition = cameraTransformPosition;
    }

    public void HandleLockOn()
    {
        // availableTarget �ʱ�ȭ �ʿ�
        availableTargets.Clear();

        float shortDist = Mathf.Infinity;
        float shortDistOfLeftTarget = Mathf.Infinity;
        float shortDistOfRightTarget = Mathf.Infinity;

        Collider[] cols = Physics.OverlapSphere(playerTransform.position, 25);
        for (int i = 0; i < cols.Length; i++)
        {
            CharacterManager character = cols[i].GetComponent<CharacterManager>();
            if (character != null)
            {
                Vector3 lockTargetDir = character.transform.position - playerTransform.position;
                float distFromTarget = Vector3.Distance(
                    playerTransform.position,
                    character.transform.position
                );
                float viewableAngle = Vector3.Angle(lockTargetDir, cameraTransform.forward);

                if (
                    character.transform.root != playerTransform.root
                    && viewableAngle > -50
                    && viewableAngle < 50
                    && distFromTarget <= maximumLockOnDist
                )
                {
                    RaycastHit hit;
                    // ������ Ÿ�� ���� ����
                    if (
                        Physics.Linecast(
                            playerManager.lockOnTransform.position,
                            character.lockOnTransform.position,
                            out hit
                        )
                    )
                    {
                        if (hit.transform.gameObject.layer != environmentLayers)
                        {
                            Debug.DrawLine(
                                playerManager.lockOnTransform.position,
                                character.lockOnTransform.position,
                                Color.red,
                                4f
                            );
                            availableTargets.Add(character);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < availableTargets.Count; i++)
        {
            float distFromTarget = Vector3.Distance(
                playerTransform.position,
                availableTargets[i].transform.position
            );

            // ���� ����� Ÿ��
            if (distFromTarget < shortDist)
            {
                shortDist = distFromTarget;
                nearestLockOnTarget = availableTargets[i].lockOnTransform;
            }
            if (inputHandler.lockOnFlag)
            {
                Vector3 relativeEnemyPos = currentLockOnTarget.InverseTransformPoint(
                    availableTargets[i].transform.position
                );

                // ���� Ÿ�� ����
                float distFromLeftTarget = Mathf.Abs(
                    currentLockOnTarget.position.x - availableTargets[i].transform.position.x
                );
                if (relativeEnemyPos.x > 0 && distFromLeftTarget < shortDistOfLeftTarget)
                {
                    shortDistOfLeftTarget = distFromLeftTarget;
                    leftLockTarget = availableTargets[i].lockOnTransform;
                }

                // ���� Ÿ�� ����
                float distFromRightTarget = Mathf.Abs(
                    currentLockOnTarget.position.x - availableTargets[i].transform.position.x
                );
                if (relativeEnemyPos.x < 0 && distFromRightTarget < shortDistOfRightTarget)
                {
                    shortDistOfRightTarget = distFromRightTarget;
                    rightLockTarget = availableTargets[i].lockOnTransform;
                }
            }
        }
    }

    public void ClearLockOnTarget()
    {
        availableTargets.Clear();
        nearestLockOnTarget = null;
        currentLockOnTarget = null;
    }

    public void SetCameraHeight(float delta)
    {
        Vector3 newLockPos = new Vector3(0, lockPivotPosition);
        Vector3 newUnlockPos = new Vector3(0, unlockPivotPosition);

        if (currentLockOnTarget != null)
        {
            cameraPivotTransfrom.localPosition = Vector3.SmoothDamp(
                cameraPivotTransfrom.localPosition,
                newLockPos,
                ref camHeightVel,
                smoothSpeed * delta
            );
        }
        else
        {
            cameraPivotTransfrom.localPosition = Vector3.SmoothDamp(
                cameraPivotTransfrom.localPosition,
                newUnlockPos,
                ref camHeightVel,
                smoothSpeed * delta
            );
        }
    }
}
