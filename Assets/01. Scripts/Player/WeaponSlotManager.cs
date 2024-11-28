using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    public WeaponItem attackingWeaponItem;

    WeaponHolderSlot rightSlot;
    WeaponHolderSlot leftSlot;

    DamageCollider leftHandDamageCol;
    DamageCollider rightHandDamageCol;

    Animator animator;
    QuickSlotsUI quickSlotsUI;
    PlayerStats playerStats;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        quickSlotsUI = FindObjectOfType<QuickSlotsUI>();
        playerStats = GetComponentInParent<PlayerStats>();

        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
        {
            if (weaponSlot.isLeftHandSlot)
            {
                leftSlot = weaponSlot;
            }
            else if (weaponSlot.isRightHandSlot)
            {
                rightSlot = weaponSlot;
            }
        }
    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
    {
        if (weaponItem == null)
            return;

        if (isLeft)
        {
            leftSlot.LoadWeaponModel(weaponItem);
            LoadLeftWeaponDamageCollider();
            quickSlotsUI.UpdateWeaponQuickSlotsUI(true, weaponItem);

            #region 왼손 무기 Idle Animation
            if (weaponItem != null)
            {
                animator.CrossFade(weaponItem.left_hand_idle, 0.2f);
            }
            else
            {
                animator.CrossFade("Left Arm Empty", 0.2f);
            }

            #endregion
        }
        else
        {
            rightSlot.LoadWeaponModel(weaponItem);
            LoadRightWeaponDamageCollider();
            quickSlotsUI.UpdateWeaponQuickSlotsUI(false, weaponItem);

            #region 오른손 무기 Idle Animation
            if (weaponItem != null)
            {
                animator.CrossFade(weaponItem.right_hand_idle, 0.2f);
            }
            else
            {
                animator.CrossFade("Right Arm Empty", 0.2f);
            }

            #endregion
        }
    }

    #region 무기 콜라이더
    private void LoadLeftWeaponDamageCollider()
    {
        leftHandDamageCol = leftSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    private void LoadRightWeaponDamageCollider()
    {
        rightHandDamageCol = rightSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    public void OpenLeftWeaponDamageCollider()
    {
        leftHandDamageCol.EnableDamageCollider();
    }

    public void OpenRightWeaponDamageCollider()
    {
        rightHandDamageCol.EnableDamageCollider();
    }

    public void CloseLeftWeaponDamageCollider()
    {
        leftHandDamageCol.DisableDamageCollider();
    }

    public void CloseRightWeaponDamageCollider()
    {
        rightHandDamageCol.DisableDamageCollider();
    }

    #endregion

    #region 스태미나 감소
    public void DrainStaminaLightAttack()
    {
        playerStats.TakeStaminaDrain(
            Mathf.RoundToInt(
                attackingWeaponItem.baseStamina * attackingWeaponItem.lightAttackMultiplier
            )
        );
    }

    public void DrainStaminaHeavyAttack()
    {
        playerStats.TakeStaminaDrain(
            Mathf.RoundToInt(
                attackingWeaponItem.baseStamina * attackingWeaponItem.heavyAttackMultiplier
            )
        );
    }

    #endregion
}
