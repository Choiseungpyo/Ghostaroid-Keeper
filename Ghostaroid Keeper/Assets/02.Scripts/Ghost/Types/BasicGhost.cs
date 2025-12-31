using System.Collections.Generic;
using UnityEngine;

public class BasicGhost : GhostBase
{
    [Header("Area")]
    [SerializeField] private Collider2D roamArea;
    [SerializeField] private Transform mapRoot;

    [Header("Masks")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Avoid")]
    [SerializeField] private float avoidRadius = 0.25f;
    [SerializeField] private int maxSampleTries = 30;

    [Header("Patrol")]
    [SerializeField] private float retargetInterval = 1.2f;

    private Collider2D[] wallCols;
    private Vector2 target;
    private float timer;

    protected override void Awake()
    {
        base.Awake();
        CacheWalls();
        PickTarget();
    }

    private void CacheWalls()
    {
        if (mapRoot == null)
        {
            wallCols = null;
            return;
        }

        Collider2D[] all = mapRoot.GetComponentsInChildren<Collider2D>(true);
        List<Collider2D> list = new List<Collider2D>(all.Length);

        for (int i = 0; i < all.Length; ++i)
        {
            Collider2D c = all[i];
            if (c == null) continue;

            if (roamArea != null && c == roamArea) continue;

            if (IsInMask(c.gameObject.layer, wallMask))
                list.Add(c);
        }

        wallCols = list.ToArray();
    }

    public override void PatrolTick(float dt)
    {
        if (rb == null) return;

        timer -= dt;
        if (timer <= 0f) PickTarget();

        Vector2 p = rb.position;
        Vector2 dir = target - p;
        float len = dir.magnitude;
        if (len <= 0.0001f) return;
        dir /= len;

        Vector2 next = p + dir * MoveSpeed * dt;
        rb.MovePosition(next);
    }

    private void PickTarget()
    {
        timer = retargetInterval;

        if (rb == null)
        {
            target = transform.position;
            return;
        }

        Bounds b;
        if (roamArea != null) b = roamArea.bounds;
        else b = new Bounds(rb.position, new Vector3(10f, 10f, 0f));

        for (int i = 0; i < maxSampleTries; ++i)
        {
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);
            Vector2 p = new Vector2(x, y);

            if (roamArea != null && !roamArea.OverlapPoint(p))
                continue;

            if (IsNearWall(p, avoidRadius))
                continue;

            if (Physics2D.OverlapCircle(p, avoidRadius, obstacleMask) != null)
                continue;

            target = p;
            return;
        }

        target = rb.position;
    }

    private bool IsNearWall(Vector2 p, float r)
    {
        if (wallCols == null) return false;

        for (int i = 0; i < wallCols.Length; ++i)
        {
            Collider2D c = wallCols[i];
            if (c == null) continue;
            if (!c.enabled) continue;

            Vector2 closest = c.ClosestPoint(p);
            if (Vector2.Distance(closest, p) <= r)
                return true;
        }

        return false;
    }

    private static bool IsInMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || collision.collider == null) return;

        if (IsInMask(collision.collider.gameObject.layer, wallMask))
            PickTarget();
    }
}