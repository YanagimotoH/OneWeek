using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] string scoreTextObjectName = "ScoreText";
    [SerializeField] Vector2 offset = new Vector2(10f, -40f);
    [SerializeField] string sortingLayerName = "UI";
    [SerializeField] int sortingOrder = 10;
    [SerializeField] string scoreFormat = "Score: {0}";
    [SerializeField] bool resetOnSceneLoad;
    [SerializeField] string resetSceneName = "GameScene";
    [SerializeField] string[] hideScoreSceneNames = new[] { "TitleScene" };

    static ScoreManager instance;

    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScoreManager>();
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
        SceneManager.sceneLoaded += OnSceneLoaded;
        HandleScene(SceneManager.GetActiveScene());
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (resetOnSceneLoad && !string.IsNullOrEmpty(resetSceneName) && scene.name == resetSceneName)
        {
            Score = 0;
        }

        HandleScene(scene);
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

    void HandleScene(Scene scene)
    {
        if (IsHiddenScene(scene.name))
        {
            DisableScoreDisplay();
            return;
        }

        scoreText = null;
        EnsureText();
        UpdateText();
    }

    bool IsHiddenScene(string sceneName)
    {
        if (hideScoreSceneNames == null || hideScoreSceneNames.Length == 0)
        {
            return false;
        }

        foreach (string hiddenScene in hideScoreSceneNames)
        {
            if (!string.IsNullOrEmpty(hiddenScene) && hiddenScene == sceneName)
            {
                return true;
            }
        }

        return false;
    }

    void DisableScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.enabled = false;
        }

        Canvas canvas = FindScoreCanvas();
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    Canvas FindScoreCanvas()
    {
        GameObject existing = GameObject.Find("ScoreCanvas");
        if (existing == null)
        {
            return null;
        }

        return existing.GetComponent<Canvas>();
    }

    void EnsureText()
    {
        if (scoreText == null && !string.IsNullOrEmpty(scoreTextObjectName))
        {
            GameObject existingText = GameObject.Find(scoreTextObjectName);
            if (existingText != null)
            {
                scoreText = existingText.GetComponent<Text>();
            }
        }

        Canvas canvas = FindScoreCanvas();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("ScoreCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        canvas.enabled = true;
        canvas.overrideSorting = true;
        canvas.sortingLayerName = sortingLayerName;
        canvas.sortingOrder = sortingOrder;

        if (scoreText != null)
        {
            scoreText.enabled = true;
            return;
        }

        GameObject textObject = new GameObject(scoreTextObjectName);
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
        if (scoreText == null || !scoreText.enabled)
        {
            return;
        }

        scoreText.text = string.Format(scoreFormat, Score);
    }
}
