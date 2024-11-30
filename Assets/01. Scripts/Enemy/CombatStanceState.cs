using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStanceState : State
{
    public AttackState attackState;
    public PursueTargetState pursueState;

    public override State Tick(EnemyManager manager, EnemyStats stats, EnemyAnimator animator)
    {
        // 공격범위 확인
        // 대상의 주변을 배회
        // 공격범위 안에 들어오면 공격상태 반환
        // 공격후 쿨타임 상태일때, 이 상태로 돌아와 대상의 주변을 배회한다
        // 만약 대상이 범위 밖으로 달아난다면, pursue target상태로 돌아가 대상을 추적한다

        manager.distFromTarget = Vector3.Distance(manager.currentTargetCharacter.transform.position, manager.transform.position);

        if (manager.currentRecoveryTime <= 0 && manager.distFromTarget <= manager.maxAttackRange)
        {
            return attackState;
        }
        else if (manager.distFromTarget > manager.maxAttackRange) 
        {
            return pursueState;
        }
        else
        {
            return this;
        }
    }
}
