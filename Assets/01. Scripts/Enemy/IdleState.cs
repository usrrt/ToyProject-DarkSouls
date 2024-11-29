using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public PursueTargetState pursueState;

    public LayerMask detectionLayer;

    public override State Tick(EnemyManager manager, EnemyStats stats, EnemyAnimator enemyEnim)
    {
        // 1. ������ Ÿ�� ã��
        // 2. Ÿ���� ã����� Ÿ����������(persue target state)�� ����
        // 3. Ÿ���� ��ã�� ��� idle ���� reutrn

        // Ư�� ���̾ ���� -> �浹ü�� CharacterStats�� �������� Ȯ�� -> ��ȣ�������� Ȯ�� -> �þ߳��� �ִ��� Ȯ�� -> ����Ͽ� �߰�
        Collider[] cols = Physics.OverlapSphere(
            transform.position,
            manager.detectionRadius,
            detectionLayer
        );

        for (int i = 0; i < cols.Length; i++)
        {
            // playerstat�� enemystat �� ������ ���� �����Ͽ� �����ʿ���� �ֳ��ϸ� �ٸ� ai�� �ο�� ai�� �ֱ⶧���� characterstat�̶�� ���� ������ ����� �����Ѵ�
            CharacterStats characterStat = cols[i].transform.GetComponent<CharacterStats>();
            if (characterStat != null)
            {
                // ��ȣ���� �������� team id�� Ȯ���ʿ�

                Vector3 targetDir = characterStat.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDir, transform.forward);

                if (
                    viewableAngle > manager.minDetectionAngle
                    && viewableAngle < manager.maxDetectionAngle
                )
                {
                    manager.currentTargetCharacter = characterStat;
                }
            }
        }

        // ���� ���� �κ�
        if (manager.currentTargetCharacter != null)
        {
            return pursueState;
        }
        else
        {
            return this;
        }
    }
}
