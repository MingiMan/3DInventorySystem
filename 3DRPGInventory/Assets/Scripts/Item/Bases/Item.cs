using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item
{
    public ItemData Data { get; private set; }
    // Ŭ���� ���ο����� ������ ����

    // �����ڴ� Ŭ������ �ν��Ͻ��� ������ �� ȣ��Ǹ�, Ŭ������ �ʱ�ȭ �۾��� �����մϴ�
    public Item(ItemData data) => Data = data;
}
