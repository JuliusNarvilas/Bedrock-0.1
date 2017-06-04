using Common.Graphics;
using Common.Grid;
using Common.Grid.Generation;
using Game.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{

    public class GridMapEditorBehaviour : MonoBehaviour
    {
        public static readonly Color NORMAL_TILE_COLOR = new Color(0f, 0f, 1f, 0.5f);
        public static readonly Color EMPTY_TILE_COLOR = new Color(0.7f, 0.7f, 1f, 0.5f);
        public static readonly Color SELECTED_TILE_COLOR = new Color(1f, 1f, 0f, 0.5f);
        public static readonly Color SELECTED_EMPTY_TILE_COLOR = new Color(1f, 1f, 0.7f, 0.5f);


        public GridPosition3D Size;
        public Vector3 TileSize;
        public IGridMapEditorTypeData MapTypeData = new GridMapEditorCuboidTypeData();
        public IGridMapEditorTypeDrawing MapTypeDrawing = new GridMapEditorCuboidTypeDrawing();
        public bool DrawTileData = true;

        private Vector3 m_LastTileSize;
        

        public void DrawGridMapObject(GridMapObjectBehaviour i_Obj)
        {
            if (DrawTileData)
            {
                MapTypeDrawing.DrawObject(this, i_Obj);
            }
        }
        

        public static float GetBlockerHeight(EGridTileSettings settingsFlags)
        {
            var blockerFlags = EGridTileSettings.BlockerFullySetStride & settingsFlags;

            switch(blockerFlags)
            {
                case EGridTileSettings.None:
                    break;
                case EGridTileSettings.BlockerExtraSmall:
                    return 1.0f / 6.0f;
                case EGridTileSettings.BlockerSmall:
                    return 2.0f / 6.0f;
                case EGridTileSettings.BlockerMedium:
                    return 3.0f / 6.0f;
                case EGridTileSettings.BlockerMediumLarge:
                    return 4.0f / 6.0f;
                case EGridTileSettings.BlockerLarge:
                    return 5.0f / 6.0f;
                case EGridTileSettings.BlockerExtraLarge:
                    return 6.0f / 6.0f;
            }
            return 0.0f;
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