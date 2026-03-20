using UnityEngine;

public class WorldHealthBar : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Vector3 offset = new Vector3(0f, 0.6f, 0f);
    [SerializeField] Vector2 size = new Vector2(0.6f, 0.08f);
    [SerializeField] Color fillColor = new Color(0f, 1f, 0f, 1f);
    [SerializeField] Color backgroundColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    [SerializeField] int sortingOrder = 10;

    Transform barRoot;
    SpriteRenderer backgroundRenderer;
    SpriteRenderer fillRenderer;

    static Sprite whiteSprite;
    static Sprite leftPivotSprite;

    void Awake()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }

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

    void LateUpdate()
    {
        if (barRoot != null)
        {
            barRoot.position = transform.position + offset;
            barRoot.rotation = Quaternion.identity;
        }
    }

    void OnHealthChanged(int current, int max)
    {
        UpdateBar();
    }

    void EnsureBar()
    {
        if (barRoot != null)
        {
            return;
        }

        GameObject rootObject = new GameObject("HealthBar");
        rootObject.transform.SetParent(transform, false);
        barRoot = rootObject.transform;
        barRoot.localPosition = offset;

        Sprite leftSprite = GetLeftPivotSprite();
        backgroundRenderer = CreateBarRenderer("Background", backgroundColor, sortingOrder, leftSprite);
        fillRenderer = CreateBarRenderer("Fill", fillColor, sortingOrder + 1, leftSprite);

        backgroundRenderer.transform.localScale = new Vector3(size.x, size.y, 1f);
        Vector3 leftAnchor = new Vector3(-size.x * 0.5f, 0f, 0f);
        backgroundRenderer.transform.localPosition = leftAnchor;
        fillRenderer.transform.localPosition = leftAnchor;
    }

    SpriteRenderer CreateBarRenderer(string name, Color color, int order, Sprite sprite)
    {
        GameObject barObject = new GameObject(name);
        barObject.transform.SetParent(barRoot, false);
        SpriteRenderer rendererComponent = barObject.AddComponent<SpriteRenderer>();
        rendererComponent.sprite = sprite;
        rendererComponent.color = color;
        rendererComponent.sortingOrder = order;
        return rendererComponent;
    }

    void UpdateBar()
    {
        if (health == null || fillRenderer == null || backgroundRenderer == null)
        {
            return;
        }

        float ratio = health.MaxHp > 0 ? (float)health.CurrentHp / health.MaxHp : 0f;
        float fillWidth = size.x * Mathf.Clamp01(ratio);
        fillRenderer.transform.localScale = new Vector3(fillWidth, size.y, 1f);
    }

    static Sprite GetWhiteSprite()
    {
        if (whiteSprite != null)
        {
            return whiteSprite;
        }

        Texture2D texture = Texture2D.whiteTexture;
        whiteSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f);
        return whiteSprite;
    }

    static Sprite GetLeftPivotSprite()
    {
        if (leftPivotSprite != null)
        {
            return leftPivotSprite;
        }

        Texture2D texture = Texture2D.whiteTexture;
        leftPivotSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0f, 0.5f), 1f);
        return leftPivotSprite;
    }
}
