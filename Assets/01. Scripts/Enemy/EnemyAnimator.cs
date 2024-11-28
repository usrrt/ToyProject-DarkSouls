using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : AnimatorManager
{
    EnemyLocomotion locomotion;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        locomotion = GetComponentInParent<EnemyLocomotion>();
    }

    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        locomotion.enemyRigid.drag = 0;
        Vector3 deltaPos = anim.deltaPosition;
        deltaPos.y = 0;
        Vector3 vel = deltaPos / delta;
        locomotion.enemyRigid.velocity = vel;
    }
}
