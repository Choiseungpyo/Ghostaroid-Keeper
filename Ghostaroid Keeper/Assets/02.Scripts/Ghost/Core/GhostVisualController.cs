using UnityEngine;
using DG.Tweening;

public sealed class GhostVisualController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer baseSr;
    [SerializeField] private SpriteRenderer staticSr;

    [Header("Flicker")]
    [SerializeField] private float jitter = 0.02f;          // 스프라이트의 로컬 위치 랜덤 이동 값
    [SerializeField] private float flickerHzMin = 20f;      // 치지직 깜빡임 속도 범위 최소
    [SerializeField] private float flickerHzMax = 45f;      // 치지직 깜빡임 속도 범위 최대
    [SerializeField] private float staticAlphaMin = 0.15f;  // 치지직 스프라이트의 알파가 랜덤 범위 최소
    [SerializeField] private float staticAlphaMax = 0.95f;  // 치지직 스프라이트의 알파가 랜덤 범위 최대

    private Tweener baseTween;
    private Tweener flickerTween;
    private Tweener jitterTween;

    private void Awake()
    {
        if (staticSr != null)
        {
            staticSr.enabled = false;
            SetAlpha(staticSr, 0f);
            staticSr.transform.localPosition = Vector3.zero;
        }
    }

    public void Show(float duration)
    {
        KillAll();

        if (baseSr == null) return;

        if (staticSr != null)
        {
            staticSr.enabled = true;
            SetAlpha(staticSr, staticAlphaMax);
            StartFlicker();
            StartJitter();
        }

        SetAlpha(baseSr, 0f);
        baseTween = baseSr.DOFade(1f, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(StopStatic);
    }

    public void Hide(float duration)
    {
        KillAll();

        if (baseSr == null) return;

        StopStatic();
        baseTween = baseSr.DOFade(0f, duration).SetEase(Ease.InOutSine);
    }

    public void SetInstantVisible(bool visible)
    {
        KillAll();
        StopStatic();

        if (baseSr == null) return;
        SetAlpha(baseSr, visible ? 1f : 0f);
    }

    private void StartFlicker()
    {
        if (staticSr == null) return;

        float firstA = Random.Range(staticAlphaMin, staticAlphaMax);
        float firstT = GetFlickerStepTime();

        flickerTween = DOTween.To(
                () => staticSr.color.a,
                a => SetAlpha(staticSr, a),
                firstA,
                firstT
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .OnStepComplete(() =>
            {
                float nextA = Random.Range(staticAlphaMin, staticAlphaMax);
                float t = GetFlickerStepTime();
                flickerTween.ChangeEndValue(nextA, t, true);
            });
    }

    private void StartJitter()
    {
        if (staticSr == null) return;

        jitterTween = DOTween.To(
                () => staticSr.transform.localPosition,
                p => staticSr.transform.localPosition = p,
                Vector3.zero,
                0.02f
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .OnStepComplete(() =>
            {
                float x = Random.Range(-jitter, jitter);
                float y = Random.Range(-jitter, jitter);
                staticSr.transform.localPosition = new Vector3(x, y, 0f);
            });
    }

    private float GetFlickerStepTime()
    {
        float hz = Random.Range(flickerHzMin, flickerHzMax);
        if (hz < 1f) hz = 1f;
        return 1f / hz;
    }

    private void StopStatic()
    {
        if (flickerTween != null) flickerTween.Kill();
        flickerTween = null;

        if (jitterTween != null) jitterTween.Kill();
        jitterTween = null;

        if (staticSr != null)
        {
            SetAlpha(staticSr, 0f);
            staticSr.enabled = false;
            staticSr.transform.localPosition = Vector3.zero;
        }
    }

    private void KillAll()
    {
        if (baseTween != null) baseTween.Kill();
        baseTween = null;

        StopStatic();
    }

    private static void SetAlpha(SpriteRenderer sr, float a)
    {
        if (sr == null) return;
        Color color = sr.color;
        color.a = a;
        sr.color = color;
    }
}