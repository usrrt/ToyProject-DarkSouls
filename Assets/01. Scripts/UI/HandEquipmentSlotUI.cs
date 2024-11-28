using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class HandEquipmentSlotUI : MonoBehaviour
{
    public Image icon;
    WeaponItem weapon;
    public UIManager uiManager;

    public bool rightHandSlot_01;
    public bool rightHandSlot_02;
    public bool leftHandSlot_01;
    public bool leftHandSlot_02;

    private void Awake()
    {
        uiManager = GetComponentInParent<UIManager>();
    }

    public void AddItem(WeaponItem newWeapon)
    {
        if (newWeapon == null || newWeapon.isUnarmed)
            return;

        weapon = newWeapon;
        gameObject.SetActive(true);
        icon.sprite = weapon.itemIcon;
        icon.enabled = true;
    }

    public void ClearEquipmentSlot()
    {
        weapon = null;
        icon.sprite = null;
        icon.enabled = false;
        gameObject.SetActive(false);
    }

    public void SelectThisSlot()
    {
        if (rightHandSlot_01)
        {
            uiManager.rightHandSlotSelected_01 = true;
        }
        else if (rightHandSlot_02)
        {
            uiManager.rightHandSlotSelected_02 = true;
        }
        else if (leftHandSlot_01)
        {
            uiManager.leftHandSlotSelected_01 = true;
        }
        else if (leftHandSlot_02)
        {
            uiManager.leftHandSlotSelected_02 = true;
        }
    }
}
