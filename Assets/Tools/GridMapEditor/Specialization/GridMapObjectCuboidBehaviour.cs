
using Common.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.Specialization
{
    public class GridMapObjectCuboidBehaviour : GridMapObjectBehaviour<GridPosition3D, int>
    {
        public List<GridMapObjectTile3D> m_Tiles = new List<GridMapObjectTile3D>();

        public override IGridMapObjectTile<GridPosition3D, int> GetTile(int index)
        {
            return m_Tiles[index];
        }

        public override int GetTileCount()
        {
            return m_Tiles.Count;
        }
    }
}
