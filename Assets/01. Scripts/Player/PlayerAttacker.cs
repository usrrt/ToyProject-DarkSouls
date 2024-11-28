using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    PlayerAnimator playerAnim;
    InputHandler inputHandler;
    WeaponSlotManager slotManager;

    public string lastAttack;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerAnim = GetComponentInChildren<PlayerAnimator>();
        slotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        slotManager.attackingWeaponItem = weapon;
        playerAnim.PlayTargetAnimation(weapon.OneHand_LightAttack_1, true);
        lastAttack = weapon.OneHand_LightAttack_1;
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        slotManager.attackingWeaponItem = weapon;
        playerAnim.PlayTargetAnimation(weapon.OneHand_HeavyAttack_1, true);
        lastAttack = weapon.OneHand_HeavyAttack_1;
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if (inputHandler.comboFlag)
        {
            playerAnim.anim.SetBool("canDoCombo", false);
            if (lastAttack == weapon.OneHand_LightAttack_1)
            {
                playerAnim.PlayTargetAnimation(weapon.OneHand_LightAttack_2, true);
            }
            else if (lastAttack == weapon.OneHand_HeavyAttack_1)
            {
                playerAnim.PlayTargetAnimation(weapon.OneHand_HeavyAttack_2, true);
            }
        }
    }
}
