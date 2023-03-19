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

        Transform _cameraObject; // main 카메라의 위치값을 저장
        InputHandler _inputHandler;
        
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform; // 나(플레이어)의 위치를 저장할 변수;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera; // 나중에 락온카메라의 역할을 한다

        [Header("Stats")]
        [SerializeField] float _movementSpeed = 5f;
        [SerializeField] float _rotationSpeed = 10f;

        private void Start()
        {
            _cameraObject = Camera.main.transform;
            _inputHandler = GetComponent<InputHandler>(); // awake가아닌 start에서 초기화한이유는 inputHandler onEnable에서 inputAction을 생성하고있기 때문이다 Awake -> OnEnable -> Start
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
            moveDirection.Normalize(); // 방향정보만 필요하므로 정규화 작업(안할시 대각선으로 움직일때 더 빨라짐)
            moveDirection.y = 0; // 캐릭터가 공중부양하는 현상 방지

            float speed = _movementSpeed;
            moveDirection *= speed;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector); // 평면에 벡터 투영작업
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(0, _inputHandler.moveAmount); // 블랜드트리 value값 변화

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }

        }

        #region Movement
        Vector3 normalVector; // 이름이 애매하지만 추후 알게된다고함
        Vector3 targetPosition;

        // 회전 조작 함수
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

            // 바라보는 방향으로 회전하기위한 과정
            float rs = _rotationSpeed;
            Quaternion tr = Quaternion.LookRotation(targetDir); // 해당 벡터 방향을 바라보는 회전상태를 쿼터니언으로 반환하여 저장
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta); // 자연스러운 연출을 위해 Slerp를 사용해 서서히 바라보게함
            myTransform.rotation = targetRotation; // 현재 회전상태를 초기화해줌
        }
        #endregion
    }
}