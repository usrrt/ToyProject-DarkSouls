using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotion : MonoBehaviour
{
    EnemyManager enemyManager;
    EnemyAnimator enemyAnim;

    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        enemyAnim = GetComponentInChildren<EnemyAnimator>();
    }

    private void HandleDetection()
    {
        /* IDLE
        
        // 특정 레이어를 감지 -> 충돌체가 CharacterStats을 가지는지 확인 -> 우호관계인지 확인 -> 시야내에 있는지 확인 -> 대상목록에 추가
        Collider[] cols = Physics.OverlapSphere(
            transform.position,
            enemyManager.detectionRadius,
            enemyManager.detectionLayer
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
                    viewableAngle > enemyManager.minDetectionAngle
                    && viewableAngle < enemyManager.maxDetectionAngle
                )
                {
                    currentTargetCharacter = characterStat;
                }
            }
        }
        */
    }

    private void HandleMoveToTarget()
    {
        ////if (enemyManager.isPerformingAction)
        ////    return;

        //Vector3 targetDir = enemyManager.currentTargetCharacter.transform.position - transform.position;
        //distFromTarget = Vector3.Distance(
        //    enemyManager.currentTargetCharacter.transform.position,
        //    transform.position
        //);
        //float viewableAngle = Vector3.Angle(targetDir, transform.forward);

        //// 동작 실행중엔 움직임을 멈춘다
        //if (enemyManager.isPerformingAction)
        //{
        //    enemyAnim.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
        //    navMeshAgent.enabled = false;
        //}
        //else
        //{
            
        //    if (distFromTarget > stoppingDist)
        //    {
               
        //        enemyAnim.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
        //    }
        //    else if (distFromTarget <= stoppingDist)
        //    {
               
        //        enemyAnim.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
        //    }
        //}

        //HandleRotateTowardsTarget();

        //navMeshAgent.transform.localPosition = Vector3.zero;
        //navMeshAgent.transform.localRotation = Quaternion.identity;
    }


}
