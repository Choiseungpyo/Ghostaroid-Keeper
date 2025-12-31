using UnityEngine;

public abstract class GhostStateBase
{
    public abstract GhostState State { get; }

    public virtual void Enter(GhostBase g) { }
    public virtual void Exit(GhostBase g) { }
    public virtual void Tick(GhostBase g, float dt) { }

    public virtual void OnFrequency(GhostBase g, float duration) { }
    public virtual void OnStun(GhostBase g, float duration) { }
    public virtual void OnTransferHit(GhostBase g, Vector2 hitDir, float force, float extendStun) { }
    public virtual void OnSeal(GhostBase g) { }
}