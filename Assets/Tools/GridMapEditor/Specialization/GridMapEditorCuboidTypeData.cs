using System;
using Common.Grid;
using UnityEngine;
using Common;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using Common.Utility;
using Common.Graphics;
using Common.Grid.Generation;

namespace Tools
{   
    public class GridMapEditorCuboidTypeData : GridMapEditorTypeData<GridPosition3D, int>
    {
        public static readonly Quaternion ROTATION_90_DEG = Quaternion.Euler(0, 90, 0);

        [MenuItem("Assets/Create/Grid/MapEditorTypeData/Cuboid")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<GridMapEditorCuboidTypeData>();
        }

        public override int RotationToSnapPoint(Quaternion i_Rotation)
        {
            var eulerRotation = i_Rotation.eulerAngles.y;
            while (eulerRotation < 0)
            {
                eulerRotation += 360.0f;
            }
            return (int)((eulerRotation + 45.0f) / 90.0f) % 4;
        }

        public override Quaternion SnapPointToRotation(int i_RotationSnapPoint)
        {
            float finalRotation = i_RotationSnapPoint * 90.0f;
            return Quaternion.Euler(0, finalRotation, 0);
        }

        public override GridPosition3D GetAbsolutePosition(GridPosition3D i_Origin, GridPosition3D i_Size, GridPosition3D i_Offset, int i_OriginRotation)
        {
            switch (i_OriginRotation)
            {
                case 0:
                    break;
                case 1:
                    i_Offset = new GridPosition3D(i_Offset.Z, i_Offset.Y, i_Size.X - i_Offset.X);
                    break;
                case 2:
                    i_Offset = new GridPosition3D(i_Size.X - i_Offset.X, i_Offset.Y, i_Size.Z - i_Offset.Z);
                    break;
                case 3:
                    i_Offset = new GridPosition3D(i_Size.Z - i_Offset.Z, i_Offset.Y, i_Offset.X);
                    break;
                default:
                    Log.DebugLogWarning("GridMapEditorCuboidTypeData.GetAbsolutePosition: Invalid rotation: {0}", i_OriginRotation);
                    break;
            }
            return i_Origin + i_Offset;
        }



        public override GridPosition3D RotateGridVector(GridPosition3D i_Direction, int i_Rotation)
        {
            switch (i_Rotation)
            {
                case 0:
                    break;
                case 1:
                    i_Direction = new GridPosition3D(i_Direction.Z, i_Direction.Y, -i_Direction.X);
                    break;
                case 2:
                    i_Direction = new GridPosition3D(-i_Direction.X, i_Direction.Y, -i_Direction.Z);
                    break;
                case 3:
                    i_Direction = new GridPosition3D(- i_Direction.Z, i_Direction.Y, i_Direction.X);
                    break;
                default:
                    Log.DebugLogWarning("GridMapEditorCuboidTypeData.RotateGridDirection: Invalid rotation: {0}", i_Rotation);
                    break;
            }
            return i_Direction;
        }

        public override GridPosition3D SnapToGrid(Transform i_Transform, Vector3 i_TileSize)
        {
            i_Transform.localRotation = SnapPointToRotation(RotationToSnapPoint(i_Transform.localRotation));
            Vector3 halfSize = i_TileSize * 0.5f;
            Vector3 position = i_Transform.localPosition + halfSize;

            return new GridPosition3D(
                Mathf.FloorToInt(position.x / i_TileSize.x),
                Mathf.FloorToInt(position.z / i_TileSize.z),
                Mathf.FloorToInt(position.y / i_TileSize.y)
                );
        }

        public override void SnapToGrid(GridPosition3D i_Position, Vector3 i_TileSize, int i_RotationSnapPoint, Transform o_Output)
        {
            o_Output.localRotation = SnapPointToRotation(i_RotationSnapPoint);
            o_Output.localPosition = new Vector3(i_Position.X * i_TileSize.x, i_Position.Z * i_TileSize.y, i_Position.Y * i_TileSize.z);
        }

        public override void DrawGridBounds(Vector3 i_WorldPosition, GridPosition3D i_GridSize, Vector3 i_TileSize)
        {
            Vector3 sizeVec = new Vector3(i_GridSize.X * i_TileSize.x, i_GridSize.Z * i_TileSize.y, i_GridSize.Y * i_TileSize.z);
            i_WorldPosition += sizeVec * 0.5f;
            Gizmos.DrawWireCube(i_WorldPosition, sizeVec);
        }

        public override void DrawObjectBounds(Vector3 i_GridOrigin, GridPosition3D i_GridPosition, GridPosition3D i_GridObjectSize, Vector3 i_TileSize, int i_RotationSnapPoint)
        {
            i_GridObjectSize = RotateGridVector(i_GridObjectSize, i_RotationSnapPoint);
            Vector3 gridOffset = new Vector3(i_GridPosition.X * i_TileSize.x, i_GridPosition.Y * i_TileSize.y, i_GridPosition.Z * i_TileSize.z);
            var worldPos = i_GridOrigin + gridOffset;
            Vector3 sizeVec = new Vector3(i_GridObjectSize.X * i_TileSize.x, i_GridObjectSize.Z * i_TileSize.y, i_GridObjectSize.Y * i_TileSize.z);
            worldPos += sizeVec * 0.5f;

            Gizmos.DrawWireCube(worldPos, sizeVec);
        }

        public override void DrawTile(Vector3 i_GridOrigin, GridPosition3D i_GridPosition, Vector3 i_TileSize, int i_RotationSnapPoint, int i_Settings, bool i_Selected, List<GridMapEditorDrawRef> o_DrawCalls)
        {
            Vector3 finalGlobalPosition = i_GridOrigin;
            finalGlobalPosition.x += i_GridPosition.X * i_TileSize.x;
            finalGlobalPosition.y += i_GridPosition.Z * i_TileSize.y;
            finalGlobalPosition.z += i_GridPosition.Y * i_TileSize.z;

            Vector3 halfTileSize = i_TileSize * 0.5f;
            var rotation = SnapPointToRotation(i_RotationSnapPoint);
            Vector3 tileCentre = finalGlobalPosition + (rotation * halfTileSize);
            tileCentre.y = 0;

            Vector3 localLeft = rotation * Vector3.left;
            Vector3 localForward = (ROTATION_90_DEG * localLeft);
            Vector3 localRight = -localLeft;
            Vector3 localBack = -localForward;

            if (i_Selected)
                Gizmos.color = GridMapEditorBehaviour<GridPosition3D, int>.SELECTED_TILE_COLOR;
            else
                Gizmos.color = GridMapEditorBehaviour<GridPosition3D, int>.NORMAL_TILE_COLOR;

            
            Vector3 camPos = SceneView.currentDrawingSceneView.camera.transform.position;
            o_DrawCalls.Add(new GridMapEditorDrawRef(
                (camPos - tileCentre).sqrMagnitude,
                () => {
                    Color lastColor = Gizmos.color;
                    if (i_Selected)
                        Gizmos.color = GridMapEditorBehaviour<GridPosition3D, int>.SELECTED_EMPTY_TILE_COLOR;
                    else
                        Gizmos.color = GridMapEditorBehaviour<GridPosition3D, int>.EMPTY_TILE_COLOR;
                    Gizmos.DrawCube(tileCentre, new Vector3(0.85f * i_TileSize.x, 0.05f * i_TileSize.y, 0.85f * i_TileSize.z));
                    Gizmos.color = lastColor;
                }));

            /*
            var orderedDraws = drawElements.OrderByDescending(x => x.Distance);
            foreach (var item in orderedDraws)
            {
                item.Draw();
            }
            */
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
