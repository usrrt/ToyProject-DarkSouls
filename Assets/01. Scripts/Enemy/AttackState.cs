using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public override State Tick(EnemyManager manager, EnemyStats stats, EnemyAnimator animator)
    {
        // attack score����� �� �پ��� ���� ����� �ϳ��� ����
        // ���õ� ������ ������ �Ÿ��� �������� �ʴٸ�, ���ο� ������ ����
        // ������ �����ϴٸ�, �������� ���߰� Ÿ���� ����
        // recovery timer�� �̿��� recovery time�� ȸ��
        // combatstance���·� ���ư���

        return this;
    }

    
}
