using UnityEngine;

public sealed class GhostSpawner : Singleton<GhostSpawner>, IEventListener<AppExploreMapSelectedEvent>
{
    [SerializeField] private GhostPoolManager poolManager;
    [SerializeField] private MapAreaRegistry areaRegistry;

    [SerializeField] private int spawnCount = 5;

    protected override void Awake()
    {
        base.Awake();

        EventDispatcher.RegisterListener(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventDispatcher.UnregisterListener(this);
    }

    public void SpawnAll(MapType mayType)
    {
        if (poolManager == null) return;
        if (areaRegistry == null) return;

        var gameDb = DataManager.Instance.GameDb;
        if (gameDb == null || gameDb.GhostDb == null) return;

        for (int i = 0; i < spawnCount; ++i)
        {
            if (!gameDb.GhostDb.TryPickSpawn(mayType, EvaluateCondition, out int ghostId, out AreaType areaType))
                return;

            if (!areaRegistry.TryGetRandomPoint(areaType, out Vector2 pos))
                continue;

            poolManager.Spawn(ghostId, pos, Quaternion.identity);
        }
    }

    private bool EvaluateCondition(GhostSpawnCondition cond)
    {
        switch (cond)
        {
            case GhostSpawnCondition.Always:
                return true;

            default:
                return true;
        }
    }

    public void OnEvent(AppExploreMapSelectedEvent appExploreMapSelectedEvent)
    {
        SpawnAll(appExploreMapSelectedEvent.MapType);
    }
}
