using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupContainer : MonoBehaviour
{
    public InventoryItemData data;

    public void OnPickupItem(InventorySystem SystemToAddTo) {
        SystemToAddTo.AddItems(data);
    }
}