using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FrequencyRingPulse : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Transform center;
    [SerializeField] private int segments = 128;
    [SerializeField] private float lineWidth = 0.2f;

    [Header("Pulse")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float maxRadius = 20f;

    [Header("Detect")]
    [SerializeField] private LayerMask ghostMask;
    [SerializeField] private float revealDuration = 2f;
    [SerializeField] private int overlapBufferSize = 64;
    [SerializeField] private bool detectTriggers = true;

    private LineRenderer lr;
    private float radius;
    private Collider2D[] overlapBuffer;
    private ContactFilter2D contactFilter;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();

        lr.loop = true;
        lr.useWorldSpace = true;

        if (segments < 3) segments = 3;
        lr.positionCount = segments;

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        overlapBuffer = new Collider2D[overlapBufferSize];
        radius = 0f;

        contactFilter = new ContactFilter2D();
        contactFilter.useLayerMask = true;
        contactFilter.SetLayerMask(ghostMask);
        contactFilter.useTriggers = detectTriggers;
    }

    private void Update()
    {
        if (center == null)
            return;

        radius += speed * Time.deltaTime;

        if (radius >= maxRadius)
            radius = 0f;

        DrawCircle(center.position, radius);
        DetectGhosts(center.position, radius, lineWidth);
    }

    private void DrawCircle(Vector3 c, float r)
    {
        float step = (Mathf.PI * 2f) / segments;
        float z = c.z;

        for (int i = 0; i < segments; ++i)
        {
            float a = step * i;
            float x = Mathf.Cos(a) * r;
            float y = Mathf.Sin(a) * r;

            lr.SetPosition(i, new Vector3(c.x + x, c.y + y, z));
        }
    }

    private void DetectGhosts(Vector2 c, float r, float width)
    {
        float half = width * 0.5f;
        float searchR = r + half;

        int cnt = Physics2D.OverlapCircle(c, searchR, contactFilter, overlapBuffer);

        for (int i = 0; i < cnt; ++i)
        {
            Collider2D col = overlapBuffer[i];
            if (col == null) continue;

            float d = Vector2.Distance(c, col.transform.position);
            if (Mathf.Abs(d - r) <= half)
            {
                var reveal = col.GetComponent<IGhostReveal>();
                if (reveal != null)
                    reveal.Reveal(revealDuration);
            }

            overlapBuffer[i] = null;
        }
    }
}