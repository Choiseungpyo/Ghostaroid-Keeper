using UnityEngine;

public sealed class GhostTrackedState : GhostStateBase
{
    public override GhostState State => GhostState.Tracked;

    public override void Enter(GhostBase g)
    {
        g.SetStunFx(false);
    }

    public override void Tick(GhostBase g, float dt)
    {
        g.PatrolTick(dt);

        if (g.RevealTimer > 0f)
        {
            g.RevealTimer -= dt;
            if (g.RevealTimer <= 0f)
                g.ChangeState(GhostState.Untracked);
        }
    }

    public override void OnStun(GhostBase g, float duration)
    {
        float dur = duration > 0f ? duration : g.DefaultStunDuration;
        g.StunTimer = dur;
        g.SetStunFx(true);
        g.ChangeState(GhostState.Stunned);
    }
}