using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PursueTargetState : State
{
    public CombatStanceState combatStanceState;
    public override State Tick(EnemyManager manager, EnemyStats stats, EnemyAnimator enemyAnim)
    {
        // 타겟 쫒기
        // 공격범위 안에 들어올시 전투자세 상태로 변환
        // 타겟이 범위를 벗어나면, 현재상태로 돌아오고 계속해서 추적한다

        if (manager.isPerformingAction)
            return this;

        Vector3 targetDir = manager.currentTargetCharacter.transform.position - transform.position;
        manager.distFromTarget = Vector3.Distance(
            manager.currentTargetCharacter.transform.position,
            transform.position
        );
        manager.viewableAngle = Vector3.Angle(targetDir, transform.forward);

        if (manager.distFromTarget > manager.maxAttackRange)
        {
            enemyAnim.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
        }

        HandleRotateTowardsTarget(manager);

        manager.navMeshAgent.transform.localPosition = Vector3.zero;
        manager.navMeshAgent.transform.localRotation = Quaternion.identity;

        if (manager.distFromTarget <= manager.maxAttackRange)
        {
            return combatStanceState;
        }
        else
        {
            return this;
        }
    }

    // Locomotion에서 옮기는 과정에서 캐릭터가 회전하지 않는 문제가 있었다. 이유는 rotation을 할때 캐릭터가 아닌 pursue오브젝트의 transform을 참조하고 있어서 캐릭터가 회전하지 않았다. enemy에서 받아온 tranform을 rotation을 사용해야 정상적으로 작동한다.
    private void HandleRotateTowardsTarget(EnemyManager enemyManager)
    {
        // 수동 회전
        if (enemyManager.isPerformingAction)
        {
            Vector3 dir = enemyManager.currentTargetCharacter.transform.position - enemyManager.transform.position;
            dir.y = 0;
            dir.Normalize();

            if (dir == Vector3.zero)
                dir = enemyManager.transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            enemyManager.transform.rotation = Quaternion.Slerp(
                enemyManager.transform.rotation,
                targetRotation,
                enemyManager.rotationSpeed / Time.deltaTime
            );
        }
        // 길찾기(navmesh)를 통한 회전
        else
        {
            Vector3 relativeDir = enemyManager.transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
            Vector3 targetVel = enemyManager.enemyRigid.velocity;

            enemyManager.navMeshAgent.enabled = true;
            enemyManager.navMeshAgent.SetDestination(enemyManager.currentTargetCharacter.transform.position);
            enemyManager.enemyRigid.velocity = targetVel;
            enemyManager.transform.rotation = Quaternion.Slerp(
                enemyManager.transform.rotation,
                enemyManager.navMeshAgent.transform.rotation,
                enemyManager.rotationSpeed / Time.deltaTime
            );
        }

    }
}
