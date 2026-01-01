using UnityEngine;

[CreateAssetMenu(menuName = "Game/Map/MapData")]
public class MapDataSO : ScriptableObject
{
    [field: Header("Identity")]
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public MapType Type { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Sprite Thumbnail { get; private set; }

    [field: Header("Unlock")]
    [field: SerializeField] public MapUnlockInfo Unlock { get; private set; } = new MapUnlockInfo();
}