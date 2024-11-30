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
        //attack score기반을 둔 다양한 공격 방식중 하나를 선택
        // 선택된 공격의 각도나 거리가 적절하지 않다면, 새로운 공격을 선택
        // 공격이 적절하다면, 움직임을 멈추고 타겟을 공격
        // recovery timer를 이용해 recovery time을 회복
        // combatstance상태로 돌아간다

        if (manager.isPerformingAction)
            return combatStanceState;

        if (currentAttack != null)
        {
            // 적과 너무 가까우면서 current attack을 수행했다면, 새로운 공격을 얻는다
            if (manager.distFromTarget < currentAttack.minDistToAttack)
            {
                return this;
            }
            // 공격할 수 있을 정도로 가까워질 경우
            else if (manager.distFromTarget < currentAttack.maxDistToAttack)
            {
                // 공격 대상이 시야각내에 있다면 공격
                if (manager.viewableAngle <= currentAttack.maxAttackAngle && manager.viewableAngle >= currentAttack.minAttackAngle)
                {
                    // 쿨타임 회복하고 현재 동작을 수행중이지 않다면공격
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
