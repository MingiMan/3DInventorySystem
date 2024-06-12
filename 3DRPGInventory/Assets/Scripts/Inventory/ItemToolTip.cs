using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField]
    private Text _titleText;   // 아이템 이름 텍스트

    [SerializeField]
    private Text _contentText; // 아이템 설명 텍스트

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);


    // 마우스를 갖다대면 data 이름과 정보들을 넘겨 받는 함수입니다.
    public void SetItemInfo(ItemData data)
    {
        _titleText.text = data.Name;
        _contentText.text = data.Tooltip;
    }
}
