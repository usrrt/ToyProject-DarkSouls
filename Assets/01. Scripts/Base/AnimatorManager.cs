using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator anim;

    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);

        // 애니메이션 상태간의 부드러운 전환을 가능하게 한다(자연스러운 애니메이션 시퀀스)
        anim.CrossFade(targetAnim, 0.2f);
    }
}
