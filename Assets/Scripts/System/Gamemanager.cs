using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    [Header("Stage Flip")]
    [SerializeField] float flipIntervalSeconds = 20f;
    [SerializeField] Color frontColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] Color backColor = new Color(0.2f, 0.1f, 0.4f, 1f);

    public bool IsBackSide { get; private set; }

    float flipTimer;
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        ApplyStageColor();
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
    }

    void ApplyStageColor()
    {
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = IsBackSide ? backColor : frontColor;
        }
    }
}
