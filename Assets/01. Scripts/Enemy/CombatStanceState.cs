using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStanceState : State
{
    public override State Tick(EnemyManager manager, EnemyStats stats, EnemyAnimator animator)
    {
        // ���ݹ��� Ȯ��
        // ����� �ֺ��� ��ȸ
        // ���ݹ��� �ȿ� ������ ���ݻ��� ��ȯ
        // ������ ��Ÿ�� �����϶�, �� ���·� ���ƿ� ����� �ֺ��� ��ȸ�Ѵ�
        // ���� ����� ���� ������ �޾Ƴ��ٸ�, pursue target���·� ���ư� ����� �����Ѵ�

        return this;
    }
}
