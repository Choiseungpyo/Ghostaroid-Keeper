using System;
using UnityEngine;

[Serializable]
public class RewardInfo
{
    [field: SerializeField] public int AffectionMilestone { get; private set; }
    [field: SerializeField] public int ItemId { get; private set; }
    [field: SerializeField] public int Amount { get; private set; } = 1;
}