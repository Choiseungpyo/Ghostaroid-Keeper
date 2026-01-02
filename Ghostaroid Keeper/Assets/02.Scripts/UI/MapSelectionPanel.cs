using System.Collections.Generic;
using UnityEngine;

public class MapSelectionPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private Transform poolRoot;
    [SerializeField] private MapSelectionButton buttonPrefab;

    private MapDatabaseSO mapDb;
    private PlayerSaveData saveData;

    private readonly List<MapDataSO> sortedMaps = new List<MapDataSO>(32);

    private MapSelectionButtonPool pool;
    private readonly List<MapSelectionButton> buttonList = new List<MapSelectionButton>(32);

    private void Awake()
    {
        TryResolveRefs();

        if (contentRoot != null && buttonPrefab != null)
            pool = new MapSelectionButtonPool(buttonPrefab, contentRoot, poolRoot, 16, 128);
    }

    private void OnEnable()
    {
        int playerLevel = GetPlayerLevelSafe();
        Rebuild(playerLevel);
    }

    private void OnDisable()
    {
        ClearButtonList();
    }

    private void TryResolveRefs()
    {
        if (mapDb != null && saveData != null)
            return;

        if (DataManager.Instance == null)
            return;

        var gameDb = DataManager.Instance.GameDb;
        if (gameDb != null)
            mapDb = gameDb.MapDb;

        saveData = DataManager.Instance.SaveData;
    }

    private int GetPlayerLevelSafe()
    {
        if (saveData == null || saveData.profile == null)
            return 1;

        return Mathf.Max(1, saveData.profile.level);
    }

    private void Rebuild(int playerLevel)
    {
        if (pool == null)
            return;

        if (mapDb == null || mapDb.List == null)
            return;

        ClearButtonList();

        BuildSortedList(mapDb.List);

        SetButtons(playerLevel);
    }

    private void BuildSortedList(List<MapDataSO> source)
    {
        sortedMaps.Clear();

        for (int i = 0; i < source.Count; ++i)
        {
            var m = source[i];
            if (m != null)
                sortedMaps.Add(m);
        }

        sortedMaps.Sort((a, b) => a.Id.CompareTo(b.Id));
    }

    private void SetButtons(int playerLevel)
    {
        for (int i = 0; i < sortedMaps.Count; ++i)
        {
            MapDataSO map = sortedMaps[i];
            if (map == null) continue;

            bool unlocked = IsUnlocked(map, playerLevel);

            MapSelectionButton btn = pool.Get();
            btn.Bind(map, unlocked, OnClickMap);
            buttonList.Add(btn);

            ApplyPlacement(btn.transform as RectTransform, contentRoot, map.UiAnchor01, map.UiOffset);
        }
    }

    private static void ApplyPlacement(RectTransform rect, RectTransform parent, Vector2 anchor01, Vector2 offset)
    {
        if (rect == null || parent == null) return;

        rect.SetParent(parent, false);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchorMin = anchor01;
        rect.anchorMax = anchor01;
        rect.anchoredPosition = offset;
        rect.localScale = Vector3.one;
    }

    private bool IsUnlocked(MapDataSO map, int playerLevel)
    {
        if (map == null || map.Unlock == null)
            return true;

        return playerLevel >= map.Unlock.RequiredPlayerLevel;
    }

    private void OnClickMap(MapDataSO map)
    {
        if (map == null)
            return;

        EventDispatcher.Dispatch(new AppExploreMapSelectedEvent
        {
            MapId = map.Id,
            MapType = map.Type
        });

        gameObject.SetActive(false);
    }

    private void ClearButtonList()
    {
        if (pool == null) return;

        for (int i = 0; i < buttonList.Count; ++i)
            pool.Release(buttonList[i]);

        buttonList.Clear();
    }
}