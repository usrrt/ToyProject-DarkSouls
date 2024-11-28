using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/Enemy Action/Attack")]
public class EnemyAttackAction : EnemyAction
{
    public int attackScore = 3;
    public float recoveryTime = 2f;

    public float maxAttackAngle = 35f;
    public float minAttackAngle = -35f;

    public float maxDistToAttack = 3f;
    public float minDistToAttack = 0f;
}
