using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    #region Option Fields
    [Tooltip("������ ������ �̹���")]
    [SerializeField] private Image _iconImage;

    [Tooltip("������ ���� �ؽ�Ʈ")]
    [SerializeField] private Text _amountText;

    [Tooltip("������ ��Ŀ���� �� ��Ÿ���� ���̶���Ʈ �̹���")]
    [SerializeField] private Image _highlightImage;

    [Space]
    [Tooltip("���̶���Ʈ �̹��� ���� ��")]
    [SerializeField] private float _highlightAlpha = 0.5f;

    [Tooltip("���̶���Ʈ �ҿ� �ð�")]
    [SerializeField] private float _highlightFadeDuration = 0.2f;

    #endregion

    #region Properties && References
    // RectTransform �Ӽ����� ������ RectTransform�� ��ȯ�մϴ�.
    public RectTransform SlotRect => _slotRect;
    // RectTransform �Ӽ����� �������� RectTransform�� ��ȯ�մϴ�.
    public RectTransform IconRect => _iconRect;

    // �� ���Կ� �ε��� ��ȣ�� �ο�
    public int Index { get; private set; }
    // ���Կ� �ε����� �����ϴ� �Լ�
    public void SetSlotIndex(int index) => Index = index;

    // InventoryUI�� ����
    private InventoryUI _inventoryUI;

    // _slotImage�� ����
    private Image _slotImage;

    // ������ �������� �����ϰ� �ִ��� ����
    public bool HasItem => _iconImage.sprite != null;

    // ���� ������ �������� ����
    public bool IsAccessible => _isAccessibleSlot && _isAccessibleItem;

    #endregion

    #region ColorConstants
    // ��Ȱ��ȭ�� ������ ���� 
    private static readonly Color InaccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    //  ��Ȱ��ȭ�� ������ ����
    private static readonly Color InaccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    #endregion

    #region Show/Hide Methods
    // �����ܰ� �ؽ�Ʈ�� �����ְų� ����� ���� �Լ���
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
    // ���� ���̶���Ʈ ���İ�
    private float _currentHLAlpha = 0f;

    // ���԰� �������� ���� ���� ���θ� ��Ÿ���� �Ҹ� ������
    private bool _isAccessibleSlot = true; // ���� ���ٰ��� ����
    private bool _isAccessibleItem = true; // ������ ���ٰ��� ����

    #endregion

    #region UI Variables
    private RectTransform _slotRect;
    private RectTransform _iconRect;
    private RectTransform _highlightRect;
    #endregion

    public ItemSlotType _slotType;

    private void OnEnable()
    {
        InitComponents(); // ItemSlotUi Ŭ������ ���� �������� �ʱ�ȭ �մϴ�.
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
        // ���� ������ ���� ���� ���°� ������� �ʾҴٸ�,
        // �Լ��� �����Ͽ� �ߺ�ó���� �����մϴ�.

        if (_isAccessibleSlot == value)
            return;

        // ���� true ��� ���Կ� �̹����� ���������� �����մϴ�.
        if (value)
        {
            _slotImage.color = Color.black;
            ShowIcon(); // ���� �ȿ� ������ �̹����� ���̰� �մϴ�.
            ShowText(); // ���� �ȿ� �����ۿ� ������ ���̰� �մϴ�.
        }

        // false��� �����ܰ� �ؽ�Ʈ�� ����ϴ�.
        else
        {
            _slotImage.color = InaccessibleSlotColor;
            HideIcon();
            HideText();
        }

        // _isAccessibleSlot ���¸� value���·� ������Ʈ�մϴ�.
        _isAccessibleSlot = value;
    }


    //  �ٸ� ���԰� ������ ������ ��ȯ
    public void SwapOrMoveIcon(ItemSlotUI other)
    {
        if (other == null)
        {
            return;
        }

        // �ڱ� �ڽŰ� ��ȯ �Ұ�
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

        // 1. ��� �������� �ִ� ��� : ��ȯ
        if (other.HasItem)
        {
            SetItem(other._iconImage.sprite);
        }
        // 2. ���� ��� : �̵�
        else
        {
            RemoveItem();
        }

        other.SetItem(temp);
    }
    // ���Կ� ������ ���
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

    // ���Կ��� ������ ����
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
        // ���̶�� _highlightRect(���̶���Ʈ) �κ��� �ֻ����� ���̰� �ϰ�
        if (value)
            _highlightRect.SetAsLastSibling();
        // �װ��� �ƴ϶�� �������� ���̰� �Ѵ�.
        else
            _highlightRect.SetAsFirstSibling();
    }

    //  ���Կ� ���̶���Ʈ ǥ��/���� 
    public void Highlight(bool show)
    {
        if (show)
            StartCoroutine(nameof(HighlightFadeInRoutine));
        else
            StartCoroutine(nameof(HighlightFadeOutRoutine));
    }

    // ���̶���Ʈ ���İ� ������ ����
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

    // ���̶���Ʈ ���İ� 0%���� ������ ���� 
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
