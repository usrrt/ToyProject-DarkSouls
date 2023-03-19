using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SW
{
    public class PlayerLocomotion : MonoBehaviour
    {
        // ###############################################
        //             NAME : HongSW                      
        //             MAIL : gkenfktm@gmail.com         
        // ###############################################

        Transform _cameraObject; // main ī�޶��� ��ġ���� ����
        InputHandler _inputHandler;
        
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform; // ��(�÷��̾�)�� ��ġ�� ������ ����;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera; // ���߿� ����ī�޶��� ������ �Ѵ�

        [Header("Stats")]
        [SerializeField] float _movementSpeed = 5f;
        [SerializeField] float _rotationSpeed = 10f;

        private void Start()
        {
            _cameraObject = Camera.main.transform;
            _inputHandler = GetComponent<InputHandler>(); // awake���ƴ� start���� �ʱ�ȭ�������� inputHandler onEnable���� inputAction�� �����ϰ��ֱ� �����̴� Awake -> OnEnable -> Start
            myTransform = transform;
            rigidbody = GetComponent<Rigidbody>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            animatorHandler.Init();
            
        }

        private void Update()
        {
            float delta = Time.deltaTime;

            _inputHandler.TickInput(delta);

            moveDirection = _cameraObject.forward * _inputHandler.vertical;
            moveDirection += _cameraObject.right * _inputHandler.horizontal;
            moveDirection.Normalize(); // ���������� �ʿ��ϹǷ� ����ȭ �۾�(���ҽ� �밢������ �����϶� �� ������)
            moveDirection.y = 0; // ĳ���Ͱ� ���ߺξ��ϴ� ���� ����

            float speed = _movementSpeed;
            moveDirection *= speed;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector); // ��鿡 ���� �����۾�
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(0, _inputHandler.moveAmount); // ����Ʈ�� value�� ��ȭ

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }

        }

        #region Movement
        Vector3 normalVector; // �̸��� �ָ������� ���� �˰Եȴٰ���
        Vector3 targetPosition;

        // ȸ�� ���� �Լ�
        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveAmountOverride = _inputHandler.moveAmount;

            targetDir = _cameraObject.forward * _inputHandler.vertical;
            targetDir += _cameraObject.right * _inputHandler.horizontal;
            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = myTransform.forward;
            }

            // �ٶ󺸴� �������� ȸ���ϱ����� ����
            float rs = _rotationSpeed;
            Quaternion tr = Quaternion.LookRotation(targetDir); // �ش� ���� ������ �ٶ󺸴� ȸ�����¸� ���ʹϾ����� ��ȯ�Ͽ� ����
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta); // �ڿ������� ������ ���� Slerp�� ����� ������ �ٶ󺸰���
            myTransform.rotation = targetRotation; // ���� ȸ�����¸� �ʱ�ȭ����
        }
        #endregion
    }
}