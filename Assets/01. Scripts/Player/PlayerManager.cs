using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    public InteractableUI interactableUI;

    PlayerLocomotion locomotion;
    InputHandler inputHandler;
    Animator anim;
    CameraHandler cameraHandler;

    public bool isInteracting;

    [Header("Player Flags")]
    public bool isSprinting;
    public bool isInAir;
    public bool isGrounded;
    public bool canDoCombo;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        locomotion = GetComponent<PlayerLocomotion>();
    }

    private void Start()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
        interactableUI = FindObjectOfType<InteractableUI>();
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        isInteracting = anim.GetBool("isInteracting");
        canDoCombo = anim.GetBool("canDoCombo");
        anim.SetBool("isInAir", isInAir);

        inputHandler.TickInput(delta);

        locomotion.HandleRollAndSprintAnim(delta);
        //locomotion.HandleJumping();

        CheckForInteractableObject();
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        locomotion.HandleMovement(delta);
        locomotion.HandleFalling(delta, locomotion.moveDirection);
    }

    private void LateUpdate()
    {
        inputHandler.rollFlag = false;
        inputHandler.rb_Input = false;
        inputHandler.rt_Input = false;
        inputHandler.d_Pad_Up = false;
        inputHandler.d_Pad_Down = false;
        inputHandler.d_Pad_Left = false;
        inputHandler.d_Pad_Right = false;
        inputHandler.a_Input = false;
        inputHandler.jump_Input = false;
        inputHandler.inventory_Input = false;
        inputHandler.lockOn_Input = false;
        inputHandler.rStick_Left_Input = false;
        inputHandler.rStick_Right_Input = false;

        float delta = Time.deltaTime;
        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }

        if (isInAir)
        {
            locomotion.inAirTimer += delta;
        }
    }

    public void CheckForInteractableObject()
    {
        RaycastHit hit;
        if (
            Physics.SphereCast(
                transform.position,
                0.3f,
                transform.forward,
                out hit,
                1f,
                cameraHandler.ignoreLayers
            )
        )
        {
            if (hit.collider.tag == "Interactable")
            {
                Interactable interactableObj = hit.collider.GetComponent<Interactable>();
                if (interactableObj != null)
                {
                    string interactTxt = interactableObj.interactableObjTxt;
                    interactableUI.interactionTxt.text = interactTxt;
                    interactableUI.interactionObj.SetActive(true);

                    if (inputHandler.a_Input)
                    {
                        interactableObj.Interact(this);
                    }
                }
            }
        }
        else
        {
            if (interactableUI.interactionObj != null)
            {
                interactableUI.interactionObj.SetActive(false);
            }

            if (interactableUI.itemInteractableObj != null && inputHandler.a_Input)
            {
                interactableUI.itemInteractableObj.SetActive(false);
            }
        }
    }
}
