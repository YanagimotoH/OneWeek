using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Vector2 size = new Vector2(200f, 20f);
    [SerializeField] Vector2 offset = new Vector2(10f, -10f);
    [SerializeField] Color fillColor = new Color(0f, 1f, 0f, 1f);
    [SerializeField] Color backgroundColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    [SerializeField] string sortingLayerName = "UI";
    [SerializeField] int sortingOrder = 20;
    [SerializeField] Canvas canvas;

    RectTransform barRoot;
    Image fillImage;

    void Awake()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }

        EnsureCanvas();
        EnsureBar();
    }

    void OnEnable()
    {
        if (health != null)
        {
            health.HealthChanged += OnHealthChanged;
        }

        UpdateBar();
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.HealthChanged -= OnHealthChanged;
        }
    }

    void OnHealthChanged(int current, int max)
    {
        UpdateBar();
    }

    void EnsureCanvas()
    {
        if (canvas == null)
        {
            GameObject existing = GameObject.Find("PlayerHealthCanvas");
            if (existing != null)
            {
                canvas = existing.GetComponent<Canvas>();
            }
        }

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("PlayerHealthCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        canvas.overrideSorting = true;
        canvas.sortingLayerName = sortingLayerName;
        canvas.sortingOrder = sortingOrder;
    }

    void EnsureBar()
    {
        if (barRoot != null || canvas == null)
        {
            return;
        }

        barRoot = new GameObject("PlayerHealthBar").AddComponent<RectTransform>();
        barRoot.SetParent(canvas.transform, false);
        barRoot.anchorMin = new Vector2(0f, 1f);
        barRoot.anchorMax = new Vector2(0f, 1f);
        barRoot.pivot = new Vector2(0f, 1f);
        barRoot.anchoredPosition = offset;
        barRoot.sizeDelta = size;

        Image background = new GameObject("Background").AddComponent<Image>();
        background.transform.SetParent(barRoot, false);
        background.color = backgroundColor;
        RectTransform backgroundRect = background.rectTransform;
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        fillImage = new GameObject("Fill").AddComponent<Image>();
        fillImage.transform.SetParent(barRoot, false);
        fillImage.color = fillColor;
        RectTransform fillRect = fillImage.rectTransform;
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(0f, 1f);
        fillRect.pivot = new Vector2(0f, 0.5f);
        fillRect.anchoredPosition = Vector2.zero;
        fillRect.sizeDelta = new Vector2(size.x, 0f);
    }

    void UpdateBar()
    {
        if (health == null || fillImage == null)
        {
            return;
        }

        float ratio = health.MaxHp > 0 ? (float)health.CurrentHp / health.MaxHp : 0f;
        ratio = Mathf.Clamp01(ratio);
        RectTransform fillRect = fillImage.rectTransform;
        fillRect.sizeDelta = new Vector2(size.x * ratio, 0f);
    }
}
