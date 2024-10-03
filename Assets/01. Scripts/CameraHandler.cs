using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public static CameraHandler singleton;

    public Transform targetTransform; // ī�޶� ���� ��ü�� ��ġ
    public Transform cameraTransform; // ���� ī�޶��� ��ġ
    public Transform cameraPivotTransfrom; // pivot ���� ȸ���ϱ� ����

    private Transform myTranform;

    private Vector3 cameraTransformPosition;
    private LayerMask ignoreLayers; // ī�޶� �ٸ� ��ü�� �浹ó����
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public float lookSpeed = 0.1f,
        followSpeed = 0.1f,
        pivotSpeed = 0.03f,
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

    private void Awake()
    {
        singleton = this;
        myTranform = transform;
        defaultPosition = cameraTransform.localPosition.z; // �θ���ġ ����(local)���� z ����
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);

        // ���̾��Ű ���׷� �Ҵ�ȵɰ�� ���
        targetTransform = FindObjectOfType<PlayerManager>().transform;
    }

    public void FollowTarget(float delta)
    {
        // ref cameraFollowVelocity �� ������ ���޵Ǹ�, ���� �ӵ��� �����ϰ� �����ϴµ� ����Ѵ�
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
        lookAngle += (mouseInputX * lookSpeed) / delta; // �¿�
        pivotAngle -= (mouseInputY * pivotSpeed) / delta; // ���Ʒ�
        pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);

        // y���� �������� ȸ���Ѵ� -> �� ��
        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        myTranform.rotation = targetRotation;

        // x���� �������� ȸ���Ѵ� -> �� �Ʒ�
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransfrom.localRotation = targetRotation;
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
}
