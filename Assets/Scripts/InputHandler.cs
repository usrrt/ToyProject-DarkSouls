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
            // ���۽� inputAction Ȱ��ȭ ���� null�̸� �Ҵ����ִ� ����
            if (_inputActions == null)
            {
                _inputActions = new PlayerControls();
                // �ش� inputAction�� ���� ���簡 �ʿ��Ұ����� ����
                _inputActions.PlayerMovement.Movement.performed += i_movement => movementInput = i_movement.ReadValue<Vector2>();
                _inputActions.PlayerMovement.Camera.performed += i_camera => cameraInput = i_camera.ReadValue<Vector2>();
            }

            _inputActions.Enable();
        }

        private void OnDisable()
        {
            // Ȱ��ȭ�Ǿ��ٸ� ��Ȱ��ȭ ������ �翬�� �־���Ѵ�!
            _inputActions.Disable();
        }

        // �ٸ� ��ũ��Ʈ���� �����Ҷ� ȣ��Ǵ� ���������� �Լ� ĸ��ȭ�� �⺻
        public void TickInput(float delta)
        {
            MoveInput(delta);
        }

        private void MoveInput(float delta)
        {
            // �Է°� ���Ϳ� �Ҵ�
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical)); // �Է°� �� ���� ��Ƶд� (blend tree�� ���Ͱ���) �Ҽ����� �����Ƿ� clamp01����ѵ� ����
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
    }
}
