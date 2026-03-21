using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultSceneUI : MonoBehaviour
{
    [SerializeField] string restartSceneName;
    [SerializeField] string titleSceneName = "TitleScene";
    [SerializeField] int sortingOrder = 20;
    [SerializeField] string scoreTextObjectName = "ResultScoreText";

    Canvas canvas;

    void Awake()
    {
        EnsureCanvas();
        BuildUI();
    }

    void EnsureCanvas()
    {
        GameObject canvasObject = new GameObject("ResultCanvas");
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
    }

    void BuildUI()
    {
        int score = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;

        Text scoreText = FindExistingScoreText();
        if (scoreText == null)
        {
            scoreText = CreateText("ScoreText", string.Format("Score: {0}", score), 32, TextAnchor.MiddleCenter);
            RectTransform scoreRect = scoreText.rectTransform;
            scoreRect.anchorMin = new Vector2(0.5f, 0.6f);
            scoreRect.anchorMax = new Vector2(0.5f, 0.6f);
            scoreRect.pivot = new Vector2(0.5f, 0.5f);
            scoreRect.sizeDelta = new Vector2(400f, 60f);
            scoreRect.anchoredPosition = Vector2.zero;
        }
        else
        {
            scoreText.text = string.Format("Score: {0}", score);
        }

        Button restartButton = CreateButton("RestartButton", "Restart");
        RectTransform restartRect = restartButton.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.5f, 0.4f);
        restartRect.anchorMax = new Vector2(0.5f, 0.4f);
        restartRect.pivot = new Vector2(0.5f, 0.5f);
        restartRect.sizeDelta = new Vector2(200f, 50f);
        restartRect.anchoredPosition = new Vector2(0f, 0f);
        restartButton.onClick.AddListener(RestartGame);

        Button titleButton = CreateButton("TitleButton", "Title");
        RectTransform titleRect = titleButton.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.3f);
        titleRect.anchorMax = new Vector2(0.5f, 0.3f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(200f, 50f);
        titleRect.anchoredPosition = new Vector2(0f, 0f);
        titleButton.onClick.AddListener(GoToTitle);
    }

    Text FindExistingScoreText()
    {
        if (string.IsNullOrEmpty(scoreTextObjectName))
        {
            return null;
        }

        GameObject existing = GameObject.Find(scoreTextObjectName);
        if (existing == null)
        {
            return null;
        }

        return existing.GetComponent<Text>();
    }

    Text CreateText(string name, string text, int fontSize, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(canvas.transform, false);
        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.color = Color.white;
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        return textComponent;
    }

    Button CreateButton(string name, string label)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(canvas.transform, false);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        Button button = buttonObject.AddComponent<Button>();

        Text labelText = CreateText(name + "Label", label, 24, TextAnchor.MiddleCenter);
        labelText.transform.SetParent(buttonObject.transform, false);
        RectTransform labelRect = labelText.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        return button;
    }

    void RestartGame()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        string targetScene = !string.IsNullOrEmpty(restartSceneName)
            ? restartSceneName
            : ScoreManager.Instance != null ? ScoreManager.Instance.LastGameplayScene : string.Empty;

        if (string.IsNullOrEmpty(targetScene))
        {
            return;
        }

        SceneManager.LoadScene(targetScene);
    }

    void GoToTitle()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        if (string.IsNullOrEmpty(titleSceneName))
        {
            return;
        }

        SceneManager.LoadScene(titleSceneName);
    }
}
