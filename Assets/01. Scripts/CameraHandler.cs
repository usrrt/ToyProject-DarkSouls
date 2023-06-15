using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SW
{
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
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10); // 무시할 layer 비트연산으로 처리
        }

        public void FollowTarget(float delta)
        {
            // 선형보간
            // 일정한 속도로 두 벡터사이를 이동한다
            //Vector3 targetPosition = Vector3.Lerp(
            //    myTranform.position,
            //    targetTransform.position,
            //    delta / followSpeed
            //);

            // 감속하는 보간
            // 목표로의 이동이 점차적으로 느려져 더 자연스러운 이동가능
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

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            lookAngle += (mouseXInput * lookSpeed) / delta; // 좌우
            pivotAngle -= (mouseYInput * pivotSpeed) / delta; // 위아래
            pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot); // 위아래 시점 제한

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

            // 카메라 피봇 위치에서 카메라 방향으로 구형 충돌 판별 수행
            // 제외할 레이어 외에 충돌 판별 수행
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
                // 카메라 피봇과 충돌 지점 사이의 거리
                float dist = Vector3.Distance(cameraPivotTransfrom.position, hit.point);

                // 계산한 거리에다가 편차를 계산한 값을 목표위치로 설정한다
                targetPosition = -(dist - cameraCollisionOffset);
            }

            // 만일 목표 위치가 최소 편차보다 작을경우
            // 목표위치를 최소값으로 설정한다
            // 이는 카메라가 캐릭터와 너무 가까워 지는것을 방지함
            if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            // z축으로 이동시 부드럽게 감쇠이동하여 자연스럽게 해준다
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
}
