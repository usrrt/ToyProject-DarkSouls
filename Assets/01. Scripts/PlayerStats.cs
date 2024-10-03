using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int healthLv = 10;
    public int maxHealth;
    public int currentHealth;

    public HealthBar healthBar;
    private AnimatorHandler animHandler;

    private void Awake()
    {
        animHandler = GetComponentInChildren<AnimatorHandler>();
    }

    private void Start()
    {
        maxHealth = SetMaxHealthFromHealthLv();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public int SetMaxHealthFromHealthLv()
    {
        maxHealth = healthLv * 10;
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetCurrentHealth(currentHealth);
        animHandler.PlayTargetAnimation("Hit_B", true);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animHandler.PlayTargetAnimation("Death_1", true);
        }
    }
}
