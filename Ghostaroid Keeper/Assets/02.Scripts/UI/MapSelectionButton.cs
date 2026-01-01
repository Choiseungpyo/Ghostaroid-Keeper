using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectionButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;

    [Header("Lock")]
    [SerializeField] private GameObject lockRoot;
    [SerializeField] private TMP_Text lockText;

    private MapDataSO map;
    private bool unlocked;
    private Action<MapDataSO> onClick;

    private void Awake()
    {
        if (button != null)
            button.onClick.AddListener(HandleClick);
    }

    public void Bind(MapDataSO map, bool unlocked, Action<MapDataSO> onClick)
    {
        this.map = map;
        this.unlocked = unlocked;
        this.onClick = onClick;

        if (nameText != null)
            nameText.text = (map != null && !string.IsNullOrEmpty(map.DisplayName)) ? map.DisplayName : "MAP";

        if (iconImage != null)
        {
            iconImage.sprite = (map != null) ? map.Thumbnail : null;
            iconImage.enabled = (iconImage.sprite != null);
        }

        if (button != null)
            button.interactable = unlocked;

        if (lockRoot != null)
            lockRoot.SetActive(!unlocked);

        if (!unlocked && lockText != null && map != null && map.Unlock != null)
        {
            int reqLv = map.Unlock.RequiredPlayerLevel;
            lockText.text = $"LV {reqLv}";
        }
        else if (lockText != null)
        {
            lockText.text = "";
        }
    }

    public void Unbind()
    {
        map = null;
        unlocked = false;
        onClick = null;

        if (button != null)
            button.interactable = false;

        if (lockRoot != null)
            lockRoot.SetActive(false);

        if (lockText != null)
            lockText.text = "";

        if (nameText != null)
            nameText.text = "";

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    private void HandleClick()
    {
        if (!unlocked) return;
        if (map == null) return;
        onClick?.Invoke(map);
    }
}