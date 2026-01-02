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

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private static readonly int GhostStateHash = Animator.StringToHash("GhostState");

    private GhostStateBase currentState;

    private GhostUntrackedState sUntracked;
    private GhostTrackedState sTracked;
    private GhostStunnedState sStunned;
    private GhostSealedState sSealed;

    protected Rigidbody2D rb;
    private Collider2D col;

    public GhostVisualController Visual { get; private set; }

    public int DefinitionId { get; private set; }

    protected float MoveSpeed => moveSpeed;
    public float RevealTimer { get; set; }
    public float StunTimer { get; set; }

    public float DefaultRevealDuration => defaultRevealDuration;
    public float DefaultStunDuration => defaultStunDuration;

    public GhostState CurrentState => currentState != null ? currentState.State : GhostState.Untracked;

    public GhostMovePatternType MovePatternType { get; private set; } = GhostMovePatternType.Wander;
    public GhostSpecialPatternType SpecialPatternType { get; private set; } = GhostSpecialPatternType.None;

    public float PatternTurnInterval { get; private set; } = 1f;

    private int stunImmuneHitsLeft = 0;
    private float stunDurationMultiplier = 1f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        Visual = GetComponent<GhostVisualController>();

        if (animator == null)
            animator = GetComponent<Animator>();

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
            default: SetState(sUntracked); break;
        }
    }

    private void SetState(GhostStateBase next)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = next;
        currentState.Enter(this);

        ApplyAnimatorState();
    }

    private void ApplyAnimatorState()
    {
        if (animator == null || currentState == null) return;
        animator.SetInteger(GhostStateHash, (int)currentState.State);
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

    public void OnSpawned(GhostDefinitionSO def)
    {
        DefinitionId = def != null ? def.Id : 0;

        ReviveForPool();

        if (def != null)
            ApplyDefinition(def);

        ChangeState(GhostState.Untracked);
    }

    public void OnDespawned()
    {
        RevealTimer = 0f;
        StunTimer = 0f;

        SetStunFx(false);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    protected virtual void ApplyDefinition(GhostDefinitionSO def)
    {
        if (def == null) return;

        if (def.MovePattern != null)
        {
            MovePatternType = def.MovePattern.Type;
            moveSpeed = def.MovePattern.MoveSpeed;
            PatternTurnInterval = def.MovePattern.TurnInterval;
        }

        if (def.SpecialPattern != null)
        {
            SpecialPatternType = def.SpecialPattern.Type;
        }

        if (def.StunResist != null)
        {
            stunImmuneHitsLeft = Mathf.Max(0, def.StunResist.ImmuneHits);
            stunDurationMultiplier = Mathf.Max(0f, def.StunResist.StunDurationMultiplier);
        }
        else
        {
            stunImmuneHitsLeft = 0;
            stunDurationMultiplier = 1f;
        }
    }

    public bool RequestStun(float duration)
    {
        if (stunImmuneHitsLeft > 0)
        {
            stunImmuneHitsLeft--;
            return false;
        }

        float baseDur = duration > 0f ? duration : DefaultStunDuration;
        float finalDur = baseDur * stunDurationMultiplier;

        StunTimer = finalDur;
        SetStunFx(true);
        ChangeState(GhostState.Stunned);
        return true;
    }

    private void ReviveForPool()
    {
        if (col != null) col.enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (Visual != null) Visual.SetInstantVisible(false);
        SetStunFx(false);

        RevealTimer = 0f;
        StunTimer = 0f;
    }

    public abstract void PatrolTick(float dt);
}