using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    WeaponSlotManager WeaponSlotManager;

    public WeaponItem rightWeapon;
    public WeaponItem leftWeapon;

    private void Awake()
    {
        WeaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    private void Start()
    {
        WeaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        WeaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
    }
}
