using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : Interactable
{
    public WeaponItem weapon;

    public override void Interact(PlayerManager manager)
    {
        base.Interact(manager);

        // �������� �ݰ� �κ��丮�� �߰��Ѵ�
        PickUpItem(manager);
    }

    private void PickUpItem(PlayerManager manager)
    {
        PlayerInventory inventory;
        PlayerLocomotion locomotion;
        PlayerAnimator animator;
        inventory = manager.GetComponent<PlayerInventory>();
        locomotion = manager.GetComponent<PlayerLocomotion>();
        animator = manager.GetComponentInChildren<PlayerAnimator>();

        locomotion.rigidbody.velocity = Vector3.zero; // �������� �ݴµ��� �������� �ʰ�
        animator.PlayTargetAnimation("Pick Up Item", true);
        inventory.weaponInventory.Add(weapon);
        manager.interactableUI.itemTxt.text = weapon.name;
        manager.interactableUI.itemImage.texture = weapon.itemIcon.texture;
        manager.interactableUI.itemInteractableObj.SetActive(true);
        Destroy(gameObject);
    }
}
