using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TabBtn : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private ItemSlotType _slotType;
    [SerializeField] private Transform _itemSlotsTypeParents;
    private TapGroup _tabGroup;
    private Text _itemName;
    private Image _backGround;

    public TapGroup TabGroup => _tabGroup;
    public Transform ItemSlotsTypeParents => _itemSlotsTypeParents;
    public Text ItemName => _itemName;
    public Image BackGround => _backGround;

    private void Start()
    {
        _itemName = GetComponentInChildren<Text>();
        _tabGroup = GetComponentInParent<TapGroup>();
        _backGround = GetComponent<Image>();
        _tabGroup.SubScribe(this);
        _tabGroup.ResetSlotTab(_slotType,this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tabGroup.OnTabExit(this);
    }
}
