using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Launcher : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform launchOrigin;
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform arrow;

    [Header("Power")]
    [SerializeField] private float maxPullWorld = 2.5f;
    [SerializeField] private float maxImpulse = 18f;
    [SerializeField] private AnimationCurve powerCurve = AnimationCurve.Linear(0f, 0.0f, 1f, 1f);

    [Header("Arrow Visual (Scale Y Only)")]
    [SerializeField] private float arrowMinHeight = 0.2f;
    [SerializeField] private float arrowMaxHeight = 1.2f;

    [Header("Arrow Offset")]
    [SerializeField] private float arrowGapWorld = 0.35f;
    [SerializeField] private float arrowAngleOffsetDeg = 0f;

    [Header("Up Only")]
    [SerializeField] private float minUpPullY = 0.05f;

    [Header("Grab Rule")]
    [SerializeField] private float grabRadiusWorld = 0.25f;

    private Camera cam;
    private bool aiming;
    private Vector2 pullClamped;
    private Vector3 arrowBaseScale;

    private enum PointerPhase { Began, Moved, Stationary, Ended, Canceled }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        cam = Camera.main;

        if (arrow != null)
        {
            arrowBaseScale = arrow.localScale;
            arrow.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (arrow != null) arrow.gameObject.SetActive(false);
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if (cam == null) cam = Camera.main;

        if (!TryGetPointer(out Vector2 screenPos, out PointerPhase phase))
            return;

        if (phase == PointerPhase.Began)
        {
            aiming = false;
            pullClamped = Vector2.zero;

            if (projectile == null || launchOrigin == null)
                return;

            Vector2 w = ScreenToWorld(screenPos);

            if (!projectile.IsGrabbed(w, grabRadiusWorld))
                return;

            aiming = true;

            if (arrow != null) arrow.gameObject.SetActive(true);

            projectile.BeginAim(launchOrigin.position);
            return;
        }

        if (!aiming)
            return;

        if (phase == PointerPhase.Moved || phase == PointerPhase.Stationary)
        {
            if (launchOrigin == null || projectile == null) return;

            Vector2 origin = launchOrigin.position;
            Vector2 pointerWorld = ScreenToWorld(screenPos);

            Vector2 pull = origin - pointerWorld;
            if (pull.y < 0f) pull.y = 0f;

            if (pull.magnitude > maxPullWorld)
                pull = pull.normalized * maxPullWorld;

            pullClamped = pull;

            float power01 = Mathf.Clamp01(pull.magnitude / maxPullWorld);
            float eased = powerCurve.Evaluate(power01);

            UpdateArrow(pull, eased);

            projectile.HoldAt(origin);
        }

        if (phase == PointerPhase.Ended || phase == PointerPhase.Canceled)
        {
            Fire();

            aiming = false;
            if (arrow != null) arrow.gameObject.SetActive(false);
        }
    }

    private void Fire()
    {
        if (projectile == null || launchOrigin == null)
            return;

        Vector2 origin = launchOrigin.position;

        if (pullClamped.y < minUpPullY)
        {
            projectile.CancelAim(origin);
            return;
        }

        float power01 = Mathf.Clamp01(pullClamped.magnitude / maxPullWorld);
        float eased = powerCurve.Evaluate(power01);

        if (eased <= 0.001f)
        {
            projectile.CancelAim(origin);
            return;
        }

        Vector2 dir = pullClamped.normalized;
        Vector2 impulse = dir * (maxImpulse * eased);

        projectile.Shoot(origin, impulse);
    }

    private void UpdateArrow(Vector2 pull, float easedPower01)
    {
        if (arrow == null || launchOrigin == null)
            return;

        Vector2 dir = pull.sqrMagnitude > 0.0001f ? pull.normalized : Vector2.up;

        Vector2 origin = launchOrigin.position;
        Vector2 pos = origin + dir * arrowGapWorld;
        arrow.position = pos;

        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        ang = ang - 90f + arrowAngleOffsetDeg;
        arrow.rotation = Quaternion.Euler(0f, 0f, ang);

        float h = Mathf.Lerp(arrowMinHeight, arrowMaxHeight, easedPower01);
        arrow.localScale = new Vector3(arrowBaseScale.x, h, arrowBaseScale.z);
    }

    private Vector2 ScreenToWorld(Vector2 screen)
    {
        Vector3 s = new Vector3(screen.x, screen.y, 0f);
        s.z = -cam.transform.position.z;
        Vector3 w = cam.ScreenToWorldPoint(s);
        return new Vector2(w.x, w.y);
    }

    private bool TryGetPointer(out Vector2 screenPos, out PointerPhase phase)
    {
        if (Touch.activeTouches.Count > 0)
        {
            var t = Touch.activeTouches[0];
            screenPos = t.screenPosition;

            switch (t.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    phase = PointerPhase.Began;
                    return true;

                case UnityEngine.InputSystem.TouchPhase.Moved:
                    phase = PointerPhase.Moved;
                    return true;

                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    phase = PointerPhase.Stationary;
                    return true;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                    phase = PointerPhase.Ended;
                    return true;

                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    phase = PointerPhase.Canceled;
                    return true;
            }
        }

        var mouse = Mouse.current;
        if (mouse != null)
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                screenPos = mouse.position.ReadValue();
                phase = PointerPhase.Began;
                return true;
            }

            if (mouse.leftButton.isPressed)
            {
                screenPos = mouse.position.ReadValue();
                phase = PointerPhase.Moved;
                return true;
            }

            if (mouse.leftButton.wasReleasedThisFrame)
            {
                screenPos = mouse.position.ReadValue();
                phase = PointerPhase.Ended;
                return true;
            }
        }

        screenPos = default;
        phase = default;
        return false;
    }
}