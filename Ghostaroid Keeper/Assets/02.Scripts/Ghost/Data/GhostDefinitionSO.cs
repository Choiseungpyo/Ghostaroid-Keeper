using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ghost/GhostDefinition")]
public class GhostDefinitionSO : ScriptableObject
{
    [field: Header("Identity")]
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }

    [field: Header("Prefab")]
    [field: SerializeField] public GhostBase Prefab { get; private set; }
    [field: SerializeField] public GhostRarity Rarity { get; private set; }

    [field: Header("Codex")]
    [field: SerializeField] public string DiscoveryHint { get; private set; }

    [field: Header("Spawn")]
    [field: SerializeField] public List<GhostSpawnInfo> Spawns { get; private set; } = new List<GhostSpawnInfo>();

    [field: Header("Patterns")]
    [field: SerializeField] public GhostMovePattern MovePattern { get; private set; } = new GhostMovePattern();
    [field: SerializeField] public GhostSpecialPattern SpecialPattern { get; private set; } = new GhostSpecialPattern();

    [field: Header("Combat Rules")]
    [field: SerializeField] public GhostStunResist StunResist { get; private set; } = new GhostStunResist();

    [field: Header("Growth Rewards")]
    [field: SerializeField] public List<RewardInfo> GrowthRewards { get; private set; } = new List<RewardInfo>();
}