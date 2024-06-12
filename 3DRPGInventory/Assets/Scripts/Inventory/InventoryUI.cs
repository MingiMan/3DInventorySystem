using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    #region Slot Option
    [Header("Options")]
    [Range(0, 5)]
    [Tooltip("���� ���� ����")]
    [SerializeField] private int _horizontalSlotCount = 5;

    [Range(0, 5)]
    [Tooltip("���� ���� ����")]
    [SerializeField] private int _verticalSlotCount = 5;

    [Tooltip("���԰��� �����¿� ����")]
    [SerializeField] private float _slotMargin = 8f;

    [Tooltip("�κ��丮 ������ ���� ����")]
    [SerializeField] private float _contentAreaPadding = 20f;

    [Range(64, 100)]
    [Tooltip("�� ���� ũ��")]
    [SerializeField] private float _slotSize = 100f;

    [Header("Connected Objects")]

    [Tooltip("���� ��ġ ����")]
    [SerializeField] private RectTransform _contentAreaRT;

    [Tooltip("���� ������")]
    [SerializeField] private GameObject _slotUiPrefab;

    [Tooltip("������ ���� ����")]
    [SerializeField] private ItemToolTip _itemTooltip;

    [Space]
    //  [SerializeField] private bool _showHighlight = true; // ���̶���Ʈ ������ On/Off
    [SerializeField] private bool _showTooltip = true; //  ������ ���� On/Off

    // ������ ������ŭ ���� ���� ���� ���Ե� ���� ����
    // �� ������ ������
    private List<ItemSlotUI> _weaponSlotList = new List<ItemSlotUI>();
    private List<ItemSlotUI> _armorSlotList = new List<ItemSlotUI>();
    private List<ItemSlotUI> _portionSlotList = new List<ItemSlotUI>();


    [SerializeField] public Transform _weaponItemSlot;
    [SerializeField] public Transform _armorItemSlot;
    [SerializeField] public Transform _portionItemSlot;
    #endregion

    #region Variables
    // Unity���� 2D �� 3D UI ��ҿ� ���� ����ĳ������ �����ϴ� ������Ʈ�Դϴ�.
    private GraphicRaycaster _gr;

    // PointerEventData�� ����Ƽ �̺�Ʈ �ý��ۿ��� ���Ǵ� ������ Ŭ������,
    // ������ ���� �̺�Ʈ�� ���� ������ ��� �ֽ��ϴ�.
    private PointerEventData _ped;

    private List<RaycastResult> _rrList;

    private ItemSlotUI _beginDragSlot; // ���� �巡�׸� ������ ����
    private Transform _beginDragIconTransform; // �ش� ������ ������ Ʈ������

    private Vector3 _beginDragIconPoint;   // �巡�� ���� �� ������ ��ġ
    private Vector3 _beginDragCursorPoint; // �巡�� ���� �� Ŀ���� ��ġ
    private int _beginDragSlotSiblingIndex;

    private ItemSlotUI _pointerOverSlot;  // ���� �����Ͱ� ��ġ�� ���� ����

    [SerializeField] Inventory _inventory;

    private List<ItemData> tempItems = new List<ItemData>();
    #endregion



    // �κ��丮�� ���� �ʴ� ���¿��� �������� ���� ��
    // ��� �ӽ÷� �������� �����ѵ� �κ��丮�� Ȱ��ȭ�Ǹ� �Լ��� ȣ���Ѵ�.
    private void OnEnable()
    {
        if (tempItems != null)
        {
            foreach (var item in tempItems)
            {
                _inventory.AddItem(item);
            }
            tempItems.Clear();
        }
    }


    private void Awake()
    {
        Init();
        InitWeaponSlot();
        InitPortionSlot();
        InitArmorSlot();
    }

    private void Update()
    {
        _ped.position = Input.mousePosition;
        OnPointerEnterAndExit();

        if (_showTooltip)
            ShowOrHideItemTooltip();

        OnPointerDown();
        OnPointerUp();
        OnPointerDrag();
    }

    private void Init()
    {
        // GraphicRaycaster ������Ʈ�� ���� GameObject���� ã�Ƽ� _gr ������ �Ҵ��ϴ� �õ��� �մϴ�.
        TryGetComponent(out _gr);

        // _gr �� null�� ���  ���� GameObject�� GraphicRaycaster�� �߰��ϰ�
        //  GraphicRaycaster�� ������Ʈ��  _gr�� �Ҵ��մϴ�.
        if (_gr == null)
            _gr = gameObject.AddComponent<GraphicRaycaster>();

        _ped = new PointerEventData(EventSystem.current);

        // Graphic Raycaster
        _rrList = new List<RaycastResult>(10);
    }

    #region ������ ������ �ʱ�ȭ�ϰ� ��ġ�ϴ� �Լ��Դϴ�.
    void InitItemSlots(ItemSlotType itemType, List<ItemSlotUI> slotList, Transform itemSlotParent)
    {
        // �ʱ�ȭ
        slotList.Clear();
        // ItemSlotUI ������Ʈ�� itemSlot�� �����մϴ�.
        _slotUiPrefab.TryGetComponent(out ItemSlotUI itemSlot);
        if (itemSlot == null)
            _slotUiPrefab.AddComponent<ItemSlotUI>();
        // _slotUiPrefab �� ��Ȱ��ȭ �մϴ�
        _slotUiPrefab.SetActive(false);

        // beginPos = 50 , -50 ���� ��ġ�� �����մϴ�.
        Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);

        // ������ġ�� ���� ��ġ�� �ʱ�ȭ�մϴ�.
        Vector2 curPos = beginPos;

        for (int j = 0; j < _verticalSlotCount; j++)
        {
            for (int i = 0; i < _horizontalSlotCount; i++)
            {
                int slotIndex = (_horizontalSlotCount * j) + i;

                var slotRT = CloneSlot(); // CloneSlot() �Լ� ȣ�� ��  RectTransform�� ��ȯ�մϴ�. 

                slotRT.pivot = new Vector2(0f, 1f); // Left Top
                slotRT.anchoredPosition = curPos;
                // RectTransform(slotRT)�� ���� curPos ������ ���� ����Ͽ�
                // �ش� ������ ��Ŀ��(anchored) ��ġ�� �����մϴ�.

                slotRT.gameObject.SetActive(true); //slotRT �� Ȱ��ȭ�մϴ�.

                slotRT.gameObject.name = $"{itemType} Slot [{slotIndex}]"; ;
                // ������ ���Կ� �̸��� "Item Slot [0]" ���� ���·� �����մϴ�.

                var slotUI = slotRT.GetComponent<ItemSlotUI>();
                // slotRT ������ �ִ� ItemSlotUI �� slotUI�� �����Ѵ�.

                // ItemSlotUI Ŭ������ SetSlotIndex �Լ��� ȣ���Ѵ�. �׸��� ���������� �ε��� ��ȣ�� �ο��޴´�.
                slotUI.SetSlotIndex(slotIndex);

                slotUI.transform.SetParent(itemSlotParent);

                slotUI._slotType = itemType;

                // _slotUIList�� slotUI�� �߰��Ѵ�.
                slotList.Add(slotUI);

                curPos.x += (_slotMargin + _slotSize);
            }
            curPos.x = beginPos.x;
            curPos.y -= (_slotMargin + _slotSize);
        }

        // -- ���� �Լ� --
        RectTransform CloneSlot()
        {
            GameObject slotGo = Instantiate(_slotUiPrefab);
            RectTransform rt = slotGo.GetComponent<RectTransform>();
            rt.SetParent(_contentAreaRT);
            return rt;
        }
    }

    void InitWeaponSlot()
    {
        InitItemSlots(ItemSlotType.Weapon, _weaponSlotList, _weaponItemSlot);
    }

    void InitArmorSlot()
    {
        InitItemSlots(ItemSlotType.Armor, _armorSlotList, _armorItemSlot);
    }
    void InitPortionSlot()
    {
        InitItemSlots(ItemSlotType.Portion, _portionSlotList, _portionItemSlot);
    }
    #endregion

    #region Swap & Drag
    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear(); // _rrList�� ����Ʈ���� �ʱ�ȭ�մϴ�.

        _gr.Raycast(_ped, _rrList); //rrList�� ����ĳ��Ʈ�� �������� �����.

        //���� ���̰� ���� ��ü�� ���ٸ� null�� ��ȯ �Լ� ����
        if (_rrList.Count == 0)
            return null;

        // �ִٸ� ���� ù ��° ���� ��ȯ�Ѵ�.
        return _rrList[0].gameObject.GetComponent<T>();
    }

    public void OnPointerDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ���� Ŭ���� ������ ������ �ְ� �� ���Կ� �������� �������ִٸ� ���ǹ��� ����
            _beginDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

            if (_beginDragSlot != null && _beginDragSlot.HasItem)
            {
                // ��ġ ���, ���� ���
                _beginDragIconTransform = _beginDragSlot.IconRect.transform;
                _beginDragIconPoint = _beginDragIconTransform.position;
                _beginDragCursorPoint = Input.mousePosition;

                // �� ���� ���̱�
                _beginDragSlotSiblingIndex = _beginDragSlot.transform.GetSiblingIndex();
                _beginDragSlot.transform.SetAsLastSibling();

                // �ش� ������ ���̶���Ʈ �̹����� �����ܺ��� �ڿ� ��ġ��Ű��
                _beginDragSlot.SetHighlightOnTop(false);
            }
            else
            {
                _beginDragSlot = null;
            }
        }

        else if (Input.GetMouseButtonDown(1))
        {
            ItemSlotUI slot = RaycastAndGetFirstComponent<ItemSlotUI>();
            if (slot != null && slot.HasItem && slot.IsAccessible)
            {
                TryUseItem(slot._slotType, slot.Index);
            }
        }
    }

    void TryUseItem(ItemSlotType slotType, int index)
    {
        _inventory.Use(slotType, index);
    }

    private void OnPointerDrag()
    {
        if (_beginDragSlot == null)
            return;

        if (Input.GetMouseButton(0))
        {
            // ��ġ �̵�
            _beginDragIconTransform.position =
                _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
        }
    }

    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (_beginDragSlot != null)
            {
                // ��ġ ����
                _beginDragIconTransform.position = _beginDragIconPoint;

                // UI ���� ����
                // �巡�� ���� UI ������ ������ �巡�׸� �����ϱ� ���� ������ �ǵ����� ������ �մϴ�.
                _beginDragSlot.transform.SetSiblingIndex(_beginDragSlotSiblingIndex);

                // �巡�� �Ϸ� ó��
                EndDrag();

                // ���� ����
                _beginDragSlot = null;
                _beginDragIconTransform = null;
            }
        }
    }

    private void EndDrag()
    {
        ItemSlotUI endDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        if (endDragSlot != null && endDragSlot.IsAccessible)
        {
            TrySwapItems(_beginDragSlot, endDragSlot);
        }
    }


    // �� ������ ������ ��ȯ 
    private void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
    {
        // �ش� ������ �����ϴٸ� �Լ��� �����մϴ�.
        if (from == to)
            return;

        from.SwapOrMoveIcon(to);
        _inventory.Swap(from._slotType, from.Index, to.Index); // from�� �ִ� ���� Ÿ�԰� to ����Ÿ���� ���� ������ 
    }

    #endregion


    #region Highlight

    // �� �����Ӹ��� ���� �����ӿ� ���콺�� ��ġ�ߴ� ����,
    // ���� �����ӿ� ���콺�� ��ġ�� ������ Ȯ���Ͽ� ���� ���� ���̶���Ʈ ǥ��/������ �����Ѵ�.
    private void OnPointerEnterAndExit()
    {
        // ���� �������� ����
        var prevSlot = _pointerOverSlot;

        // ���� �������� ����
        var curSlot = _pointerOverSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        if (prevSlot == null)
        {
            // Enter
            if (curSlot != null)
            {
                OnCurrentEnter();
            }
        }
        else
        {
            // Exit
            if (curSlot == null)
            {
                OnPrevExit();
            }

            // Change
            else if (prevSlot != curSlot)
            {
                OnPrevExit();
                OnCurrentEnter();
            }
        }

        // ���� �Լ�
        void OnCurrentEnter()
        {
            curSlot.Highlight(true);
        }
        void OnPrevExit()
        {
            prevSlot.Highlight(false);
        }
    }
    #endregion

    private void SetSlotListState(List<ItemSlotUI> slotList, int accessibleSlotCount)
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].SetSlotAccessibleState(i < accessibleSlotCount);
        }
    }

    public void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        SetSlotListState(_weaponSlotList, accessibleSlotCount);
        SetSlotListState(_armorSlotList, accessibleSlotCount);
        SetSlotListState(_portionSlotList, accessibleSlotCount);
    }

    public void SetItemIcon(ItemSlotType slotType, int index, Sprite icon)
    {
        GetSlot(slotType, index).SetItem(icon);
    }


    //  �ش� ������ ������ ���� �ؽ�Ʈ ����
    public void SetItemAmountText(ItemSlotType slotType, int index, int amount)
    {
        GetSlot(slotType, index).SetItemAmount(amount);
    }

    public void RemoveItem(ItemSlotType slotType, int index)
    {
        GetSlot(slotType, index).RemoveItem();
    }


    public void HideItemAmountText(ItemSlotType slotType, int index)
    {
        GetSlot(slotType, index).SetItemAmount(1);
    }

    private ItemSlotUI GetSlot(ItemSlotType slotType, int index)
    {
        switch (slotType)
        {
            case ItemSlotType.Weapon:
                return _weaponSlotList[index];
            case ItemSlotType.Armor:
                return _armorSlotList[index];
            case ItemSlotType.Portion:
                return _portionSlotList[index];
            default:
                return null;
        }
    }


    public void AddTempItem(ItemData item)
    {
        tempItems.Add(item);
    }

    #region  UIToolTip
    void ShowOrHideItemTooltip()
    {
        // ���콺�� ��ȿ�� ������ ������ ���� �ö�� �ִٸ� ���� �����ֱ�
        bool isValid = _pointerOverSlot != null && _pointerOverSlot.HasItem && _pointerOverSlot.IsAccessible
                         && (_pointerOverSlot != _beginDragSlot);

        if (isValid)
        {
            UpdateTooltipUI(_pointerOverSlot);
            _itemTooltip.Show();
        }
        else
            _itemTooltip.Hide();
    }

    // ���� UI�� ���� ������ ����
    private void UpdateTooltipUI(ItemSlotUI slot)
    {
        if (!slot.IsAccessible || !slot.HasItem)
            return;

        // ���� ���� ����
        _itemTooltip.SetItemInfo(_inventory.GetItemData(slot._slotType, slot.Index));
    }
    #endregion
}
