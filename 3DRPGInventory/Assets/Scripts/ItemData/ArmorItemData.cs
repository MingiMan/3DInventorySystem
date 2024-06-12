using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Armor_", menuName = "Inventory System/Item Data/Armor", order = 2)]
public class ArmorItemData : EquipmentItemData
{
    public int Defence => _defence;
    public override ItemSlotType slotType => ItemSlotType.Armor;

    [SerializeField] private int _defence = 1;
    public override Item CreateItem()
    {
        return new ArmorItem(this);
    }

    public override void AssignItemToPlayer(PlayerDisplayEquipment playerEquipmentController)
    {
        // 아직 미완성
    }

}
