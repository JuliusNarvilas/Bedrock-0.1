using Common.Grid;
using UnityEngine;

namespace Tools
{
    public interface IGridMapEditorTypeData
    {
        GridPosition3D RotateGridPosition(GridPosition3D i_Position, Quaternion i_Rotation);
        int RotateGridDirectionType(int directionType, Quaternion i_Rotation);
        Quaternion SnapRotationToGrid(Quaternion i_Rotation);
    }
}