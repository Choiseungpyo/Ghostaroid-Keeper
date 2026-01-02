using UnityEngine;

public sealed class AreaZone : MonoBehaviour
{
    private SpriteRenderer regionSprite;

    [Header("Optional Padding (World Units)")]
    [SerializeField] private Vector2 padding;

    [field: SerializeField] public AreaType Type { get; private set; }

    private void Awake()
    {
        regionSprite = GetComponent<SpriteRenderer>();
        regionSprite.enabled = false;
    }

    public Vector2 GetRandomPoint()
    {
        if (regionSprite == null)
            return transform.position;

        Bounds b = regionSprite.bounds;

        float minX = b.min.x + padding.x;
        float maxX = b.max.x - padding.x;
        float minY = b.min.y + padding.y;
        float maxY = b.max.y - padding.y;

        if (minX > maxX) { float t = minX; minX = maxX; maxX = t; }
        if (minY > maxY) { float t = minY; minY = maxY; maxY = t; }

        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        return new Vector2(x, y);
    }
}