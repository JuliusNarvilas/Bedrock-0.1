using Common.Graphics;
using Common.Grid;
using Common.Grid.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tools
{
    public class GridMapEditorTileDrawing
    {
        public void DrawTiles(GridMapEditorBehaviour i_Editor, GridMapObjectBehaviour i_Obj)
        {
            Vector3 originPos = i_Editor.transform.position;
            var objRotation = i_Obj.transform.rotation;
            var tileDirectinoModifier = i_Editor.EditorMapTypeData.RotateGridPosition(new GridPosition3D(1, 1, 1), objRotation);
            
            int instanceId = i_Obj.GetInstanceID();
            GridPosition3D objOrigin = i_Obj.GetFinalGridPosition();

            int tileCount = i_Obj.Tiles.Count;
            for (int i = 0; i < tileCount; ++i)
            {
                var tile = i_Obj.Tiles[i];
                bool selected = (GridMapObjectBehaviour.ActiveGridObject == instanceId) && (GridMapObjectBehaviour.ActiveGridTileIndex == i);
                var adjustedTilePosition = i_Editor.EditorMapTypeData.RotateGridPosition(tile.Position, objRotation);

                DrawTileDisplay(originPos, i_Editor.TileSize, objRotation, objOrigin + adjustedTilePosition, (int)tile.Settings | (int)tile.TileBlockerSettings, selected);
            }

            int connectionCount = i_Obj.Connections.Count;
            for (int i = 0; i < connectionCount; ++i)
            {
                var connection = i_Obj.Connections[i];

                DrawConnectionDisplay(originPos, i_Editor.TileSize, objOrigin + connection.Position, (int)connection.TileLocation | (int)connection.Settings);
            }

            if (GridMapObjectBehaviour.ActiveGridObject == instanceId)
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = GridMapEditorBehaviour.NORMAL_TILE_COLOR;
        }



        private void DrawTileDisplay(Vector3 i_Origin, Vector3 i_TileSize, Quaternion i_Rotation, GridPosition3D i_FinalPosition, int i_Settings, bool i_Selected)
        {
            Vector3 finalGlobalPosition = i_Origin;
            finalGlobalPosition.x += i_FinalPosition.X * i_TileSize.x;
            finalGlobalPosition.y += i_FinalPosition.Z * i_TileSize.y;
            finalGlobalPosition.z += i_FinalPosition.Y * i_TileSize.z;

            Gizmos.DrawSphere(finalGlobalPosition, 0.2f);


            Vector4 heights = GetHeights((GridTileBlockerFlags)i_Settings) * i_TileSize.y;

            Vector3 fullBlockerSize = i_Rotation * new Vector3(0.1f, i_TileSize.y, i_TileSize.z * 0.9f);
            Vector3 rotatedFullBlockerSize = Quaternion.Euler(0, 90, 0) * fullBlockerSize;




















            Vector3 finalBasePos = finalGlobalPosition;
            var rotatedTileSize = i_Rotation * i_TileSize;
            finalBasePos.x += rotatedTileSize.x * 0.5f;
            finalBasePos.z += rotatedTileSize.z * 0.5f;
            if ((i_Settings & (int)GridTileBlockerFlags.BottomBlocker) == (int)GridTileBlockerFlags.BottomBlocker)
            {
                if (i_Selected)
                    Gizmos.color = GridMapEditorBehaviour.SELECTED_TILE_COLOR;
                else
                    Gizmos.color = GridMapEditorBehaviour.NORMAL_TILE_COLOR;

                Gizmos.DrawCube(finalBasePos, new Vector3(0.9f * i_TileSize.x, 0.1f * i_TileSize.y, 0.9f * i_TileSize.z));
            }
            else
            {
                if (i_Selected)
                    Gizmos.color = GridMapEditorBehaviour.SELECTED_EMPTY_TILE_COLOR;
                else
                    Gizmos.color = GridMapEditorBehaviour.EMPTY_TILE_COLOR;

                Gizmos.DrawCube(finalBasePos, new Vector3(0.9f * i_TileSize.x, 0f, 0.9f * i_TileSize.z));
            }

            var forwardDirection = i_Rotation * Vector3.forward;
            var rightDirection = i_Rotation * Vector3.right;

            if (i_Selected)
                Gizmos.color = GridMapEditorBehaviour.SELECTED_TILE_COLOR;
            else
                Gizmos.color = GridMapEditorBehaviour.NORMAL_TILE_COLOR;

            if (heights.x > 0.001f)
            {
                Vector3 size = fullBlockerSize;
                size.y = heights.x;
                Vector3 blockerPos = finalGlobalPosition + (size * 0.5f);
                Gizmos.DrawCube(blockerPos, size);
            }
            if (heights.y > 0.001f)
            {
                Vector3 size = rotatedFullBlockerSize;
                size.y = heights.y;
                Vector3 blockerPos = finalGlobalPosition + (size * 0.5f);
                blockerPos += new Vector3(i_TileSize.x * forwardDirection.x, i_TileSize.y * forwardDirection.y, i_TileSize.z * forwardDirection.z);
                Gizmos.DrawCube(blockerPos, size);
            }
            if (heights.z > 0.001f)
            {
                Vector3 size = fullBlockerSize;
                size.y = heights.z;
                Vector3 blockerPos = finalGlobalPosition + (size * 0.5f);
                blockerPos += new Vector3(i_TileSize.x * rightDirection.x, i_TileSize.y * rightDirection.y, i_TileSize.z * rightDirection.z);
                Gizmos.DrawCube(blockerPos, size);
            }
            if (heights.w > 0.001f)
            {
                Vector3 size = rotatedFullBlockerSize;
                size.y = heights.w;
                Vector3 blockerPos = finalGlobalPosition + (size * 0.5f);
                blockerPos -= new Vector3(rotatedFullBlockerSize.x * forwardDirection.x, rotatedFullBlockerSize.y * forwardDirection.y, rotatedFullBlockerSize.z * forwardDirection.z);
                Gizmos.DrawCube(blockerPos, size);
            }
        }

        private static Vector4 GetHeights(GridTileBlockerFlags blockerFlags)
        {
            Vector4 heights;
            heights.x = GridMapEditorBehaviour.GetBlockerDirectionHeight(blockerFlags, GridTileLocation.Left);
            heights.y = GridMapEditorBehaviour.GetBlockerDirectionHeight(blockerFlags, GridTileLocation.Forward);
            heights.z = GridMapEditorBehaviour.GetBlockerDirectionHeight(blockerFlags, GridTileLocation.Right);
            heights.w = GridMapEditorBehaviour.GetBlockerDirectionHeight(blockerFlags, GridTileLocation.Backward);

            return heights;
        }

        private void DrawConnectionDisplay(Vector3 i_Origin, Vector3 i_TileSize, GridPosition3D i_FinalPosition, int i_Settings)
        {
            Gizmos.color = Color.black;

            Vector3 finalGlobalPosition = i_Origin;
            finalGlobalPosition.x += i_FinalPosition.X * i_TileSize.x;
            finalGlobalPosition.y += i_FinalPosition.Z * i_TileSize.y;
            finalGlobalPosition.z += i_FinalPosition.Y * i_TileSize.z;

            GridTileLocation location = (GridTileLocation)(i_Settings & GridHelpers.GRID_TILE_LOCATION_STRIDE_MASK);
            Vector3 locationPos = finalGlobalPosition;

            switch (location)
            {
                case GridTileLocation.Left:
                    //TODO add rotation
                    locationPos.y += i_TileSize.y * 0.5f;
                    locationPos.z += i_TileSize.z * 0.5f;
                    DrawConnectionDirection(i_Settings, locationPos, Vector3.right);
                    break;
                case GridTileLocation.Right:
                    //TODO add rotation
                    locationPos.x += i_TileSize.x;
                    locationPos.y += i_TileSize.y * 0.5f;
                    locationPos.z += i_TileSize.z * 0.5f;
                    DrawConnectionDirection(i_Settings, locationPos, Vector3.left);
                    break;
                case GridTileLocation.Forward:
                    //TODO add rotation
                    locationPos.x += i_TileSize.x * 0.5f;
                    locationPos.y += i_TileSize.y * 0.5f;
                    locationPos.z += i_TileSize.z;
                    DrawConnectionDirection(i_Settings, locationPos, Vector3.back);
                    break;
                case GridTileLocation.Backward:
                    //TODO add rotation
                    locationPos.x += i_TileSize.x * 0.5f;
                    locationPos.y += i_TileSize.y * 0.5f;
                    DrawConnectionDirection(i_Settings, locationPos, Vector3.forward);
                    break;
                case GridTileLocation.Top:
                    //TODO add rotation
                    locationPos.x += i_TileSize.x * 0.5f;
                    locationPos.y += i_TileSize.y;
                    locationPos.z += i_TileSize.z * 0.5f;
                    DrawConnectionDirection(i_Settings, locationPos, Vector3.down);
                    break;
                case GridTileLocation.Bottom:
                    //TODO add rotation
                    locationPos.x += i_TileSize.x * 0.5f;
                    locationPos.z += i_TileSize.z * 0.5f;
                    DrawConnectionDirection(i_Settings, locationPos, Vector3.up);
                    break;
            }
        }


        private static void DrawConnectionDirection(int i_Settings, Vector3 i_LocationPos, Vector3 i_InwardDirection)
        {
            Vector3 scaledDirection = i_InwardDirection * 0.5f;

            if ((i_Settings & (int)ConnectionSettings.Inward) != 0)
                DrawHelper.DrawArrow.ForGizmo(i_LocationPos, scaledDirection);
            if ((i_Settings & (int)ConnectionSettings.Outward) != 0)
                DrawHelper.DrawArrow.ForGizmo(i_LocationPos + scaledDirection, scaledDirection * -1.0f);
        }
    }
}
