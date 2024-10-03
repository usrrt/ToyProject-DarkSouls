using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    WeaponHolderSlot rightSlot;
    WeaponHolderSlot leftSlot;

    DamageCollider leftHandDamageCol;
    DamageCollider rightHandDamageCol;

    private void Awake()
    {
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
        if (isLeft)
        {
            leftSlot.LoadWeaponModel(weaponItem);
            LoadLeftWeaponDamageCollider();
        }
        else
        {
            rightSlot.LoadWeaponModel(weaponItem);
            LoadRightWeaponDamageCollider();
        }
    }

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
}
