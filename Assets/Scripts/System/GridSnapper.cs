using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class GridSnapper : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] bool snapOnEnable = true;

    void OnEnable()
    {
        if (!snapOnEnable)
        {
            return;
        }

        SnapToGrid();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!snapOnEnable)
        {
            return;
        }

        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            SnapToGrid();
        }
    }
#endif

    public void SnapToGrid()
    {
        if (grid == null)
        {
            return;
        }

        Vector3Int cell = grid.WorldToCell(transform.position);
        Vector3 target = grid.GetCellCenterWorld(cell);
        target.z = transform.position.z;
        transform.position = target;
    }
}
