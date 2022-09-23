using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem
{
    private readonly Dictionary<InventoryItemData, InventoryItem> ItemDic = new();
    public List<InventoryItem> Inventory { get; private set; } = new();

    public void AddItems(InventoryItemData data) {
        if (ItemDic.TryGetValue(data, out var value))
            value.AddToStack();
        else {
            InventoryItem newItem = new(data);
            Inventory.Add(newItem);
            ItemDic.Add(data, newItem);
        }
    }

    public void RemoveItems(InventoryItemData data) {
        if(ItemDic.TryGetValue(data, out var value)) {
            value.RemoveFromStack();

            if(value.stackSize < 1) {
                Inventory.Remove(value);
                ItemDic.Remove(data);
            }
        }
    }

    public InventoryItem GetItem(InventoryItemData data) {
        if (ItemDic.TryGetValue(data, out var value)) {
            return value;
        }
        return null;
    }
}