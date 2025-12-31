using UnityEngine;

public enum ProjectileType
{
    Frequency,
    Stun,
    Transfer
}

[CreateAssetMenu(menuName = "Game/ProjectileItemDefinition")]
public sealed class ProjectileItemData : ScriptableObject
{
    public ProjectileType projectileType;
    public Sprite icon;
    public Projectile prefab;
}