
using Common.Grid;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class GridMapObjectBehaviour : MonoBehaviour
    {
        private static GridMapEditorBehaviour Editor;

        public static int ActiveGridObject = -1;
        public static int ActiveGridTileIndex = -1;

        public string Id;
        public GridPosition3D Offset;
        public GridPosition3D Size;
        public int ObjectSettings;
        public List<GridMapObjectTile> Tiles;
        public List<GridMapObjectConnection> Connections;

        private void Start()
        {
            
        }

        public GridPosition3D GetFinalGridPosition()
        {
            if (Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour>();
            }
            if (Editor != null)
            {
                return GetFinalPositionRecursive(Editor.EditorMapTypeData, transform.parent, Offset);
            }
            return GetFinalPositionRecursive(new GridMapEditorCuboidTypeData(), transform.parent, Offset);
        }

        private static GridPosition3D GetFinalPositionRecursive(GridMapEditorCuboidTypeData i_MapTypeData, Transform i_Target, GridPosition3D i_Pos)
        {
            var parent = i_Target.GetComponent<GridMapObjectBehaviour>();
            if(parent != null)
            {
                var rotatedPos = i_MapTypeData.RotateGridPosition(i_Pos, parent.transform.rotation);
                return GetFinalPositionRecursive(i_MapTypeData, parent.transform.parent, rotatedPos + parent.Offset);
            }
            return i_Pos;
        }

        private void OnDrawGizmos()
        {
            if(Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour>();
            }
            if (Editor != null)
            {
                Editor.DrawGridMapObject(this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour>();
            }
            if (Editor != null)
            {
                int instanceId = GetInstanceID();
                if (ActiveGridObject != instanceId)
                {
                    ActiveGridObject = instanceId;
                    SceneView.RepaintAll();
                    Editor.DrawGridMapObject(this);
                }
            }
        }

        public void SnapToGrid(bool fromTransform = false)
        {
            transform.localScale = Vector3.one;

            if (Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour>();
            }
            if (Editor != null)
            {
                var gridObjRotation = Editor.EditorMapTypeData.SnapRotationToGrid(transform.rotation);
                transform.rotation = gridObjRotation;

                Vector3 tileSize = Editor.TileSize;
                if (fromTransform)
                {
                    Vector3 halfSize = tileSize * 0.5f;
                    Vector3 position = transform.localPosition + halfSize;

                    Offset = new GridPosition3D(
                        Mathf.FloorToInt(position.x / tileSize.x),
                        Mathf.FloorToInt(position.z / tileSize.z),
                        Mathf.FloorToInt(position.y / tileSize.y)
                        );
                }

                transform.localPosition = new Vector3(Offset.X * tileSize.x, Offset.Z * tileSize.y, Offset.Y * tileSize.z);
            }
        }
    }
}
