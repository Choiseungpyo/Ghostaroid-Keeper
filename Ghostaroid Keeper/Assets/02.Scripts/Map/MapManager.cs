using UnityEngine;

public class MapManager : Singleton<MapManager>, IEventListener<AppExploreMapSelectedEvent>
{
    [Header("View")]
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer wallTop;
    [SerializeField] private SpriteRenderer wallBottomL;
    [SerializeField] private SpriteRenderer wallBottomR;
    [SerializeField] private SpriteRenderer wallLeft;
    [SerializeField] private SpriteRenderer wallRight;

    [Header("Runtime")]
    [SerializeField] private MapAreaRegistry areaRegistry;
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    private MapDatabaseSO mapDb;

    protected override void Awake()
    {
        base.Awake();

        var gameDb = DataManager.Instance.GameDb;
        mapDb = gameDb.MapDb;

        EventDispatcher.RegisterListener(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventDispatcher.UnregisterListener(this);
    }


    private void OnMapSelected(AppExploreMapSelectedEvent e)
    {
        ApplyMap(e.MapId, e.MapType);
    }

    private void ApplyMap(int mapId, MapType mapType)
    {
        if (mapDb == null) return;

        if (!mapDb.TryGet(mapId, out MapDataSO map))
        {
            Debug.LogError("Map not found: " + mapId);
            return;
        }

        ApplySprites(map);

        //if (obstacleSpawner != null)
        //{
        //    obstacleSpawner.ClearAll();
        //    obstacleSpawner.SpawnForMap(map);
        //}
    }

    private void ApplySprites(MapDataSO map)
    {
        if (map == null) return;

        if (background != null) background.sprite = map.BGSprite;

        if (wallTop != null) wallTop.sprite = map.WallTopSprite;
        if (wallBottomL != null) wallBottomL.sprite = map.WallBottomSprite;
        if (wallBottomR != null) wallBottomR.sprite = map.WallBottomSprite;
        if (wallLeft != null) wallLeft.sprite = map.WallLeftSprite;
        if (wallRight != null) wallRight.sprite = map.WallRightSprite;
    }

    public void OnEvent(AppExploreMapSelectedEvent appExploreMapSelectedEvent)
    {

    }
}
