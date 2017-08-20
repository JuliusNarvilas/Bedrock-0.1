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

        public abstract TPosition RotateGridOffset(TPosition i_Offset, int i_RotationSnapPoint);
        public abstract TPosition SnapToGrid(Transform i_Transform, Vector3 i_TileSize);
        public abstract void SnapToGrid(TPosition i_Position, Vector3 i_TileSize, int i_RotationSnapPoint, Transform o_Output);


        public abstract void DrawGridBounds(Vector3 i_WorldPosition, TPosition i_GridSize, Vector3 i_TileSize);
        public abstract void DrawObjectBounds(Vector3 i_GridOrigin, TPosition i_GridPosition, TPosition i_GridObjectSize, Vector3 i_TileSize);
        public abstract void DrawTile(Vector3 i_GridOrigin, TPosition i_GridPosition, Vector3 i_TileSize, int i_RotationSnapPoint, TTileSettings i_Settings, bool i_Selected, List<GridMapEditorDrawRef> o_DrawCalls);

        
        public void DrawObject(GridMapEditorBehaviour i_Editor, GridMapObjectBehaviour i_Obj)
        {
            var objRotation = i_Obj.transform.rotation;
            int rotationSnapPoint = RotationToSnapPoint(objRotation);
            GridPosition3D objOrigin = i_Obj.GetFinalGridPosition();
            Vector3 rootOriginPos = i_Editor.transform.position;
            
            int instanceId = i_Obj.GetInstanceID();

            int tileCount = i_Obj.Tiles.Count;
            for (int i = 0; i < tileCount; ++i)
            {
                var tile = i_Obj.Tiles[i];
                bool selected = (GridMapObjectBehaviour.ActiveGridObject == instanceId) && (GridMapObjectBehaviour.ActiveGridTileIndex == i);
                var adjustedTilePosition = GetAbsolutePosition(objOrigin, i_Obj.Size, tile.Position, rotationSnapPoint);
                 = RotateGridOffset(tile.Position, rotationSnapPoint);

                DrawTile(rootOriginPos, i_Editor.TileSize, objRotation, objOrigin + adjustedTilePosition, (int)tile.Settings | (int)tile.TileBlockerSettings, selected);
            }

            int connectionCount = i_Obj.Connections.Count;
            for (int i = 0; i < connectionCount; ++i)
            {
                var connection = i_Obj.Connections[i];
                var adjustedTilePosition = i_Editor.MapTypeData.RotateGridPosition(connection.Position, objRotation);
                //var adjustedDirectionType = i_Editor.MapTypeData.RotateGridDirectionType(((int)connection.TileLocation) & GridHelpers.GRID_TILE_LOCATION_STRIDE_MASK, objRotation);
                //DrawConnectionDisplay(rootOriginPos, i_Editor.TileSize, objRotation, objOrigin + adjustedTilePosition, adjustedDirectionType | (int)connection.Settings);
            }

            if (GridMapObjectBehaviour.ActiveGridObject == instanceId)
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = GridMapEditorBehaviour.NORMAL_TILE_COLOR;
            
            var rotatedSize = i_Editor.MapTypeData.RotateGridPosition(i_Obj.Size, i_Obj.transform.rotation);
            DrawAreaDisplay(i_Obj.transform.position, rotatedSize, i_Editor.TileSize);
        }
        
    }
}