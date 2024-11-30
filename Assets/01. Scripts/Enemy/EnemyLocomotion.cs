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
        
        // Ư�� ���̾ ���� -> �浹ü�� CharacterStats�� �������� Ȯ�� -> ��ȣ�������� Ȯ�� -> �þ߳��� �ִ��� Ȯ�� -> ����Ͽ� �߰�
        Collider[] cols = Physics.OverlapSphere(
            transform.position,
            enemyManager.detectionRadius,
            enemyManager.detectionLayer
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

        //// ���� �����߿� �������� �����
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
