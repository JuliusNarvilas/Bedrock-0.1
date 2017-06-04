using Common.Graphics;
using Common.Grid;
using Common.Grid.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
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

    public class GridMapEditorCuboidTypeDrawing : IGridMapEditorTypeDrawing
    {
        public static readonly Quaternion ROTATION_90_DEG = Quaternion.Euler(0, 90, 0);

        public void DrawObject(GridMapEditorBehaviour i_Editor, GridMapObjectBehaviour i_Obj)
        {
            var objRotation = i_Obj.transform.rotation;
            GridPosition3D objOrigin = i_Obj.GetFinalGridPosition();
            Vector3 rootOriginPos = i_Editor.transform.position;
            
            int instanceId = i_Obj.GetInstanceID();

            int tileCount = i_Obj.Tiles.Count;
            for (int i = 0; i < tileCount; ++i)
            {
                var tile = i_Obj.Tiles[i];
                bool selected = (GridMapObjectBehaviour.ActiveGridObject == instanceId) && (GridMapObjectBehaviour.ActiveGridTileIndex == i);
                var adjustedTilePosition = i_Editor.MapTypeData.RotateGridPosition(tile.Position, objRotation);

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


        private void DrawAreaDisplay(Vector3 i_FinalGlobalPosition, GridPosition3D i_Size, Vector3 i_TileSize)
        {
            Vector3 sizeVec = new Vector3(i_Size.X * i_TileSize.x, i_Size.Z * i_TileSize.y, i_Size.Y * i_TileSize.z);
            i_FinalGlobalPosition += sizeVec * 0.5f;

            Gizmos.DrawWireCube(i_FinalGlobalPosition, sizeVec);
        }


        private void DrawTile(Vector3 i_Origin, Vector3 i_TileSize, Quaternion i_Rotation, GridPosition3D i_FinalPosition, int i_Settings, bool i_Selected)
        {
            Vector3 finalGlobalPosition = i_Origin;
            finalGlobalPosition.x += i_FinalPosition.X * i_TileSize.x;
            finalGlobalPosition.y += i_FinalPosition.Z * i_TileSize.y;
            finalGlobalPosition.z += i_FinalPosition.Y * i_TileSize.z;
            
            Vector3 halfTileSize = i_TileSize * 0.5f;
            Vector3 tileCentre = finalGlobalPosition + (i_Rotation * halfTileSize);
            tileCentre.y = 0;

            Vector3 localLeft = i_Rotation * Vector3.left;
            Vector3 localForward = (ROTATION_90_DEG * localLeft);
            Vector3 localRight = -localLeft;
            Vector3 localBack = -localForward;
            
            if (i_Selected)
                Gizmos.color = GridMapEditorBehaviour.SELECTED_TILE_COLOR;
            else
                Gizmos.color = GridMapEditorBehaviour.NORMAL_TILE_COLOR;


            List<GridMapEditorDrawRef> drawElements = new List<GridMapEditorDrawRef>(4);
            Vector3 camPos = SceneView.currentDrawingSceneView.camera.transform.position;
            
            drawElements.Add(new GridMapEditorDrawRef(
                (camPos - tileCentre).sqrMagnitude,
                () => {
                    Color lastColor = Gizmos.color;
                    if (i_Selected)
                        Gizmos.color = GridMapEditorBehaviour.SELECTED_EMPTY_TILE_COLOR;
                    else
                        Gizmos.color = GridMapEditorBehaviour.EMPTY_TILE_COLOR;
                    Gizmos.DrawCube(tileCentre, new Vector3(0.85f * i_TileSize.x, 0.05f * i_TileSize.y, 0.85f * i_TileSize.z));
                    Gizmos.color = lastColor;
                }));

            var orderedDraws = drawElements.OrderByDescending(x => x.Distance);
            foreach(var item in orderedDraws)
            {
                item.Draw();
            }
        }

        private void DrawWall(Vector3 i_Position, Vector3 i_InwardDirection, float i_Width, float i_Height)
        {
            if (i_Height > 0.1f)
            {
                const float WALL_EDGE_OFFSET_FACTOR = 0.07f;
                const float WALL_THICKNESS_FACTOR = 0.05f;
                const float WALL_WIDTH_FACTOR = 1.0f - (2.0f * WALL_EDGE_OFFSET_FACTOR);
                var adjustedPos = i_Position + (i_InwardDirection * ((i_Width * WALL_EDGE_OFFSET_FACTOR) + (i_Width * WALL_THICKNESS_FACTOR * 0.5f)));
                adjustedPos.y = i_Height * 0.5f;

                Vector3 size = (i_InwardDirection * WALL_THICKNESS_FACTOR) + (ROTATION_90_DEG * (i_InwardDirection * i_Width * WALL_WIDTH_FACTOR));
                Vector3 absSize = new Vector3(Mathf.Abs(size.x), i_Height, Mathf.Abs(size.z));

                Gizmos.DrawCube(adjustedPos, absSize);
            }
        }


        private static void DrawConnectionDirection(int i_Settings, Vector3 i_LocationPos, Vector3 i_InwardDirection)
        {
            Vector3 scaledDirection = i_InwardDirection * 0.5f;

            if ((i_Settings & (int)EConnectionSettings.Inward) != 0)
                DrawHelper.DrawArrow.ForGizmo(i_LocationPos, scaledDirection);
            if ((i_Settings & (int)EConnectionSettings.Outward) != 0)
                DrawHelper.DrawArrow.ForGizmo(i_LocationPos + scaledDirection, scaledDirection * -1.0f);
        }
    }
}
