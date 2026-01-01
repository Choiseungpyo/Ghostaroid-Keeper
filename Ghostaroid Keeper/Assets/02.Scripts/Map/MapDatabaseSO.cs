using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Map/MapDatabase")]
public class MapDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<MapDataSO> List { get; private set; } = new List<MapDataSO>();

    private readonly Dictionary<int, MapDataSO> byId = new Dictionary<int, MapDataSO>();

    public void Build()
    {
        byId.Clear();

        for (int i = 0; i < List.Count; ++i)
        {
            var map = List[i];
            if (map == null)
            {
                Debug.LogWarning($"MapDatabaseSO: null at index {i}");
                continue;
            }

            if (map.Id <= 0)
            {
                Debug.LogWarning($"MapDatabaseSO: invalid id {map.Id} in {map.name}");
                continue;
            }

            if (!byId.TryAdd(map.Id, map))
            {
                Debug.LogWarning($"MapDatabaseSO: duplicate map id {map.Id}");
            }
        }
    }

    public bool TryGet(int id, out MapDataSO map)
    {
        return byId.TryGetValue(id, out map);
    }

    public IReadOnlyList<MapDataSO> All => List;
}