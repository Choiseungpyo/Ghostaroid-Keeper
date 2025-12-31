using UnityEngine;

public sealed class GhostStunnedState : GhostStateBase
{
    public override GhostState State => GhostState.Stunned;

    public override void Enter(GhostBase g)
    {
        g.SetStunFx(true);
    }

    public override void Exit(GhostBase g)
    {
        g.SetStunFx(false);
    }

    public override void Tick(GhostBase g, float dt)
    {
        if (g.StunTimer > 0f)
        {
            g.StunTimer -= dt;
            if (g.StunTimer <= 0f)
                g.ChangeState(GhostState.Tracked);
        }
    }

    public override void OnTransferHit(GhostBase g, Vector2 hitDir, float force, float extendStun)
    {
        g.Knockback(hitDir, force);

        if (extendStun > 0f)
            g.StunTimer = Mathf.Max(g.StunTimer, extendStun);
    }
}