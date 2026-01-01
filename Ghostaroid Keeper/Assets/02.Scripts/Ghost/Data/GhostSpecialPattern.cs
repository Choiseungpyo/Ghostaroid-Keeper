using System;
using UnityEngine;

[Serializable]
public class GhostSpecialPattern
{
    [field: SerializeField] public GhostSpecialPatternType Type { get; private set; } = GhostSpecialPatternType.None;

    [field: SerializeField] public float Chance { get; private set; } = 1f;
    [field: SerializeField] public float Cooldown { get; private set; } = 5f;

    [field: SerializeField] public int Count { get; private set; } = 1;
    [field: SerializeField] public float Duration { get; private set; } = 1.5f;
}