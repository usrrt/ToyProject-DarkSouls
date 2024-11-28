using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    EnemyAnimator enemyAnim;

    private void Awake()
    {
        enemyAnim = GetComponentInChildren<EnemyAnimator>();
    }

    private void Start()
    {
        maxHealth = SetMaxHealthFromHealthLv();
        currentHealth = maxHealth;
    }

    public int SetMaxHealthFromHealthLv()
    {
        maxHealth = healthLv * 10;
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        enemyAnim.PlayTargetAnimation("Hit_B", true);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            enemyAnim.PlayTargetAnimation("Death_1", true);
        }
    }
}
