using System.Collections.Generic;
using System;

[Serializable]
public class GhostOwnedSaveData
{
    public int ghostId;

    public bool discovered;
    public bool owned;

    public int affection;

    public List<int> claimedMilestones = new List<int>(8);
}