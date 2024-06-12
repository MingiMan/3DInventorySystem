using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField]
    private Text _titleText;   // ������ �̸� �ؽ�Ʈ

    [SerializeField]
    private Text _contentText; // ������ ���� �ؽ�Ʈ

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);


    // ���콺�� ���ٴ�� data �̸��� �������� �Ѱ� �޴� �Լ��Դϴ�.
    public void SetItemInfo(ItemData data)
    {
        _titleText.text = data.Name;
        _contentText.text = data.Tooltip;
    }
}
