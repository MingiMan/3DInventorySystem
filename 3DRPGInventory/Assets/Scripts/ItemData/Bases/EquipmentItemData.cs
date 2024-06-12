using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItemData : ItemData
{
    public int MaxDurability => _maxDurability;

    [SerializeField] private int _maxDurability = 100;

    public Vector3 ItemLocalPosition => _itemLocalPosition;

    // �� �� ������ �÷��̾ �������� �˸°� �����ϵ��� ����ڰ� �̸� �����س����ϴ�.
    [SerializeField] Vector3 _itemLocalPosition; // �����ۿ� ���� ��ǥ
    [SerializeField] Vector3 _itemLocalRotation; // �����ۿ� ���� ȸ��

    public GameObject GetPrefab()
    {
        return ItemPrefab;
    }

    public Quaternion GetLocalRotation()
    {
        return Quaternion.Euler(_itemLocalRotation);
    }

    public abstract void AssignItemToPlayer(PlayerDisplayEquipment playerEquipmentController);

}
