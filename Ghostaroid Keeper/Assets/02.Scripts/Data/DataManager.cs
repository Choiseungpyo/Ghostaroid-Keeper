using UnityEngine;

/// <summary>
/// 정적 데이터와 플레이어 세이브 데이터 관리
/// </summary>
public class DataManager : Singleton<DataManager>
{
    [Header("Static DB")]
    [SerializeField] private GameDatabase gameDb;

    public GameDatabase GameDb => gameDb;
    public PlayerSaveData SaveData { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (gameDb != null)
            gameDb.BuildAll();

        SaveData = LoadOrCreate();
    }

    private PlayerSaveData LoadOrCreate()
    {
        // 나중에 Json/파일 로드 붙이면 됨
        return new PlayerSaveData();
    }
}
