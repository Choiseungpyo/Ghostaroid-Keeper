using UnityEngine;

public sealed class GhostUntrackedState : GhostStateBase
{
    public override GhostState State => GhostState.Untracked;

    public override void Enter(GhostBase g)
    {
        if (g.Visual != null) g.Visual.SetInstantVisible(false);
    }

    public override void Tick(GhostBase g, float dt)
    {
        g.PatrolTick(dt);
    }

    public override void OnFrequency(GhostBase g, float duration)
    {
        float dur = duration > 0f ? duration : g.DefaultRevealDuration;
        g.RevealTimer = dur;

        if (g.Visual != null) g.Visual.Show(dur);

        g.ChangeState(GhostState.Tracked);
    }
}