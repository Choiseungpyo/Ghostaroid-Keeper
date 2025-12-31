using UnityEngine;

public abstract class GhostBase : MonoBehaviour, IGhostReveal, IGhostStunnable, IGhostCapturePushable, IGhostSealable
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("Durations")]
    [SerializeField] private float defaultRevealDuration = 2f;
    [SerializeField] private float defaultStunDuration = 2f;

    [Header("FX")]
    [SerializeField] private GameObject stunFx;
    [SerializeField] private GameObject sealFx;

    [Header("Knockback")]
    [SerializeField] private float defaultKnockbackForce = 10f;

    private GhostStateBase currentState;

    private GhostUntrackedState sUntracked;
    private GhostTrackedState sTracked;
    private GhostStunnedState sStunned;
    private GhostSealedState sSealed;

    protected Rigidbody2D rb;
    private Collider2D col;

    public GhostVisualController Visual { get; private set; }

    protected float MoveSpeed => moveSpeed;
    public float RevealTimer { get; set; }
    public float StunTimer { get; set; }

    public float DefaultRevealDuration => defaultRevealDuration;
    public float DefaultStunDuration => defaultStunDuration;

    public GhostState CurrentState => currentState != null ? currentState.State : GhostState.Untracked;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        Visual = GetComponent<GhostVisualController>();

        ApplyRbDefaults();

        sUntracked = new GhostUntrackedState();
        sTracked = new GhostTrackedState();
        sStunned = new GhostStunnedState();
        sSealed = new GhostSealedState();

        SetState(sUntracked);
    }

    protected virtual void FixedUpdate()
    {
        if (currentState != null)
            currentState.Tick(this, Time.fixedDeltaTime);
    }

    private void ApplyRbDefaults()
    {
        if (rb == null) return;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearDamping = 8f;
        rb.angularDamping = 999f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void ChangeState(GhostState state)
    {
        if (currentState != null && currentState.State == state) return;

        switch (state)
        {
            case GhostState.Untracked: SetState(sUntracked); break;
            case GhostState.Tracked: SetState(sTracked); break;
            case GhostState.Stunned: SetState(sStunned); break;
            case GhostState.Sealed: SetState(sSealed); break;
            default:
                Debug.LogWarning(state);
                SetState(sUntracked);
                break;
        }
    }

    private void SetState(GhostStateBase next)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = next;
        currentState.Enter(this);
    }

    void IGhostReveal.Reveal(float duration) => currentState.OnFrequency(this, duration);
    void IGhostStunnable.Stun(float duration) => currentState.OnStun(this, duration);
    void IGhostCapturePushable.Push(Vector2 hitDir, float force, float extendStun) => currentState.OnTransferHit(this, hitDir, force, extendStun);
    void IGhostSealable.Seal() => currentState.OnSeal(this);

    public void SetStunFx(bool on)
    {
        if (stunFx == null) return;
        stunFx.SetActive(on);
    }

    public void SealNow()
    {
        if (sealFx != null)
        {
            sealFx.SetActive(false);
            sealFx.SetActive(true);
        }

        if (Visual != null) Visual.SetInstantVisible(false);
        SetStunFx(false);

        if (col != null) col.enabled = false;
        if (rb != null) rb.simulated = false;
    }

    public void Knockback(Vector2 hitDir, float force)
    {
        if (rb == null) return;

        Vector2 dir = hitDir.sqrMagnitude > 0f ? hitDir.normalized : Vector2.right;
        float f = force > 0f ? force : defaultKnockbackForce;

        rb.AddForce(-dir * f, ForceMode2D.Impulse);
    }

    public abstract void PatrolTick(float dt);
}