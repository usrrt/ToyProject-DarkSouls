using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats playerStat = other.GetComponent<PlayerStats>();
        if (playerStat != null)
        {
            playerStat.TakeDamage(damage);
        }
    }
}
