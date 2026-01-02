using System.Collections.Generic;
using UnityEngine;

public sealed class GhostPoolManager : Singleton<GhostPoolManager>
{
    private GameDatabase gameDb;
    [SerializeField] private Transform poolRoot;

    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 100;

    private readonly Dictionary<int, GhostPool> pools = new Dictionary<int, GhostPool>(64);

    protected override void Awake()
    {
        base.Awake();

        if (poolRoot == null) poolRoot = transform;
        gameDb = DataManager.Instance.GameDb;
    }

    public GhostBase Spawn(int ghostId, Vector3 pos, Quaternion rot)
    {
        if (gameDb == null || gameDb.GhostDb == null)
        {
            Debug.LogError("GhostPoolManager: GameDatabase or GhostDb is missing");
            return null;
        }

        if (!gameDb.GhostDb.TryGet(ghostId, out GhostDefinitionSO def) || def == null)
        {
            Debug.LogError("GhostPoolManager: invalid ghostId " + ghostId);
            return null;
        }

        if (def.Prefab == null)
        {
            Debug.LogError("GhostPoolManager: Prefab is null for ghostId " + ghostId);
            return null;
        }

        if (!pools.TryGetValue(ghostId, out GhostPool pool))
        {
            pool = new GhostPool(def, poolRoot, defaultCapacity, maxSize);
            pools.Add(ghostId, pool);
        }

        GhostBase g = pool.Get();
        g.transform.SetPositionAndRotation(pos, rot);
        return g;
    }

    public void Despawn(GhostBase g)
    {
        if (g == null) return;

        int id = g.DefinitionId;
        if (!pools.TryGetValue(id, out GhostPool pool))
        {
            g.gameObject.SetActive(false);
            g.transform.SetParent(poolRoot, false);
            return;
        }

        pool.Release(g);
    }

    public void Prewarm(int ghostId, int count)
    {
        if (count <= 0) return;

        if (gameDb == null || gameDb.GhostDb == null) return;
        if (!gameDb.GhostDb.TryGet(ghostId, out GhostDefinitionSO def) || def == null || def.Prefab == null) return;

        if (!pools.TryGetValue(ghostId, out GhostPool pool))
        {
            pool = new GhostPool(def, poolRoot, defaultCapacity, maxSize);
            pools.Add(ghostId, pool);
        }

        for (int i = 0; i < count; ++i)
        {
            GhostBase g = pool.Get();
            pool.Release(g);
        }
    }
}