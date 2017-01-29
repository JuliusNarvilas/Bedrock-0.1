using Common.Grid;
using System;
using System.Collections.Generic;
using Common.Grid.Path;

namespace Assets.Common.Scripts.Grid
{
    public class HexGrid<TTile, TTerrain, TContext> : IGridControl<TTile, TTerrain, GridPosition2D, TContext>
        where TTile : GridTile<TTerrain, GridPosition2D, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        public int GetHeuristicDistance(GridPosition2D i_From, GridPosition2D i_To)
        {
            throw new NotImplementedException();
        }

        public GridPath<TTile, TTerrain, GridPosition2D, TContext> GetPath(GridPosition2D i_Start, GridPosition2D i_End, TContext i_Context)
        {
            throw new NotImplementedException();
        }

        public TTile GetTile(GridPosition2D i_Position)
        {
            throw new NotImplementedException();
        }

        public void GetConnected(GridPosition2D i_Position, List<TTile> o_ConnectedElements)
        {
            throw new NotImplementedException();
        }

        public void GetPathArea(GridPosition2D i_Min, GridPosition2D i_Max, TContext i_Context)
        {
            throw new NotImplementedException();
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public bool TryGetTile(GridPosition2D i_Position, out TTile o_Tile)
        {
            throw new NotImplementedException();
        }
    }
}
