using UnityEngine;

public sealed class MapSelectionButtonPool : ObjectPoolBase<MapSelectionButton>
{
    private readonly MapSelectionButton prefab;
    private readonly Transform contentRoot;
    private readonly Transform poolRoot;

    public MapSelectionButtonPool(
        MapSelectionButton prefab,
        Transform contentRoot,
        Transform poolRoot,
        int defaultCapacity = 16,
        int maxSize = 128
    ) : base(defaultCapacity, maxSize)
    {
        this.prefab = prefab;
        this.contentRoot = contentRoot;
        this.poolRoot = poolRoot;
    }

    protected override MapSelectionButton CreateObject()
    {
        MapSelectionButton inst = Object.Instantiate(prefab, poolRoot != null ? poolRoot : contentRoot);
        inst.gameObject.SetActive(false);
        return inst;
    }

    protected override void OnGet(MapSelectionButton obj)
    {
        if (obj == null) return;
        obj.transform.SetParent(contentRoot, false);
        obj.gameObject.SetActive(true);
    }

    protected override void OnRelease(MapSelectionButton obj)
    {
        if (obj == null) return;
        obj.Unbind();
        obj.gameObject.SetActive(false);

        if (poolRoot != null)
            obj.transform.SetParent(poolRoot, false);
        else
            obj.transform.SetParent(contentRoot, false);
    }

    protected override void OnDestroy(MapSelectionButton obj)
    {
        if (obj == null) return;
        Object.Destroy(obj.gameObject);
    }
}