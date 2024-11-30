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
    public State currentState; // ù ������ IDLE STATE�� �����Ѵ�
    public CharacterStats currentTargetCharacter;

    public bool isPerformingAction; // ai�� �ൿ ���� ���� Ȯ��

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


}
