using System;
using System.Collections.Generic;

namespace Common.Grid.Path
{
    public class GridPathData2DProvider<TTile, TTerrain, TContext> : IGridPathDataProvider<TTile, TTerrain, GridPosition2D, TContext>
        where TTile : GridTile<TTerrain, GridPosition2D, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        List<IGridPathData<TTile, TTerrain, GridPosition2D, TContext>> m_Data = new List<IGridPathData<TTile, TTerrain, GridPosition2D, TContext>>();

        public IGridPathData<TTile, TTerrain, GridPosition2D, TContext> GetGridPathData(GridPosition2D i_Size)
        {
            throw new NotImplementedException();
        }

        public void Recycle(IGridPathData<TTile, TTerrain, GridPosition2D, TContext> i_Data)
        {
            i_Data.Clean();
            throw new NotImplementedException();
        }
    }
}
