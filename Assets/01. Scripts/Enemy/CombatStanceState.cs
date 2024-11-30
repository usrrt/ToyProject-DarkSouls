using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStanceState : State
{
    public AttackState attackState;
    public PursueTargetState pursueState;

    public override State Tick(EnemyManager manager, EnemyStats stats, EnemyAnimator animator)
    {
        // ���ݹ��� Ȯ��
        // ����� �ֺ��� ��ȸ
        // ���ݹ��� �ȿ� ������ ���ݻ��� ��ȯ
        // ������ ��Ÿ�� �����϶�, �� ���·� ���ƿ� ����� �ֺ��� ��ȸ�Ѵ�
        // ���� ����� ���� ������ �޾Ƴ��ٸ�, pursue target���·� ���ư� ����� �����Ѵ�

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
