using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public static class PointerInput
{
    public enum PointerPhase { Began, Moved, Stationary, Ended, Canceled }

    public static bool TryGetPointer(out Vector2 screenPos, out PointerPhase phase, out int pointerId)
    {
        if (Touch.activeTouches.Count > 0)
        {
            var t = Touch.activeTouches[0];
            screenPos = t.screenPosition;
            pointerId = t.touchId;

            switch (t.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began: phase = PointerPhase.Began; return true;
                case UnityEngine.InputSystem.TouchPhase.Moved: phase = PointerPhase.Moved; return true;
                case UnityEngine.InputSystem.TouchPhase.Stationary: phase = PointerPhase.Stationary; return true;
                case UnityEngine.InputSystem.TouchPhase.Ended: phase = PointerPhase.Ended; return true;
                case UnityEngine.InputSystem.TouchPhase.Canceled: phase = PointerPhase.Canceled; return true;
            }
        }

        var mouse = Mouse.current;
        if (mouse != null)
        {
            screenPos = mouse.position.ReadValue();
            pointerId = -1;

            if (mouse.leftButton.wasPressedThisFrame) { phase = PointerPhase.Began; return true; }
            if (mouse.leftButton.isPressed) { phase = PointerPhase.Moved; return true; }
            if (mouse.leftButton.wasReleasedThisFrame) { phase = PointerPhase.Ended; return true; }
        }

        screenPos = default;
        phase = default;
        pointerId = -1;
        return false;
    }
    public static Vector2 ScreenToWorld(Camera cam, Vector2 screen)
    {
        Vector3 s = new Vector3(screen.x, screen.y, 0f);
        s.z = -cam.transform.position.z;
        Vector3 w = cam.ScreenToWorldPoint(s);
        return new Vector2(w.x, w.y);
    }

    public static bool IsPointerOverUI(int pointerId)
    {
        if (EventSystem.current == null) return false;

        // 터치면 pointerId 사용
        if (pointerId >= 0)
            return EventSystem.current.IsPointerOverGameObject(pointerId);

        // 마우스
        return EventSystem.current.IsPointerOverGameObject();
    }
}