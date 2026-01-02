using System.Collections.Generic;
using UnityEngine;

public sealed class MapAreaRegistry : MonoBehaviour
{
    private readonly Dictionary<AreaType, List<AreaZone>> zonesByType = new Dictionary<AreaType, List<AreaZone>>(64);

    private void Awake()
    {
        Build();
    }

    public void Build()
    {
        zonesByType.Clear();

        AreaZone[] zones = GetComponentsInChildren<AreaZone>(true);
        for (int i = 0; i < zones.Length; ++i)
        {
            AreaZone z = zones[i];
            if (z == null)
                continue;

            AreaType t = z.Type;

            if (!zonesByType.TryGetValue(t, out List<AreaZone> list))
            {
                list = new List<AreaZone>(4);
                zonesByType.Add(t, list);
            }

            if (!list.Contains(z))
                list.Add(z);
        }
    }

    public bool TryGetRandomPoint(AreaType type, out Vector2 point)
    {
        point = default;

        if (!zonesByType.TryGetValue(type, out List<AreaZone> list))
            return false;

        if (list == null || list.Count == 0)
            return false;

        AreaZone z = list[Random.Range(0, list.Count)];
        if (z == null)
            return false;

        point = z.GetRandomPoint();
        return true;
    }
}