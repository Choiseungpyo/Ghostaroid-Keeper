using System;
using UnityEngine;

[Serializable]
public class GhostMovePattern
{
    [field: SerializeField] public GhostMovePatternType Type { get; private set; }

    [field: SerializeField] public float MoveSpeed { get; private set; } = 1f;
    [field: SerializeField] public float TurnInterval { get; private set; } = 1f;

    [field: SerializeField] public float Radius { get; private set; } = 2f;
    [field: SerializeField] public float DashDistance { get; private set; } = 3f;
    [field: SerializeField] public float Cooldown { get; private set; } = 2f;
}