using UnityEngine;
using static UnityEditor.Progress;

public class GameDatabase : MonoBehaviour
{
    [field: SerializeField] public GhostDatabaseSO GhostDb { get; private set; }
    [field: SerializeField] public MapDatabaseSO MapDb { get; private set; }
    public void BuildAll()
    {
        if (GhostDb == null) Debug.LogError("GameDatabaseSO: GhostDb is missing");
        else GhostDb.Build();

        if(MapDb == null) Debug.LogError("MapDatabaseSO: MapDb is missing");
        else MapDb.Build();
    }

}
