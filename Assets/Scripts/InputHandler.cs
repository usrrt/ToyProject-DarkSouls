using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SW
{
    public class InputHandler : MonoBehaviour
    {
        // ###############################################
        //             NAME : HongSW                      
        //             MAIL : gkenfktm@gmail.com         
        // ###############################################

        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        PlayerControls _inputActions;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void OnEnable()
        {
            // 시작시 inputAction 활성화 만일 null이면 할당해주는 과정
            if (_inputActions == null)
            {
                _inputActions = new PlayerControls();
                // 해당 inputAction은 따로 조사가 필요할것으로 보임
                _inputActions.PlayerMovement.Movement.performed += i_movement => movementInput = i_movement.ReadValue<Vector2>();
                _inputActions.PlayerMovement.Camera.performed += i_camera => cameraInput = i_camera.ReadValue<Vector2>();
            }

            _inputActions.Enable();
        }

        private void OnDisable()
        {
            // 활성화되었다면 비활성화 과정도 당연히 있어야한다!
            _inputActions.Disable();
        }

        // 다른 스크립트에서 참조할때 호출되는 공개수준의 함수 캡슐화의 기본
        public void TickInput(float delta)
        {
            MoveInput(delta);
        }

        private void MoveInput(float delta)
        {
            // 입력값 벡터에 할당
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical)); // 입력값 총 량을 담아둔다 (blend tree에 쓸것같음) 소수값이 나오므로 clamp01사용한듯 보임
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
    }
}
