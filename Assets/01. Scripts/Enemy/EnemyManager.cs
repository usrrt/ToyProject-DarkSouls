using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    EnemyLocomotion locomotion;
    EnemyAnimator enemyAnim;

    public bool isPerformingAction; // ai의 행동 수행 여부 확인

    public EnemyAttackAction[] enemyAttacks;
    public EnemyAttackAction currentAttack;

    [Header("AI Setting")]
    public float detectionRadius = 20;
    public float maxDetectionAngle = 50f;
    public float minDetectionAngle = -50f;
    public float currentRecoveryTime;

    public LayerMask detectionLayer;

    private void Awake()
    {
        locomotion = GetComponent<EnemyLocomotion>();
        enemyAnim = GetComponentInChildren<EnemyAnimator>();
    }

    private void FixedUpdate()
    {
        locomotion.HandleCurrentAction();
    }

    private void Update()
    {
        HandleRecoveryTimer();
    }

    public void HandleRecoveryTimer()
    {
        if (currentRecoveryTime > 0)
        {
            currentRecoveryTime -= Time.deltaTime;
        }

        if (isPerformingAction)
        {
            if (currentRecoveryTime <= 0)
            {
                isPerformingAction = false;
            }
        }
    }

    public void AttackTarget()
    {
        if (isPerformingAction)
            return;

        if (currentAttack == null)
        {
            GetNewAttack();
        }
        else
        {
            isPerformingAction = true;
            currentRecoveryTime = currentAttack.recoveryTime;
            enemyAnim.PlayTargetAnimation(currentAttack.actionAnimation, true);
            currentAttack = null; // 공격을 재설정하지않으면 동일한공격을 계속 반복
        }
    }

    public void GetNewAttack()
    {
        Vector3 targetDir =
            locomotion.currentTargetCharacter.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetDir, transform.forward);
        locomotion.distFromTarget = Vector3.Distance(
            locomotion.currentTargetCharacter.transform.position,
            transform.position
        );

        int maxScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction attackAction = enemyAttacks[i];

            if (
                locomotion.distFromTarget <= attackAction.maxDistToAttack
                && locomotion.distFromTarget >= attackAction.minDistToAttack
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
                locomotion.distFromTarget <= attackAction.maxDistToAttack
                && locomotion.distFromTarget >= attackAction.minDistToAttack
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
                        Debug.Log(currentAttack);
                    }
                }
            }
        }
    }
}
