using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (col == null) col = GetComponent<Collider2D>();
    }

    public bool IsGrabbed(Vector2 worldPos, float grabRadiusWorld)
    {
        if (col != null && col.OverlapPoint(worldPos))
            return true;

        if (rb == null)
            return false;

        float d = Vector2.Distance(worldPos, rb.position);
        return d <= grabRadiusWorld;
    }

    public void BeginAim(Vector2 origin)
    {
        if (rb == null) return;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = origin;
        rb.rotation = 0f;
    }

    public void HoldAt(Vector2 origin)
    {
        if (rb == null) return;
        rb.position = origin;
    }

    public void CancelAim(Vector2 origin)
    {
        if (rb == null) return;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = origin;
        rb.rotation = 0f;
    }

    public void Shoot(Vector2 origin, Vector2 impulse)
    {
        if (rb == null) return;

        rb.position = origin;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.AddForce(impulse, ForceMode2D.Impulse);
    }
}