using UnityEngine;

public class TilemapStageSwitcher : MonoBehaviour
{
    [SerializeField] Gamemanager gamemanager;
    [SerializeField] GameObject frontStageRoot;
    [SerializeField] GameObject backStageRoot;

    bool lastIsBackSide;

    void Start()
    {
        if (gamemanager == null)
        {
            gamemanager = FindObjectOfType<Gamemanager>();
        }

        ApplyState();
    }

    void Update()
    {
        if (gamemanager == null)
        {
            return;
        }

        if (gamemanager.IsBackSide == lastIsBackSide)
        {
            return;
        }

        ApplyState();
    }

    void ApplyState()
    {
        if (gamemanager == null)
        {
            return;
        }

        lastIsBackSide = gamemanager.IsBackSide;

        if (frontStageRoot != null)
        {
            frontStageRoot.SetActive(!lastIsBackSide);
        }

        if (backStageRoot != null)
        {
            backStageRoot.SetActive(lastIsBackSide);
        }
    }
}
