using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public EquipmentWindowUI equipmentWindowUI;

    [Header("UI Window")]
    public GameObject selectWindow;
    public GameObject hudWindow;
    public GameObject weaponInventoryWindow;
    public GameObject equipmentScreenWindow;

    [Header("Weapon Inventory")]
    public GameObject weaponInventorySlotPf;
    public Transform weaponInventorySlotsParent;

    [Header("Equipment Hand Slot Selected")]
    public bool rightHandSlotSelected_01;
    public bool rightHandSlotSelected_02;
    public bool leftHandSlotSelected_01;
    public bool leftHandSlotSelected_02;

    [SerializeField]
    WeaponInventorySlot[] weaponInventorySlots;

    private void Awake()
    {
        equipmentWindowUI = GetComponentInChildren<EquipmentWindowUI>(true);
    }

    private void Start()
    {
        // includeInactive매개변수는 활성화되지 않은 자식 오브젝트도 검색할지 여부를 결정한다. true로 설정하면 비활성화된 자식오브젝트에서도 컴포넌트를 찾아 반환한다
        // Weapon Inventory Slot은 비활성화 상태에서 시작하므로 true로 해야 컴포넌트를 찾을수있다
        //weaponInventorySlots = GetComponentsInChildren<WeaponInventorySlot>();
        equipmentWindowUI.LoadWeaponsOnEquipmentUI(playerInventory);
    }

    public void UpdateUI()
    {
        for (int i = 0; i < weaponInventorySlots.Length; i++)
        {
            if (i < playerInventory.weaponInventory.Count)
            {
                if (weaponInventorySlots.Length < playerInventory.weaponInventory.Count)
                {
                    Instantiate(weaponInventorySlotPf, weaponInventorySlotsParent);
                    weaponInventorySlots =
                        weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
                }
                weaponInventorySlots[i].AddItem(playerInventory.weaponInventory[i]);
            }
            else
            {
                Debug.Log("jfkdlsa;jfdksal;");
                weaponInventorySlots[i].ClearInventorySlot();
            }
        }
    }

    public void OpenSelectWindow()
    {
        selectWindow.SetActive(true);
    }

    public void CloseSelectWindow()
    {
        selectWindow.SetActive(false);
    }

    public void CloseAllInventoryWindows()
    {
        ResetAllSelectedHandSlots();
        weaponInventoryWindow.SetActive(false);
        equipmentScreenWindow.SetActive(false);
    }

    public void ResetAllSelectedHandSlots()
    {
        rightHandSlotSelected_01 = false;
        rightHandSlotSelected_02 = false;
        leftHandSlotSelected_01 = false;
        leftHandSlotSelected_02 = false;
    }
}
