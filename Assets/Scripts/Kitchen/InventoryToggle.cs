using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel; 

    public void ToggleInventory()
    {
        if (inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
            Debug.Log("inventory is close");
        }
        else
        {
            inventoryPanel.SetActive(true);
            Debug.Log("inventory is open");
        }
    }
}
