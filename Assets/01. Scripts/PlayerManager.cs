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
                // inputAction에서 가져온 마우스 x, y값 넣어줌
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
        }

        private void LateUpdate()
        {
            inputHandler.rollFlag = false;
            inputHandler.sprintFlag = false;
            isSprinting = inputHandler.b_Input; // 버튼을 뗄 경우 isSprinting을 다시 false로 바꾼다
        }
    }
}
