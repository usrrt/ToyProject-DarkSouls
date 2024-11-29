using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public override State Tick(EnemyManager manager, EnemyStats stats, EnemyAnimator animator)
    {
        // attack score기반을 둔 다양한 공격 방식중 하나를 선택
        // 선택된 공격의 각도나 거리가 적절하지 않다면, 새로운 공격을 선택
        // 공격이 적절하다면, 움직임을 멈추고 타겟을 공격
        // recovery timer를 이용해 recovery time을 회복
        // combatstance상태로 돌아간다

        return this;
    }

    
}
