using UnityEngine;

public class PlayerDisplayEquipment : MonoBehaviour
{
    [Header("Anchor")]
    [SerializeField] Transform leftHandAnchor;
    [SerializeField] Transform rightHandAnchor;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] MeeleFighter meeleFighter;

    GameObject currentLeftHandObj;
    GameObject currentRightHandObj;

    Animator animor;

    private void Awake()
    {
        animor = GetComponent<Animator>();
    }

    public void AssignHandItem(EquipmentItemData item)
    {
        if (item is WeaponItemData weaponItem)
        {
            switch (weaponItem.hand)
            {
                case Hand.LEFT:
                    // ���� ����ִ� �������� �����ϰ� �� ���������� �Ҵ�޴´�.
                    DestoryIfNotNull(currentLeftHandObj);
                    currentLeftHandObj = CreateNewItemInstance(weaponItem, leftHandAnchor);
                    meeleFighter.GetLeftWeapon(currentLeftHandObj);
                    break;

                case Hand.RIGHT:
                    DestoryIfNotNull(currentRightHandObj);
                    currentRightHandObj = CreateNewItemInstance(weaponItem, rightHandAnchor);
                    meeleFighter.GetRightWeapon(currentRightHandObj);
                    break;
            }
            meeleFighter.GetAttackData(weaponItem.attackData);
            animor.SetInteger("Weapon", (int)weaponItem.type);
        }
    }

    private void DestoryIfNotNull(GameObject obj)
    {
        if (obj)
            Destroy(obj);
    }

    GameObject CreateNewItemInstance(WeaponItemData item, Transform anchor)
    {
        var itemInstance = Instantiate(item.GetPrefab(), anchor);
        itemInstance.transform.localPosition = item.ItemLocalPosition;
        itemInstance.transform.localRotation = item.GetLocalRotation();
        return itemInstance;
    }
}
