using System;
using Common.Grid;
using UnityEngine;
using Common;
using System.Collections.Generic;
using Common.Utility;
using Common.Graphics;
using Common.Grid.Generation;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tools
{   
    public class GridMapEditorCuboidTypeData : GridMapEditorTypeData<GridPosition3D, int>
    {
        public static readonly Quaternion ROTATION_90_DEG = Quaternion.Euler(0, 90, 0);

        public Material MatTileBaseDefault;
        public Material MatTileBaseEmpty;
        public Material MatTileBaseSelected;
        public Material MatTileBaseSelectedEmpty;

        public Material MatTileWallDefault;
        public Material MatTileWallSelected;

        public Material MatGridObjectDefault;
        public Material MatGridObjectSelected;

        public Material MatGridDefault;

        [Range(0.001f, 1.0f)]
        public float TileBaseLift = 0.1f;

        public bool HideDebugObjectsInScene = true;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Grid/MapEditorTypeData/Cuboid")]
#endif
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<GridMapEditorCuboidTypeData>();
        }

        
        private void OnValidate()
        {
            Updated = true;
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

        public override GridPosition3D SnapToGrid(Transform i_NewTransform, GridPosition3D i_ObjectPosition, ref int i_OldRotationSnapPoint, GridPosition3D i_ObjSize, Vector3 i_TileSize)
        {
            var newRotation = i_NewTransform.localRotation;
            var oldRotation = SnapPointToRotation(i_OldRotationSnapPoint);

            GridPosition3D result = i_ObjectPosition;
            if(newRotation.eulerAngles.y != oldRotation.eulerAngles.y)
            {
                //adjust rotation
                var newSnapPoint = RotationToSnapPoint(newRotation);
                i_OldRotationSnapPoint = newSnapPoint;
                i_NewTransform.localRotation = SnapPointToRotation(newSnapPoint);
                var newAdjustedGridPosition = GetAbsolutePosition(i_ObjectPosition, i_ObjSize, new GridPosition3D(), newSnapPoint);
                i_NewTransform.localPosition = new Vector3(newAdjustedGridPosition.X * i_TileSize.x, newAdjustedGridPosition.Y * i_TileSize.y, newAdjustedGridPosition.Z * i_TileSize.z);
            }
            else
            {
                //adjust position
                Vector3 halfSize = i_TileSize * 0.5f;
                Vector3 newWorldPositionLarger = i_NewTransform.localPosition + halfSize;
                var newGridPosition = new GridPosition3D(
                    Mathf.FloorToInt(newWorldPositionLarger.x / i_TileSize.x),
                    Mathf.FloorToInt(newWorldPositionLarger.y / i_TileSize.y),
                    Mathf.FloorToInt(newWorldPositionLarger.z / i_TileSize.z)
                    );
                
                i_NewTransform.localPosition =  new Vector3(newGridPosition.X * i_TileSize.x, newGridPosition.Y * i_TileSize.y, newGridPosition.Z * i_TileSize.z);
                result = i_ObjectPosition + newGridPosition - GetAbsolutePosition(i_ObjectPosition, i_ObjSize, new GridPosition3D(), i_OldRotationSnapPoint);
            }
            return result;
        }

        public override void SnapToGrid(GridPosition3D i_ObjectPosition, GridPosition3D i_ObjSize, Vector3 i_TileSize, int i_RotationSnapPoint, Transform o_Output)
        {
            o_Output.localRotation = SnapPointToRotation(i_RotationSnapPoint);
            var newAbsoluteGridPosition = GetAbsolutePosition(i_ObjectPosition, i_ObjSize, new GridPosition3D(), i_RotationSnapPoint);
            o_Output.localPosition = new Vector3(newAbsoluteGridPosition.X * i_TileSize.x, newAbsoluteGridPosition.Y * i_TileSize.y, newAbsoluteGridPosition.Z * i_TileSize.z);
        }

        
        /*
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

            
            var orderedDraws = drawElements.OrderByDescending(x => x.Distance);
            foreach (var item in orderedDraws)
            {
                item.Draw();
            }
        }
        */

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

        public GameObject BuildTileDebug(GridPosition3D i_GridPosition, Vector3 i_TileSize, int i_Settings, bool i_Selected)
        {
#if UNITY_EDITOR
            var result = new GameObject("Debug_GridTile");
            result.hideFlags = HideDebugObjectsInScene ? HideFlags.HideAndDontSave : HideFlags.DontSave;
            result.transform.localPosition = new Vector3(i_GridPosition.X * i_TileSize.x, i_GridPosition.Y * i_TileSize.y, i_GridPosition.Z * i_TileSize.z);
            var renderer = result.AddComponent<MeshRenderer>();
            EditorUtility.SetSelectedRenderState(renderer, EditorSelectedRenderState.Hidden);
            var filter = result.AddComponent<MeshFilter>();
            var mesh = new Mesh();
            
            float baseLift = TileBaseLift * i_TileSize.y;
            var verts = new List<Vector3>(4);
            verts.Add(new Vector3(0, baseLift, i_TileSize.z));
            verts.Add(new Vector3(i_TileSize.x, baseLift, i_TileSize.z));
            verts.Add(new Vector3(i_TileSize.x, baseLift, 0));
            verts.Add(new Vector3(0, baseLift, 0));
            mesh.SetVertices(verts);

            var triangles = new int[]
            {
                0, 1, 3, 1, 2, 3
            };
            mesh.SetTriangles(triangles, 0);
            filter.mesh = mesh;
            
            switch(i_Settings)
            {
                case 0:
                    renderer.material = i_Selected ? MatTileBaseSelectedEmpty : MatTileBaseEmpty;
                    break;
                default:
                    renderer.material = i_Selected ? MatTileBaseSelected : MatTileBaseEmpty;
                    break;
            }

            return result;
#else
            throw new NotSupportedException();
#endif
        }

        private GameObject addDebugBounds(GameObject i_Go, Material i_Mat, GridPosition3D i_ObjSize, Vector3 i_TileSize)
        {
#if UNITY_EDITOR
            var renderer = i_Go.AddComponent<MeshRenderer>();
            EditorUtility.SetSelectedRenderState(renderer, EditorSelectedRenderState.Hidden);
            var filter = i_Go.AddComponent<MeshFilter>();
            var mesh = new Mesh();

            var heightAdjustment = new Vector3(0, i_TileSize.y * i_ObjSize.Y, 0);
            var verts = new List<Vector3>(4);
            verts.Add(new Vector3(0, 0, i_TileSize.z * i_ObjSize.Z));
            verts.Add(new Vector3(i_TileSize.x * i_ObjSize.X, 0, i_TileSize.z * i_ObjSize.Z));
            verts.Add(new Vector3(i_TileSize.x * i_ObjSize.X, 0, 0));
            verts.Add(new Vector3());

            verts.Add(verts[0] + heightAdjustment);
            verts.Add(verts[1] + heightAdjustment);
            verts.Add(verts[2] + heightAdjustment);
            verts.Add(verts[3] + heightAdjustment);
            mesh.SetVertices(verts);

            var tris = new int[]
            {
                0, 1, 1, 2, 2, 3, 3, 0,
                4, 5, 5, 6, 6, 7, 7, 4,
                0, 4, 1, 5, 2, 6, 3, 7
            };
            mesh.SetIndices(tris, MeshTopology.Lines, 0);

            filter.mesh = mesh;
            renderer.material = i_Mat;

            return i_Go;
#else
            throw new NotSupportedException();
#endif
        }

        public override GameObject BuildObjectDebug(GridMapObjectBehaviour<GridPosition3D, int> i_Obj, Vector3 i_TileSize)
        {
            var result = new GameObject("Debug_GridObject");
            result.hideFlags = HideDebugObjectsInScene ? HideFlags.HideAndDontSave : HideFlags.DontSave;
            int instanceId = i_Obj.GetInstanceID();
            var isSelected = (GridMapObjectBehaviour<GridPosition3D, int>.ActiveGridObject == instanceId);
            addDebugBounds(result, isSelected ? MatGridObjectSelected : MatGridObjectDefault, i_Obj.Size, i_TileSize);

            int tileCount = i_Obj.GetTileCount();
            for (int i = 0; i < tileCount; ++i)
            {
                var tile = i_Obj.GetTile(i);
                var tileSelected = isSelected && (GridMapObjectBehaviour<GridPosition3D, int>.ActiveGridTileIndex == i);
                var tileGO = BuildTileDebug(tile.Position, i_TileSize, tile.Settings, tileSelected);
                tileGO.transform.SetParent(result.transform, false);
            }

            int connectionCount = i_Obj.Connections.Count;
            for (int i = 0; i < connectionCount; ++i)
            {
                //var connection = i_Obj.Connections[i];
                //var adjustedTilePosition = i_Editor.MapTypeData.RotateGridPosition(connection.Position, objRotation);
                //var adjustedDirectionType = i_Editor.MapTypeData.RotateGridDirectionType(((int)connection.TileLocation) & GridHelpers.GRID_TILE_LOCATION_STRIDE_MASK, objRotation);
                //DrawConnectionDisplay(rootOriginPos, i_Editor.TileSize, objRotation, objOrigin + adjustedTilePosition, adjustedDirectionType | (int)connection.Settings);
            }

            return result;
        }

        public override GameObject BuildGridDebug(GridMapEditorBehaviour<GridPosition3D, int> i_Grid)
        {
            var result = new GameObject("Debug_Grid");
            result.hideFlags = HideDebugObjectsInScene ? HideFlags.HideAndDontSave : HideFlags.DontSave;
            addDebugBounds(result, MatGridDefault, i_Grid.Size, i_Grid.TileSize);

            return result;
        }
    }
}
