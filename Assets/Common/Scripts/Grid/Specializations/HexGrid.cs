using System;
using System.Collections.Generic;
using Common.Grid.Path;

namespace Common.Grid.Specializations
{
    //TODO: WIP
    public class HexGrid<TTileData, TContext> : IGridControl<GridPosition2D, TTileData, TContext>
    {
        public int GetHeuristicDistance(GridPosition2D i_From, GridPosition2D i_To)
        {
            throw new NotImplementedException();
        }

        public GridPath<GridPosition2D, TTileData, TContext> GetPath(GridPosition2D i_Start, GridPosition2D i_End, TContext i_Context)
        {
            throw new NotImplementedException();
        }

        public void GetConnected(GridPosition2D i_Position, List<GridTile<GridPosition2D, TTileData, TContext>> o_ConnectedTiles)
        {
            throw new NotImplementedException();
        }

        public void GetPathArea(GridPosition2D i_Min, GridPosition2D i_Max, TContext i_Context)
        {
            throw new NotImplementedException();
        }

        public bool TryGetTile(GridPosition2D i_Position, out GridTile<GridPosition2D, TTileData, TContext> o_Tile)
        {
            throw new NotImplementedException();
        }

        public GridArea<GridPosition2D, TTileData, TContext> GetPathArea(GridPosition2D i_Min, GridPosition2D i_Max, GridPosition2D i_Origin, TContext i_Context)
        {
            throw new NotImplementedException();
        }
    }
}
