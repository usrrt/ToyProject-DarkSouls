using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
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
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        isInteracting = anim.GetBool("isInteracting");
        canDoCombo = anim.GetBool("canDoCombo");

        inputHandler.TickInput(delta);
        locomotion.HandleMovement(delta);
        locomotion.HandleRollAndSprintAnim(delta);
        locomotion.HandleFalling(delta, locomotion.moveDirection);
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }
    }

    private void LateUpdate()
    {
        inputHandler.rollFlag = false;
        inputHandler.sprintFlag = false;
        inputHandler.rb_Input = false;
        inputHandler.rt_Input = false;

        if (isInAir)
        {
            locomotion.inAirTimer += Time.deltaTime;
        }
    }
}
