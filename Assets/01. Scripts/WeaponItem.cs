using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon item")]
public class WeaponItem : Item
{
    public GameObject modelPf;
    public bool isUnarmed;

    // �� ���⸶�� ���� ��� �ٸ��� �����ϱ� ���ؼ� ���Ⱑ ��������� ������ ����
    [Header("One Hand Attack Animations")]
    public string OneHand_LightAttack_1;
    public string OneHand_LightAttack_2;
    public string OneHand_HeavyAttack_1;
    public string OneHand_HeavyAttack_2;
}
