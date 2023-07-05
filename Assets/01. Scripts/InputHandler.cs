using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SW
{
    public class InputHandler : MonoBehaviour
    {
        // ###############################################
        //             NAME : HongSW
        //             MAIL : gkenfktm@gmail.com
        // ###############################################

        PlayerControls inputActions;

        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool b_Input;
        public bool rollFlag;
        public bool sprintFlag;
        public float rollInputTimer;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void OnEnable()
        {
            // ���۽� inputAction Ȱ��ȭ ���� null�̸� �Ҵ����ִ� ����
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                // �ش� inputAction�� ���� ���簡 �ʿ��Ұ����� ����
                inputActions.PlayerMovement.Movement.performed += i_movement =>
                    movementInput = i_movement.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i_camera =>
                    cameraInput = i_camera.ReadValue<Vector2>();
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
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

        private void HandleRollInput(float delta)
        {
            b_Input = inputActions.PlayerActions.Roll.phase == InputActionPhase.Performed;

            if (b_Input)
            {
                rollInputTimer += Time.deltaTime;
                sprintFlag = true;
            }
            else
            {
                // ��ư�� ª�� ���� ���(0 ~ 0.5��)
                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    sprintFlag = false;
                    rollFlag = true;
                }
                rollInputTimer = 0;
            }
        }
    }
}
