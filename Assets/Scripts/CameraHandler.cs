using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SW
{
    public class CameraHandler : MonoBehaviour
    {
        // ###############################################
        //             NAME : HongSW                      
        //             MAIL : gkenfktm@gmail.com         
        // ###############################################

        public static CameraHandler singleton;

        public Transform targetTransform; // 카메라가 따라갈 객체의 위치
        public Transform cameraTransform; // 실제 카메라의 위치
        public Transform cameraPivotTransfrom; // pivot 주위 회전하기 위함

        private Transform _myTranform;
        private Vector3 _cameraTransformPosition;
        private LayerMask _ignoreLayers; // 카메라가 다른 물체와 충돌처리용

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float _defaultPosition;
        private float _lookAngle;   // 좌우 시야각
        private float _pivotAngle;  // 위 아래 시야각
        public float minPivot = -35;
        public float maxPivot = 35;

        private void Awake()
        {
            singleton = this;
            _myTranform = transform;
            _defaultPosition = cameraTransform.localPosition.z; // 부모위치 기준(local)으로 z 설정
            _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10); // 무시할 layer 비트연산으로 처리
        }

        // 업데이트에서 매 프레임마다 호출되는 함수로 카메라가 targetTransform의 상위 위치를 따라다니게끔 한다.
        public void FollowTarget(float delta)
        {
            // lerp : 부드러운 카메라 움직임
            Vector3 targetPosition = Vector3.Lerp(_myTranform.position, targetTransform.position, delta / followSpeed);
            _myTranform.position = targetPosition;
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            _lookAngle += (mouseXInput * lookSpeed) / delta;    // 좌우
            _pivotAngle -= (mouseYInput * pivotSpeed) / delta;  // 위아래
            _pivotAngle = Mathf.Clamp(_pivotAngle, minPivot, maxPivot); // 위아래 시점 제한

            // y축을 기준으로 회전한다 -> 좌 우
            Vector3 rotation = Vector3.zero;
            rotation.y = _lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            _myTranform.rotation = targetRotation;

            // x축을 기준으로 회전한다 -> 위 아래
            rotation = Vector3.zero;
            rotation.x = _pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransfrom.localRotation = targetRotation;
        }
    }
}