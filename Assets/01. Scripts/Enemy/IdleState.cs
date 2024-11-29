using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public PursueTargetState pursueState;

    public LayerMask detectionLayer;

    public override State Tick(EnemyManager manager, EnemyStats stats, EnemyAnimator enemyEnim)
    {
        // 1. 잠재적 타겟 찾기
        // 2. 타겟을 찾을경우 타겟추적상태(persue target state)로 변경
        // 3. 타겟을 못찾을 경우 idle 상태 reutrn

        // 특정 레이어를 감지 -> 충돌체가 CharacterStats을 가지는지 확인 -> 우호관계인지 확인 -> 시야내에 있는지 확인 -> 대상목록에 추가
        Collider[] cols = Physics.OverlapSphere(
            transform.position,
            manager.detectionRadius,
            detectionLayer
        );

        for (int i = 0; i < cols.Length; i++)
        {
            // playerstat과 enemystat 두 유형을 따로 감지하여 만들필요없음 왜냐하면 다른 ai와 싸우는 ai가 있기때문에 characterstat이라는 공통 유형을 만들어 감지한다
            CharacterStats characterStat = cols[i].transform.GetComponent<CharacterStats>();
            if (characterStat != null)
            {
                // 우호적인 관계인지 team id로 확인필요

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

        // 상태 변경 부분
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
