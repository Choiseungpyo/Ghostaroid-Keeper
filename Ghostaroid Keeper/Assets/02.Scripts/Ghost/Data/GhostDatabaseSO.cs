using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ghost/GhostDatabase")]
public class GhostDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<GhostDefinitionSO> List { get; private set; } = new List<GhostDefinitionSO>();

    private readonly Dictionary<int, GhostDefinitionSO> byId = new Dictionary<int, GhostDefinitionSO>();

    private struct SpawnCandidate
    {
        public int ghostId;
        public AreaType areaType;
        public GhostSpawnCondition condition;
        public float weight;
    }

    private readonly Dictionary<MapType, List<SpawnCandidate>> spawnsByMap = new Dictionary<MapType, List<SpawnCandidate>>(32);

    public void Build()
    {
        byId.Clear();
        spawnsByMap.Clear();

        for (int i = 0; i < List.Count; ++i)
        {
            var def = List[i];
            if (def == null) continue;

            if (!byId.TryAdd(def.Id, def))
                Debug.LogWarning($"Duplicate ghost id: {def.Id} in {name}");

            BuildSpawnCache(def);
        }
    }

    private void BuildSpawnCache(GhostDefinitionSO def)
    {
        var spawns = def.Spawns;
        if (spawns == null) return;

        for (int i = 0; i < spawns.Count; ++i)
        {
            var s = spawns[i];
            if (s == null) continue;
            if (s.Weight <= 0f) continue;

            if (!spawnsByMap.TryGetValue(s.MapType, out var list))
            {
                list = new List<SpawnCandidate>(32);
                spawnsByMap.Add(s.MapType, list);
            }

            list.Add(new SpawnCandidate
            {
                ghostId = def.Id,
                areaType = s.AreaType,
                condition = s.Condition,
                weight = s.Weight
            });
        }
    }

    public bool TryGet(int id, out GhostDefinitionSO def)
    {
        return byId.TryGetValue(id, out def);
    }

    public IReadOnlyList<GhostDefinitionSO> All => List;

    public bool TryPickSpawn(MapType mapType,
        Func<GhostSpawnCondition, bool> conditionCheck,
        out int ghostId,
        out AreaType areaType)
    {
        ghostId = 0;
        areaType = AreaType.Anywhere;

        if (!spawnsByMap.TryGetValue(mapType, out var list)) return false;
        if (list == null || list.Count == 0) return false;

        float total = 0f;

        for (int i = 0; i < list.Count; ++i)
        {
            var c = list[i];

            bool ok = conditionCheck == null ? (c.condition == GhostSpawnCondition.Always) : conditionCheck(c.condition);
            if (!ok) continue;

            if (c.weight > 0f)
                total += c.weight;
        }

        if (total <= 0f) return false;

        float r = UnityEngine.Random.value * total;

        for (int i = 0; i < list.Count; ++i)
        {
            var c = list[i];

            bool ok = conditionCheck == null ? (c.condition == GhostSpawnCondition.Always) : conditionCheck(c.condition);
            if (!ok) continue;

            r -= c.weight;
            if (r < 0f)
            {
                ghostId = c.ghostId;
                areaType = c.areaType;
                return true;
            }
        }

        return false;
    }
}