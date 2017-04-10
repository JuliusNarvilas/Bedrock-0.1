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
        public bool AddFloorToChildren = false;

        private Vector3 m_LastTileSize;
        

        public void DrawGridMapObject(GridMapObjectBehaviour i_Obj)
        {
            if (DrawTileData)
            {
                MapTypeDrawing.DrawObject(this, i_Obj);
            }
        }
        

        public static float GetBlockerDirectionHeight(GridTileBlockerFlags blockerFlags, GridTileLocation location)
        {
            int stride = (int)GridTileBlockerHeight.BitStride * (int)location;
            if ((((int)GridTileBlockerHeight.FullySetStride << stride) & (int)blockerFlags) != 0)
            {
                if ((((int)GridTileBlockerHeight.ExtraLargeBlocker << stride) & (int)blockerFlags) == ((int)GridTileBlockerHeight.ExtraLargeBlocker << stride))
                    return 1.0f;
                else if ((((int)GridTileBlockerHeight.LargeBlocker << stride) & (int)blockerFlags) == ((int)GridTileBlockerHeight.LargeBlocker << stride))
                    return 0.75f;
                else if ((((int)GridTileBlockerHeight.MediumBlocker << stride) & (int)blockerFlags) == ((int)GridTileBlockerHeight.MediumBlocker << stride))
                    return 0.5f;
                else if ((((int)GridTileBlockerHeight.SmallBlocker << stride) & (int)blockerFlags) == ((int)GridTileBlockerHeight.SmallBlocker << stride))
                    return 0.25f;
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

            if(AddFloorToChildren)
            {
                AddFloorToChildren = false;
                if(gridMapObjects == null)
                {
                    gridMapObjects = GetComponentsInChildren<GridMapObjectBehaviour>();
                }
                
                foreach (var gridObject in gridMapObjects)
                {
                    if(gridObject.isActiveAndEnabled)
                    {
                        var size = gridObject.Size;
                        var fillTracker = new bool[size.X * size.Y];
                        foreach (var tile in gridObject.Tiles)
                        {
                            var pos = tile.Position;
                            if (pos.Z == 0)
                            {
                                int index = size.X * pos.Y + pos.X;
                                fillTracker[index] = true;
                                tile.TileBlockerSettings |= GridTileBlockerFlags.BottomBlocker;
                            }
                        }


                        for (int itX = 0; itX < size.X; ++itX)
                        {
                            for(int itY = 0; itY < size.Y; ++itY)
                            {
                                if(!fillTracker[size.X * itY + itX])
                                {
                                    var temp = new GridMapObjectTile();
                                    temp.Position = new GridPosition3D(itX, itY, 0);
                                    temp.TileBlockerSettings |= GridTileBlockerFlags.BottomBlocker;
                                    gridObject.Tiles.Add(temp);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}