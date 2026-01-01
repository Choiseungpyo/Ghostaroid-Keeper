using UnityEngine;
using static PointerInput;

public class HomeClickController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera cam;

    [Header("Raycast")]
    [SerializeField] private LayerMask clickableMask;

    [Header("Tap Rule")]
    [SerializeField] private float cancelMovePixels = 25f;

    private bool pressing;
    private Vector2 pressScreen;
    private HomeSection pressedTarget;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        if (!TryGetPointer(out Vector2 screenPos, out PointerPhase phase, out int pointerId))
            return;

        if (phase == PointerPhase.Began)
        {
            pressing = false;
            pressedTarget = null;
            pressScreen = screenPos;

            if (IsPointerOverUI(pointerId))
                return;

            pressedTarget = RaycastHotspot(screenPos);
            if (pressedTarget == null)
                return;

            pressing = true;
            return;
        }

        if (!pressing)
            return;

        if (phase == PointerPhase.Moved || phase == PointerPhase.Stationary)
        {
            float moved = Vector2.Distance(screenPos, pressScreen);
            if (moved > cancelMovePixels)
            {
                pressing = false;
                pressedTarget = null;
            }
            return;
        }

        if (phase == PointerPhase.Ended)
        {
            if (!IsPointerOverUI(pointerId))
            {
                var upTarget = RaycastHotspot(screenPos);
                if (upTarget != null && upTarget == pressedTarget)
                {
                    OnClickAppSection(upTarget);
                }
            }

            pressing = false;
            pressedTarget = null;
            return;
        }

        if (phase == PointerPhase.Canceled)
        {
            pressing = false;
            pressedTarget = null;
        }
    }

    private HomeSection RaycastHotspot(Vector2 screenPos)
    {
        Vector2 world = PointerInput.ScreenToWorld(cam, screenPos);
        RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero, 0f, clickableMask);
        if (!hit.collider) return null;
        return hit.collider.GetComponent<HomeSection>();
    }

    private void OnClickAppSection(HomeSection h)
    {
        EventDispatcher.Dispatch(new AppNavigateRequestEvent(h.Section));
        Debug.Log($"App Section click: {h.Section}");
    }
}