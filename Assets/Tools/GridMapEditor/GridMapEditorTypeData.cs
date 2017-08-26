using Common.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public struct GridMapEditorDrawRef
    {
        public float Distance;
        public Action Draw;

        public GridMapEditorDrawRef(float i_Distance, Action i_DrawCall)
        {
            Distance = i_Distance;
            Draw = i_DrawCall;
        }
    }

    public abstract class GridMapEditorTypeData<TPosition, TTileSettings> : ScriptableObject
    {
        public abstract int RotationToSnapPoint(Quaternion i_Rotation);
        public abstract Quaternion SnapPointToRotation(int i_RotationSnapPoint);
        public abstract TPosition GetAbsolutePosition(TPosition i_Origin, TPosition i_Size, TPosition i_Offset, int i_OriginRotation);
        public abstract TPosition RotateGridVector(TPosition i_Direction, int i_RotationSnapPoint);

        public abstract TPosition SnapToGrid(Transform i_NewTransform, TPosition i_OldPosition, ref int i_OldRotation, TPosition i_ObjSize, Vector3 i_TileSize);
        public abstract void SnapToGrid(TPosition i_Position, TPosition i_ObjSize, Vector3 i_TileSize, int i_RotationSnapPoint, Transform o_Output);

        public abstract GameObject BuildObjectDebug(GridMapObjectBehaviour<TPosition, TTileSettings> i_Obj, Vector3 i_TileSize);
        public abstract GameObject BuildGridDebug(GridMapEditorBehaviour<TPosition, TTileSettings> i_Grid);

        [NonSerialized]
        public bool Updated;
    }
}