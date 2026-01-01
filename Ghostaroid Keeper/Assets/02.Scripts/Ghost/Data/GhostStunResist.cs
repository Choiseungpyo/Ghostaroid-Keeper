using System;
using UnityEngine;

[Serializable]
public class GhostStunResist
{
    [field: SerializeField] public int ImmuneHits { get; private set; } = 0;
    [field: SerializeField] public float StunDurationMultiplier { get; private set; } = 1f;
}