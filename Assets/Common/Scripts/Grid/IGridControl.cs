using Common.Grid.Path;
using Common.Grid.Physics;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Grid
{
    public interface IGridControl<TPosition, TContext, TTile> where TTile : GridTile<TPosition, TContext, TTile>
    {
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="i_Start">The start position.</param>
        /// <param name="i_End">The end position.</param>
        /// <returns></returns>
        GridPath<TPosition, TContext, TTile> GetPath(TPosition i_Start, TPosition i_End, EGridPathAvoidanceStrategy i_AvoidanceStrategy, TContext i_Context);

        /// <summary>
        /// Gets the path area.
        /// </summary>
        /// <param name="i_Min">The minimum area coordinate.</param>
        /// <param name="i_Max">The maximum area coordinate.</param>
        /// <param name="i_Origin">The origin pathing origin.</param>
        /// <param name="i_Context">The context.</param>
        /// <returns></returns>
        GridPathArea<TPosition, TContext, TTile> GetPathArea(TPosition i_Min, TPosition i_Max, TPosition i_Origin, EGridPathAvoidanceStrategy i_AvoidanceStrategy, TContext i_Context);

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

        bool TryGetTilePhysicalData(TPosition i_Position, out GridTilePhysicalData o_Tile);

        /// <summary>
        /// Populates a list of intersection data between two tiles.
        /// Intersections are tested using a simple ray cast
        /// and the source tile does not appear in the intersaction results
        /// as the ray only hits the back face of the origin tile shape.
        /// </summary>
        /// <param name="i_Source">Position of the source tile.</param>
        /// <param name="i_Target">Position of the target tile.</param>
        /// <param name="o_TilesBetweenPositions">List that is populated with results.</param>
        /// <param name="i_SourceOffset">Offset within the source tile. Must be within the tile shape bounds!</param>
        /// <param name="i_TargetOffset">Offset within the target tile. Must be within the tile shape bounds!</param>
        void GetIntersectionsBetween(
            TPosition i_Source, TPosition i_Target,
            List<GridTileRayIntersection<TPosition, TContext, TTile>> o_TilesBetweenPositions,
            Vector3 i_SourceOffset = new Vector3(), Vector3 i_TargetOffset = new Vector3());
    }
}
