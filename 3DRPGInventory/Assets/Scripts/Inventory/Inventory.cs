using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region  ������ �����ѵ�
    // ������ ���� �ѵ�
    public int Capacity { get; private set; }

    // �ʱ� ���� �ѵ� 
    [SerializeField, Range(5, 25)]
    private int _initalCapacity = 10;

    // �ִ� ���� �ѵ�(������ �迭 ũ��)
    //[SerializeField, Range(5, 25)]
    //private int _maxCapacity = 25;

    [SerializeField]
    private InventoryUI _inventoryUI; // ����� �κ��丮 UI

    // Weapon ������ ���
    [SerializeField]
    private Item[] _weaponItems;

    // Armor ������ ���
    [SerializeField]
    private Item[] _armorItems;

    // Portion ������ ���
    [SerializeField]
    private Item[] _portionItems;

    // Weapon ������ ���� �ѵ�
    [SerializeField, Range(5, 25)]
    private int _weaponCapacity = 10;

    // Armor ������ ���� �ѵ�
    [SerializeField, Range(5, 25)]
    private int _armorCapacity = 10;

    // Portion ������ ���� �ѵ�
    [SerializeField, Range(5, 25)]
    private int _portionCapacity = 10;

    [SerializeField]
    // �κ��丮 â
    private GameObject _mainInventory;

    #endregion

    // �÷��̾� ����
    [SerializeField]
    private PlayerDisplayEquipment _playerEquipmentController;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Capacity = _initalCapacity;

        _weaponItems = new Item[_weaponCapacity];
        _armorItems = new Item[_armorCapacity];
        _portionItems = new Item[_portionCapacity];
    }

    private void OnEnable()
    {
        if (_mainInventory.gameObject.activeSelf) // �����ذ�
        {
            UpdateAccessibleStatesAll();
        }
    }

    public void UpdateAccessibleStatesAll()
    {
        _inventoryUI.SetAccessibleSlotRange(Capacity);
    }

    // ���� �ִ� ������ (CountableItemData) Ÿ���� ��ü�� �ε����� ã�� ��ȯ�ϴ� �Լ��Դϴ�. 
    private int FindCountableItemSlotIndex(CountableItemData target, int startIndex = 0)
    {
        // startIndex 
        for (int i = startIndex; i < Capacity; i++)
        {
            //  ���� �ε����� �ش��ϴ� _items �迭�� ��Ҹ� �����ɴϴ�.
            var current = _portionItems[i];

            // ���� �ε����� ��Ұ� �ƹ��͵� ���ٸ� �ݺ����� ��� �����մϴ�.
            if (current == null)
                continue;

            // ���� ������ �����Ϳ� ���� ����Ű�� �ִ� ��� �����۰� ��ġ�ϰ�
            // ���� ������ Ÿ���� �����ִ� ������ Ÿ�� �̶�� �Ʒ������� �����մϴ�.
            if (current.Data == target && current is CountableItem)
            {
                CountableItem ci = (CountableItem)current;

                if (!ci.IsMax)
                    return i;
            }
        }
        return -1;
    }

    public int AddItem(ItemData itemData, int amount = 1)
    {
        int index;
        Item[] itemArray = GetItemArray(itemData.slotType);

        if (itemData is CountableItemData && itemArray == _portionItems)
        {
            CountableItemData ciData = (CountableItemData)itemData;
            bool findNextCountable = true;
            index = -1;

            while (amount > 0)
            {
                // �̹� �ش� �������� �κ��丮 ���� �����ϰ�, ���� ���� �ִ��� �˻�
                if (findNextCountable)
                {
                    index = FindCountableItemSlotIndex(ciData, index + 1);
                    // ���� �����ִ� ������ ������ ���̻� ���ٰ� �Ǵܵ� ���,
                    // �� ���Ժ��� Ž�� ����
                    if (index == -1)
                    {
                        findNextCountable = false;
                    }

                    // ������ ������ ã�� ���, �� ������Ű�� �ʰ��� ���� �� amount�� �ʱ�ȭ
                    else
                    {
                        CountableItem ci = itemArray[index] as CountableItem;
                        amount = ci.AddAmountAndGetExcess(amount);
                        // ���Կ� �߰�
                        itemArray[index] = ci;
                        UpdateSlot(itemData.slotType, index);
                    }
                }

                // 1-2. �� ���� Ž��
                else
                {
                    //  �� ������ ������ ã�Ƽ� �󽽷��� ���� ���� -1�� ��ȯ�Ͽ� �ش� �Լ��� �����Ѵ�.
                    index = FindEmptySlotIndex(itemArray, index + 1);

                    // �� �������� ���� ��� ����
                    if (index == -1)
                    {
                        break;
                    }

                    // �� ���� �߰� ��, ���Կ� ������ �߰� �� �׿��� ���
                    else
                    {
                        // ���ο� ������ ����
                        CountableItem ci = ciData.CreateItem() as CountableItem;
                        ci.SetAmount(amount);

                        // ���Կ� �߰�
                        itemArray[index] = ci;

                        // ���� ���� ���
                        amount = (amount > ciData.MaxAmount) ? (amount - ciData.MaxAmount) : 0;

                        UpdateSlot(itemData.slotType, index);
                    }
                }
            }
        }

        // 2. ������ ���� ������
        else
        {
            if (amount == 1)
            {
                //  �� ������ ������ ã�Ƽ� �󽽷��� ���� ���� -1�� ��ȯ�Ͽ� �ش� �Լ��� �����ϰ� �װ��̾ƴ϶�� ���Կ� �߰��Ѵ�.
                index = FindEmptySlotIndex(itemArray);

                if (index != -1)
                {
                    // �������� �����Ͽ� ���Կ� �߰�
                    itemArray[index] = itemData.CreateItem();
                    amount = 0;
                    UpdateSlot(itemData.slotType, index);
                }
            }

            //  2�� �̻��� ���� ���� �������� ���ÿ� �߰��ϴ� ���
            index = -1;

            for (; amount > 0; amount--)
            {
                // ������ ���� �ε����� ���� �ε������� ���� Ž��
                index = FindEmptySlotIndex(itemArray, index + 1);

                // �� ���� ���� ��� ���� ����
                if (index == -1)
                {
                    break;
                }
                // �������� �����Ͽ� ���Կ� �߰�
                itemArray[index] = itemData.CreateItem();
                UpdateSlot(itemData.slotType, index);
            }
        }
        return amount;
    }

    public void UpdateSlot(ItemSlotType slotType, int index)
    {
        if (!IsValidIndex(index))
            return;

        Item[] itemArray = GetItemArray(slotType); ;
        Item item = itemArray[index];

        // ���Կ� �������� �ִ� ���
        if (item != null)
        {
            _inventoryUI.SetItemIcon(slotType, index, item.Data.IconSprite);

            if (item is CountableItem ci)
            {
                // ������ 0�� ���, ������ ����
                if (ci.IsEmpty)
                {
                    item = null;
                    RemoveIcon();
                    return;
                }
                // ���� �ؽ�Ʈ ǥ��
                else
                {
                    _inventoryUI.SetItemAmountText(slotType, index, ci.Amount);
                }
            }
            //  �� �� ���� �������� ��� ���� �ؽ�Ʈ ����
            else
            {
                _inventoryUI.HideItemAmountText(slotType, index);
            }
        }
        // ���Կ� �������� ���°��
        else
        {
            RemoveIcon();
        }

        // ���� �Լ�
        void RemoveIcon()
        {
            _inventoryUI.RemoveItem(slotType, index);
            _inventoryUI.HideItemAmountText(slotType, index);
        }
    }

    public void Swap(ItemSlotType slotType, int indexA, int indexB)
    {
        if (!IsValidIndex(indexA) || !IsValidIndex(indexB)) return;

        Item[] itemArray = GetItemArray(slotType);


        if (itemArray == null)
            return;

        Item itemA = itemArray[indexA];
        Item itemB = itemArray[indexB];

        if (itemA != null && itemB != null && itemA.Data == itemB.Data &&
            itemA is CountableItem ciA && itemB is CountableItem ciB)
        {
            int maxAmount = ciB.MaxAmount;
            int sum = ciA.Amount + ciB.Amount;

            if (sum <= maxAmount)
            {
                ciA.SetAmount(0);
                ciB.SetAmount(sum);
            }
            else
            {
                ciA.SetAmount(sum - maxAmount);
                ciB.SetAmount(maxAmount);
            }
        }
        else
        {
            // Swap items in the array
            itemArray[indexA] = itemB;
            itemArray[indexB] = itemA;

            // Update slots for the swapped items
            UpdateSlot(slotType, indexA, indexB);
        }
    }
    private void UpdateSlot(ItemSlotType slotType, params int[] indices)
    {
        foreach (var i in indices)
        {
            UpdateSlot(slotType, i);
        }
    }


    // �ε����� ���� ���� ���� �ִ��� �˻� 
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < Capacity;
    }

    private int FindEmptySlotIndex(Item[] items, int startIndex = 0)
    {
        for (int i = startIndex; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                return i;
            }
        }
        return -1;
    }
    public void Use(ItemSlotType slotType, int index)
    {

        Item[] itemArray = GetItemArray(slotType);

        if (itemArray[index] == null)
            return;

        // ��� ������ �������� ���
        if (itemArray[index] is IUsableItem CuItem && itemArray[index] is CountableItem)
        {
            // ������ ���
            bool succeeded = CuItem.Use();

            if (succeeded)
            {
                UpdateSlot(slotType, index);
            }
        }

        if (itemArray[index] is EquipmentItem eqitem)
        {
            if (eqitem.EquipmentData is EquipmentItemData equipmentData)
            {
                equipmentData.AssignItemToPlayer(_playerEquipmentController);
            }
        }
    }

    // �ش� ������ ������ ���� ����
    public ItemData GetItemData(ItemSlotType slotType, int index)
    {
        Item[] itemArray = GetItemArray(slotType);

        if (!IsValidIndex(index)) return null;
        if (itemArray[index] == null) return null;

        return itemArray[index].Data;
    }

    private Item[] GetItemArray(ItemSlotType slotType)
    {
        switch (slotType)
        {
            case ItemSlotType.Portion:
                return _portionItems;
            case ItemSlotType.Weapon:
                return _weaponItems;
            case ItemSlotType.Armor:
                return _armorItems;
            default:
                return null;
        }
    }
}
