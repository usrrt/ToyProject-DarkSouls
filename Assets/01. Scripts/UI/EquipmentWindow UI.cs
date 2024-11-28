using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentWindowUI : MonoBehaviour
{
    public bool rightHandSlotSelected_01;
    public bool rightHandSlotSelected_02;
    public bool leftHandSlotSelected_01;
    public bool leftHandSlotSelected_02;

    [SerializeField]
    HandEquipmentSlotUI[] handEquipmentSlotUIs;

    private void Awake()
    {
        handEquipmentSlotUIs = GetComponentsInChildren<HandEquipmentSlotUI>(true);
    }

    public void LoadWeaponsOnEquipmentUI(PlayerInventory inventory)
    {
        for (int i = 0; i < handEquipmentSlotUIs.Length; i++)
        {
            if (handEquipmentSlotUIs[i].rightHandSlot_01)
            {
                handEquipmentSlotUIs[i].AddItem(inventory.weaponsInRightHandSlots[0]);
            }
            else if (handEquipmentSlotUIs[i].rightHandSlot_02)
            {
                handEquipmentSlotUIs[i].AddItem(inventory.weaponsInRightHandSlots[1]);
            }
            else if (handEquipmentSlotUIs[i].leftHandSlot_01)
            {
                handEquipmentSlotUIs[i].AddItem(inventory.weaponsInLeftHandSlots[0]);
            }
            else if (handEquipmentSlotUIs[i].leftHandSlot_02)
            {
                handEquipmentSlotUIs[i].AddItem(inventory.weaponsInLeftHandSlots[1]);
            }
        }
    }

    public void SelectRightHandSlot_01()
    {
        rightHandSlotSelected_01 = true;
    }

    public void SelectRightHandSlot_02()
    {
        rightHandSlotSelected_02 = true;
    }

    public void SelectLeftHandSlot_01()
    {
        leftHandSlotSelected_01 = true;
    }

    public void SelectLeftHandSlot_02()
    {
        leftHandSlotSelected_02 = true;
    }
}
