using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldSectionData
{
    [field: SerializeField] public AppSection Section { get; private set; }
    [field: SerializeField] public GameObject Root { get; private set; }
}

public class WorldSectionManager : Singleton<WorldSectionManager>, IEventListener<AppNavigateRequestEvent>
{
    [SerializeField] private List<WorldSectionData> worldSectionList = new List<WorldSectionData>();

    private readonly Dictionary<AppSection, GameObject> worldSectionDict = new Dictionary<AppSection, GameObject>();

    protected override void Awake()
    {
        base.Awake();

        Build();

        EventDispatcher.RegisterListener(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventDispatcher.UnregisterListener(this);
    }

    private void Build()
    {
        worldSectionDict.Clear();

        for (int i = 0; i < worldSectionList.Count; ++i)
        {
            var data = worldSectionList[i];

            if (data == null || data.Root == null)
            {
                Debug.LogWarning($"WorldSectionData is null or Root is null at index {i}");
                continue;
            }

            if (!worldSectionDict.TryAdd(data.Section, data.Root))
            {
                Debug.LogWarning($"Duplicate AppSection: {data.Section} at index {i}");
            }
        }
    }

    public void Show(AppSection section)
    {
        foreach (var kv in worldSectionDict)
        {
            if (kv.Value == null) continue;
            kv.Value.SetActive(kv.Key == section);
        }
    }
    
    public void OnEvent(AppNavigateRequestEvent appNavigateRequestEvent)
    {
        Show(appNavigateRequestEvent.Section);
    }
}