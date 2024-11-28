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
    UIManager uiManager;
    CameraHandler cameraHandler;

    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool a_Input;
    public bool b_Input;
    public bool rb_Input;
    public bool rt_Input;
    public bool jump_Input;
    public bool inventory_Input;
    public bool lockOn_Input;
    public bool rStick_Right_Input;
    public bool rStick_Left_Input;

    public bool d_Pad_Up;
    public bool d_Pad_Down;
    public bool d_Pad_Left;
    public bool d_Pad_Right;

    public bool rollFlag;
    public bool sprintFlag;
    public bool comboFlag;
    public bool inventoryFlag;
    public bool lockOnFlag;

    public float rollInputTimer;

    Vector2 movementInput;
    Vector2 cameraInput;

    private void Awake()
    {
        playerAttacker = GetComponent<PlayerAttacker>();
        playerInventory = GetComponent<PlayerInventory>();
        manager = GetComponent<PlayerManager>();
        uiManager = FindObjectOfType<UIManager>();
        cameraHandler = FindObjectOfType<CameraHandler>();
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
            inputActions.PlayerActions.RB.performed += i_attack => rb_Input = true;
            inputActions.PlayerActions.RT.performed += i_attack => rt_Input = true;
            inputActions.PlayerDPadActions.DPadRight.performed += i_quickSlot => d_Pad_Right = true;
            inputActions.PlayerDPadActions.DPadLeft.performed += i_quickSlot => d_Pad_Left = true;
            inputActions.PlayerActions.Inventory.performed += i_inventory => inventory_Input = true;
            inputActions.PlayerActions.A.performed += i_interact => a_Input = true;
            inputActions.PlayerActions.Jump.performed += i_jump => jump_Input = true;
            inputActions.PlayerActions.LockOn.performed += i_lockOn => lockOn_Input = true;
            inputActions.PlayerMovement.LockOnTargetLeft.performed += i_rStick =>
                rStick_Left_Input = true;
            inputActions.PlayerMovement.LockOnTargetRight.performed += i_rStick =>
                rStick_Right_Input = true;
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void TickInput(float delta)
    {
        HandleMoveInput();
        HandleRollInput(delta);
        HandleAttackInput();
        HandleQuickSlotsInput();
        HandleInventoryInput();
        HandleLockOnInput(delta);
    }

    private void HandleMoveInput()
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
        sprintFlag = b_Input;
        if (b_Input)
        {
            rollInputTimer += delta;
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

    private void HandleAttackInput()
    {
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

    private void HandleQuickSlotsInput()
    {
        if (d_Pad_Right)
        {
            playerInventory.ChangeRightWeapon();
        }
        else if (d_Pad_Left)
        {
            playerInventory.ChangeLeftWeapon();
        }
    }

    private void HandleInventoryInput()
    {
        if (inventory_Input)
        {
            inventoryFlag = !inventoryFlag;
            if (inventoryFlag)
            {
                uiManager.OpenSelectWindow();
                uiManager.UpdateUI();
                uiManager.hudWindow.SetActive(false);
            }
            else
            {
                uiManager.CloseSelectWindow();
                uiManager.CloseAllInventoryWindows();
                uiManager.hudWindow.SetActive(true);
            }
        }
    }

    private void HandleLockOnInput(float delta)
    {
        if (lockOn_Input && lockOnFlag == false)
        {
            cameraHandler.HandleLockOn();
            if (cameraHandler.nearestLockOnTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                lockOnFlag = true;
            }
        }
        else if (lockOn_Input && lockOnFlag) // ���� ��ư �ѹ� �� ���� ����
        {
            lockOnFlag = false;
            cameraHandler.ClearLockOnTarget();
        }

        if (lockOnFlag && rStick_Left_Input)
        {
            cameraHandler.HandleLockOn();
            if (cameraHandler.leftLockTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
            }
        }

        if (lockOnFlag && rStick_Right_Input)
        {
            cameraHandler.HandleLockOn();
            if (cameraHandler.rightLockTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
            }
        }

        cameraHandler.SetCameraHeight(delta);
    }
}
