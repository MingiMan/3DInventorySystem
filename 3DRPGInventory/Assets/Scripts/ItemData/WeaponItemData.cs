using System.Collections.Generic;
using UnityEngine;

public enum Hand
{
    LEFT,
    RIGHT
};

public enum WeaponType
{
    None, // 0
    Sword, // 1
    Axe, // 2 
    Bow // 3
};

[CreateAssetMenu(fileName = "Item_Weapon_", menuName = "Inventory System/Item Data/Weapon", order = 1)]
public class WeaponItemData : EquipmentItemData
{
    public int Damage => _damage;
    public Hand hand;
    public WeaponType type;
    public List<AttackData> attackData; // �� ������ ��� �޺�

    public override ItemSlotType slotType => ItemSlotType.Weapon;

    [SerializeField] private int _damage = 1;

    // �� �Լ��� ���� �ش� �����͸� ����Ͽ� WeaponItem �ν��Ͻ��� �����ϰ� ��ȯ�մϴ�.
    public override Item CreateItem()
    {
        return new WeaponItem(this);
    }

    public override void AssignItemToPlayer(PlayerDisplayEquipment playerEquipmentController)
    {
        playerEquipmentController.AssignHandItem(this);
    }
}
