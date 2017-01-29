using Common.Grid.Path;
using System.Collections.Generic;

namespace Common.Grid
{
    public interface IGridControl<TTile, TTerrain, TPosition, TContext>
        where TTile : GridTile<TTerrain, TPosition, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="i_Start">The start position.</param>
        /// <param name="i_End">The end position.</param>
        /// <returns></returns>
        GridPath<TTile, TTerrain, TPosition, TContext> GetPath(TPosition i_Start, TPosition i_End, TContext i_Context);

        void GetPathArea(TPosition i_Min, TPosition i_Max, TContext i_Context);

        TTile GetTile(TPosition i_Position);

        bool TryGetTile(TPosition i_Position, out TTile o_Tile);

        int GetHeuristicDistance(TPosition i_From, TPosition i_To);

        void GetConnected(TPosition i_Position, List<TTile> o_ConnectedElements);

        void Draw();
    }
}
