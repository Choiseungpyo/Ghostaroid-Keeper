using System;
using UnityEngine;

[Serializable]
public class GhostSpawnInfo
{
    [field: SerializeField] public MapType MapType { get; private set; }
    [field: SerializeField] public string AreaTag { get; private set; }
    [field: SerializeField] public GhostSpawnCondition Condition { get; private set; } = GhostSpawnCondition.Always;
    [field: SerializeField] public float Weight { get; private set; } = 1f;
}