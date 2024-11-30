using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public CombatStanceState combatStanceState;

    public EnemyAttackAction[] enemyAttacks;
    public EnemyAttackAction currentAttack;
    public override State Tick(EnemyManager manager, EnemyStats stats, EnemyAnimator enemyAnim)
    {
        //attack score����� �� �پ��� ���� ����� �ϳ��� ����
        // ���õ� ������ ������ �Ÿ��� �������� �ʴٸ�, ���ο� ������ ����
        // ������ �����ϴٸ�, �������� ���߰� Ÿ���� ����
        // recovery timer�� �̿��� recovery time�� ȸ��
        // combatstance���·� ���ư���

        if (manager.isPerformingAction)
            return combatStanceState;

        if (currentAttack != null)
        {
            // ���� �ʹ� �����鼭 current attack�� �����ߴٸ�, ���ο� ������ ��´�
            if (manager.distFromTarget < currentAttack.minDistToAttack)
            {
                return this;
            }
            // ������ �� ���� ������ ������� ���
            else if (manager.distFromTarget < currentAttack.maxDistToAttack)
            {
                // ���� ����� �þ߰����� �ִٸ� ����
                if (manager.viewableAngle <= currentAttack.maxAttackAngle && manager.viewableAngle >= currentAttack.minAttackAngle)
                {
                    // ��Ÿ�� ȸ���ϰ� ���� ������ ���������� �ʴٸ����
                    if (manager.currentRecoveryTime <= 0 && manager.isPerformingAction == false)
                    {
                        enemyAnim.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                        enemyAnim.anim.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                        enemyAnim.PlayTargetAnimation(currentAttack.actionAnimation, true);
                        manager.isPerformingAction = true;
                        manager.currentRecoveryTime = currentAttack.recoveryTime;
                        currentAttack = null;
                        return combatStanceState;
                    }
                }
            }
        }
        else
        {
            GetNewAttack(manager);
        }

        return combatStanceState;
    }

    public void GetNewAttack(EnemyManager manager)
    {
        Vector3 targetDir =
            manager.currentTargetCharacter.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetDir, transform.forward);
        manager.distFromTarget = Vector3.Distance(
            manager.currentTargetCharacter.transform.position,
            transform.position
        );

        int maxScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction attackAction = enemyAttacks[i];

            if (
                manager.distFromTarget <= attackAction.maxDistToAttack
                && manager.distFromTarget >= attackAction.minDistToAttack
            )
            {
                if (
                    viewableAngle <= attackAction.maxAttackAngle
                    && viewableAngle >= attackAction.minAttackAngle
                )
                {
                    maxScore += attackAction.attackScore;
                }
            }
        }

        int randomValue = Random.Range(0, maxScore);
        int tempScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction attackAction = enemyAttacks[i];

            if (
                manager.distFromTarget <= attackAction.maxDistToAttack
                && manager.distFromTarget >= attackAction.minDistToAttack
            )
            {
                if (
                    viewableAngle <= attackAction.maxAttackAngle
                    && viewableAngle >= attackAction.minAttackAngle
                )
                {
                    if (currentAttack != null)
                        return;

                    tempScore += attackAction.attackScore;

                    if (tempScore > randomValue)
                    {
                        currentAttack = attackAction;
                    }
                }
            }
        }
    }

}
