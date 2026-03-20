using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Vector2 offset = new Vector2(10f, -40f);
    [SerializeField] int sortingOrder = 10;
    [SerializeField] string scoreFormat = "Score: {0}";

    static ScoreManager instance;

    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                ScoreManager existing = FindObjectOfType<ScoreManager>();
                if (existing != null)
                {
                    instance = existing;
                }
                else
                {
                    GameObject scoreObject = new GameObject("ScoreManager");
                    instance = scoreObject.AddComponent<ScoreManager>();
                }
            }

            return instance;
        }
    }

    public int Score { get; private set; }
    public string LastGameplayScene { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureText();
        UpdateText();
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Score += amount;
        UpdateText();
    }

    public void ResetScore()
    {
        Score = 0;
        UpdateText();
    }

    public void SetLastGameplayScene(string sceneName)
    {
        LastGameplayScene = sceneName;
    }

    void EnsureText()
    {
        if (scoreText != null)
        {
            return;
        }

        GameObject existing = GameObject.Find("ScoreCanvas");
        Canvas canvas = null;
        if (existing != null)
        {
            canvas = existing.GetComponent<Canvas>();
        }

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("ScoreCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject textObject = new GameObject("ScoreText");
        textObject.transform.SetParent(canvas.transform, false);
        scoreText = textObject.AddComponent<Text>();
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.color = Color.white;
        scoreText.alignment = TextAnchor.UpperLeft;

        RectTransform rect = scoreText.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = offset;
        rect.sizeDelta = new Vector2(300f, 30f);
    }

    void UpdateText()
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.text = string.Format(scoreFormat, Score);
    }
}
