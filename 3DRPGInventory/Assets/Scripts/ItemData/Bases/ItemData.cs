using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public int ID => _id;

    public string Name => _name;

    public string Tooltip => _tooltip;

    public Sprite IconSprite => _iconSprite;

    public GameObject ItemPrefab => _itemPrefab;

    public virtual ItemSlotType slotType => ItemSlotType.None;

    [SerializeField] private int _id;
    [SerializeField] private string _name;    // 아이템 이름
    [Multiline]
    [SerializeField] private string _tooltip; // 아이템 설명
    [SerializeField] private Sprite _iconSprite; // 아이템 아이콘
    [SerializeField] private GameObject _itemPrefab; // 바닥에 떨어질 때 생성할 프리팹

    // 타입에 맞는 새로운 아이템 생성
    public abstract Item CreateItem();
}


