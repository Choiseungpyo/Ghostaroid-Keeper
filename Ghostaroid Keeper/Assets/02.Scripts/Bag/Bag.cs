using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BagStack
{
    public ProjectileItemData def;
    public int count;
}

public sealed class Bag : MonoBehaviour
{
    [SerializeField] private List<BagStack> stacks = new List<BagStack>();
    [SerializeField] private int selectedIndex = 0;

    public int SlotCount => stacks != null ? stacks.Count : 0;

    public void EnsureSlotCount(int count)
    {
        if (count < 0) count = 0;

        if (stacks == null)
            stacks = new List<BagStack>();

        while (stacks.Count < count)
            stacks.Add(new BagStack { def = null, count = 0 });

        if (selectedIndex >= stacks.Count)
            selectedIndex = Mathf.Max(0, stacks.Count - 1);
    }

    public void AddSlots(int add)
    {
        EnsureSlotCount(SlotCount + Mathf.Max(0, add));
    }

    public bool TryConsumeAt(int slotIndex, int amount)
    {
        if (stacks == null) return false;
        if (slotIndex < 0 || slotIndex >= stacks.Count) return false;
        if (amount <= 0) return false;

        BagStack s = stacks[slotIndex];
        if (s.def == null) return false;
        if (s.count < amount) return false;

        s.count -= amount;
        if (s.count <= 0)
        {
            s.count = 0;
            s.def = null;
        }

        stacks[slotIndex] = s;
        return true;
    }

    public BagStack GetStack(int index)
    {
        if (stacks == null) return default;
        if (index < 0 || index >= stacks.Count) return default;
        return stacks[index];
    }

    public void Select(int index)
    {
        if (stacks == null) return;
        if (index < 0 || index >= stacks.Count) return;
        selectedIndex = index;
    }
}