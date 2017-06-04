using System;
using System.Collections.Generic;
using Common.Grid.Path;
using Common.Grid.Physics;
using UnityEngine;

namespace Common.Grid.Specializations
{
    //TODO: WIP
    public class HexGrid<TContext, TTile> : IGridControl<GridPosition2D, TContext, TTile> where TTile : GridTile<GridPosition2D, TContext, TTile>
    {
        public int GetHeuristicDistance(GridPosition2D i_From, GridPosition2D i_To)
        {
            throw new NotImplementedException();
        }

        public GridPath<GridPosition2D, TContext, TTile> GetPath(GridPosition2D i_Start, GridPosition2D i_End, TContext i_Context)
        {
            throw new NotImplementedException();
        }

        public void GetConnected(GridPosition2D i_Position, List<TTile> o_ConnectedTiles)
        {
            throw new NotImplementedException();
        }

        public void GetPathArea(GridPosition2D i_Min, GridPosition2D i_Max, TContext i_Context)
        {
            throw new NotImplementedException();
        }

        public bool TryGetTile(GridPosition2D i_Position, out TTile o_Tile)
        {
            throw new NotImplementedException();
        }

        public GridPathArea<GridPosition2D, TContext, TTile> GetPathArea(GridPosition2D i_Min, GridPosition2D i_Max, GridPosition2D i_Origin, TContext i_Context)
        {
            throw new NotImplementedException();
        }

        public bool TryGetTilePhysicalData(GridPosition2D i_Position, out GridTilePhysicalData o_Tile)
        {
            throw new NotImplementedException();
        }

        public void GetIntersectionsBetween(GridPosition2D i_Source, GridPosition2D i_Target, List<GridTileRayIntersection<GridPosition2D, TContext, TTile>> o_TilesBetweenPositions, Vector3 i_SourceOffset = default(Vector3), Vector3 i_TargetOffset = default(Vector3))
        {
            throw new NotImplementedException();
        }
    }
}
