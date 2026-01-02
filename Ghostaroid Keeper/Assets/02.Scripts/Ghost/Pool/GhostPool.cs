using UnityEngine;

public sealed class GhostPool : ObjectPoolBase<GhostBase>
{
    private readonly GhostDefinitionSO def;
    private readonly Transform poolRoot;

    public GhostPool(GhostDefinitionSO def, Transform poolRoot, int defaultCapacity = 10, int maxSize = 100)
        : base(defaultCapacity, maxSize)
    {
        this.def = def;
        this.poolRoot = poolRoot;
    }

    protected override GhostBase CreateObject()
    {
        GhostBase g = Object.Instantiate(def.Prefab, poolRoot);
        g.gameObject.SetActive(false);
        return g;
    }

    protected override void OnGet(GhostBase obj)
    {
        obj.gameObject.SetActive(true);
        obj.OnSpawned(def);
    }

    protected override void OnRelease(GhostBase obj)
    {
        obj.OnDespawned();
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(poolRoot, false);
    }

    protected override void OnDestroy(GhostBase obj)
    {
        if (obj != null)
            Object.Destroy(obj.gameObject);
    }
}