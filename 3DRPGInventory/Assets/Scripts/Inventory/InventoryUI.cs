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
    [Tooltip("슬롯 가로 갯수")]
    [SerializeField] private int _horizontalSlotCount = 5;

    [Range(0, 5)]
    [Tooltip("슬롯 세로 갯수")]
    [SerializeField] private int _verticalSlotCount = 5;

    [Tooltip("슬롯간에 상하좌우 여백")]
    [SerializeField] private float _slotMargin = 8f;

    [Tooltip("인벤토리 영역의 내부 여백")]
    [SerializeField] private float _contentAreaPadding = 20f;

    [Range(64, 100)]
    [Tooltip("각 슬롯 크기")]
    [SerializeField] private float _slotSize = 100f;

    [Header("Connected Objects")]

    [Tooltip("슬롯 위치 영역")]
    [SerializeField] private RectTransform _contentAreaRT;

    [Tooltip("슬롯 프리팹")]
    [SerializeField] private GameObject _slotUiPrefab;

    [Tooltip("아이템 정보 툴팁")]
    [SerializeField] private ItemToolTip _itemTooltip;

    [Space]
    //  [SerializeField] private bool _showHighlight = true; // 하이라이트 슬롯을 On/Off
    [SerializeField] private bool _showTooltip = true; //  아이템 설명 On/Off

    // 지정된 개수만큼 슬롯 영역 내에 슬롯들 동적 생성
    // 각 아이템 종류들
    private List<ItemSlotUI> _weaponSlotList = new List<ItemSlotUI>();
    private List<ItemSlotUI> _armorSlotList = new List<ItemSlotUI>();
    private List<ItemSlotUI> _portionSlotList = new List<ItemSlotUI>();


    [SerializeField] public Transform _weaponItemSlot;
    [SerializeField] public Transform _armorItemSlot;
    [SerializeField] public Transform _portionItemSlot;
    #endregion

    #region Variables
    // Unity에서 2D 및 3D UI 요소에 대한 레이캐스팅을 수행하는 컴포넌트입니다.
    private GraphicRaycaster _gr;

    // PointerEventData는 유니티 이벤트 시스템에서 사용되는 데이터 클래스로,
    // 포인터 관련 이벤트에 대한 정보를 담고 있습니다.
    private PointerEventData _ped;

    private List<RaycastResult> _rrList;

    private ItemSlotUI _beginDragSlot; // 현재 드래그를 시작한 슬롯
    private Transform _beginDragIconTransform; // 해당 슬롯의 아이콘 트랜스폼

    private Vector3 _beginDragIconPoint;   // 드래그 시작 시 슬롯의 위치
    private Vector3 _beginDragCursorPoint; // 드래그 시작 시 커서의 위치
    private int _beginDragSlotSiblingIndex;

    private ItemSlotUI _pointerOverSlot;  // 현재 포인터가 위치한 곳의 슬롯

    [SerializeField] Inventory _inventory;

    private List<ItemData> tempItems = new List<ItemData>();
    #endregion



    // 인벤토리를 열지 않는 상태에서 아이템을 먹을 때
    // 잠시 임시로 아이템을 저장한뒤 인벤토리가 활성화되면 함수를 호출한다.
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
        // GraphicRaycaster 컴포넌트를 현재 GameObject에서 찾아서 _gr 변수에 할당하는 시도를 합니다.
        TryGetComponent(out _gr);

        // _gr 이 null인 경우  현재 GameObject에 GraphicRaycaster를 추가하고
        //  GraphicRaycaster를 컴포넌트를  _gr에 할당합니다.
        if (_gr == null)
            _gr = gameObject.AddComponent<GraphicRaycaster>();

        _ped = new PointerEventData(EventSystem.current);

        // Graphic Raycaster
        _rrList = new List<RaycastResult>(10);
    }

    #region 아이템 슬롯을 초기화하고 배치하는 함수입니다.
    void InitItemSlots(ItemSlotType itemType, List<ItemSlotUI> slotList, Transform itemSlotParent)
    {
        // 초기화
        slotList.Clear();
        // ItemSlotUI 컴포넌트를 itemSlot에 참조합니다.
        _slotUiPrefab.TryGetComponent(out ItemSlotUI itemSlot);
        if (itemSlot == null)
            _slotUiPrefab.AddComponent<ItemSlotUI>();
        // _slotUiPrefab 를 비활성화 합니다
        _slotUiPrefab.SetActive(false);

        // beginPos = 50 , -50 시작 위치를 설정합니다.
        Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);

        // 시작위치를 현재 위치에 초기화합니다.
        Vector2 curPos = beginPos;

        for (int j = 0; j < _verticalSlotCount; j++)
        {
            for (int i = 0; i < _horizontalSlotCount; i++)
            {
                int slotIndex = (_horizontalSlotCount * j) + i;

                var slotRT = CloneSlot(); // CloneSlot() 함수 호출 후  RectTransform를 반환합니다. 

                slotRT.pivot = new Vector2(0f, 1f); // Left Top
                slotRT.anchoredPosition = curPos;
                // RectTransform(slotRT)에 대해 curPos 변수의 값을 사용하여
                // 해당 슬롯의 앵커된(anchored) 위치를 설정합니다.

                slotRT.gameObject.SetActive(true); //slotRT 을 활성화합니다.

                slotRT.gameObject.name = $"{itemType} Slot [{slotIndex}]"; ;
                // 아이템 슬롯에 이름은 "Item Slot [0]" 같은 형태로 지정합니다.

                var slotUI = slotRT.GetComponent<ItemSlotUI>();
                // slotRT 가지고 있는 ItemSlotUI 를 slotUI에 참조한다.

                // ItemSlotUI 클래스에 SetSlotIndex 함수를 호출한다. 그리고 순차적으로 인덱스 번호를 부여받는다.
                slotUI.SetSlotIndex(slotIndex);

                slotUI.transform.SetParent(itemSlotParent);

                slotUI._slotType = itemType;

                // _slotUIList를 slotUI를 추가한다.
                slotList.Add(slotUI);

                curPos.x += (_slotMargin + _slotSize);
            }
            curPos.x = beginPos.x;
            curPos.y -= (_slotMargin + _slotSize);
        }

        // -- 지역 함수 --
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
        _rrList.Clear(); // _rrList에 리스트들을 초기화합니다.

        _gr.Raycast(_ped, _rrList); //rrList에 레이캐스트에 정보들이 저장됨.

        //만약 레이가 닿은 물체가 없다면 null을 반환 함수 종료
        if (_rrList.Count == 0)
            return null;

        // 있다면 가장 첫 번째 꺼를 반환한다.
        return _rrList[0].gameObject.GetComponent<T>();
    }

    public void OnPointerDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 내가 클릭한 아이템 슬롯이 있고 그 슬롯에 아이템을 가지고있다면 조건문을 실행
            _beginDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

            if (_beginDragSlot != null && _beginDragSlot.HasItem)
            {
                // 위치 기억, 참조 등록
                _beginDragIconTransform = _beginDragSlot.IconRect.transform;
                _beginDragIconPoint = _beginDragIconTransform.position;
                _beginDragCursorPoint = Input.mousePosition;

                // 맨 위에 보이기
                _beginDragSlotSiblingIndex = _beginDragSlot.transform.GetSiblingIndex();
                _beginDragSlot.transform.SetAsLastSibling();

                // 해당 슬롯의 하이라이트 이미지를 아이콘보다 뒤에 위치시키기
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
            // 위치 이동
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
                // 위치 복원
                _beginDragIconTransform.position = _beginDragIconPoint;

                // UI 순서 복원
                // 드래그 중인 UI 슬롯의 순서를 드래그를 시작하기 전의 순서로 되돌리는 역할을 합니다.
                _beginDragSlot.transform.SetSiblingIndex(_beginDragSlotSiblingIndex);

                // 드래그 완료 처리
                EndDrag();

                // 참조 제거
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


    // 두 슬롯의 아이템 교환 
    private void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
    {
        // 해당 슬롯이 동일하다면 함수를 종료합니다.
        if (from == to)
            return;

        from.SwapOrMoveIcon(to);
        _inventory.Swap(from._slotType, from.Index, to.Index); // from에 있는 슬롯 타입과 to 슬롯타입은 같기 때문에 
    }

    #endregion


    #region Highlight

    // 매 프레임마다 이전 프레임에 마우스가 위치했던 슬롯,
    // 현재 프레임에 마우스가 위치한 슬롯을 확인하여 위와 같이 하이라이트 표시/해제를 구현한다.
    private void OnPointerEnterAndExit()
    {
        // 이전 프레임의 슬롯
        var prevSlot = _pointerOverSlot;

        // 현재 프레임의 슬롯
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

        // 지역 함수
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


    //  해당 슬롯의 아이템 개수 텍스트 지정
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
        // 마우스가 유효한 아이템 아이콘 위에 올라와 있다면 툴팁 보여주기
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

    // 툴팁 UI의 슬롯 데이터 갱신
    private void UpdateTooltipUI(ItemSlotUI slot)
    {
        if (!slot.IsAccessible || !slot.HasItem)
            return;

        // 툴팁 정보 갱신
        _itemTooltip.SetItemInfo(_inventory.GetItemData(slot._slotType, slot.Index));
    }
    #endregion
}
