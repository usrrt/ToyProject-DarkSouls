using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    Collider damageCol;

    public int weaponDamage;

    private void Awake()
    {
        damageCol = GetComponent<Collider>();
        damageCol.gameObject.SetActive(true);
        damageCol.isTrigger = true;
        damageCol.enabled = false;
    }

    private void Start()
    {
        weaponDamage = 25;
    }

    public void EnableDamageCollider()
    {
        damageCol.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCol.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.tag == "Player")
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(weaponDamage);
            }
        }

        if (other.tag == "Enemy")
        {
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(weaponDamage);
            }
        }
    }
}
