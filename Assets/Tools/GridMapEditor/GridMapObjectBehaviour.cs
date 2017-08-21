
using System;
using Common.Grid;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Tools
{
    [ExecuteInEditMode]
    public abstract class GridMapObjectBehaviour<TPosition, TTileSettings> : MonoBehaviour
    {
        private static GridMapEditorBehaviour<TPosition, TTileSettings> Editor;

        public static int ActiveGridObject = -1;
        public static int ActiveGridTileIndex = -1;

        public string Id;
        public TPosition Position;
        public TPosition Size;
        public int ObjectSettings;

        public abstract int GetTileCount();
        public abstract IGridMapObjectTile<TPosition, TTileSettings> GetTile(int index);

        public List<GridMapObjectConnection> Connections = new List<GridMapObjectConnection>();
        


        public TPosition GetFinalGridPosition()
        {
            if (Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour<TPosition, TTileSettings>>();
            }
            if (Editor != null)
            {
                return GetFinalPositionRecursive(Editor.GetMapTypeData(), transform.parent, Position);
            }
            throw new NotImplementedException("GridMapEditorBehaviour not found");
        }

        private TPosition GetFinalPositionRecursive(GridMapEditorTypeData<TPosition, TTileSettings> i_MapTypeData, Transform i_Target, TPosition i_Pos)
        {
            var parent = i_Target.GetComponent<GridMapObjectBehaviour<TPosition, TTileSettings>>();
            if(parent != null)
            {
                var rotatedPos = i_MapTypeData.GetAbsolutePosition(parent.Position, parent.Size, i_Pos, i_MapTypeData.RotationToSnapPoint(parent.transform.rotation));
                return GetFinalPositionRecursive(i_MapTypeData, parent.transform.parent, rotatedPos);
            }
            return i_Pos;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour<TPosition, TTileSettings>>();
            }
            if (Editor != null)
            {
                Editor.DrawGridMapObject(this, m_GizmoDrawCalls);
            }

            if (m_GizmoDrawCalls.Count > 0)
            {
                var orderedDraws = m_GizmoDrawCalls.OrderByDescending(x => x.Distance);
                foreach (var item in orderedDraws)
                {
                    item.Draw();
                }
                m_GizmoDrawCalls.Clear();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour<TPosition, TTileSettings>>();
            }
            if (Editor != null)
            {
                int instanceId = GetInstanceID();
                if (ActiveGridObject != instanceId)
                {
                    ActiveGridObject = instanceId;
                    SceneView.RepaintAll();
                    Editor.DrawGridMapObject(this, m_GizmoDrawCalls);
                }
            }

            if (m_GizmoDrawCalls.Count > 0)
            {
                var orderedDraws = m_GizmoDrawCalls.OrderByDescending(x => x.Distance);
                foreach (var item in orderedDraws)
                {
                    item.Draw();
                }
                m_GizmoDrawCalls.Clear();
            }
        }
        
        
        private bool m_ValidateUpdate;
        private List<GridMapEditorDrawRef> m_GizmoDrawCalls = new List<GridMapEditorDrawRef>();

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
                Editor = FindObjectOfType<GridMapEditorBehaviour<TPosition, TTileSettings>>();
            }
            if (Editor != null)
            {
                if (fromTransform)
                {
                    Position = Editor.GetMapTypeData().SnapToGrid(transform, Editor.TileSize);
                }
                else
                {
                    Editor.GetMapTypeData().SnapToGrid(Position, Editor.TileSize, 0, transform);
                }
            }
        }
    }
}
