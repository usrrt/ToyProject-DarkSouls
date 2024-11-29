using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PursueTargetState : State
{
    public override State Tick(EnemyManager manager, EnemyStats stats, EnemyAnimator enemyAnim)
    {
        // 타겟 쫒기
        // 공격범위 안에 들어올시 전투자세 상태로 변환
        // 타겟이 범위를 벗어나면, 현재상태로 돌아오고 계속해서 추적한다

        Vector3 targetDir = manager.currentTargetCharacter.transform.position - transform.position;
        float distFromTarget = Vector3.Distance(
            manager.currentTargetCharacter.transform.position,
            transform.position
        );
        float viewableAngle = Vector3.Angle(targetDir, transform.forward);

        // 동작 실행중엔 움직임을 멈춘다
        if (manager.isPerformingAction)
        {
            enemyAnim.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            manager.navMeshAgent.enabled = false;
        }
        else
        {

            if (distFromTarget > manager.stoppingDist)
            {

                enemyAnim.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }
            else if (distFromTarget <= manager.stoppingDist)
            {

                enemyAnim.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            }
        }

        if (manager.isPerformingAction)
        {
            Vector3 dir = manager.currentTargetCharacter.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();

            if (dir == Vector3.zero)
                dir = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                manager.rotationSpeed / Time.deltaTime
            );
        }
        // 길찾기(navmesh)를 통한 회전
        else
        {
            Vector3 relativeDir = transform.InverseTransformDirection(manager.navMeshAgent.desiredVelocity);
            Vector3 targetVel = manager.enemyRigid.velocity;

            manager.navMeshAgent.enabled = true;
            manager.navMeshAgent.SetDestination(manager.currentTargetCharacter.transform.position);
            manager.enemyRigid.velocity = targetVel;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                manager.navMeshAgent.transform.rotation,
                manager.rotationSpeed / Time.deltaTime
            );
        }

        manager.navMeshAgent.transform.localPosition = Vector3.zero;
        manager.navMeshAgent.transform.localRotation = Quaternion.identity;

        return this;
    }
}
