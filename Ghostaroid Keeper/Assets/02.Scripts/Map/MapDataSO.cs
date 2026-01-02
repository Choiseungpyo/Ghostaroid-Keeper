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

    [field: Header("UI Placement")]
    [field: SerializeField] public Vector2 UiAnchor01 { get; private set; } = new Vector2(0.5f, 0.5f);
    [field: SerializeField] public Vector2 UiOffset { get; private set; } = Vector2.zero;

    [field: Header("Object Placement")]
    [field: SerializeField] public Sprite BGSprite { get; private set; }
    [field: SerializeField] public Sprite WallTopSprite { get; private set; }
    [field: SerializeField] public Sprite WallBottomSprite { get; private set; }
    [field: SerializeField] public Sprite WallLeftSprite { get; private set; }
    [field: SerializeField] public Sprite WallRightSprite { get; private set; }

    [field: Header("Unlock")]
    [field: SerializeField] public MapUnlockInfo Unlock { get; private set; } = new MapUnlockInfo();
}