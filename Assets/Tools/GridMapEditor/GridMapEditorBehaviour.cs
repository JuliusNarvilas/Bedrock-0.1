using Common;
using Common.Graphics;
using Common.Grid;
using Common.Grid.Generation;
using Common.Grid.Serialization;
using Common.Grid.Serialization.Specialization;
using Game.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{

    public class GridMapEditorBehaviour<TPosition, TTileSettings> : MonoBehaviour
    {
        public static readonly Color NORMAL_TILE_COLOR = new Color(0f, 0f, 1f, 0.5f);
        public static readonly Color EMPTY_TILE_COLOR = new Color(0.7f, 0.7f, 1f, 0.5f);
        public static readonly Color SELECTED_TILE_COLOR = new Color(1f, 1f, 0f, 0.5f);
        public static readonly Color SELECTED_EMPTY_TILE_COLOR = new Color(1f, 1f, 0.7f, 0.5f);

        public GridSave<TPosition, TTileSettings> SaveAsset;

        public TPosition Size;
        public Vector3 TileSize;
        public GridMapEditorTypeData<TPosition, TTileSettings> MapTypeData;
        public bool DrawTileData = true;

        private Vector3 m_LastTileSize;

        public void Load()
        {
            if (SaveAsset != null)
            {
                Size = SaveAsset.GridSize;
                TileSize = SaveAsset.PhysicalTileSize;
            }
            else
            {
                Log.ProductionLogWarning("GridMapEditorBehaviour.Load(): No save asset given.");
            }
        }

        public void Save()
        {
            if (SaveAsset != null)
            {
                SaveAsset.GridSize = Size;
                SaveAsset.PhysicalTileSize = TileSize;
            }
            else
            {
                Log.ProductionLogWarning("GridMapEditorBehaviour.Save(): No save asset given.");
            }
        }




        public void DrawGridMapObject(GridMapObjectBehaviour i_Obj)
        {
            if (DrawTileData)
            {
                MapTypeData.DrawObjectBounds();
                //MapTypeDrawing.DrawObject(this, i_Obj);
            }
        }
        

        private void OnDrawGizmos()
        {
            //MapTypeDrawing.DrawBounds(this);
        }

        public void OnValidate()
        {
            GridMapObjectBehaviour[] gridMapObjects = null;
            if (m_LastTileSize != TileSize)
            {
                m_LastTileSize = TileSize;
                gridMapObjects = GetComponentsInChildren<GridMapObjectBehaviour>();

                foreach (var gridObject in gridMapObjects)
                {
                    gridObject.SnapToGrid(false);
                }
            }

            
        }


    }
}