using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToPickUp;

    public void PickupItem(int id)
    {
        bool result = inventoryManager.AddItem(itemsToPickUp[id]);
        if (result) Debug.Log("ITEM ADDED");
        else Debug.Log("No MORE SPACE");
    }

    public void GetSelectedItem()
    {
        Item receievedItem = inventoryManager.GetSelectedItem(false);

        if (receievedItem != null)
        {
            Debug.Log("Received Item: " + receievedItem);
        }
        else
        {
            Debug.Log("No item received");
        }
    }

    public void UseGetSelectedItem()
    {
        Item receievedItem = inventoryManager.GetSelectedItem(true);

        if (receievedItem != null)
        {
            Debug.Log("Used Item: " + receievedItem);
        }
        else
        {
            Debug.Log("No item received");
        }
    }
}
