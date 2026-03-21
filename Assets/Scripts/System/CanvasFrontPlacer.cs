using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CanvasFrontPlacer : MonoBehaviour
{
    [SerializeField] Canvas targetCanvas;
    [SerializeField] bool useOwnCanvas;
    [SerializeField] int orderOffset = 1;
    [SerializeField] bool setAsLastSibling = true;
    [SerializeField] bool addGraphicRaycaster = true;

    void Awake()
    {
        Apply();
    }

    void OnEnable()
    {
        Apply();
    }

    public void Apply()
    {
        if (targetCanvas == null)
        {
            targetCanvas = GetComponentInParent<Canvas>();
        }

        if (targetCanvas == null)
        {
            return;
        }

        if (transform.parent != targetCanvas.transform)
        {
            transform.SetParent(targetCanvas.transform, false);
        }

        if (useOwnCanvas)
        {
            Canvas ownCanvas = GetComponent<Canvas>();
            if (ownCanvas == null)
            {
                ownCanvas = gameObject.AddComponent<Canvas>();
            }

            ownCanvas.overrideSorting = true;
            ownCanvas.sortingLayerID = targetCanvas.sortingLayerID;
            ownCanvas.sortingOrder = GetTopSortingOrder(targetCanvas.sortingLayerID) + orderOffset;

            if (addGraphicRaycaster && GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        if (setAsLastSibling)
        {
            transform.SetAsLastSibling();
        }
    }

    int GetTopSortingOrder(int sortingLayerId)
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int maxOrder = targetCanvas.sortingOrder;
        foreach (Canvas canvas in canvases)
        {
            if (canvas == null || canvas.sortingLayerID != sortingLayerId)
            {
                continue;
            }

            if (canvas.sortingOrder > maxOrder)
            {
                maxOrder = canvas.sortingOrder;
            }
        }

        return maxOrder;
    }
}
