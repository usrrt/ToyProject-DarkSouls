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
    public float rotationSpeed = 20f;
    public float maxAttackRange = 1.5f;

    //public EnemyAttackAction[] enemyAttacks;
    //public EnemyAttackAction currentAttack;
    public State currentState; // 첫 시작은 IDLE STATE로 시작한다
    public CharacterStats currentTargetCharacter;

    public bool isPerformingAction; // ai의 행동 수행 여부 확인

    [Header("AI Setting")]
    public float detectionRadius = 20;
    public float maxDetectionAngle = 50f;
    public float minDetectionAngle = -50f;
    public float currentRecoveryTime;
    public float viewableAngle;

    

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

    // 상태 업데이트를 위해 코드를 계속 실행하면서 타겟을 탐색하고 따라다니며 공격범위를 알려주고 다음 상태로 전환되는 식으로 진행된다
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


}
