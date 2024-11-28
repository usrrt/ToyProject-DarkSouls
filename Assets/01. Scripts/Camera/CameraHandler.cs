using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public static CameraHandler singleton;

    InputHandler inputHandler;
    PlayerManager playerManager;

    public Transform playerTransform; // 카메라가 따라갈 플레이어의 위치
    public Transform cameraTransform; // 실제 카메라의 위치
    public Transform cameraPivotTransfrom; // pivot 주위 회전하기 위함
    private Transform holderTransform;

    public LayerMask ignoreLayers; // 카메라가 다른 물체와 충돌처리용
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
        lookAngle, // 좌우 시야각
        pivotAngle; // 위 아래 시야각

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
        defaultPosition = cameraTransform.localPosition.z; // 부모위치 기준(local)으로 z 설정
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        environmentLayers = LayerMask.NameToLayer("Environment");

        // 하이어라키 버그로 할당안될경우 대비
        playerManager = FindObjectOfType<PlayerManager>();
        inputHandler = playerManager.GetComponent<InputHandler>();
        playerTransform = playerManager.transform;
    }

    public void FollowTarget(float delta)
    {
        // ref cameraFollowVelocity 는 참조로 전달되며, 현재 속도를 저장하고 갱신하는데 사용한다
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
            lookAngle += (mouseInputX * lookSpeed) / delta; // 좌우

            // -=는 마우스의 위로 움직임은 카메라를 위로, 아래로 움직임은 카메라를 아래로, +=는 상하움직임이 반전됨
            pivotAngle -= (mouseInputY * pivotSpeed) / delta; // 위아래
            pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);

            // y축을 기준으로 회전한다 -> 좌 우
            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;

            // Euler함수를 사용해 회전벡터를 쿼터니언으로 변환
            Quaternion targetRotation = Quaternion.Euler(rotation);
            holderTransform.rotation = targetRotation; // 객체에 적용

            // x축을 기준으로 회전한다 -> 위 아래
            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);

            // localRotation을 사용하는 이유는 부모를 중심으로 상대적으로 회전하도록 하기 위함
            cameraPivotTransfrom.localRotation = targetRotation;
        }
        else
        {
            // 타겟의 위치로 향하는 방향벡터 계산
            Vector3 dir = currentLockOnTarget.position - holderTransform.position;
            dir.Normalize();

            // y축방향을 제거해 수평방향만 남김. 주 회전 대상이 y축을 기준으로만 회전하도록 하기 위함
            dir.y = 0;

            Quaternion tr = Quaternion.LookRotation(dir); // 방향벡터 기반으로 회전
            Quaternion targetRotation = Quaternion.Slerp(
                holderTransform.rotation,
                tr,
                rotationSpeed * delta
            );
            holderTransform.rotation = targetRotation; // 회전 적용

            dir = currentLockOnTarget.position - cameraPivotTransfrom.position;
            dir.Normalize();

            tr = Quaternion.LookRotation(dir);
            Vector3 eulerAngle = tr.eulerAngles;

            // y축 각도를 제거하여 pivot이 y축을 기준으로 회전하지 않게 한다. pivot의 회전이 x축(상하)방향으로만 이루어지도록 하기 위함
            eulerAngle.y = 0;
            cameraPivotTransfrom.localEulerAngles = eulerAngle;
        }
    }

    private void HandleCameraCollisions(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;

        // 메인 카메라 위치와 카메라 피봇의 위치 사이의 방향 계산
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

            // 계산한 거리에다가 편차를 계산한 값을 목표위치로 설정한다
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

        // 카메라 로컬 위치로 설정하여 카메라 최종 위치를 조정한다
        cameraTransform.localPosition = cameraTransformPosition;
    }

    public void HandleLockOn()
    {
        // availableTarget 초기화 필요
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
                    // 가려진 타겟 락온 방지
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

            // 가장 가까운 타겟
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

                // 좌측 타겟 설정
                float distFromLeftTarget = Mathf.Abs(
                    currentLockOnTarget.position.x - availableTargets[i].transform.position.x
                );
                if (relativeEnemyPos.x > 0 && distFromLeftTarget < shortDistOfLeftTarget)
                {
                    shortDistOfLeftTarget = distFromLeftTarget;
                    leftLockTarget = availableTargets[i].lockOnTransform;
                }

                // 우측 타겟 설정
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
