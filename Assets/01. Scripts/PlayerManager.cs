using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SW
{
    public class PlayerManager : MonoBehaviour
    {
        PlayerLocomotion locomotion;
        InputHandler inputHandler;
        Animator anim;
        CameraHandler cameraHandler;

        public bool isInteracting;

        [Header("Player Flags")]
        public bool isSprinting;

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            locomotion = GetComponent<PlayerLocomotion>();
        }

        private void Update()
        {
            float delta = Time.deltaTime;

            isInteracting = anim.GetBool("isInteracting");

            inputHandler.TickInput(delta);
            locomotion.HandleMovement(delta);
            locomotion.HandleRollingAndSprinting(delta);
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                // inputAction���� ������ ���콺 x, y�� �־���
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
        }

        private void LateUpdate()
        {
            inputHandler.rollFlag = false;
            inputHandler.sprintFlag = false;
            isSprinting = inputHandler.b_Input; // ��ư�� �� ��� isSprinting�� �ٽ� false�� �ٲ۴�
        }
    }
}
