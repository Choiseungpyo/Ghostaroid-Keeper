using System;

[Serializable]
public class PlayerSaveData
{
    public int saveVersion = 1;

    public PlayerProfileData profile = new PlayerProfileData();
    public GhostCodexSaveData ghostCodex = new GhostCodexSaveData();
    public InventorySaveData inventory = new InventorySaveData();
    public ExploreSessionSaveData exploreSession = new ExploreSessionSaveData();
}
