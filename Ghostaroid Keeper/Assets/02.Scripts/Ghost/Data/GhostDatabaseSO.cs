using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ghost/GhostDatabase")]
public class GhostDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<GhostDefinitionSO> List { get; private set; } = new List<GhostDefinitionSO>();

    private readonly Dictionary<int, GhostDefinitionSO> byId = new Dictionary<int, GhostDefinitionSO>();


    public void Build()
    {
        byId.Clear();

        for (int i = 0; i < List.Count; ++i)
        {
            var def = List[i];
            if (def == null) continue;

            if (!byId.TryAdd(def.Id, def))
                Debug.LogWarning($"Duplicate ghost id: {def.Id} in {name}");
        }
    }

    public bool TryGet(int id, out GhostDefinitionSO def)
    {
        return byId.TryGetValue(id, out def);
    }

    public IReadOnlyList<GhostDefinitionSO> All => List;
}