using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItemData : ItemData
{
    public int MaxDurability => _maxDurability;

    [SerializeField] private int _maxDurability = 100;

    public Vector3 ItemLocalPosition => _itemLocalPosition;

    // 이 두 변수는 플레이어가 아이템을 알맞게 착용하도록 사용자가 미리 설정해놓습니다.
    [SerializeField] Vector3 _itemLocalPosition; // 아이템에 로컬 좌표
    [SerializeField] Vector3 _itemLocalRotation; // 아이템에 로컬 회전

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
