using Common.Grid;
using UnityEngine;

namespace Tools
{   
    public class GridMapEditorCuboidTypeData : IGridMapEditorTypeData
    {
        public GridPosition3D RotateGridPosition(GridPosition3D i_Position, Quaternion i_Rotation)
        {
            int rotationSnapPoint = GetRotationSnapPoint(i_Rotation);
            switch (rotationSnapPoint)
            {
                case 0:
                    return i_Position;
                case 1:
                    return new GridPosition3D(i_Position.Y, -i_Position.X, i_Position.Z);
                case 2:
                    return new GridPosition3D(-i_Position.X, -i_Position.Y, i_Position.Z);
                case 3:
                    return new GridPosition3D(-i_Position.Y, i_Position.X, i_Position.Z);
                default:
                    return i_Position;
            }
        }

        public int RotateGridDirectionType(int directionType, Quaternion i_Rotation)
        {
            if (directionType >= 4)
                return directionType;

            int rotationSnapPoint = GetRotationSnapPoint(i_Rotation);
            return (directionType + rotationSnapPoint) % 4;
        }

        public Quaternion SnapRotationToGrid(Quaternion i_Rotation)
        {
            int rotationSnapPoint = GetRotationSnapPoint(i_Rotation);
            float finalRotation = rotationSnapPoint * 90.0f;
            return Quaternion.Euler(0, finalRotation, 0);
        }

        private int GetRotationSnapPoint(Quaternion i_Rotation)
        {
            var eulerRotation = i_Rotation.eulerAngles.y;
            while (eulerRotation < 0)
            {
                eulerRotation += 360.0f;
            }
            return (int)((eulerRotation + 45.0f) / 90.0f) % 4;
        }
    }
}
