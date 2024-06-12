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
    [SerializeField] private string _name;    // ������ �̸�
    [Multiline]
    [SerializeField] private string _tooltip; // ������ ����
    [SerializeField] private Sprite _iconSprite; // ������ ������
    [SerializeField] private GameObject _itemPrefab; // �ٴڿ� ������ �� ������ ������

    // Ÿ�Կ� �´� ���ο� ������ ����
    public abstract Item CreateItem();
}


