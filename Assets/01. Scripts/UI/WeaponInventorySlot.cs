using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventorySlot : MonoBehaviour
{
    PlayerInventory playerInventory;
    UIManager uiManager;
    WeaponSlotManager weaponSlotManager;

    public Image icon;
    WeaponItem item;

    private void Awake()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
        uiManager = GetComponentInParent<UIManager>();
    }

    public void AddItem(WeaponItem newItem)
    {
        if (newItem == null || newItem.isUnarmed)
            return;
        item = newItem;
        gameObject.SetActive(true);
        icon.sprite = newItem.itemIcon;
        icon.enabled = true;
    }

    public void ClearInventorySlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        gameObject.SetActive(false);
    }

    public void EquipSlectItem()
    {
        // 1. 현재 장착한 아이템 인벤토리에 넣기
        // 2. 인벤토리에서 선택한 아이템 가져오기
        // 3. 새로운 아이템 장착
        // 4. 인벤토리에서 선택한 아이템 제거
        if (uiManager == null)
            return;

        if (uiManager.rightHandSlotSelected_01)
        {
            // 현재 장착한 아이템 인벤토리에 넣기
            if (playerInventory.weaponsInRightHandSlots[0] != null)
            {
                playerInventory.weaponInventory.Add(playerInventory.weaponsInRightHandSlots[0]);
            }
            // 인벤토리 아이템 장착
            playerInventory.weaponsInRightHandSlots[0] = item;
            // 인벤토리에서 아이템 제거
            playerInventory.weaponInventory.Remove(item);
        }
        else if (uiManager.rightHandSlotSelected_02)
        {
            if (playerInventory.weaponsInRightHandSlots[1] != null)
            {
                playerInventory.weaponInventory.Add(playerInventory.weaponsInRightHandSlots[1]);
            }
            playerInventory.weaponsInRightHandSlots[1] = item;
            playerInventory.weaponInventory.Remove(item);
        }
        else if (uiManager.leftHandSlotSelected_01)
        {
            if (playerInventory.weaponsInLeftHandSlots[0] != null)
            {
                playerInventory.weaponInventory.Add(playerInventory.weaponsInLeftHandSlots[0]);
            }
            playerInventory.weaponsInLeftHandSlots[0] = item;
            playerInventory.weaponInventory.Remove(item);
        }
        else if (uiManager.leftHandSlotSelected_02)
        {
            if (playerInventory.weaponsInLeftHandSlots[1] != null)
            {
                playerInventory.weaponInventory.Add(playerInventory.weaponsInLeftHandSlots[1]);
            }
            playerInventory.weaponsInLeftHandSlots[1] = item;
            playerInventory.weaponInventory.Remove(item);
        }
        else
            return;

        playerInventory.rightWeapon = playerInventory.weaponsInRightHandSlots[
            playerInventory.currentRightWeaponIdx
        ];
        playerInventory.leftWeapon = playerInventory.weaponsInLeftHandSlots[
            playerInventory.currentLeftWeaponIdx
        ];

        // 손에 무기를 리로드
        weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
        weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);

        uiManager.equipmentWindowUI.LoadWeaponsOnEquipmentUI(playerInventory);
        uiManager.ResetAllSelectedHandSlots();
    }
}
