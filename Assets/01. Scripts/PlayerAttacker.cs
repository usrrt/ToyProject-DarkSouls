using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    AnimatorHandler animatorHandler;
    InputHandler inputHandler;

    public string lastAttack;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        animatorHandler.PlayTargetAnimation(weapon.OneHand_LightAttack_1, true);
        lastAttack = weapon.OneHand_LightAttack_1;
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        animatorHandler.PlayTargetAnimation(weapon.OneHand_HeavyAttack_1, true);
        lastAttack = weapon.OneHand_HeavyAttack_1;
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if (inputHandler.comboFlag)
        {
            animatorHandler.anim.SetBool("canDoCombo", false);
            if (lastAttack == weapon.OneHand_LightAttack_1)
            {
                animatorHandler.PlayTargetAnimation(weapon.OneHand_LightAttack_2, true);
            }
            else if (lastAttack == weapon.OneHand_HeavyAttack_1)
            {
                animatorHandler.PlayTargetAnimation(weapon.OneHand_HeavyAttack_2, true);
            }
        }
    }
}
