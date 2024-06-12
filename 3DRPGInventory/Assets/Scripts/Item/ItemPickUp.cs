using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] public ItemData item;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

            if (playerInventory != null)
            {
                playerInventory.InItItem(item);
                Destroy(gameObject);
            }
        }
    }
}
