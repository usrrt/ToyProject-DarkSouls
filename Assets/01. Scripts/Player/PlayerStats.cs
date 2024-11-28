using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public HealthBar healthBar;
    public StaminaBar staminaBar;

    private PlayerAnimator playerAnim;

    private void Awake()
    {
        healthBar = FindObjectOfType<HealthBar>();
        staminaBar = FindObjectOfType<StaminaBar>();
        playerAnim = GetComponentInChildren<PlayerAnimator>();
    }

    private void Start()
    {
        maxHealth = SetMaxHealthFromHealthLv();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetCurrentHealth(currentHealth);

        maxStamina = SetMaxStaminaFromStaminaLv();
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
        staminaBar.SetCurrentStamina(currentStamina);
    }

    private int SetMaxHealthFromHealthLv()
    {
        maxHealth = healthLv * 10;
        return maxHealth;
    }

    private int SetMaxStaminaFromStaminaLv()
    {
        maxStamina = staminaLv * 10;
        return maxStamina;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetCurrentHealth(currentHealth);
        playerAnim.PlayTargetAnimation("Hit_B", true);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            playerAnim.PlayTargetAnimation("Death_1", true);
        }
    }

    public void TakeStaminaDrain(int drain)
    {
        currentStamina -= drain;
        staminaBar.SetCurrentStamina(currentStamina);
    }
}
