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

    // �� ���⸶�� ���� ��� �ٸ��� �����ϱ� ���ؼ� ���Ⱑ ��������� ������ ����
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
