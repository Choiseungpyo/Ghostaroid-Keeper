using System.Collections.Generic;
using System;

[Serializable]
public class GhostCodexSaveData
{
    public List<GhostOwnedSaveData> list = new List<GhostOwnedSaveData>(64);
}