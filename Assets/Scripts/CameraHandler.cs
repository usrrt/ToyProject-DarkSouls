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

        public Transform targetTransform; // ī�޶� ���� ��ü�� ��ġ
        public Transform cameraTransform; // ���� ī�޶��� ��ġ
        public Transform cameraPivotTransfrom; // pivot ���� ȸ���ϱ� ����

        private Transform _myTranform;
        private Vector3 _cameraTransformPosition;
        private LayerMask _ignoreLayers; // ī�޶� �ٸ� ��ü�� �浹ó����

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float _defaultPosition;
        private float _lookAngle;   // �¿� �þ߰�
        private float _pivotAngle;  // �� �Ʒ� �þ߰�
        public float minPivot = -35;
        public float maxPivot = 35;

        private void Awake()
        {
            singleton = this;
            _myTranform = transform;
            _defaultPosition = cameraTransform.localPosition.z; // �θ���ġ ����(local)���� z ����
            _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10); // ������ layer ��Ʈ�������� ó��
        }

        // ������Ʈ���� �� �����Ӹ��� ȣ��Ǵ� �Լ��� ī�޶� targetTransform�� ���� ��ġ�� ����ٴϰԲ� �Ѵ�.
        public void FollowTarget(float delta)
        {
            // lerp : �ε巯�� ī�޶� ������
            Vector3 targetPosition = Vector3.Lerp(_myTranform.position, targetTransform.position, delta / followSpeed);
            _myTranform.position = targetPosition;
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            _lookAngle += (mouseXInput * lookSpeed) / delta;    // �¿�
            _pivotAngle -= (mouseYInput * pivotSpeed) / delta;  // ���Ʒ�
            _pivotAngle = Mathf.Clamp(_pivotAngle, minPivot, maxPivot); // ���Ʒ� ���� ����

            // y���� �������� ȸ���Ѵ� -> �� ��
            Vector3 rotation = Vector3.zero;
            rotation.y = _lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            _myTranform.rotation = targetRotation;

            // x���� �������� ȸ���Ѵ� -> �� �Ʒ�
            rotation = Vector3.zero;
            rotation.x = _pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransfrom.localRotation = targetRotation;
        }
    }
}