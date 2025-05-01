// PlayerInventory.cs
using System.Collections.Generic;

public static class PlayerInventory
{
    public static Dictionary<ItemSO, int> ownedItems = new Dictionary<ItemSO, int>();

    public static bool HasItem(ItemSO item) => ownedItems.ContainsKey(item) && ownedItems[item] > 0;

    public static int GetCount(ItemSO item) => ownedItems.TryGetValue(item, out int count) ? count : 0;

    public static void AddItem(ItemSO item, int count = 1)
    {
        if (!ownedItems.ContainsKey(item))
            ownedItems[item] = 0;
        ownedItems[item] += count;
    }

    public static bool UseItem(ItemSO item, int count = 1)
    {
        if (!HasItem(item) || ownedItems[item] < count)
            return false;

        ownedItems[item] -= count;
        return true;
    }
}