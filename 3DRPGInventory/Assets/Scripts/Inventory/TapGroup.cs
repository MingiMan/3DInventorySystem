using System.Collections.Generic;
using UnityEngine;

public class TapGroup : MonoBehaviour
{
    public List<TabBtn> tabBtn;
    private TabBtn selectedTab;

    private void OnEnable()
    {
        TabBtn tempTab = selectedTab;

        for (int i = tabBtn.Count - 1; i >= 0; i--)
            OnTabSelected(tabBtn[i]);

        if (tempTab != null)
            OnTabSelected(tempTab);
    }

    public void SubScribe(TabBtn btn)
    {
        if (tabBtn == null)
        {
            tabBtn = new List<TabBtn>();
        }
        tabBtn.Add(btn);
    }

    public void ResetSlotTab(ItemSlotType slotType, TabBtn btn)
    {
        switch (slotType)
        {
            case ItemSlotType.Weapon: // Weapon 탭을 기본으로 설정한다.
                selectedTab = btn;
                selectedTab.BackGround.color = Color.white;
                selectedTab.ItemName.gameObject.SetActive(true);
                selectedTab.ItemSlotsTypeParents.gameObject.SetActive(true);
                return; // 함수를 종료한다.
            default:
                break;
        }
        // 그 외에 탭들은 off한다.
        btn.BackGround.color = Color.gray;
        btn.ItemName.gameObject.SetActive(false);
        btn.ItemSlotsTypeParents.gameObject.SetActive(false);
    }


    public void OnTabEnter(TabBtn btn)
    {
        ReSetTabs();
        if (selectedTab == null || btn != selectedTab)
        {
            btn.BackGround.color = Color.gray;
        }
    }

    public void OnTabSelected(TabBtn btn)
    {
        if (selectedTab != null)
        {
            selectedTab.ItemName.gameObject.SetActive(false);
            selectedTab.ItemSlotsTypeParents.gameObject.SetActive(false);
        }
        selectedTab = btn;
        selectedTab.ItemName.gameObject.SetActive(true);
        selectedTab.ItemSlotsTypeParents.gameObject.SetActive(true);
        ReSetTabs();
        btn.BackGround.color = Color.white;
    }


    public void OnTabExit(TabBtn btn)
    {
        ReSetTabs();
    }

    public void ReSetTabs()
    {
        foreach (TabBtn btn in tabBtn)
        {
            if (selectedTab != null && btn == selectedTab)
                continue;

            btn.BackGround.color = Color.gray;
        }
    }
}
