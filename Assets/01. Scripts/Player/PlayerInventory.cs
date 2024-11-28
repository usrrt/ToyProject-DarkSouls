using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    WeaponSlotManager weaponSlotManager;

    public WeaponItem rightWeapon;
    public WeaponItem leftWeapon;

    public WeaponItem unarmedWeapon;

    public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[2];
    public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[2];

    public List<WeaponItem> weaponInventory;

    public int currentRightWeaponIdx = -1;
    public int currentLeftWeaponIdx = -1;

    private void Awake()
    {
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    private void Start()
    {
        rightWeapon = weaponsInRightHandSlots[0] ?? unarmedWeapon;
        leftWeapon = weaponsInLeftHandSlots[0] ?? unarmedWeapon;

        weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
    }

    public void ChangeRightWeapon()
    {
        currentRightWeaponIdx++;
        if (currentRightWeaponIdx >= 0 && currentRightWeaponIdx < weaponsInRightHandSlots.Length)
        {
            if (weaponsInRightHandSlots[currentRightWeaponIdx] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIdx];
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            }
            else
            {
                currentRightWeaponIdx++;
            }
        }

        if (currentRightWeaponIdx > weaponsInRightHandSlots.Length - 1)
        {
            currentRightWeaponIdx = -1;
            rightWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        }
    }

    public void ChangeLeftWeapon()
    {
        currentLeftWeaponIdx++;
        if (currentLeftWeaponIdx >= 0 && currentLeftWeaponIdx < weaponsInLeftHandSlots.Length)
        {
            if (weaponsInLeftHandSlots[currentLeftWeaponIdx] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIdx];
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            }
            else
            {
                currentLeftWeaponIdx++;
            }
        }

        if (currentLeftWeaponIdx > weaponsInLeftHandSlots.Length - 1)
        {
            currentLeftWeaponIdx = -1;
            leftWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }
    }
}
