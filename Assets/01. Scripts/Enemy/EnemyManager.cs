using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : CharacterManager
{
    EnemyLocomotion locomotion;
    EnemyAnimator enemyAnim;
    EnemyStats enemyStats;

    public Rigidbody enemyRigid;
    public NavMeshAgent navMeshAgent;
    public float distFromTarget;
    public float stoppingDist = 1f;
    public float rotationSpeed = 20f;

    //public EnemyAttackAction[] enemyAttacks;
    //public EnemyAttackAction currentAttack;
    public State currentState; // ù ������ IDLE STATE�� �����Ѵ�
    public CharacterStats currentTargetCharacter;

    public bool isPerformingAction; // ai�� �ൿ ���� ���� Ȯ��

    [Header("AI Setting")]
    public float detectionRadius = 20;
    public float maxDetectionAngle = 50f;
    public float minDetectionAngle = -50f;
    public float currentRecoveryTime;

    

    private void Awake()
    {
        locomotion = GetComponent<EnemyLocomotion>();
        enemyAnim = GetComponentInChildren<EnemyAnimator>();
        enemyStats = GetComponent<EnemyStats>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        navMeshAgent.enabled = false;
    }

    private void Update()
    {
        HandleStateMachine();
        HandleRecoveryTimer();
    }

    // ���� ������Ʈ�� ���� �ڵ带 ��� �����ϸ鼭 Ÿ���� Ž���ϰ� ����ٴϸ� ���ݹ����� �˷��ְ� ���� ���·� ��ȯ�Ǵ� ������ ����ȴ�
    private void HandleStateMachine()
    {
        if (currentState != null)
        {
            State nextState = currentState.Tick(this, enemyStats, enemyAnim);
            if (nextState != null)
            {
                SwitchNextStete(nextState);
            }
        }
    }

    private void SwitchNextStete(State state)
    {
        currentState = state;
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
        /* OLD CODE
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
            currentAttack = null; // ������ �缳������������ �����Ѱ����� ��� �ݺ�
        }
         */

    }

    public void GetNewAttack()
    {
        /* OLD CODE
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

        */

    }
}
