using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int healthLv = 10;
    public int maxHealth;
    public int currentHealth;

    Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
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

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            anim.Play("Death_01");
        }
    }
}
