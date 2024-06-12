using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    Inventory _inventory;
    [SerializeField]
    GameObject _inventoryUI;


    public void InItItem(ItemData item)
    {
        if (_inventoryUI.gameObject.activeSelf)
        {
            _inventory.AddItem(item);
        }
        else
        {
            InventoryUI inventoryUi = _inventoryUI.GetComponent<InventoryUI>();
            inventoryUi.AddTempItem(item);
        }
    }
}
