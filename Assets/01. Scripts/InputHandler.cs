using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    PlayerControls inputActions;
    PlayerAttacker playerAttacker;
    PlayerInventory playerInventory;
    PlayerManager manager;

    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool b_Input;
    public bool rb_Input;
    public bool rt_Input;

    public bool rollFlag;
    public bool sprintFlag;
    public bool comboFlag;

    public float rollInputTimer;

    Vector2 movementInput;
    Vector2 cameraInput;

    private void Awake()
    {
        playerAttacker = GetComponent<PlayerAttacker>();
        playerInventory = GetComponent<PlayerInventory>();
        manager = GetComponent<PlayerManager>();
    }

    private void OnEnable()
    {
        // 시작시 inputAction 활성화 만일 null이면 할당해주는 과정
        if (inputActions == null)
        {
            inputActions = new PlayerControls();
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
        MoveInput();
        HandleRollInput();
        HandleAttackInput();
    }

    private void MoveInput()
    {
        // 입력값 벡터에 할당
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical)); // 입력값 총 량을 담아둔다 (blend tree에 쓸것같음) 소수값이 나오므로 clamp01사용한듯 보임
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void HandleRollInput()
    {
        b_Input = inputActions.PlayerActions.Roll.phase == InputActionPhase.Performed;

        if (b_Input)
        {
            rollInputTimer += Time.deltaTime;
            sprintFlag = true;
        }
        else
        {
            // 버튼을 짧게 누른 경우(0 ~ 0.5초)
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                sprintFlag = false;
                rollFlag = true;
            }
            rollInputTimer = 0;
        }
    }

    public void HandleAttackInput()
    {
        inputActions.PlayerActions.RB.performed += _ => rb_Input = true;
        inputActions.PlayerActions.RT.performed += _ => rt_Input = true;

        if (rb_Input)
        {
            if (manager.canDoCombo)
            {
                comboFlag = true;
                playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                comboFlag = false;
            }
            else
            {
                if (manager.isInteracting)
                    return;
                if (manager.canDoCombo)
                    return;

                playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
            }
        }

        if (rt_Input)
        {
            if (manager.canDoCombo)
            {
                comboFlag = true;
                playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                comboFlag = false;
            }
            else
            {
                if (manager.isInteracting)
                    return;
                if (manager.canDoCombo)
                    return;

                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
            }
        }
    }
}
