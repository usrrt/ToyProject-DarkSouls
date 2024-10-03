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
        // ���۽� inputAction Ȱ��ȭ ���� null�̸� �Ҵ����ִ� ����
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
        // �Է°� ���Ϳ� �Ҵ�
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical)); // �Է°� �� ���� ��Ƶд� (blend tree�� ���Ͱ���) �Ҽ����� �����Ƿ� clamp01����ѵ� ����
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
            // ��ư�� ª�� ���� ���(0 ~ 0.5��)
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
