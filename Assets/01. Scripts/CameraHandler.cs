using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public static CameraHandler singleton;

    public Transform targetTransform; // 카메라가 따라갈 객체의 위치
    public Transform cameraTransform; // 실제 카메라의 위치
    public Transform cameraPivotTransfrom; // pivot 주위 회전하기 위함

    private Transform myTranform;

    private Vector3 cameraTransformPosition;
    private LayerMask ignoreLayers; // 카메라가 다른 물체와 충돌처리용
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public float lookSpeed = 0.1f,
        followSpeed = 0.1f,
        pivotSpeed = 0.03f,
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

    private void Awake()
    {
        singleton = this;
        myTranform = transform;
        defaultPosition = cameraTransform.localPosition.z; // 부모위치 기준(local)으로 z 설정
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);

        // 하이어라키 버그로 할당안될경우 대비
        targetTransform = FindObjectOfType<PlayerManager>().transform;
    }

    public void FollowTarget(float delta)
    {
        // ref cameraFollowVelocity 는 참조로 전달되며, 현재 속도를 저장하고 갱신하는데 사용한다
        Vector3 targetPosition = Vector3.SmoothDamp(
            myTranform.position,
            targetTransform.position,
            ref cameraFollowVelocity,
            delta / followSpeed
        );
        myTranform.position = targetPosition;
        HandleCameraCollisions(delta);
    }

    public void HandleCameraRotation(float delta, float mouseInputX, float mouseInputY)
    {
        lookAngle += (mouseInputX * lookSpeed) / delta; // 좌우
        pivotAngle -= (mouseInputY * pivotSpeed) / delta; // 위아래
        pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);

        // y축을 기준으로 회전한다 -> 좌 우
        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        myTranform.rotation = targetRotation;

        // x축을 기준으로 회전한다 -> 위 아래
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransfrom.localRotation = targetRotation;
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
}
