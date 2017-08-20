
using Common.Grid;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    [ExecuteInEditMode]
    public class GridMapObjectBehaviour<TPosition, TTileSettings> : MonoBehaviour
    {
        private static GridMapEditorBehaviour<TPosition, TTileSettings> Editor;

        public static int ActiveGridObject = -1;
        public static int ActiveGridTileIndex = -1;

        public string Id;
        public TPosition Position;
        public TPosition Size;
        public int ObjectSettings;
        public List<GridMapObjectTile3D> Tiles;
        public List<GridMapObjectConnection> Connections;



        public GridPosition3D GetFinalGridPosition()
        {
            if (Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour<GridPosition3D, int>>();
            }
            if (Editor != null)
            {
                return GetFinalPositionRecursive(Editor.MapTypeData, transform.parent, Position);
            }
            return GetFinalPositionRecursive(new GridMapEditorCuboidTypeData(), transform.parent, Position);
        }

        private static GridPosition3D GetFinalPositionRecursive(GridMapEditorTypeData<GridPosition3D, int> i_MapTypeData, Transform i_Target, GridPosition3D i_Pos)
        {
            var parent = i_Target.GetComponent<GridMapObjectBehaviour>();
            if(parent != null)
            {
                var rotatedPos = i_MapTypeData.RotateGridOffset(i_Pos, i_MapTypeData.RotationToSnapPoint(parent.transform.rotation));
                return GetFinalPositionRecursive(i_MapTypeData, parent.transform.parent, rotatedPos + parent.Position);
            }
            return i_Pos;
        }

        private void OnDrawGizmos()
        {
            if(Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour<GridPosition3D, int>>();
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
                Editor = FindObjectOfType<GridMapEditorBehaviour<GridPosition3D, int>>();
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
        
        
#if UNITY_EDITOR
        private bool m_ValidateUpdate;

        private void OnValidate()
        {
            m_ValidateUpdate = true;
        }


        private void Update()
        {
            if (m_ValidateUpdate)
            {
                m_ValidateUpdate = false;
                SnapToGrid(false);
            }
            else if (transform.hasChanged)
            {
                transform.hasChanged = false;
                SnapToGrid(true);
            }
        }
#endif

        public void SnapToGrid(bool fromTransform = false)
        {
            transform.localScale = Vector3.one;

            if (Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour<GridPosition3D, int>>();
            }
            if (Editor != null)
            {
                if (fromTransform)
                {
                    Position = Editor.MapTypeData.SnapToGrid(transform, Editor.TileSize);
                }
                else
                {
                    Editor.MapTypeData.SnapToGrid(Position, Editor.TileSize, 0, transform);
                }
            }
        }
    }
}
