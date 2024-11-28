using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotion : MonoBehaviour
{
    EnemyManager enemyManager;
    EnemyAnimator enemyAnim;
    NavMeshAgent navMeshAgent;
    public Rigidbody enemyRigid;

    public CharacterStats currentTargetCharacter;

    public float distFromTarget;
    public float stoppingDist = 1f;
    public float rotationSpeed = 20f;

    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        enemyAnim = GetComponentInChildren<EnemyAnimator>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        navMeshAgent.enabled = false;
        enemyRigid.isKinematic = false;
    }

    public void HandleCurrentAction()
    {
        if ((currentTargetCharacter != null))
        {
            distFromTarget = Vector3.Distance(
                currentTargetCharacter.transform.position,
                transform.position
            );
        }
        if (currentTargetCharacter == null)
        {
            HandleDetection();
        }
        else
        {
            HandleMoveToTarget();
            if (distFromTarget <= stoppingDist)
            {
                enemyManager.AttackTarget();
            }
        }
        //else if (distFromTarget > stoppingDist)
        //{
        //    //HandleMoveToTarget();
        //}
        //else if (distFromTarget <= stoppingDist)
        //{
        //    // attack
        //    //enemyManager.AttackTarget();
        //}
    }

    private void HandleDetection()
    {
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
    }

    private void HandleMoveToTarget()
    {
        //if (enemyManager.isPerformingAction)
        //    return;

        Vector3 targetDir = currentTargetCharacter.transform.position - transform.position;
        distFromTarget = Vector3.Distance(
            currentTargetCharacter.transform.position,
            transform.position
        );
        float viewableAngle = Vector3.Angle(targetDir, transform.forward);

        // ���� �����߿� �������� �����
        if (enemyManager.isPerformingAction)
        {
            enemyAnim.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            navMeshAgent.enabled = false;
        }
        else
        {
            Debug.Log("1111111111111111111111");
            if (distFromTarget > stoppingDist)
            {
                Debug.Log("222222222222222222");
                enemyAnim.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }
            else if (distFromTarget <= stoppingDist)
            {
                Debug.Log("3333333333333333");
                enemyAnim.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            }
        }

        HandleRotateTowardsTarget();

        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;
    }

    private void HandleRotateTowardsTarget()
    {
        // ���� ȸ��
        if (enemyManager.isPerformingAction)
        {
            Vector3 dir = currentTargetCharacter.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();

            if (dir == Vector3.zero)
                dir = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed / Time.deltaTime
            );
        }
        // ��ã��(navmesh)�� ���� ȸ��
        else
        {
            Vector3 relativeDir = transform.InverseTransformDirection(navMeshAgent.desiredVelocity);
            Vector3 targetVel = enemyRigid.velocity;

            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(currentTargetCharacter.transform.position);
            enemyRigid.velocity = targetVel;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                navMeshAgent.transform.rotation,
                rotationSpeed / Time.deltaTime
            );
        }
    }
}
