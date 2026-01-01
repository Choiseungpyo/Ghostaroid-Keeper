public enum GhostState
{
    Untracked,
    Tracked,
    Stunned,
    Sealed
}

public enum GhostRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum GhostMovePatternType
{
    Wander,
    EdgePatrol,
    AnchorOrbit,
    Zigzag,
    DashPause,
    Teleport,
    HideAndPeek,
    ChaseWhenSeen
}

public enum GhostSpecialPatternType
{
    None,
    Decoy,
    PhaseShift,
    ShieldPulse,
    RageMove,
    SwapSpot
}

public enum GhostSpawnCondition
{
    Always,
    Day,
    Night,
    EventOnly
}