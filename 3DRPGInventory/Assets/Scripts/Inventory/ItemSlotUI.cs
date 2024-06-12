using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    #region Option Fields
    [Tooltip("아이템 아이콘 이미지")]
    [SerializeField] private Image _iconImage;

    [Tooltip("아이템 개수 텍스트")]
    [SerializeField] private Text _amountText;

    [Tooltip("슬롯이 포커스될 때 나타나는 하이라이트 이미지")]
    [SerializeField] private Image _highlightImage;

    [Space]
    [Tooltip("하이라이트 이미지 알파 값")]
    [SerializeField] private float _highlightAlpha = 0.5f;

    [Tooltip("하이라이트 소요 시간")]
    [SerializeField] private float _highlightFadeDuration = 0.2f;

    #endregion

    #region Properties && References
    // RectTransform 속성으로 슬롯의 RectTransform을 반환합니다.
    public RectTransform SlotRect => _slotRect;
    // RectTransform 속성으로 아이콘의 RectTransform을 반환합니다.
    public RectTransform IconRect => _iconRect;

    // 각 슬롯에 인덱스 번호를 부여
    public int Index { get; private set; }
    // 슬롯에 인덱스를 설정하는 함수
    public void SetSlotIndex(int index) => Index = index;

    // InventoryUI를 참조
    private InventoryUI _inventoryUI;

    // _slotImage를 참조
    private Image _slotImage;

    // 슬롯이 아이템을 보유하고 있는지 여부
    public bool HasItem => _iconImage.sprite != null;

    // 접근 가능한 슬롯인지 여부
    public bool IsAccessible => _isAccessibleSlot && _isAccessibleItem;

    #endregion

    #region ColorConstants
    // 비활성화된 슬롯의 색상 
    private static readonly Color InaccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    //  비활성화된 아이콘 색상
    private static readonly Color InaccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    #endregion

    #region Show/Hide Methods
    // 아이콘과 텍스트를 보여주거나 숨기기 위한 함수들
    private void ShowIcon() => _iconGo.SetActive(true);
    private void HideIcon() => _iconGo.SetActive(false);
    private void ShowText() => _textGo.SetActive(true);
    private void HideText() => _textGo.SetActive(false);

    #endregion

    #region GameObject Variables
    private GameObject _iconGo;
    private GameObject _textGo;
    private GameObject _highlightGo;
    #endregion

    #region Highlight and Accessibility Variables
    // 현재 하이라이트 알파값
    private float _currentHLAlpha = 0f;

    // 슬롯과 아이템의 접근 가능 여부를 나타내는 불린 변수들
    private bool _isAccessibleSlot = true; // 슬롯 접근가능 여부
    private bool _isAccessibleItem = true; // 아이템 접근가능 여부

    #endregion

    #region UI Variables
    private RectTransform _slotRect;
    private RectTransform _iconRect;
    private RectTransform _highlightRect;
    #endregion

    public ItemSlotType _slotType;

    private void OnEnable()
    {
        InitComponents(); // ItemSlotUi 클래스의 여러 변수들을 초기화 합니다.
    }

    private void InitComponents()
    {
        _inventoryUI = GetComponentInParent<InventoryUI>();

        // Rects
        _slotRect = GetComponent<RectTransform>();
        _iconRect = _iconImage.rectTransform;
        _highlightRect = _highlightImage.rectTransform;

        // Game Objects
        _iconGo = _iconRect.gameObject;
        _textGo = _amountText.gameObject;
        _highlightGo = _highlightImage.gameObject;

        // Images
        _slotImage = GetComponent<Image>();
    }

    public void SetSlotAccessibleState(bool value)
    {
        // 현재 슬롯의 접근 가능 상태가 변경되지 않았다면,
        // 함수를 종료하여 중복처리를 피해합니다.

        if (_isAccessibleSlot == value)
            return;

        // 만약 true 라면 슬롯에 이미지를 검정색으로 설정합니다.
        if (value)
        {
            _slotImage.color = Color.black;
            ShowIcon(); // 슬롯 안에 아이템 이미지를 보이게 합니다.
            ShowText(); // 슬롯 안에 아이템에 수량을 보이게 합니다.
        }

        // false라면 아이콘과 텍스트를 숨깁니다.
        else
        {
            _slotImage.color = InaccessibleSlotColor;
            HideIcon();
            HideText();
        }

        // _isAccessibleSlot 상태를 value상태로 업데이트합니다.
        _isAccessibleSlot = value;
    }


    //  다른 슬롯과 아이템 아이콘 교환
    public void SwapOrMoveIcon(ItemSlotUI other)
    {
        if (other == null)
        {
            return;
        }

        // 자기 자신과 교환 불가
        if (other == this)
        {
            return;
        }

        if (!this.IsAccessible)
        {
            return;
        }

        if (!other.IsAccessible)
        {
            return;
        }

        var temp = _iconImage.sprite;

        // 1. 대상에 아이템이 있는 경우 : 교환
        if (other.HasItem)
        {
            SetItem(other._iconImage.sprite);
        }
        // 2. 없는 경우 : 이동
        else
        {
            RemoveItem();
        }

        other.SetItem(temp);
    }
    // 슬롯에 아이템 등록
    public void SetItem(Sprite itemSprite)
    {
        if (itemSprite != null)
        {
            _iconImage.sprite = itemSprite;
            _iconImage.color = Color.white;
            ShowIcon();
        }
        else
        {
            RemoveItem();
        }
    }

    // 슬롯에서 아이템 제거
    public void RemoveItem()
    {
        _iconImage.sprite = null;
        HideIcon();
        HideText();
    }
    public void SetItemAmount(int amount)
    {
        if (HasItem && amount > 1)
            ShowText();
        else
            HideText();

        _amountText.text = amount.ToString();
    }

    #region Slot Highlight

    public void SetHighlightOnTop(bool value)
    {
        // 참이라면 _highlightRect(하이라이트) 부분을 최상위로 보이게 하고
        if (value)
            _highlightRect.SetAsLastSibling();
        // 그것이 아니라면 최하위로 보이게 한다.
        else
            _highlightRect.SetAsFirstSibling();
    }

    //  슬롯에 하이라이트 표시/해제 
    public void Highlight(bool show)
    {
        if (show)
            StartCoroutine(nameof(HighlightFadeInRoutine));
        else
            StartCoroutine(nameof(HighlightFadeOutRoutine));
    }

    // 하이라이트 알파값 서서히 증가
    private IEnumerator HighlightFadeInRoutine()
    {
        StopCoroutine(nameof(HighlightFadeOutRoutine));
        _highlightGo.SetActive(true);

        float unit = _highlightAlpha / _highlightFadeDuration;

        for (; _currentHLAlpha <= _highlightAlpha; _currentHLAlpha += unit * Time.deltaTime)
        {
            _highlightImage.color = new Color(
                _highlightImage.color.r,
                _highlightImage.color.g,
                _highlightImage.color.b,
                _currentHLAlpha
            );

            yield return null;
        }
    }

    // 하이라이트 알파값 0%까지 서서히 감소 
    private IEnumerator HighlightFadeOutRoutine()
    {
        StopCoroutine(nameof(HighlightFadeInRoutine));

        float unit = _highlightAlpha / _highlightFadeDuration;

        for (; _currentHLAlpha >= 0f; _currentHLAlpha -= unit * Time.deltaTime)
        {
            _highlightImage.color = new Color(
                _highlightImage.color.r,
                _highlightImage.color.g,
                _highlightImage.color.b,
                _currentHLAlpha
            );

            yield return null;
        }

        _highlightGo.SetActive(false);
    }

    #endregion

}
