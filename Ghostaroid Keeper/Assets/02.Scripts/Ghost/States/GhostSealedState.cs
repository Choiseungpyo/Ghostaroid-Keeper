using UnityEngine;

public sealed class GhostSealedState : GhostStateBase
{
    public override GhostState State => GhostState.Sealed;

    public override void Enter(GhostBase g)
    {
        if (g.Visual != null) g.Visual.SetInstantVisible(false);
        g.SetStunFx(false);
        g.RevealTimer = 0f;
        g.StunTimer = 0f;
    }

    public override void OnSeal(GhostBase g)
    {
        
    }
}