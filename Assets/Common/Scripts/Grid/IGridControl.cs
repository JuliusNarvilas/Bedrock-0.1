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

        /// <summary>
        /// Gets the path area.
        /// </summary>
        /// <param name="i_Min">The minimum area coordinate.</param>
        /// <param name="i_Max">The maximum area coordinate.</param>
        /// <param name="i_Origin">The origin pathing origin.</param>
        /// <param name="i_Context">The context.</param>
        /// <returns></returns>
        GridArea<TTile, TTerrain, TPosition, TContext> GetPathArea(TPosition i_Min, TPosition i_Max, TPosition i_Origin, TContext i_Context);

        /// <summary>
        /// Tries to get a tile at given position.
        /// </summary>
        /// <param name="i_Position">The position.</param>
        /// <param name="o_Tile">The tile result.</param>
        /// <returns>True if a tile was found and false otherwise (usually because position is outside grid bounds)</returns>
        bool TryGetTile(TPosition i_Position, out TTile o_Tile);

        /// <summary>
        /// Gets the heuristic distance.
        /// </summary>
        /// <param name="i_From">The source position.</param>
        /// <param name="i_To">The destination position.</param>
        /// <returns></returns>
        int GetHeuristicDistance(TPosition i_From, TPosition i_To);

        /// <summary>
        /// Gets the connected tiles for the given position.
        /// </summary>
        /// <param name="i_Position">The position.</param>
        /// <param name="o_ConnectedTiles">The connected tiles.</param>
        void GetConnected(TPosition i_Position, List<TTile> o_ConnectedTiles);
    }
}
