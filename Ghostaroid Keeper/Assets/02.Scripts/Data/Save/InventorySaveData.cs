using System.Collections.Generic;
using System;

[Serializable]
public class InventorySaveData
{
    public List<ItemStackSaveData> storage = new List<ItemStackSaveData>(64);

    public List<int> bagSlots = new List<int>(8);
}