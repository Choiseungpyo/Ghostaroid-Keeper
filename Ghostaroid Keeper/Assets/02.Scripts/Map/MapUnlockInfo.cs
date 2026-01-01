using System;
using UnityEngine;

[Serializable]
public class MapUnlockInfo
{
    [field: SerializeField] public int RequiredPlayerLevel { get; private set; } = 1;
}