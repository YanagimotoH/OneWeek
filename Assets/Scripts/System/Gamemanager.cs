using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    [Header("Stage Flip")]
    [SerializeField] float flipIntervalSeconds = 20f;
    [SerializeField] Color frontColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] Color backColor = new Color(0.2f, 0.1f, 0.4f, 1f);
    [SerializeField] GameObject frontBackgroundPanel;
    [SerializeField] GameObject backBackgroundPanel;
    [SerializeField] string backgroundSortingLayerName = "Default";
    [SerializeField] int backgroundSortingOrder = 0;

    public bool IsBackSide { get; private set; }

    float flipTimer;
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        ApplyStageColor();
        ApplyBackgroundPanels();
    }

    void Update()
    {
        flipTimer += Time.deltaTime;
        if (flipTimer >= flipIntervalSeconds)
        {
            flipTimer = 0f;
            ToggleStageSide();
        }
    }

    void ToggleStageSide()
    {
        IsBackSide = !IsBackSide;
        ApplyStageColor();
        ApplyBackgroundPanels();
    }

    void ApplyStageColor()
    {
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = IsBackSide ? backColor : frontColor;
        }
    }

    void ApplyBackgroundPanels()
    {
        ApplyPanelSorting(frontBackgroundPanel);
        ApplyPanelSorting(backBackgroundPanel);

        if (frontBackgroundPanel != null)
        {
            frontBackgroundPanel.SetActive(!IsBackSide);
        }

        if (backBackgroundPanel != null)
        {
            backBackgroundPanel.SetActive(IsBackSide);
        }
    }

    void ApplyPanelSorting(GameObject panel)
    {
        if (panel == null)
        {
            return;
        }

        Canvas panelCanvas = panel.GetComponent<Canvas>();
        if (panelCanvas == null)
        {
            panelCanvas = panel.GetComponentInParent<Canvas>();
        }

        if (panelCanvas == null)
        {
            return;
        }

        panelCanvas.overrideSorting = true;
        panelCanvas.sortingLayerName = backgroundSortingLayerName;
        panelCanvas.sortingOrder = backgroundSortingOrder;
    }
}
