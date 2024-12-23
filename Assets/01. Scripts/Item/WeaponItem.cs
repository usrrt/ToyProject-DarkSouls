using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon item")]
public class WeaponItem : Item
{
    public GameObject modelPf;
    public bool isUnarmed;

    [Header("Idle Animations")]
    public string right_hand_idle;
    public string left_hand_idle;

    // 각 무기마다 공격 모션 다르게 적용하기 위해서 무기가 모션정보를 가지는 구조
    [Header("One Hand Attack Animations")]
    public string OneHand_LightAttack_1;
    public string OneHand_LightAttack_2;
    public string OneHand_HeavyAttack_1;
    public string OneHand_HeavyAttack_2;

    [Header("Stamina Costs")]
    public int baseStamina;
    public float lightAttackMultiplier;
    public float heavyAttackMultiplier;
}
