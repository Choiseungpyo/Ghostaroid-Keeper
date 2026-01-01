using UnityEngine;

public interface IGhostReveal
{
    void Reveal(float duration);
}

public interface IGhostStunnable
{
    void Stun(float duration);
}

public interface IGhostCapturePushable
{
    void Push(Vector2 hitDir, float force, float extendStun);
}

public interface IGhostSealable
{
    void Seal();
}