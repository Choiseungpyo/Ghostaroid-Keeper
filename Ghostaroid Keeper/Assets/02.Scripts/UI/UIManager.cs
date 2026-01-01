using System;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType
{
    Home,
    Explore,
    MapSelect,
    Growth,
    Book,
    Bag
}

[Serializable]
public class PanelData
{
    [field: SerializeField] public PanelType Type { get; private set; }
    [field: SerializeField] public GameObject Panel { get; private set; }
}

public class UIManager : Singleton<UIManager>, IEventListener<AppNavigateRequestEvent>
{
    [SerializeField] private List<PanelData> panelList = new List<PanelData>();

    private readonly Dictionary<PanelType, GameObject> panelDict = new Dictionary<PanelType, GameObject>();

    protected override void Awake()
    {
        base.Awake();

        BulidDict();
        Show(PanelType.Home);
        EventDispatcher.RegisterListener(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventDispatcher.UnregisterListener(this);
    }

    private void BulidDict()
    {
        panelDict.Clear();

        for (int i = 0; i < panelList.Count; ++i)
        {
            PanelData data = panelList[i];

            if (data == null)
            {
                Debug.LogWarning($"PanelData is null at index {i}");
                continue;
            }

            if (data.Panel == null)
            {
                Debug.LogWarning($"Panel is null for type {data.Type} at index {i}");
                continue;
            }

            if (!panelDict.TryAdd(data.Type, data.Panel))
            {
                Debug.LogWarning($"Duplicate PanelType: {data.Type}. Keeping first, ignoring index {i}");
            }
        }
    }

    private void Show(params PanelType[] types)
    {
        if (types == null || types.Length == 0)
        {
            HideAll();
            return;
        }

        HashSet<PanelType> set = new HashSet<PanelType>(types);

        foreach (var kv in panelDict)
        {
            GameObject go = kv.Value;
            if (go == null) continue;

            bool on = set.Contains(kv.Key);
            if (go.activeSelf != on)
                go.SetActive(on);
        }
    }

    private void HideAll()
    {
        foreach (var kv in panelDict)
        {
            if (kv.Value != null && kv.Value.activeSelf)
                kv.Value.SetActive(false);
        }
    }

    public void OnEvent(AppNavigateRequestEvent appNavigateRequestEvent)
    {
        switch(appNavigateRequestEvent.Section)
        {
            case AppSection.Home:
                Show(PanelType.Home);
                break;

            case AppSection.Explore:
                Show(PanelType.MapSelect, PanelType.Explore);
                break;

            case AppSection.Growth:
                Show(PanelType.Growth);
                break;

            case AppSection.Bag:
                Show(PanelType.Bag);
                break;

            case AppSection.Book:
                Show(PanelType.Book);
                break;

            default:
                Debug.LogWarning(appNavigateRequestEvent.Section);
                break;
        }
    }
}