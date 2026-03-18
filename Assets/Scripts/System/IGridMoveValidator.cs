using UnityEngine;

public interface IGridMoveValidator
{
    bool CanMoveTo(Vector3 target);
}
