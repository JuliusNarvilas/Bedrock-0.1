
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    [ExecuteInEditMode]
    public abstract class GridMapObjectBehaviour<TPosition, TTileSettings> : MonoBehaviour
    {
        private static GridMapEditorBehaviour<TPosition, TTileSettings> Editor;

        public static int ActiveGridObject = -1;
        public static int ActiveGridTileIndex = -1;

        public string Id;
        public int RotationSnapPoint;
        public TPosition Position;
        public TPosition Size;
        public int ObjectSettings;

        public abstract int GetTileCount();
        public abstract IGridMapObjectTile<TPosition, TTileSettings> GetTile(int index);

        [HideInInspector]
        public List<GridMapObjectConnection> Connections = new List<GridMapObjectConnection>();


        /*
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
            if (parent != null)
            {
                var rotatedPos = i_MapTypeData.GetAbsolutePosition(parent.Position, parent.Size, i_Pos, i_MapTypeData.RotationToSnapPoint(parent.transform.rotation));
                return GetFinalPositionRecursive(i_MapTypeData, parent.transform.parent, rotatedPos);
            }
            return i_Pos;
        }
        */

#if UNITY_EDITOR

        private bool m_ValidateUpdate;
        private GameObject m_DebugDisplay;

        private void Start()
        {
            if (Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour<TPosition, TTileSettings>>();
            }
            if (Editor != null)
            {
                RotationSnapPoint = Editor.GetMapTypeData().RotationToSnapPoint(transform.localRotation);
                Position = Editor.GetMapTypeData().SnapToGrid(transform, Position, ref RotationSnapPoint, Size, Editor.TileSize);
            }
        }

        public void OnValidate()
        {
            m_ValidateUpdate = true;
        }

        private void Update()
        {
            if (m_ValidateUpdate)
            {
                m_ValidateUpdate = false;
                SnapToGrid(false);
                transform.hasChanged = false;
                DrawDebug();
            }
            else if (transform.hasChanged)
            {
                transform.hasChanged = false;
                SnapToGrid(true);
            }
        }

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
                    Position = Editor.GetMapTypeData().SnapToGrid(transform, Position, ref RotationSnapPoint, Size, Editor.TileSize);
                }
                else
                {
                    Editor.GetMapTypeData().SnapToGrid(Position, Size, Editor.TileSize, RotationSnapPoint, transform);
                }
            }
        }

        public void DrawDebug()
        {
            if (m_DebugDisplay != null)
            {
                DestroyImmediate(m_DebugDisplay);
            }
            if (Editor == null)
            {
                Editor = FindObjectOfType<GridMapEditorBehaviour<TPosition, TTileSettings>>();
            }
            if (Editor != null)
            {
                m_DebugDisplay = Editor.GetMapTypeData().BuildObjectDebug(this, Editor.TileSize);
                m_DebugDisplay.transform.SetParent(transform, false);
            }
        }
#endif
    }
}
