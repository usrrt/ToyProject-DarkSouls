using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PursueTargetState : State
{
    public CombatStanceState combatStanceState;
    public override State Tick(EnemyManager manager, EnemyStats stats, EnemyAnimator enemyAnim)
    {
        // Ÿ�� �i��
        // ���ݹ��� �ȿ� ���ý� �����ڼ� ���·� ��ȯ
        // Ÿ���� ������ �����, ������·� ���ƿ��� ����ؼ� �����Ѵ�

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

    // Locomotion���� �ű�� �������� ĳ���Ͱ� ȸ������ �ʴ� ������ �־���. ������ rotation�� �Ҷ� ĳ���Ͱ� �ƴ� pursue������Ʈ�� transform�� �����ϰ� �־ ĳ���Ͱ� ȸ������ �ʾҴ�. enemy���� �޾ƿ� tranform�� rotation�� ����ؾ� ���������� �۵��Ѵ�.
    private void HandleRotateTowardsTarget(EnemyManager enemyManager)
    {
        // ���� ȸ��
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
        // ��ã��(navmesh)�� ���� ȸ��
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
