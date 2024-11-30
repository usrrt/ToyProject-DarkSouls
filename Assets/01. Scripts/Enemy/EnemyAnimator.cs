using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : AnimatorManager
{
    EnemyManager manager;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        manager = GetComponentInParent<EnemyManager>();
    }

    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        manager.enemyRigid.drag = 0;
        Vector3 deltaPos = anim.deltaPosition;
        deltaPos.y = 0;
        Vector3 vel = deltaPos / delta;
        manager.enemyRigid.velocity = vel;
    }
}
