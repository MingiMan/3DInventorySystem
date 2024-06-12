using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region  아이템 수용한도
    // 아이템 수용 한도
    public int Capacity { get; private set; }

    // 초기 수용 한도 
    [SerializeField, Range(5, 25)]
    private int _initalCapacity = 10;

    // 최대 수용 한도(아이템 배열 크기)
    //[SerializeField, Range(5, 25)]
    //private int _maxCapacity = 25;

    [SerializeField]
    private InventoryUI _inventoryUI; // 연결된 인벤토리 UI

    // Weapon 아이템 목록
    [SerializeField]
    private Item[] _weaponItems;

    // Armor 아이템 목록
    [SerializeField]
    private Item[] _armorItems;

    // Portion 아이템 목록
    [SerializeField]
    private Item[] _portionItems;

    // Weapon 아이템 수용 한도
    [SerializeField, Range(5, 25)]
    private int _weaponCapacity = 10;

    // Armor 아이템 수용 한도
    [SerializeField, Range(5, 25)]
    private int _armorCapacity = 10;

    // Portion 아이템 수용 한도
    [SerializeField, Range(5, 25)]
    private int _portionCapacity = 10;

    [SerializeField]
    // 인벤토리 창
    private GameObject _mainInventory;

    #endregion

    // 플레이어 장착
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
        if (_mainInventory.gameObject.activeSelf) // 버그해결
        {
            UpdateAccessibleStatesAll();
        }
    }

    public void UpdateAccessibleStatesAll()
    {
        _inventoryUI.SetAccessibleSlotRange(Capacity);
    }

    // 셀수 있는 아이템 (CountableItemData) 타입의 객체와 인덱스를 찾아 반환하는 함수입니다. 
    private int FindCountableItemSlotIndex(CountableItemData target, int startIndex = 0)
    {
        // startIndex 
        for (int i = startIndex; i < Capacity; i++)
        {
            //  현재 인덱스에 해당하는 _items 배열의 요소를 가져옵니다.
            var current = _portionItems[i];

            // 현재 인덱스의 요소가 아무것도 없다면 반복문을 계속 진행합니다.
            if (current == null)
                continue;

            // 현재 아이템 데이터와 내가 가르키고 있는 대상에 아이템과 일치하고
            // 현재 아이템 타입이 셀수있는 아이템 타입 이라면 아래문장을 실행합니다.
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
                // 이미 해당 아이템이 인벤토리 내에 존재하고, 개수 여유 있는지 검사
                if (findNextCountable)
                {
                    index = FindCountableItemSlotIndex(ciData, index + 1);
                    // 개수 여유있는 기존재 슬롯이 더이상 없다고 판단될 경우,
                    // 빈 슬롯부터 탐색 시작
                    if (index == -1)
                    {
                        findNextCountable = false;
                    }

                    // 기존재 슬롯을 찾은 경우, 양 증가시키고 초과량 존재 시 amount에 초기화
                    else
                    {
                        CountableItem ci = itemArray[index] as CountableItem;
                        amount = ci.AddAmountAndGetExcess(amount);
                        // 슬롯에 추가
                        itemArray[index] = ci;
                        UpdateSlot(itemData.slotType, index);
                    }
                }

                // 1-2. 빈 슬롯 탐색
                else
                {
                    //  빈 아이템 슬롯을 찾아서 빈슬롯이 없는 경우는 -1를 반환하여 해당 함수를 종료한다.
                    index = FindEmptySlotIndex(itemArray, index + 1);

                    // 빈 슬롯조차 없는 경우 종료
                    if (index == -1)
                    {
                        break;
                    }

                    // 빈 슬롯 발견 시, 슬롯에 아이템 추가 및 잉여량 계산
                    else
                    {
                        // 새로운 아이템 생성
                        CountableItem ci = ciData.CreateItem() as CountableItem;
                        ci.SetAmount(amount);

                        // 슬롯에 추가
                        itemArray[index] = ci;

                        // 남은 개수 계산
                        amount = (amount > ciData.MaxAmount) ? (amount - ciData.MaxAmount) : 0;

                        UpdateSlot(itemData.slotType, index);
                    }
                }
            }
        }

        // 2. 수량이 없는 아이템
        else
        {
            if (amount == 1)
            {
                //  빈 아이템 슬롯을 찾아서 빈슬롯이 없는 경우는 -1를 반환하여 해당 함수를 종료하고 그것이아니라면 슬롯에 추가한다.
                index = FindEmptySlotIndex(itemArray);

                if (index != -1)
                {
                    // 아이템을 생성하여 슬롯에 추가
                    itemArray[index] = itemData.CreateItem();
                    amount = 0;
                    UpdateSlot(itemData.slotType, index);
                }
            }

            //  2개 이상의 수량 없는 아이템을 동시에 추가하는 경우
            index = -1;

            for (; amount > 0; amount--)
            {
                // 아이템 넣은 인덱스의 다음 인덱스부터 슬롯 탐색
                index = FindEmptySlotIndex(itemArray, index + 1);

                // 다 넣지 못한 경우 루프 종료
                if (index == -1)
                {
                    break;
                }
                // 아이템을 생성하여 슬롯에 추가
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

        // 슬롯에 아이템이 있는 경우
        if (item != null)
        {
            _inventoryUI.SetItemIcon(slotType, index, item.Data.IconSprite);

            if (item is CountableItem ci)
            {
                // 수량이 0인 경우, 아이템 제거
                if (ci.IsEmpty)
                {
                    item = null;
                    RemoveIcon();
                    return;
                }
                // 수량 텍스트 표시
                else
                {
                    _inventoryUI.SetItemAmountText(slotType, index, ci.Amount);
                }
            }
            //  셀 수 없는 아이템인 경우 수량 텍스트 제거
            else
            {
                _inventoryUI.HideItemAmountText(slotType, index);
            }
        }
        // 슬롯에 아이템이 없는경우
        else
        {
            RemoveIcon();
        }

        // 로컬 함수
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


    // 인덱스가 수용 범위 내에 있는지 검사 
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

        // 사용 가능한 아이템인 경우
        if (itemArray[index] is IUsableItem CuItem && itemArray[index] is CountableItem)
        {
            // 아이템 사용
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

    // 해당 슬롯의 아이템 정보 리턴
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
